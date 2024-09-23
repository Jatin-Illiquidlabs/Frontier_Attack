using UnityEngine;

namespace WerewolfBearer {
    public class StraightProjectile : PlayerProjectile {
        [SerializeField]
        private float _speed = 5.0f;

        public void Set(Vector2 direction, float projectileSpeedMultiplier) {
            Rigidbody2D.velocity = direction * projectileSpeedMultiplier * _speed;
            AttackData.KnockbackDirection = Rigidbody2D.velocity;
        }
    }
}
