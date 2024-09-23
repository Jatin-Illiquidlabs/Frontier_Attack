using UnityEngine;

namespace WerewolfBearer {
    public class ClockLancetProjectile : PlayerProjectile {
        [SerializeField]
        private float _speed = 5.0f;

        public void Set(float angle, float projectileSpeedMultiplier) {
            Transform.rotation = Quaternion.Euler(0, 0, angle);
            Vector3 direction = Transform.rotation * Vector3.up;
            Rigidbody2D.velocity = direction * projectileSpeedMultiplier * _speed;
            AttackData.KnockbackDirection = direction;
        }
    }
}
