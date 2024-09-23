using Sirenix.OdinInspector;
using UnityEngine;

namespace WerewolfBearer {
    public class EnemyDefinition : MonoBehaviour {
        public float BaseScale = 1;
        public float BaseHealth = 10; // 50
        public float BaseDamage = 10; // 20
        public float BaseMovementSpeed = 2;
        public float MovementSpeedVariationDelta = 0.2f;
        public float RotationSpeed = 3f;
        public float KnockbackScale = 1f;
        public float DamageCooldown = 0.3f;

        public GameObject ProjectilePrefab;

        [BoxGroup("Projectile", VisibleIf = "@ProjectilePrefab")]
        public float ProjectileBaseDamage = 6;

        [BoxGroup("Projectile", VisibleIf = "@ProjectilePrefab")]
        public float ProjectileMovementSpeed = 3;

        [BoxGroup("Projectile", VisibleIf = "@ProjectilePrefab")]
        public float ProjectileInterval = 2;

        [BoxGroup("Projectile", VisibleIf = "@ProjectilePrefab")]
        public float MinDistanceToPlayerForProjectileAttack = 20;

        [BoxGroup("Projectile", VisibleIf = "@ProjectilePrefab")]
        public float ProjectileDespawnTime = 5f;

        [BoxGroup("Projectile", VisibleIf = "@ProjectilePrefab")]
        public float ProjectileRotationSpeed = 0f;

        [BoxGroup("Projectile", VisibleIf = "@ProjectilePrefab")]
        public float ProjectileScale = 1f;

        public bool StaggeredMovement;
        public float StaggeredMovementDuration = 1f;

        public bool IsBoss;

        /*
         [Min(0)]
        public float RunAwayDistance;

        public bool RunsAway => RunAwayDistance != 0;

        [Min(0)]
        public float SelfDestructTimer;

        [BoxGroup("SelfDestruct", VisibleIf = "@HasSelfDestruct")]
        public float SelfDestructRadius = 5;

        public bool HasSelfDestruct => SelfDestructTimer != 0;
        */

        public bool HasCatchUp;
    }
}
