using System;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Scripting;
using Zenject;
using Random = UnityEngine.Random;

namespace WerewolfBearer {
    [Preserve]
    public partial class PlayerCharacterController : MonoBehaviour {
        [SerializeField]
        private PlayerCharacterView _view;

        [SerializeField]
        private float _moveSpeed;

        [SerializeField]
        private Transform _swordAttackSpawnPoint;

        [Inject]
        private PlayerCharacterModel _model;

        [Inject]
        private GameStateModel _gameStateModel;

        [Inject]
        private GameplayPools _gameplayPools;

        private Transform _transform;

        private DisposablesList _subscriptions = new();
        private Dictionary<WeaponId, Action<WeaponStateModel>> _weaponAttackActions;

        public PlayerCharacterView View => _view;

        private GameplayController _gameplayController;

        private CharacterDefinition _characterDefinition;

        public void Initialize(GameplayController gameplayController, CharacterDefinition characterDefinition) {
            _gameplayController = gameplayController;
            _characterDefinition = characterDefinition;
            _subscriptions.Dispose();

            _weaponAttackActions = CreateWeaponAttackActions();

            WeaponDefinition startingWeaponDefinition = WeaponDatabase.Instance.GetWeaponById(characterDefinition.StartingWeapon);
            WeaponStateModel startingWeaponState = new(startingWeaponDefinition);

            _model.Reset();
            _model.Weapons.Add(startingWeaponState);

            UpdateStats();
            UpdateExperience();

            _model.Health.Value = _model.MaxHealth.Value;

            _subscriptions +=
                _model.Experience
                    .TakeUntilDisable(this)
                    .Where(exp => exp >= _model.ExperienceForNextLevel.Value)
                    .Subscribe(_ => {
                        _model.Level.Value++;
                        UpdateExperience();
                        _gameStateModel.State.Value = GameplayState.LevelUpReward;
                    });

            View.Initialize();
        }

        private void Awake() {
            _transform = transform;
        }

        private void OnDisable() {
            _subscriptions.Dispose();
        }

        private void FixedUpdate() {
            if (_model.Health.Value <= 0)
                return;

            UpdateStats();
            UpdateExperience();
            ApplyPassiveItemEffects();
            MovePlayer();
            FireWeapons();
            CollectPickupItems();
        }

        private void Update() {
#if DEBUG
            if (Input.GetKeyDown(KeyCode.L)) {
                _model.Experience.Value = _model.ExperienceForNextLevel.Value;
            }

            if (Input.GetKeyDown(KeyCode.M)) {
                _model.Coins.Value += 300;
            }

            if (Input.GetKeyDown(KeyCode.N)) {
                _gameStateModel.Timer.Value += 5 * 60;
            }
#endif
        }

        public void OnTriggerEnter2D(Collider2D other) {
            HandleCollision(other);
        }

        public void OnTriggerStay2D(Collider2D other) {
            HandleCollision(other);
        }

        private void HandleCollision(Collider2D other) {
            if (other.gameObject.layer == SRLayers.EnemyWeapon) {
                AttackData attackData = other.gameObject.GetComponentInParent<AttackData>();
                Assert.IsNotNull(attackData);
                if (attackData.PierceRemaining <= 0)
                    return;

                if (_model.ReceivedDamageCooldown.Value <= 0) {
                    _model.TakeDamage.Execute(attackData.Damage);
                }

                attackData.DecrementRemainingPierce();
            }
        }

        private void UpdateExperience() {
            static int CalculateExperienceForLevel(int level) {
                /*
                Player starts at level 1 and has to collect 5 XP to level up to level 2.
                Thereafter, the requirement increases by 10 XP each level until level 20
                (i.e. 15 XP is required to go from level 2 to 3, 25 XP from 3 to 4 and so on).
                From level 21 to 40 the requirement increases by 13 XP each level, and from level 41 onwards
                the requirement increases by 16 XP each level.

                Additionally, at levels 20 and 40 an additional amount of XP - 600 and 2400
                respectively - is required to level up to the next level. However,
                at these levels the player also gains +100% Growth,
                increasing their experience gain, until they reach the next level.
                 */
                int experienceForLevel = 0;
                for (int levelCounter = 1; levelCounter <= level; levelCounter++) {
                    experienceForLevel += levelCounter switch {
                        <= 1 => 0,
                        2 => 5,
                        >= 3 and <= 20 => 10,
                        >= 21 and <= 40 => 13,
                        >= 41 => 16
                    };
                }

                return experienceForLevel;
            }

            int level = _model.Level.Value;
            _model.ExperienceForCurrentLevel.Value = CalculateExperienceForLevel(level);
            _model.ExperienceForNextLevel.Value = CalculateExperienceForLevel(level + 1);
        }

        private void MovePlayer() {
            Vector2 moveSpeed;
            moveSpeed.x = Input.GetAxis("Horizontal");
            moveSpeed.y = Input.GetAxis("Vertical");

            Vector2 moveDirection = moveSpeed;

            moveSpeed *= _moveSpeed * _model.MovementSpeedMultiplier.Value * Time.deltaTime;

            if (Mathf.Abs(moveSpeed.x) > Vector2.kEpsilon) {
                _model.FaceRight = moveSpeed.x > 0;
            }

            bool isMoving = moveSpeed.magnitude > Vector2.kEpsilon;
            _view.UpdateView(_model.FaceRight, isMoving);

            Vector3 position = _transform.position;
            position += (Vector3) moveSpeed;
            _transform.position = position;

            if (moveDirection.sqrMagnitude > 0.1) {
                _model.MoveDirection.Value = moveDirection;
            }

            _model.Position.Value = position;
        }

        private void ApplyPassiveItemEffects() {
            _model.ShieldCountdownTimer -= Time.deltaTime;

            float health = _model.Health.Value;
            health += _model.HealthRecoveryPerSecond.Value * Time.deltaTime;
            health = Mathf.Min(health, _model.MaxHealth.Value);
            _model.Health.Value = health;
        }

        private void CollectPickupItems() {
            const float basePickupRadius = 1f;
            ContactFilter2D pickupItemContactFilter = new ContactFilter2D().NoFilter();
            pickupItemContactFilter.SetLayerMask(SRLayerMask.PickupItems);
            Collider2D[] pickupItems = System.Buffers.ArrayPool<Collider2D>.Shared.Rent(24);
            int pickupItemsFound = Physics2D.OverlapCircle(
                _transform.position,
                basePickupRadius * _model.PickupItemRangeMultiplier.Value,
                pickupItemContactFilter,
                pickupItems);

            for (int i = 0; i < pickupItemsFound; i++) {
                IPickupItem pickupItem = pickupItems[i].GetComponentInParent<IPickupItem>();
                pickupItem?.CollectItem(_model);
            }

            System.Buffers.ArrayPool<Collider2D>.Shared.Return(pickupItems);
        }
    }
}
