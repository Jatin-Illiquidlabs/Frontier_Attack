using UnityEngine;

namespace WerewolfBearer {
    public class CrossProjectile : PlayerProjectile {
        [SerializeField]
        private float _rotationSpeed = 750f;

        [SerializeField]
        private float _movementSpeed = 7.5f;

        [SerializeField]
        private float _projectileReturnTime = 0.4f;

        private Vector3 _direction;

        private float _returnTimer;

        private void Update() {
            Transform.Rotate(_rotationSpeed * Time.deltaTime * new Vector3(0, 0, -1));
        }

        private void FixedUpdate() {
            if (_returnTimer <= _projectileReturnTime) {
                _returnTimer += Time.deltaTime;
                Rigidbody2D.velocity = Vector3.Lerp(Rigidbody2D.velocity, _direction * _movementSpeed, 0.02f);
            }
            else {
                Rigidbody2D.velocity = -_direction * _movementSpeed;
            }

            AttackData.KnockbackDirection = Rigidbody2D.velocity;
        }

        public void Set(Vector3 targetPosition, float projectileSpeedMultiplier) {
            _direction = (targetPosition - Transform.position).normalized;
            Rigidbody2D.velocity = _direction * projectileSpeedMultiplier * _movementSpeed;
            AttackData.KnockbackDirection = Rigidbody2D.velocity;
        }
    }
}
