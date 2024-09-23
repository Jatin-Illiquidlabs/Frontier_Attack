using UnityEngine;

namespace WerewolfBearer {
    public class CarrelloProjectile : PlayerProjectile {
        [SerializeField]
        private float _speed = 5.0f;

        [SerializeField]
        private float _rotateSpeed = 5.0f;

        [SerializeField]
        private Transform _spriteTransform;

        private void Update() {
            _spriteTransform.Rotate(0, 0, _rotateSpeed * Time.deltaTime, Space.Self);
        }

        public void Set(Vector2 direction, float areaMultiplier, float projectileSpeedMultiplier) {
            Transform.localScale = Vector3.one * areaMultiplier;
            Rigidbody2D.velocity = direction * projectileSpeedMultiplier * _speed;
            AttackData.KnockbackDirection = Rigidbody2D.velocity;
        }
    }
}
