using UnityEngine;

namespace WerewolfBearer {
    public class KnifeProjectile : PlayerProjectile {
        [SerializeField]
        private float _speed = 5.0f;

        public void Set(float projectileSpeedMultiplier) {
            Transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            Rigidbody2D.velocity = projectileSpeedMultiplier * _speed * transform.up;
            AttackData.KnockbackDirection = Rigidbody2D.velocity;
        }
    }
}
