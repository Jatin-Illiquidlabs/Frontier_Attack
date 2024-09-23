using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WerewolfBearer {

    public partial class PlayerCharacterController {
        private Dictionary<WeaponId, Action<WeaponStateModel>> CreateWeaponAttackActions() {
            return new Dictionary<WeaponId, Action<WeaponStateModel>> {
                { WeaponId.Whip, SpawnWhipWeapon },
                { WeaponId.MagicWand, SpawnMagicWandWeapon },
                { WeaponId.Knife, SpawnKnifeWeapon },
                { WeaponId.Axe, wsb => SpawnAxe(wsb, 1) },
                { WeaponId.Cross, SpawnCross },
                { WeaponId.KingBible, SpawnKingBible },
                { WeaponId.FireWand, SpawnFireWand },
                { WeaponId.Garlic, SpawnGarlic },
                { WeaponId.SantaWater, SpawnSantaWater },
                { WeaponId.Runetracer, SpawnRunetracer },
                { WeaponId.LightningRing, SpawnLightingRing },
                { WeaponId.ClockLancet, wsm => SpawnClockLancetWeapon(wsm, false) },
                { WeaponId.Laurel, wsm => SpawnLaurelWeapon(wsm, false) },
                { WeaponId.Carrello, SpawnCarrelloWeapon },
                { WeaponId.FlamesOfMisspell, wsm => SpawnFlamesOfMisspellWeapon(wsm, false) },

                { WeaponId.BloodyTear, SpawnWhipWeapon },
                { WeaponId.HolyWand, SpawnMagicWandWeapon },
                { WeaponId.HellFire, SpawnFireWand },
                { WeaponId.NoFuture, SpawnRunetracer },
                { WeaponId.AshesOfMuspell, wsm => SpawnFlamesOfMisspellWeapon(wsm, true) },
                { WeaponId.DeathSpiral, SpawnDeathSpiralWeapon },
                { WeaponId.ThunderLoop, SpawnLightingRing },

                { WeaponId.XKnife_BigAndSlow, SpawnKnifeWeapon },
                { WeaponId.XKingBible_BiggerRadius, SpawnKingBible },
                { WeaponId.XAxe_Micro, wsb => SpawnAxe(wsb, 5) },
                { WeaponId.XCornerShot, SpawnXCornerShotWeapon },

                { WeaponId.ClockLancetEvo, wsm => SpawnClockLancetWeapon(wsm, true) },
                { WeaponId.LaurelEvo, wsm => SpawnLaurelWeapon(wsm, true) },
                { WeaponId.CrossEvo, SpawnCross },
                { WeaponId.KingBibleEvo, SpawnKingBible },

                { WeaponId.XKnifeAxe, SpawnXKnifeAxeWeapon },
            };
        }

        private void FireWeapons() {
            foreach (WeaponStateModel weaponStateModel in _model.Weapons) {
                weaponStateModel.Update(Time.deltaTime);
                if (weaponStateModel.CooldownTimer < weaponStateModel.Cooldown * _model.WeaponCooldownMultiplier.Value) {
                    continue;
                }

                Action<WeaponStateModel> weaponAttackAction = _weaponAttackActions[weaponStateModel.Definition.Id];
                weaponStateModel.CooldownTimer = 0;
                weaponStateModel.IncrementFireCounter();
                weaponAttackAction.Invoke(weaponStateModel);
            }
        }

        private int CalculateTotalProjectileCount(WeaponStateModel weaponStateModel)
            => 1 + _model.ExtraProjectilesPerAttack.Value + weaponStateModel.ExtraProjectilesPerAttack;

        private float CalculateTotalAttackArea(WeaponStateModel weaponStateModel)
            => _model.AttackAreaMultiplier.Value * weaponStateModel.Area;

        private void SpawnWhipWeapon(WeaponStateModel weaponStateModel) {
            Vector3 scale = _model.FaceRight ?
                Vector3.one :
                new Vector3(-1, 1, 1);

            int projectileCount = CalculateTotalProjectileCount(weaponStateModel);
            for (int i = 0; i < projectileCount; i++) {
                Transform swordAttackTransform = SpawnWeaponProjectile<Transform>(weaponStateModel);

                swordAttackTransform.localScale = scale;
                scale.x *= -1f;

                AttackData attackData = swordAttackTransform.GetComponent<AttackData>();
                attackData.KnockbackDirection = _model.FaceRight ? Vector2.right : Vector2.left;
            }
        }

        private void SpawnXCornerShotWeapon(WeaponStateModel weaponStateModel) {
            int projectileCount = CalculateTotalProjectileCount(weaponStateModel);
            Vector2 cameraViewportSize = CameraUtility.GetCameraViewportSize(Camera.main);

            float CalculateAngleForOffset(Vector2 offset) {
                offset.x *= cameraViewportSize.x;
                offset.y *= cameraViewportSize.y;
                float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg - 180 - 90;
                return angle;
            }

            void SpawnForOffset(Vector2 offset) {
                float baseAngle = CalculateAngleForOffset(offset);
                for (int i = 0; i < projectileCount; i++) {
                    Vector2 movementDirection = Quaternion.Euler(0, 0, baseAngle + Random.Range(-1f, 1f) * 10f) * Vector3.up;
                    StraightProjectile straightProjectile = SpawnWeaponProjectile<StraightProjectile>(weaponStateModel);
                    straightProjectile.Set(movementDirection, _model.ProjectileSpeedMultiplier.Value);
                }
            }

            SpawnForOffset(new Vector2(-1f, -1f));
            SpawnForOffset(new Vector2(-1f, 1f));
            SpawnForOffset(new Vector2(1f, 1f));
            SpawnForOffset(new Vector2(1f, -1f));
        }

        private void SpawnFlamesOfMisspellWeapon(WeaponStateModel weaponStateModel, bool isEvolution) {
            float baseAngle = Mathf.Atan2(_model.MoveDirection.Value.y, _model.MoveDirection.Value.x) * Mathf.Rad2Deg - 90f;

            const float spreadDegrees = 20f;
            const int count = 4;

            for (int i = 0; i < count; i++) {
                FlamesOfMisspellProjectile projectile = SpawnWeaponProjectile<FlamesOfMisspellProjectile>(weaponStateModel);
                projectile.Set(
                    Quaternion.Euler(0, 0, baseAngle + Random.Range(-spreadDegrees, spreadDegrees)) * Vector2.up,
                    _model.ProjectileSpeedMultiplier.Value + Random.Range(-1, 1f) * 0.1f,
                    Color.white
                );
            }

            if (isEvolution) {
                for (int i = 0; i < count; i++) {
                    FlamesOfMisspellProjectile projectile = SpawnWeaponProjectile<FlamesOfMisspellProjectile>(weaponStateModel);
                    projectile.Set(
                        Quaternion.Euler(0, 0, baseAngle + 180f + Random.Range(-spreadDegrees, spreadDegrees)) * Vector2.up,
                        _model.ProjectileSpeedMultiplier.Value + Random.Range(-1, 1f) * 0.1f,
                        Color.cyan
                    );
                }
            }
        }

        private void SpawnXKnifeAxeWeapon(WeaponStateModel weaponStateModel) {
            int projectileCount = CalculateTotalProjectileCount(weaponStateModel) * 2;
            float randomOffset = 0.02f * projectileCount;
            for (int i = 0; i < projectileCount; i++) {
                AxeProjectile axeProjectile = SpawnWeaponProjectile<AxeProjectile>(weaponStateModel);
                Vector2 direction = new Vector2(-1 + Random.Range(-0.1f, 0.1f), 0.8f + Random.Range(-0.2f, 0.2f));
                if (_model.FaceRight) {
                    direction.x *= -1;
                }

                axeProjectile.Set(
                    direction,
                    _model.ProjectileSpeedMultiplier.Value
                );
            }
        }

        private void SpawnKnifeWeapon(WeaponStateModel weaponStateModel) {
            int projectileCount = CalculateTotalProjectileCount(weaponStateModel);
            for (int i = 0; i < projectileCount; i++) {
                KnifeProjectile knifeProjectile = SpawnWeaponProjectile<KnifeProjectile>(weaponStateModel);
                knifeProjectile.Set(_model.ProjectileSpeedMultiplier.Value);
            }
        }

        private void SpawnLaurelWeapon(WeaponStateModel weaponStateModel, bool pushEnemies) {
            float shieldDuration = weaponStateModel.ProjectileLifetime;
            LaurelProjectile laurelProjectile = SpawnWeaponProjectile<LaurelProjectile>(weaponStateModel);
            laurelProjectile.Set(_transform, shieldDuration, pushEnemies);

            _model.ShieldCountdownTimer = shieldDuration;
        }

        private void SpawnCarrelloWeapon(WeaponStateModel weaponStateModel) {
            Vector2 direction = _model.FaceRight ? Vector2.right : Vector2.left;
            CarrelloProjectile carrelloProjectile = SpawnWeaponProjectile<CarrelloProjectile>(weaponStateModel);
            carrelloProjectile.Set(direction, CalculateTotalAttackArea(weaponStateModel), _model.ProjectileSpeedMultiplier.Value);
        }

        private void SpawnClockLancetWeapon(WeaponStateModel weaponStateModel, bool isEvolution) {
            int projectileCount = CalculateTotalProjectileCount(weaponStateModel);
            float baseAngleStep = 360f / projectileCount;
            const float oneTwelfthCircle = 360f / 12f;
            for (int i = 0; i < projectileCount; i++) {
                float baseAngle = baseAngleStep * i;
                int count = weaponStateModel.FireCounter % 12;
                float angle = baseAngle + count * oneTwelfthCircle;

                if (!isEvolution) {
                    ClockLancetProjectile clockLancetProjectile = SpawnWeaponProjectile<ClockLancetProjectile>(weaponStateModel);
                    clockLancetProjectile.Set(angle, _model.ProjectileSpeedMultiplier.Value);
                } else {
                    ClockLancetProjectile clockLancetProjectile1 = SpawnWeaponProjectile<ClockLancetProjectile>(weaponStateModel);
                    clockLancetProjectile1.Set(angle - 5f, _model.ProjectileSpeedMultiplier.Value);

                    ClockLancetProjectile clockLancetProjectile2 = SpawnWeaponProjectile<ClockLancetProjectile>(weaponStateModel);
                    clockLancetProjectile2.Set(angle + 5f, _model.ProjectileSpeedMultiplier.Value);
                }
            }
        }

        private void SpawnDeathSpiralWeapon(WeaponStateModel weaponStateModel) {
            const float oneNinenthCircle = 360f / 9f;
            for (int i = 0; i < 9; i++) {
                float angle = oneNinenthCircle * i;
                AxeProjectile axeProjectile = SpawnWeaponProjectile<AxeProjectile>(weaponStateModel);

                Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.up;
                axeProjectile.Set(direction, _model.ProjectileSpeedMultiplier.Value);
            }
        }

        private void SpawnMagicWandWeapon(WeaponStateModel weaponStateModel) {
            int projectileCount = CalculateTotalProjectileCount(weaponStateModel);
            for (int i = 0; i < projectileCount; i++) {
                Vector3 sourcePos = (Vector2) _transform.position + (new Vector2(UnityEngine.Random.Range(-3.0f, 3.0f),
                    UnityEngine.Random.Range(-3.0f, 3.0f)) * (i > 1 ? 1 : i));
                EnemyController nearestEnemy = _gameplayController.FindEnemyInRange(sourcePos, 30, true);
                if (nearestEnemy == null)
                    return;

                StraightProjectile straightProjectile = SpawnWeaponProjectile<StraightProjectile>(weaponStateModel);
                Vector3 direction = (nearestEnemy.PhysicsView.Rigidbody2D.position - (Vector2) transform.position).normalized;
                straightProjectile.Set(direction, _model.ProjectileSpeedMultiplier.Value);
            }
        }

        private void SpawnAxe(WeaponStateModel weaponStateModel, int projectilesPerBurst) {
            int projectileCount = CalculateTotalProjectileCount(weaponStateModel) * projectilesPerBurst;
            float randomOffset = 0.1f * projectileCount;
            for (int i = 0; i < projectileCount; i++) {
                AxeProjectile axeProjectile = SpawnWeaponProjectile<AxeProjectile>(weaponStateModel);
                axeProjectile.Set(
                    _model.FaceRight ?
                        new Vector2(0.05f + randomOffset * Random.value, 1) :
                        new Vector2(-0.05f - randomOffset * Random.value, 1),
                    _model.ProjectileSpeedMultiplier.Value
                );
            }
        }

        private void SpawnCross(WeaponStateModel weaponStateModel) {
            int projectileCount = CalculateTotalProjectileCount(weaponStateModel);
            for (int i = 0; i < projectileCount; i++) {
                Vector3 sourcePos = (Vector2) _transform.position + (new Vector2(UnityEngine.Random.Range(-3.0f, 3.0f),
                    UnityEngine.Random.Range(-3.0f, 3.0f)) * (i > 1 ? 1 : i));
                EnemyController nearestEnemy = _gameplayController.FindEnemyInRange(sourcePos, 30, true);
                if (nearestEnemy == null)
                    return;

                CrossProjectile crossProjectile = SpawnWeaponProjectile<CrossProjectile>(weaponStateModel);
                crossProjectile.Set(nearestEnemy.transform.position, _model.ProjectileSpeedMultiplier.Value);
            }
        }

        private void SpawnKingBible(WeaponStateModel weaponStateModel) {
            float baseAngle = Random.value * 360f;
            int projectileCount = CalculateTotalProjectileCount(weaponStateModel);
            float angleStep = 360f / projectileCount;
            for (int i = 0; i < projectileCount; i++) {
                float angle = baseAngle + i * angleStep;
                KingBibleProjectile kingBibleProjectile = SpawnWeaponProjectile<KingBibleProjectile>(weaponStateModel);
                kingBibleProjectile.Set(baseAngle, _transform, _model.ProjectileSpeedMultiplier.Value);
            }
        }

        private void SpawnGarlic(WeaponStateModel weapon) {
            GarlicProjectile garlicProjectile = SpawnWeaponProjectile<GarlicProjectile>(weapon);
            garlicProjectile.Set(_transform, _model.AttackAreaMultiplier.Value);
        }

        private void SpawnFireWand(WeaponStateModel weaponStateModel) {
            int projectileCount = CalculateTotalProjectileCount(weaponStateModel);
            for (int i = 0; i < projectileCount; i++) {
                Vector3 sourcePos = (Vector2) _transform.position + (new Vector2(UnityEngine.Random.Range(-3.0f, 3.0f),
                    UnityEngine.Random.Range(-3.0f, 3.0f)) * (i > 1 ? 1 : i));
                EnemyController nearestEnemy = _gameplayController.FindEnemyInRange(sourcePos, 30, true);
                if (nearestEnemy == null)
                    return;

                Vector2 direction = (nearestEnemy.PhysicsView.Rigidbody2D.position - (Vector2) transform.position).normalized;
                StraightProjectile straightProjectile = SpawnWeaponProjectile<StraightProjectile>(weaponStateModel);
                straightProjectile.Set(direction, _model.ProjectileSpeedMultiplier.Value);
            }
        }

        private void SpawnSantaWater(WeaponStateModel weaponStateModel) {
            int projectileCount = CalculateTotalProjectileCount(weaponStateModel);
            for (int i = 0; i < projectileCount; i++) {
                Vector3 sourcePos = (Vector2) _transform.position + (new Vector2(UnityEngine.Random.Range(-3.0f, 3.0f),
                    Random.Range(-3.0f, 3.0f)) * (i > 1 ? 1 : i));

                EnemyController nearestEnemy = _gameplayController.FindEnemyInRange(sourcePos, 10, true);
                if (nearestEnemy == null)
                    return;

                SantaWaterProjectile santaWaterProjectile = SpawnWeaponProjectile<SantaWaterProjectile>(weaponStateModel);
                santaWaterProjectile.gameObject.transform.position = _transform.position + Vector3.up * 10f;
                santaWaterProjectile.Set(nearestEnemy.transform.position, _model.AttackAreaMultiplier.Value);
            }
        }

        private void SpawnLightingRing(WeaponStateModel weaponStateModel) {
            int projectileCount = CalculateTotalProjectileCount(weaponStateModel);
            for (int i = 0; i < projectileCount; i++) {
                Vector3 sourcePos = (Vector2) _transform.position + (new Vector2(UnityEngine.Random.Range(-3.0f, 3.0f),
                    UnityEngine.Random.Range(-3.0f, 3.0f)) * (i > 1 ? 1 : i));

                EnemyController nearestEnemy = _gameplayController.FindEnemyInRange(sourcePos, 10, true);
                if (nearestEnemy == null)
                    return;

                LightningRingProjectile lightningRingProjectile = SpawnWeaponProjectile<LightningRingProjectile>(weaponStateModel);
                lightningRingProjectile.gameObject.transform.position = nearestEnemy.transform.position;

                lightningRingProjectile.Set(_model.AttackAreaMultiplier.Value);
            }
        }

        private void SpawnRunetracer(WeaponStateModel weaponStateModel) {
            int projectileCount = CalculateTotalProjectileCount(weaponStateModel);
            for (int i = 0; i < projectileCount; i++) {
                Vector3 sourcePos = (Vector2) _transform.position + (new Vector2(UnityEngine.Random.Range(-3.0f, 3.0f),
                    UnityEngine.Random.Range(-3.0f, 3.0f)) * (i > 1 ? 1 : i));

                EnemyController nearestEnemy = _gameplayController.FindEnemyInRange(sourcePos, 30, true);
                if (nearestEnemy == null)
                    return;

                Vector2 direction = (nearestEnemy.PhysicsView.Rigidbody2D.position - (Vector2) transform.position).normalized;
                RunetracerProjectile runetracerProjectile = SpawnWeaponProjectile<RunetracerProjectile>(weaponStateModel);

                runetracerProjectile.Set(direction, Camera.main, _model.ProjectileSpeedMultiplier.Value);
            }
        }

        private T SpawnWeaponProjectile<T>(WeaponStateModel weapon, bool despawn = true) where T : Component {
            return
                ProjectileSpawnHelper.SpawnWeaponProjectile<T>(
                    _gameplayPools,
                    weapon.Definition.WeaponPrefab,
                    new ProjectileSpawnHelper.ProjectileSpawnOptions(_swordAttackSpawnPoint.position, weapon.Damage, weapon.Definition.KnockbackScale) {
                        DespawnTime = despawn ? weapon.ProjectileLifetime * _model.AttackDurationMultiplier.Value : null,
                        Pierce = weapon.Definition.ProjectilePierce
                    }
                );
        }
    }
}
