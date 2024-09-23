using UnityEngine;

namespace WerewolfBearer {
    public class AxeProjectile : PlayerProjectile {
        [SerializeField]
        private float _rotationSpeed = 750f;

        [SerializeField]
        private float _movementSpeed = 12.5f;

        private void Update() {
            Transform.Rotate(_rotationSpeed * Time.deltaTime * new Vector3(0, 0, -1));
            AttackData.KnockbackDirection = Rigidbody2D.velocity.normalized;
        }

        public void Set(Vector3 direction, float projectileSpeedMultiplier) {
            Transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            Rigidbody2D.velocity = direction * _movementSpeed * projectileSpeedMultiplier;
            AttackData.KnockbackDirection = direction;
        }
    }
}
