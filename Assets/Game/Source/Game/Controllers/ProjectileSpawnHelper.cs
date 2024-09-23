using Lean.Pool;
using UnityEngine;

namespace WerewolfBearer {
    public static class ProjectileSpawnHelper {
        public static T SpawnWeaponProjectile<T>(GameplayPools gameplayPools, GameObject prefab, in ProjectileSpawnOptions spawnOptions) where T : Component {
            LeanGameObjectPool projectilePool = gameplayPools.GetGenericPool(prefab);
            return SpawnWeaponProjectile<T>(projectilePool, spawnOptions);
        }

        public static T SpawnWeaponProjectile<T>(LeanGameObjectPool projectilePool, in ProjectileSpawnOptions spawnOptions) where T : Component {
            GameObject weaponObj = projectilePool.Spawn(spawnOptions.Position, Quaternion.identity);
            AttackData attackData = weaponObj.GetComponent<AttackData>();
            attackData.Set(
                spawnOptions.Damage,
                spawnOptions.Pierce,
                spawnOptions.KnockbackScale
            );

            if (spawnOptions.DespawnTime != null) {
                projectilePool.Despawn(weaponObj, spawnOptions.DespawnTime.Value);
            }

            void AttackDataOnDestroyed() {
                attackData.Destroyed -= AttackDataOnDestroyed;

                if (weaponObj != null && weaponObj.activeInHierarchy) {
                    projectilePool.Despawn(weaponObj);
                }
            }

            attackData.Destroyed += AttackDataOnDestroyed;

            return weaponObj.GetComponent<T>();
        }

        public readonly struct ProjectileSpawnOptions {
            public Vector2 Position { get; }
            public float Damage { get; }
            public float KnockbackScale { get; }

            public int Pierce { get; init; }
            public float? DespawnTime { get; init; }

            public ProjectileSpawnOptions(Vector2 position, float damage, float knockbackScale) : this() {
                Position = position;
                Damage = damage;
                KnockbackScale = knockbackScale;
            }
        }
    }
}
