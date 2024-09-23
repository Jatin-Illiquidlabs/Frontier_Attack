using System.Globalization;
using Lean.Pool;
using TMPro;
using UnityEngine;
using Zenject;

namespace WerewolfBearer {
    public class EnemyEventsHandler : MonoBehaviour, IEnemyEventsListener {
        [SerializeField]
        private RectTransform _hitIndicatorsRoot;

        [Inject]
        private GameplayPools _gameplayPools;

        [Inject]
        private PlayerCharacterModel _playerCharacterModel;

        private PlayerCharacterController _playerCharacterController;

        public void HandleNewGame(PlayerCharacterController playerCharacterController) {
            _playerCharacterController = playerCharacterController;
        }

        public void OnTouchedPlayer(EnemyController enemyController) {
            if (enemyController.Model.Health <= 0)
                return;

            _playerCharacterModel.TakeDamage.Execute(enemyController.Model.Damage);
        }

        public void OnTouchedPlayerWeapon(EnemyController enemyController, AttackData attackData) {
            if (enemyController.Model.DamageCooldown > 0 ||
                enemyController.Model.Health <= 0)
                return;

            enemyController.Model.DamageCooldown = enemyController.EnemyDefinition.DamageCooldown;

            Vector2 knockbackVelocity = enemyController.Model.KnockbackVelocity ?? Vector2.zero;
            knockbackVelocity += attackData.KnockbackDirection.normalized * attackData.KnockbackScale * enemyController.EnemyDefinition.KnockbackScale;
            enemyController.Model.KnockbackVelocity = knockbackVelocity;

#if DEBUG
            Debug.DrawLine(
                enemyController.PhysicsView.Rigidbody2D.position,
                enemyController.PhysicsView.Rigidbody2D.position + knockbackVelocity * 2f,
                Color.magenta, 2f,
                false
            );
#endif

            if (attackData.Damage <= 0)
                return;

            float damage = attackData.Damage * _playerCharacterModel.DamageMultiplier.Value;
            enemyController.Model.Health -= damage;
            bool isDead = enemyController.Model.Health <= 0;
            enemyController.VisualView.StartTakeDamageAnimation(isDead);

            GameObject hitIndicatorGo = _gameplayPools.HitIndicator.Spawn(Vector3.zero, Quaternion.identity, _hitIndicatorsRoot);
            if (hitIndicatorGo != null) {
                hitIndicatorGo.GetComponent<RectTransform>().position = enemyController.VisualView.Transform.position;

                TextMeshProUGUI hitIndicatorText = hitIndicatorGo.GetComponent<TextMeshProUGUI>();
                hitIndicatorText.text = damage.ToString("0", CultureInfo.InvariantCulture);
                _gameplayPools.HitIndicator.Despawn(hitIndicatorGo, 2f);
            }

            if (isDead) {
                _playerCharacterModel.Kills.Value++;

                LeanGameObjectPool explosionPool = _gameplayPools.EnemyExplosions[Random.Range(0, _gameplayPools.EnemyExplosions.Length - 1)];
                GameObject explosionGo = explosionPool.Spawn(enemyController.VisualView.Transform.position, Quaternion.identity);

                float explosionScale = enemyController.EnemyDefinition.BaseScale;
                if (enemyController.EnemyDefinition.IsBoss) {
                    explosionScale *= 1.5f;
                }

                explosionGo.transform.localScale = Vector3.one * explosionScale;
                explosionPool.Despawn(explosionGo, 3f);
                enemyController.Destroyed?.Invoke();
                enemyController.VisualView.StartDeathAnimation();
            }

            attackData.DecrementRemainingPierce();
        }
    }
}
