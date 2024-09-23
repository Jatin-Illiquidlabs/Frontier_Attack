using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace WerewolfBearer {
    public class LevelUpRewardPresenter : MonoBehaviour {
        [SerializeField]
        private GameObject _levelUpRewardContainer;

        [SerializeField]
        private RectTransform _levelUpRewardItemsContainer;

        [SerializeField]
        private GameObject _levelUpRewardItemPrefab;

        [SerializeField]
        private int _maxRewardsToShow = 3;

        [Inject]
        private PlayerCharacterModel _playerCharacterModel;

        [Inject]
        private GameStateModel _gameStateModel;

        private DisposablesList _disposablesList = new();

        private void OnEnable() {
            _gameStateModel.StartNewGame
                .TakeUntilDisable(this)
                .Subscribe(_ => HandleNewGame());

            _gameStateModel.State
                .TakeUntilDisable(this)
                .Subscribe(state => _levelUpRewardContainer.SetActive(state == GameplayState.LevelUpReward));
        }

        private void OnLevelUp() {
            foreach (Transform child in _levelUpRewardItemsContainer) {
                Destroy(child.gameObject);
            }

            List<RewardData> possibleRewards = GeneratePossibleRewards();
            int rewardCount = Mathf.Min(_maxRewardsToShow, possibleRewards.Count);
            for (int i = 0; i < rewardCount; i++) {
                RewardData reward = possibleRewards[i];

                GameObject rewardItemGo = Instantiate(_levelUpRewardItemPrefab, _levelUpRewardItemsContainer, false);
                RewardSelectionItemView rewardSelectionItemView = rewardItemGo.GetComponent<RewardSelectionItemView>();

                switch (reward.Kind) {
                    case RewardKind.PassiveItem: {
                        PassiveItemDefinition passiveItem = reward.PassiveItem;

                        rewardSelectionItemView.Name.text = passiveItem.Name;
                        rewardSelectionItemView.Description.text = passiveItem.Description;
                        rewardSelectionItemView.Level.text = $"Level: {reward.Level}";
                        rewardSelectionItemView.IconImage.sprite = passiveItem.Icon;
                        rewardSelectionItemView.SelectClicked.Subscribe(_ => {
                            PassiveItemStateModel playerPassiveItemState =
                                _playerCharacterModel.PassiveItems.FirstOrDefault(w => w.Definition.Id == passiveItem.Id);

                            if (playerPassiveItemState != null) {
                                playerPassiveItemState.Level++;
                            } else {
                                playerPassiveItemState = new PassiveItemStateModel(passiveItem);
                                _playerCharacterModel.PassiveItems.Add(playerPassiveItemState);
                            }
                        });
                        break;
                    }
                    case RewardKind.WeaponBuyOrUpgrade: {
                        WeaponDefinition weapon = WeaponDatabase.Instance.GetWeaponById(reward.Weapon);
                        rewardSelectionItemView.Name.text = weapon.Name;
                        rewardSelectionItemView.Description.text = weapon.Description;
                        rewardSelectionItemView.Level.text = $"Level: {reward.Level}";
                        rewardSelectionItemView.IconImage.sprite = weapon.Icon;
                        rewardSelectionItemView.SelectClicked.Subscribe(_ => {
                            WeaponStateModel playerWeaponState =
                                _playerCharacterModel.Weapons.FirstOrDefault(w => w.Definition.Id == weapon.Id);

                            if (playerWeaponState != null) {
                                playerWeaponState.Level++;
                            } else {
                                playerWeaponState = new WeaponStateModel(weapon);
                                _playerCharacterModel.Weapons.Add(playerWeaponState);
                            }
                        });
                        break;
                    }
                    case RewardKind.WeaponEvolution: {
                        WeaponDefinition evolutionWeapon = WeaponDatabase.Instance.GetWeaponById(reward.WeaponEvolutionTo);

                        rewardSelectionItemView.Name.text = evolutionWeapon.Name;
                        rewardSelectionItemView.Description.text = evolutionWeapon.Description;
                        rewardSelectionItemView.Level.text = $"Level: {reward.Level}";
                        rewardSelectionItemView.IconImage.sprite = evolutionWeapon.Icon;
                        rewardSelectionItemView.SelectClicked.Subscribe(_ => {
                            WeaponStateModel oldPlayerWeaponState =
                                _playerCharacterModel.Weapons.FirstOrDefault(w => w.Definition.Id == evolutionWeapon.EvolutionFrom);

                            WeaponStateModel newPlayerWeaponState = new WeaponStateModel(evolutionWeapon);
                            _playerCharacterModel.Weapons[_playerCharacterModel.Weapons.IndexOf(oldPlayerWeaponState)] = newPlayerWeaponState;
                        });
                        break;
                    }
                    case RewardKind.WeaponUnion: {
                        WeaponDefinition unionWeapon = WeaponDatabase.Instance.GetWeaponById(reward.WeaponUnionTo);

                        rewardSelectionItemView.Name.text = unionWeapon.Name;
                        rewardSelectionItemView.Description.text = unionWeapon.Description;
                        rewardSelectionItemView.Level.text = $"Level: {reward.Level}";
                        rewardSelectionItemView.IconImage.sprite = unionWeapon.Icon;
                        rewardSelectionItemView.SelectClicked.Subscribe(_ => {
                            foreach (WeaponId unionFromWeapon in unionWeapon.UnionFrom) {
                                WeaponStateModel oldPlayerWeaponState =
                                    _playerCharacterModel.Weapons.FirstOrDefault(w => w.Definition.Id == unionFromWeapon);

                                _playerCharacterModel.Weapons.Remove(oldPlayerWeaponState);
                            }

                            WeaponStateModel newPlayerWeaponState = new WeaponStateModel(unionWeapon);
                            _playerCharacterModel.Weapons.Add(newPlayerWeaponState);
                        });
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                rewardSelectionItemView.SelectClicked.Subscribe(_ => _gameStateModel.State.Value = GameplayState.Gameplay);
            }
        }

        private List<RewardData> GeneratePossibleRewards() {
            bool HasWeaponWithMaxLevel(WeaponId weapon) {
                WeaponStateModel weaponState =
                    _playerCharacterModel.Weapons.FirstOrDefault(w => w.Definition.Id == weapon);

                return weaponState != null && weaponState.Level >= weaponState.Definition.MaxLevel;
            }

            bool HasWeapon(WeaponId weapon) {
                WeaponStateModel weaponState =
                    _playerCharacterModel.Weapons.FirstOrDefault(w => w.Definition.Id == weapon);

                return weaponState != null;
            }

            int GetWeaponLevel(WeaponId weapon) {
                WeaponStateModel weaponState =
                    _playerCharacterModel.Weapons.First(w => w.Definition.Id == weapon);

                return weaponState.Level;
            }

            List<RewardData> possibleRewards = new();
            WeaponDefinition[] weaponDefinitions = WeaponDatabase.Instance.Weapons;

            // Add all normal weapons
            foreach (WeaponDefinition weaponDefinition in weaponDefinitions) {
                if (weaponDefinition.EvolutionFrom != WeaponId.Undefined || weaponDefinition.UnionFrom.Length != 0)
                    continue;

                WeaponStateModel playerWeaponState =
                    _playerCharacterModel.Weapons.FirstOrDefault(w => w.Definition.Id == weaponDefinition.Id);

                if (playerWeaponState == null || playerWeaponState.Level < weaponDefinition.MaxLevel) {
                    possibleRewards.Add(
                        new RewardData(
                            RewardKind.WeaponBuyOrUpgrade,
                            playerWeaponState != null ? playerWeaponState.Level + 1 : 1,
                            null,
                            weaponDefinition.Id,
                            WeaponId.Undefined,
                            WeaponId.Undefined
                        )
                    );
                }
            }

            // Add weapon evolutions and remove base weapons
            foreach (WeaponDefinition weaponDefinition in weaponDefinitions) {
                if (weaponDefinition.EvolutionFrom == WeaponId.Undefined)
                    continue;

                bool hasBaseWeaponMaxLevel = HasWeaponWithMaxLevel(weaponDefinition.EvolutionFrom);
                bool hasEvolvedWeapon = HasWeapon(weaponDefinition.Id);
                bool hasEvolvedWeaponMaxLevel = HasWeaponWithMaxLevel(weaponDefinition.Id);
                if (hasBaseWeaponMaxLevel) {
                    possibleRewards.Add(
                        new RewardData(
                            RewardKind.WeaponEvolution,
                            1,
                            null,
                            weaponDefinition.EvolutionFrom,
                            weaponDefinition.Id,
                            WeaponId.Undefined
                        )
                    );
                } else if (hasEvolvedWeapon && !hasEvolvedWeaponMaxLevel) {
                    possibleRewards.Add(
                        new RewardData(
                            RewardKind.WeaponBuyOrUpgrade,
                            GetWeaponLevel(weaponDefinition.Id) + 1,
                            null,
                            weaponDefinition.Id,
                            WeaponId.Undefined,
                            WeaponId.Undefined
                        )
                    );
                }

                if (hasEvolvedWeapon) {
                    possibleRewards =
                        possibleRewards
                            .Where(r => !(r.Kind == RewardKind.WeaponBuyOrUpgrade && r.Weapon == weaponDefinition.EvolutionFrom))
                            .ToList();
                }
            }

            // Add union
            foreach (WeaponDefinition weaponDefinition in weaponDefinitions) {
                if (weaponDefinition.UnionFrom.Length == 0)
                    continue;

                bool hasAllBaseWeaponMaxLevel = weaponDefinition.UnionFrom.All(HasWeaponWithMaxLevel);
                    HasWeaponWithMaxLevel(weaponDefinition.EvolutionFrom);
                bool hasUnionWeapon = HasWeapon(weaponDefinition.Id);
                bool hasUnionWeaponMaxLevel = HasWeaponWithMaxLevel(weaponDefinition.Id);
                if (hasAllBaseWeaponMaxLevel) {
                    possibleRewards.Add(
                        new RewardData(
                            RewardKind.WeaponUnion,
                            1,
                            null,
                            WeaponId.Undefined,
                            WeaponId.Undefined,
                            weaponDefinition.Id
                        )
                    );
                } else if (hasUnionWeapon && !hasUnionWeaponMaxLevel) {
                    possibleRewards.Add(
                        new RewardData(
                            RewardKind.WeaponBuyOrUpgrade,
                            GetWeaponLevel(weaponDefinition.Id) + 1,
                            null,
                            weaponDefinition.Id,
                            WeaponId.Undefined,
                            WeaponId.Undefined
                        )
                    );
                }

                if (hasUnionWeapon) {
                    possibleRewards =
                        possibleRewards
                            .Where(r => !(r.Kind == RewardKind.WeaponBuyOrUpgrade && weaponDefinition.UnionFrom.Contains(r.Weapon)))
                            .ToList();
                }
            }

            // Passive
            PassiveItemDefinition[] passiveItemDefinitions = PassiveItemDatabase.Instance.PassiveItems;
            foreach (PassiveItemDefinition passiveItemDefinition in passiveItemDefinitions) {
                PassiveItemStateModel playerPassiveItemState =
                    _playerCharacterModel.PassiveItems.FirstOrDefault(pi => pi.Definition.Id == passiveItemDefinition.Id);

                if (playerPassiveItemState == null || playerPassiveItemState.Level < passiveItemDefinition.MaxLevel) {
                    possibleRewards.Add(
                        new RewardData(
                            RewardKind.PassiveItem,
                            playerPassiveItemState != null ? playerPassiveItemState.Level + 1 : 1,
                            passiveItemDefinition,
                            WeaponId.Undefined,
                            WeaponId.Undefined,
                            WeaponId.Undefined
                        )
                    );
                }
            }

            possibleRewards = possibleRewards.OrderBy(_ => Random.value).ToList();
            return possibleRewards;
        }

        private void HandleNewGame() {
            _disposablesList.Dispose();

            _disposablesList +=
                _gameStateModel.State
                    .TakeUntilDisable(this)
                    .Where(state => state == GameplayState.LevelUpReward)
                    .Subscribe(_ => OnLevelUp());
        }

        private enum RewardKind {
            PassiveItem,
            WeaponBuyOrUpgrade,
            WeaponEvolution,
            WeaponUnion
        }

        private record RewardData(
            RewardKind Kind,
            int Level,
            PassiveItemDefinition PassiveItem,
            WeaponId Weapon,
            WeaponId WeaponEvolutionTo,
            WeaponId WeaponUnionTo
        );
    }
}
