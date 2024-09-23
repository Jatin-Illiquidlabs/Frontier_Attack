using System;
using System.Collections.Generic;
using System.Linq;
using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace WerewolfBearer {
    public class EnemySpawner : MonoBehaviour {
        private const float EnemySpawnViewportScale = 1.2f;
        private const float EnemySpawnBossViewportScale = 5f;

        [SerializeField]
        private float _bossSpawnPeriod = 120f;

        [SerializeField, Required]
        private EnemyEventsHandler _enemyEventsHandler;

        [SerializeField, Required]
        private PickupItemSpawner _pickupItemSpawner;

        [SerializeField, Required]
        private EnemyUpdateController _enemyUpdateController;

        [SerializeField]
        private GameObject _debugEnemyPrefab;

        [Inject]
        private GameplayPools _gameplayPools;

        [Inject]
        private PlayerCharacterModel _playerCharacterModel;

        [Inject]
        private GameStateModel _gameStateModel;

        [Inject]
        private GameStartData _gameStartData;

        [Inject]
        private ToastView _toastView;

        private StageDefinition _stageDefinition;
        private float _enemySpawnCooldown;
        private float _enemyBossSpawnCooldown;

        private Dictionary<EnemyDefinition, LeanGameObjectPool> _allEnemyDefinitionToPool = new();
        private Dictionary<EnemyDefinition, LeanGameObjectPool> _normalEnemyDefinitionToPool;
        private Dictionary<EnemyDefinition, LeanGameObjectPool> _bossEnemyDefinitionToPool;

        private List<EnemyDefinition> _normalEnemyDefinitions;
        private List<EnemyDefinition> _bossEnemyDefinitions;

        private WeightedRandom<EnemyDefinition> _normalEnemyWeightedRandom;
        private WeightedRandom<EnemyDefinition> _bossEnemyWeightedRandom;

        public void HandleNewGame() {
            _gameStateModel.Enemies.Clear();

            foreach (LeanGameObjectPool enemyPool in _allEnemyDefinitionToPool.Values) {
                enemyPool.DespawnAll();
                Destroy(enemyPool.gameObject);
            }

            _allEnemyDefinitionToPool.Clear();

            _stageDefinition = StageDatabase.Instance.GetStageById(_gameStartData.StageId);

            List<(float weight, EnemyDefinition enemyDefinition)> normalEnemiesWeighted = new();
            List<(float weight, EnemyDefinition enemyDefinition)> bossEnemiesWeighted = new();
            StageDefinition.EnemySpawnData[] allEnemies = _stageDefinition.Enemies.Concat(_stageDefinition.Bosses).ToArray();
            foreach (StageDefinition.EnemySpawnData enemySpawn in allEnemies) {
                EnemyDefinition enemyDefinition = enemySpawn.EnemyPrefab.GetComponent<EnemyDefinition>();
                _allEnemyDefinitionToPool[enemyDefinition] = _gameplayPools.CreatePool(enemySpawn.EnemyPrefab);

                if (enemyDefinition.IsBoss) {
                    bossEnemiesWeighted.Add((enemySpawn.SpawnWeight, enemyDefinition));
                } else {
                    normalEnemiesWeighted.Add((enemySpawn.SpawnWeight, enemyDefinition));
                }
            }

            _normalEnemyDefinitionToPool =
                _allEnemyDefinitionToPool
                    .Where(kv => !kv.Key.IsBoss)
                    .ToDictionary(p => p.Key, p => p.Value);

            _bossEnemyDefinitionToPool =
                _allEnemyDefinitionToPool
                    .Where(kv => kv.Key.IsBoss)
                    .ToDictionary(p => p.Key, p => p.Value);

            _normalEnemyDefinitions =
                _allEnemyDefinitionToPool
                    .Where(kv => !kv.Key.IsBoss)
                    .Select(kv => kv.Key)
                    .ToList();

            _bossEnemyDefinitions =
                _allEnemyDefinitionToPool
                    .Where(kv => kv.Key.IsBoss)
                    .Select(kv => kv.Key)
                    .ToList();

            _normalEnemyWeightedRandom = new WeightedRandom<EnemyDefinition>(normalEnemiesWeighted);
            _bossEnemyWeightedRandom = new WeightedRandom<EnemyDefinition>(bossEnemiesWeighted);

            _enemyBossSpawnCooldown = _bossSpawnPeriod;
            _enemySpawnCooldown = 5f;
        }

        public void GameUpdate() {
            // Clear dead enemies
            List<SpawnedEnemy> enemies = _gameStateModel.Enemies;
            for (int i = enemies.Count - 1; i >= 0; i--) {
                SpawnedEnemy enemy = enemies[i];
                if (enemy.Controller.Model.DeathTimer > 2) {
                    enemies.RemoveAt(i);
                    enemy.Pool.Despawn(enemy.GameObject);
                }
            }

#if DEBUG
            if (_debugEnemyPrefab != null && enemies.Count > 0)
                return;

            if (_debugEnemyPrefab != null) {
                foreach (LeanGameObjectPool pool in _allEnemyDefinitionToPool.Values) {
                    pool.Clean();
                }

                _gameplayPools.GetGenericPool(_debugEnemyPrefab).Clean();
            }
#endif

            // Spawn Normal Enemy
            _enemySpawnCooldown -= Time.deltaTime;
            if (_enemySpawnCooldown <= 0) {
                SpawnedEnemy spawnedEnemy = SpawnEnemy(false);
                _gameStateModel.Enemies.Add(spawnedEnemy);
                //_enemySpawnCooldown = 3f - Mathf.Min(2.75f, _playerCharacterModel.Level.Value * 0.12f);
                _enemySpawnCooldown =
                    Mathf.Max(
                        _stageDefinition.EnemySpawnIntervalMin,
                        (_stageDefinition.EnemySpawnIntervalBase - _playerCharacterModel.Level.Value * _stageDefinition.EnemySpawnIntervalDecreasePerPlayerLevel) /
                        _playerCharacterModel.EnemyBuffMultiplier.Value
                    );
            }

            // Spawn Boss Enemy
            if (!_gameStateModel.IsBossFight) {
                _enemyBossSpawnCooldown -= Time.deltaTime;
                if (_enemyBossSpawnCooldown <= 0) {
                    _gameStateModel.IsBossFight = true;
                    _toastView.ToastAppearWithMessage("Boss approaching!");
                    SpawnedEnemy spawnedEnemy = SpawnEnemy(true);
                    _gameStateModel.Enemies.Add(spawnedEnemy);
                    _enemyBossSpawnCooldown = _bossSpawnPeriod;
                }
            }
        }

        private SpawnedEnemy SpawnEnemy(bool isBoss) {
#if DEBUG
            if (_debugEnemyPrefab != null) {
                isBoss = false;
            }
#endif

            Camera camera = Camera.main;
            Vector2 cameraViewportSize = CameraUtility.GetCameraViewportSize(camera);

            // Spawn bosses farther so they approach for longer
            cameraViewportSize *= isBoss ? EnemySpawnBossViewportScale : EnemySpawnViewportScale;

            Rect spawnRect = new(
                (Vector2) camera.transform.position - (cameraViewportSize * 0.5f),
                cameraViewportSize
            );

            // TODO: For uniform distribution along the edge of the rectangle,
            // calculate perimeter, pick a random value on that length,
            // and then detect which side was that.

            Vector2 spawnNormalizedPositionStart;
            Vector2 spawnNormalizedPositionEnd;
            int side = Random.Range(0, 4);
            switch (side) {
                case 0:
                    spawnNormalizedPositionStart = new Vector2(0, 1);
                    spawnNormalizedPositionEnd = new Vector2(1, 1);
                    break;
                case 1:
                    spawnNormalizedPositionStart = new Vector2(1, 1);
                    spawnNormalizedPositionEnd = new Vector2(1, 0);
                    break;
                case 2:
                    spawnNormalizedPositionStart = new Vector2(1, 0);
                    spawnNormalizedPositionEnd = new Vector2(0, 0);
                    break;
                case 3:
                    spawnNormalizedPositionStart = new Vector2(0, 0);
                    spawnNormalizedPositionEnd = new Vector2(0, 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Vector2 spawnPosition = Rect.NormalizedToPoint(
                spawnRect,
                Vector2.Lerp(
                    spawnNormalizedPositionStart,
                    spawnNormalizedPositionEnd,
                    Random.value
                )
            );

            EnemyDefinition enemyDefinition;
            LeanGameObjectPool enemyPool;
            if (_debugEnemyPrefab != null) {
                enemyPool = _gameplayPools.GetGenericPool(_debugEnemyPrefab);
                enemyDefinition = _debugEnemyPrefab.GetComponent<EnemyDefinition>();
            } else {
                enemyDefinition =
                    isBoss ?
                        _bossEnemyWeightedRandom.GetRandomItem() :
                        _normalEnemyWeightedRandom.GetRandomItem();
                enemyPool = _allEnemyDefinitionToPool[enemyDefinition];
            }

            GameObject enemyGo = enemyPool.Spawn(spawnPosition, Quaternion.identity);
            EnemyController enemyController = enemyGo.GetComponent<EnemyController>();
            enemyController.Model.Reset(enemyDefinition);

            enemyController.Model.Damage = enemyDefinition.BaseDamage * _playerCharacterModel.EnemyBuffMultiplier.Value;
            enemyController.Model.ProjectileDamage = enemyDefinition.ProjectileBaseDamage * _playerCharacterModel.EnemyBuffMultiplier.Value;
            enemyController.Model.Health = enemyDefinition.BaseHealth * _playerCharacterModel.EnemyBuffMultiplier.Value;
            enemyController.Model.MovementSpeed =
                enemyDefinition.BaseMovementSpeed +
                Random.Range(-enemyDefinition.MovementSpeedVariationDelta, enemyDefinition.MovementSpeedVariationDelta);
            enemyController.Model.RotationSpeed = enemyDefinition.RotationSpeed;
            enemyController.VisualView.Transform.localScale = Vector3.one * enemyDefinition.BaseScale;
            enemyController.Destroyed = () => {
                _pickupItemSpawner.SpawnRandomPickupItems(enemyController.transform.position);
                if (isBoss) {
                    _gameStateModel.IsBossFight = false;
                }
            };

            enemyController.EventsListener = _enemyEventsHandler;

            _enemyUpdateController.UpdateEnemyTarget(enemyController, false, true);
            return new SpawnedEnemy(enemyGo, enemyController, enemyPool);
        }
    }
}
