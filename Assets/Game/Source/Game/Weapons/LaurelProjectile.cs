using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace WerewolfBearer {
    public class LaurelProjectile : PlayerProjectile {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private float _pushRadius = 5f;

        [SerializeField]
        private float _pushScale = 4f;

        [SerializeField]
        private float _pushInterval = 1.75f;

        [SerializeField]
        private Color _normalColor;

        [SerializeField]
        private Color _evolutionColor;

        private Sequence _sequence;

        private bool _pushAllowed;
        private float _pushCountdown;

        private void OnDisable() {
            _sequence?.Kill();
        }

        private void Update() {
            if (!_pushAllowed)
                return;

            _pushCountdown -= Time.deltaTime;
            if (_pushCountdown > 0)
                return;

            Collider2D[] enemies = Physics2D.OverlapCircleAll(Transform.position, _pushRadius, SRLayerMask.Enemy);
            List<EnemyController> enemiesInRadar = new List<EnemyController>();
            for (int i = 0; i < enemies.Length; i++) {
                EnemyController enemyController = enemies[i].GetComponent<EnemyController>();

                Vector2 delta = enemyController.PhysicsView.Rigidbody2D.position - (Vector2) Transform.parent.position;
                float distance = delta.magnitude;
                Vector2 newDirection = delta / distance;

                float scale = 1f - Mathf.Min(distance / _pushRadius, 1);
                DOTweenUtility.EvaluateEase(scale, Ease.InQuint);
                AttackData.KnockbackDirection = newDirection;
                AttackData.KnockbackScale = scale * _pushScale;
                enemyController.EventsListener.OnTouchedPlayerWeapon(enemyController, AttackData);
            }

            _pushCountdown = _pushInterval;
        }

        public void Set(Transform playerTransform, float lifetime, bool pushEnemies) {
            Transform.SetParent(playerTransform);
            Transform.localPosition = Vector3.zero;
            Transform.localScale = Vector3.one * 0.7f;

            _spriteRenderer.color = pushEnemies ? _evolutionColor : _normalColor;
            _spriteRenderer.color = _spriteRenderer.color.WithA(0);

            _sequence?.Kill();
            _sequence =
                DOTween.Sequence()
                .Insert(0, Transform.DOScale(1f, 0.25f))
                .Insert(0, _spriteRenderer.DOColor(_spriteRenderer.color.WithA(1), 0.25f))
                .Insert(lifetime - 0.25f, _spriteRenderer.DOColor(_spriteRenderer.color.WithA(0), 0.25f))
                .Insert(lifetime - 0.25f, Transform.DOScale(0.7f, 0.25f))
                .OnKill(() => _sequence = null);

            AttackData.KnockbackDirection = Vector2.zero;
            AttackData.KnockbackScale = _pushScale;

            _pushAllowed = pushEnemies;
            _pushCountdown = 0;
        }
    }
}
