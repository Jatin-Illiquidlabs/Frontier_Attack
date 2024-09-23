using DG.Tweening;
using UniRx;
using UnityEngine;
using Zenject;

namespace WerewolfBearer {
    public class PlayerCharacterView : MonoBehaviour {
        [SerializeField]
        private Transform _transform;

        [SerializeField]
        private Transform _spriteTransform;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private Transform _physicsTransform;

        [Inject]
        private GameplayPools _gameplayPools;

        [Inject]
        private PlayerCharacterModel _model;

        private float _movementTimer;

        public Transform Transform => _transform;
        public SpriteRenderer SpriteRenderer => _spriteRenderer;
        public Transform PhysicsTransform => _physicsTransform;

        private DisposablesList _subscriptions = new();

        public void Initialize() {
            _subscriptions.Dispose();
            _subscriptions +=
                _model.IsDead
                    .TakeUntilDisable(this)
                    .Skip(1)
                    .Subscribe(isDead => {
                        if (isDead) {
                            SpriteRenderer.color = Color.white.WithA(0);
                            SpriteRenderer.DOColor(Color.red.WithA(0.5f), 0.5f);
                        } else {
                            SpriteRenderer.DOColor(Color.white.WithA(0), 0.5f);
                        }
                    });
        }

        public void UpdateView(bool faceRight, bool isMoving) {
            _spriteRenderer.flipX = !faceRight;
            Vector3 physicsTransformScale = _physicsTransform.localScale;
            physicsTransformScale.x =
                faceRight ?
                    Mathf.Abs(physicsTransformScale.x) :
                    -Mathf.Abs(physicsTransformScale.x);

            if (!isMoving) {
                Transform.rotation =
                    Quaternion.Lerp(Transform.rotation, Quaternion.identity, 10f * Time.deltaTime);
            } else {
                _movementTimer += Time.deltaTime;

                Transform.rotation =
                    CharacterUtility.UpdateBobbingRotation(
                        _movementTimer,
                        3f,
                        2f * _model.MovementSpeedMultiplier.Value
                    );
            }
        }

        public void PlayDamageAnimation() {
            _spriteRenderer.color = Color.red;
            _spriteRenderer.DOColor(Color.white.WithA(0), 0.5f);

            GameObject bloodParticles = _gameplayPools.BloodParticles.Spawn(Transform.position, Quaternion.identity);
            _gameplayPools.BloodParticles.Despawn(bloodParticles, 2f);
        }
    }
}
