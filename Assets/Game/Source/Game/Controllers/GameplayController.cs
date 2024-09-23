using DG.Tweening;
using UniRx;
using UnityEngine;
using Zenject;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

namespace WerewolfBearer {
    public class GameplayController : MonoBehaviour {
        private const float NextLevelUnlockTimeSeconds = 60f * 30f;

        [SerializeField]
        private Transform _sceneRoot;

        [SerializeField]
        private CameraFollow2D _cameraFollow;

        [SerializeField, Required]
        private EnemyEventsHandler _enemyEventsHandler;

        [SerializeField, Required]
        private EnemySpawner _enemySpawner;

        [SerializeField, Required]
        private PickupItemSpawner _pickupItemSpawner;

        [SerializeField, Required]
        private EnemyUpdateController _enemyUpdateController;

        [Inject]
        private GameplayPools _gameplayPools;

        [Inject]
        private ToastView _toastView;

        [Inject]
        private GameStateModel _gameStateModel;

        [Inject]
        private GameStartData _gameStartData;

        [Inject]
        private PlayerCharacterModel _playerCharacterModel;

        [Inject]
        private GamePreferencesRepository _gamePreferencesRepository;

        private PlayerCharacterController _playerCharacterController;

        public PlayerCharacterController PlayerCharacterController => _playerCharacterController;

        private void OnEnable() {
            _gameStateModel.StartNewGame
                .TakeUntilDisable(this)
                .Subscribe(_ => HandleNewGame());

            _playerCharacterModel.TakeDamage
                .TakeUntilDisable(this)
                .Subscribe(HandlePlayerDamage);
        }

        private void HandleNewGame() {
            _gameStateModel.Reset();
            _gameplayPools.DespawnAll();

            if (_playerCharacterController != null) {
                Destroy(_playerCharacterController.gameObject);
            }

            CharacterDefinition characterDefinition = CharacterDatabase.Instance.GetCharacterById(_gameStartData.CharacterId);
            GameObject characterGo = Instantiate(characterDefinition.Prefab, _sceneRoot, false);
            characterGo.transform.position = Vector3.zero;
            _playerCharacterController = characterGo.GetComponent<PlayerCharacterController>();
            _playerCharacterController.Initialize(this, characterDefinition);
            _cameraFollow.FollowedTransform = _playerCharacterController.transform;

            ////////////
            _enemySpawner.HandleNewGame();
            _pickupItemSpawner.HandleNewGame();
            _enemyUpdateController.HandleNewGame(_playerCharacterController);
            _enemyEventsHandler.HandleNewGame(_playerCharacterController);
        }

        private void HandlePlayerDamage(float inflictedDamage) {
            if (_playerCharacterModel.ReceivedDamageCooldown.Value > 0 ||
                _playerCharacterModel.ShieldCountdownTimer > 0 ||
                _playerCharacterModel.Health.Value <= 0)
                return;

            _playerCharacterModel.ReceivedDamageCooldown.Value = 1f;

            float damage = inflictedDamage - _playerCharacterModel.Armor.Value;
            damage *= _gamePreferencesRepository.CheatEnemyDamageScale;
            damage = Mathf.Max(1, damage);

            if (_gamePreferencesRepository.CheatGodMode) {
                damage = 0;
            }

            _playerCharacterModel.AddHealth(-damage);

            if (_playerCharacterModel.Health.Value > 0) {
                _playerCharacterController.View.PlayDamageAnimation();
            }

            if (_playerCharacterModel.Health.Value <= 0) {
                if (_playerCharacterModel.RevivedTimesCount.Value < _playerCharacterModel.MaxRevivals.Value) {
                    _playerCharacterModel.RevivedTimesCount.Value++;
                    DOTween.Sequence()
                        .InsertCallback(1.5f, () => {
                            _playerCharacterModel.Health.Value = _playerCharacterModel.MaxHealth.Value / 2;
                        });
                    return;
                }

                _gamePreferencesRepository.CoinsCollected.Value += _playerCharacterModel.Coins.Value;

                _gameStateModel.State.Value = GameplayState.GameOver;
            }
        }

        private void Update() {
            bool isPlaying = _gameStateModel.State.Value == GameplayState.Gameplay;
            bool isPlayerDead = _gameStateModel.State.Value == GameplayState.GameOver;
            if (!isPlaying && !isPlayerDead)
                return;

            if (_gameStateModel.State.Value == GameplayState.Gameplay) {
                UpdateGameplayState();
            }

            _playerCharacterModel.ReceivedDamageCooldown.Value -= Time.deltaTime;
            _enemyUpdateController.GameUpdate();
        }

        private void UpdateGameplayState() {
            UpdateTimer();

            _enemySpawner.GameUpdate();
        }

        private void UpdateTimer() {
            _gameStateModel.Timer.Value += Time.deltaTime * _gamePreferencesRepository.CheatTimerTimeScale;
            CheckNextStageUnlock();
        }

        private void CheckNextStageUnlock() {
            if (!_gameStateModel.NextLevelUnlockChecked && _gameStateModel.Timer.Value >= NextLevelUnlockTimeSeconds) {
                UnlockNextStage();
            }
        }

        private void UnlockNextStage() {
            _gameStateModel.NextLevelUnlockChecked = true;
            StageId currentStageId = _gameStartData.StageId;
            StageId nextStageId = currentStageId + 1;
            if (nextStageId <= StageId.Stage4 && !_gamePreferencesRepository.StageUnlocked[nextStageId].Value) {
                _gamePreferencesRepository.StageUnlocked[nextStageId].Value = true;

                StageDefinition nextStage = StageDatabase.Instance.GetStageById(nextStageId);
                _toastView.ToastAppearWithMessage($"Unlocked {nextStage.Name} stage!");
            }
        }

        public EnemyController FindEnemyInRange(Vector2 sourcePosition, float searchRadius, bool isClosest) {
            ContactFilter2D enemyContactFilter = new ContactFilter2D().NoFilter();
            enemyContactFilter.SetLayerMask(SRLayerMask.Enemy);
            Collider2D[] enemyColliders = System.Buffers.ArrayPool<Collider2D>.Shared.Rent(24);
            int enemiesFound = Physics2D.OverlapCircle(
                sourcePosition,
                searchRadius,
                enemyContactFilter,
                enemyColliders
            );

            EnemyController targetEnemy = null;
            if (isClosest) {
                float minDistance = float.MaxValue;

                for (int i = 0; i < enemiesFound; i++) {
                    Collider2D enemyCollider = enemyColliders[i];
                    float distance = Vector2.SqrMagnitude(enemyCollider.attachedRigidbody.position - sourcePosition);
                    if (distance < minDistance) {
                        targetEnemy = enemyCollider.GetComponentInParent<EnemyController>();
                        minDistance = distance;
                    }
                }
            } else {
                targetEnemy = enemyColliders[Random.Range(0, enemiesFound)].GetComponent<EnemyController>();
            }

            System.Buffers.ArrayPool<Collider2D>.Shared.Return(enemyColliders);
            return targetEnemy;
        }
    }
}
