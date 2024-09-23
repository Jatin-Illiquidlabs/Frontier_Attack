using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace WerewolfBearer {
    public class EnemyUpdateController : MonoBehaviour {
        [Inject]
        private GameplayPools _gameplayPools;

        [Inject]
        private GameStateModel _gameStateModel;

        [Inject]
        private PlayerCharacterModel _playerCharacterModel;

        private PlayerCharacterController _playerCharacterController;

        public void HandleNewGame(PlayerCharacterController playerCharacterController) {
            _playerCharacterController = playerCharacterController;
        }

        public void GameUpdate() {
            List<SpawnedEnemy> enemies = _gameStateModel.Enemies;
            bool isPlayerDead = _playerCharacterModel.Health.Value <= 0;
            foreach (SpawnedEnemy enemy in enemies) {
                UpdateEnemy(enemy.Controller, isPlayerDead);
            }
        }

        public void UpdateEnemyTarget(EnemyController enemyController, bool isPlayerDead, bool instant) {
            float angle = enemyController.Model.CurrentAngle;
            if (!isPlayerDead) {
                enemyController.Model.Target = _playerCharacterModel.Position.Value;
                Vector2 direction =
                    (enemyController.Model.Target - (Vector2) enemyController.VisualView.Transform.position).normalized;

                angle = Vector2.SignedAngle(Vector2.right, direction);
            }

            if (instant) {
                enemyController.Model.CurrentAngle = angle;
            } else {
                enemyController.Model.CurrentAngle =
                    Quaternion.Lerp(
                        Quaternion.Euler(0, 0, enemyController.Model.CurrentAngle),
                        Quaternion.Euler(0, 0, angle),
                        enemyController.Model.RotationSpeed * Time.deltaTime
                    ).eulerAngles.z;
            }
        }

        private void UpdateEnemy(EnemyController enemyController, bool isPlayerDead) {
            EnemyDefinition enemyDefinition = enemyController.EnemyDefinition;

            if (enemyController.Model.Health <= 0) {
                enemyController.Model.DeathTimer += Time.deltaTime;
                return;
            }

            // View
            UpdateEnemyTarget(enemyController, isPlayerDead, false);

            MoveEnemy(enemyController, enemyDefinition);

            // Logic
            enemyController.Model.DamageCooldown -= Time.deltaTime;

            FireProjectile(enemyController, enemyDefinition);
        }

        private void MoveEnemy(EnemyController enemyController, EnemyDefinition enemyDefinition) {
            float movementSpeedMagnitude;

            if (enemyDefinition.StaggeredMovement) {
                float lerp = Mathf.Clamp01(enemyController.Model.StaggeredMovementTimer / enemyDefinition.StaggeredMovementDuration);
                movementSpeedMagnitude = enemyController.Model.MovementSpeed * lerp;
                enemyController.Model.StaggeredMovementTimer -= Time.deltaTime;
                if (enemyController.Model.StaggeredMovementTimer <= 0) {
                    enemyController.Model.StaggeredMovementTimer = enemyDefinition.StaggeredMovementDuration;
                }
            } else {
                movementSpeedMagnitude = enemyController.Model.MovementSpeed;
            }

            if (enemyDefinition.HasCatchUp) {
                Vector2 enemyPosition = enemyController.VisualView.Transform.position;
                Vector2 vectorToPlayer = _playerCharacterModel.Position.Value - enemyPosition;
                float distanceToPlayerSqr = vectorToPlayer.sqrMagnitude;
                const float catchupDistance = 13f;
                if (distanceToPlayerSqr > catchupDistance * catchupDistance) {
                    movementSpeedMagnitude *= 5f;
                }
            }


            Vector2 movementSpeed =
                Quaternion.Euler(0, 0, enemyController.Model.CurrentAngle) * Vector3.right *
                movementSpeedMagnitude;

            Vector2 movementSpeedBeforeKnockback = movementSpeed;

            if (enemyController.Model.KnockbackVelocity != null) {
                if (enemyController.Model.KnockbackVelocity.Value.sqrMagnitude < 0.01f) {
                    enemyController.Model.KnockbackVelocity = null;
                } else {
                    const float knockbackScale = 8f;
                    const float knockbackLerp = 3f;
                    enemyController.Model.KnockbackVelocity = Vector2.Lerp(
                        enemyController.Model.KnockbackVelocity.Value,
                        Vector2.zero,
                        knockbackLerp * Time.deltaTime
                    );

                    movementSpeed += enemyController.Model.KnockbackVelocity.Value * knockbackScale;
                }
            }

            movementSpeed *= Time.deltaTime;

            enemyController.VisualView.Transform.position += (Vector3) movementSpeed;
            PushEnemyFromNeighbours(enemyController);
            enemyController.VisualView.UpdateView(movementSpeedBeforeKnockback.x > Vector2.kEpsilon);
        }

        private void FireProjectile(EnemyController enemyController, EnemyDefinition enemyDefinition) {
            if (enemyDefinition.ProjectilePrefab == null)
                return;

            enemyController.Model.ProjectileAttackCooldown -= Time.deltaTime;
            if (enemyController.Model.ProjectileAttackCooldown > 0)
                return;

            Vector2 enemyPosition = enemyController.PhysicsView.Rigidbody2D.position;
            Vector2 vectorToPlayer = _playerCharacterModel.Position.Value - enemyPosition;
            float distanceToPlayerSqr = vectorToPlayer.sqrMagnitude;
            float minDistanceSqr = enemyDefinition.MinDistanceToPlayerForProjectileAttack * enemyDefinition.MinDistanceToPlayerForProjectileAttack;
            if (distanceToPlayerSqr > minDistanceSqr)
                return;

            enemyController.Model.ProjectileAttackCooldown = enemyDefinition.ProjectileInterval;

            Vector2 direction = vectorToPlayer.normalized;
            Transform projectileTransform = ProjectileSpawnHelper.SpawnWeaponProjectile<Transform>(
                _gameplayPools,
                enemyDefinition.ProjectilePrefab,
                new ProjectileSpawnHelper.ProjectileSpawnOptions(enemyPosition, enemyController.Model.ProjectileDamage, 0) {
                    DespawnTime = enemyDefinition.ProjectileDespawnTime,
                    Pierce = 1,
                }
            );

            projectileTransform.localScale = Vector3.one * enemyDefinition.ProjectileScale;

            EnemyProjectileMovement enemyProjectileMovement = projectileTransform.GetComponent<EnemyProjectileMovement>();
            if (enemyProjectileMovement != null) {
                enemyProjectileMovement.Set(
                    direction,
                    _playerCharacterController.View.PhysicsTransform,
                    enemyDefinition.ProjectileMovementSpeed,
                    enemyDefinition.ProjectileRotationSpeed
                );
            }
        }

        private void PushEnemyFromNeighbours(EnemyController enemyController) {
            const float minDistanceFromPlayer = 20f;
            const float searchRadius = 2.5f;
            const float enemyRadius = 2f;
            const float pushVectorScale = 2f;
            if (Time.frameCount % 4 != 0)
                return;

            Vector2 enemyPosition = enemyController.PhysicsView.Rigidbody2D.position;
            Vector2 vectorToPlayer = _playerCharacterModel.Position.Value - enemyPosition;
            float distanceToPlayerSqr = vectorToPlayer.sqrMagnitude;
            if (distanceToPlayerSqr > minDistanceFromPlayer * minDistanceFromPlayer)
                return;

            ContactFilter2D enemyContactFilter = new ContactFilter2D().NoFilter();
            enemyContactFilter.SetLayerMask(SRLayerMask.Enemy);
            Collider2D[] enemyColliders = System.Buffers.ArrayPool<Collider2D>.Shared.Rent(4);
            int enemiesFound = Physics2D.OverlapCircle(
                enemyPosition,
                searchRadius,
                enemyContactFilter,
                enemyColliders
            );

            int counter = 0;
            Vector2 pushVector = Vector2.zero;
            for (int i = 0; i < enemiesFound; i++) {
                Collider2D enemyCollider = enemyColliders[i];
                if (enemyCollider.attachedRigidbody == enemyController.PhysicsView.Rigidbody2D)
                    continue;

                counter++;
                pushVector += enemyPosition - enemyCollider.attachedRigidbody.position;
            }

            if (counter == 0) {
                System.Buffers.ArrayPool<Collider2D>.Shared.Return(enemyColliders);
                return;
            }

            pushVector /= counter;
            float pushVectorMagnitude = pushVector.magnitude;
            pushVector /= pushVectorMagnitude;
            //pushVectorMagnitude = Mathf.Clamp(pushVectorMagnitude, enemyRadius * 0.25f, enemyRadius * 0.5f);
            pushVectorMagnitude = enemyRadius * pushVectorScale;
            pushVector *= pushVectorMagnitude;
            enemyController.VisualView.Transform.position += (Vector3) pushVector * Time.deltaTime;

            System.Buffers.ArrayPool<Collider2D>.Shared.Return(enemyColliders);
        }
    }
}
