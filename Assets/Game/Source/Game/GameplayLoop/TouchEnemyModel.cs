using UnityEngine;

namespace WerewolfBearer {
    public class TouchEnemyModel {
        public float RandomBase;

        public Vector2 Target;
        public float CurrentAngle;
        public float DamageCooldown;
        public float ProjectileAttackCooldown;
        public float DeathTimer;

        public float Health;
        public float Damage;
        public float ProjectileDamage;
        public float RotationSpeed;
        public float MovementSpeed;

        public float StaggeredMovementTimer;

        public Vector2? KnockbackVelocity;

        public void Reset(EnemyDefinition enemyDefinition) {
            RandomBase = Random.value;

            DamageCooldown = 0;
            DeathTimer = 0;

            Target = Vector2.zero;
            CurrentAngle = 0;
            KnockbackVelocity = null;

            ProjectileAttackCooldown = enemyDefinition.ProjectileInterval;

            // Make sure enemies don't launch at full speed immediately when spawned
            StaggeredMovementTimer = enemyDefinition.StaggeredMovementDuration * 0.4f;
        }
    }
}
