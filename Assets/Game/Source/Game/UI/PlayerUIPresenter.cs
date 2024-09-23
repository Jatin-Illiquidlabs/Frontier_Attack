using System;
using System.Globalization;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace WerewolfBearer {
    public class PlayerUIPresenter : MonoBehaviour {
        [SerializeField]
        private GameObject _itemStatusViewPrefab;

        [SerializeField]
        private RectTransform _weaponStatusIconsContainer;

        [SerializeField]
        private RectTransform _passiveItemStatusIconsContainer;

        [SerializeField]
        private TextMeshProUGUI _levelText;

        [SerializeField]
        private TextMeshProUGUI _killsText;

        [SerializeField]
        private TextMeshProUGUI _coinsText;

        [SerializeField]
        private TextMeshProUGUI _timerText;

        [SerializeField]
        private RectTransform _experienceBarRectTransform;

        [SerializeField]
        private Image _experienceBarImage;

        [SerializeField]
        private TextMeshProUGUI _playerStatsText;

        [Inject]
        private PlayerCharacterModel _playerCharacterModel;

        [Inject]
        private GameStateModel _gameStateModel;

        private Tween _experienceBarTween;

        private void OnEnable() {
            _experienceBarRectTransform.anchorMax = new Vector2(0, _experienceBarRectTransform.anchorMax.y);

            _gameStateModel.Timer
                .TakeUntilDisable(this)
                .Subscribe(timer => {
                    TimeSpan timerTimeSpan = TimeSpan.FromSeconds(timer);
                    string timeFormat = timerTimeSpan.Hours == 0 ? @"mm\:ss" : @"hh\:mm\:ss";
                    _timerText.text = timerTimeSpan.ToString(timeFormat, CultureInfo.InvariantCulture);
                });

            _playerCharacterModel.Kills
                .TakeUntilDisable(this)
                .Subscribe(kills => _killsText.text = kills.ToString());

            _playerCharacterModel.Coins
                .TakeUntilDisable(this)
                .Subscribe(coins => _coinsText.text = coins.ToString());

            _playerCharacterModel.Experience
                .Merge(_playerCharacterModel.ExperienceForNextLevel)
                .TakeUntilDisable(this)
                .Subscribe(_ => {
                    float ratio = Mathf.InverseLerp(
                        _playerCharacterModel.ExperienceForCurrentLevel.Value,
                        _playerCharacterModel.ExperienceForNextLevel.Value,
                        _playerCharacterModel.Experience.Value
                    );
                    Vector2 targetAnchorMax = new(ratio, _experienceBarRectTransform.anchorMax.y);
                    if (targetAnchorMax.x < _experienceBarRectTransform.anchorMax.x) {
                        _experienceBarRectTransform.anchorMax = targetAnchorMax;
                        return;
                    }

                    _experienceBarTween?.Kill();
                    _experienceBarTween =
                        _experienceBarRectTransform
                            .DOAnchorMax(targetAnchorMax, 0.25f)
                            .SetEase(Ease.OutExpo)
                            .OnKill(() => _experienceBarTween = null);
                });

            _playerCharacterModel.Level
                .TakeUntilDisable(this)
                .Subscribe(level => _levelText.text = $"Level {level}");

            _playerCharacterModel.Weapons.ObserveAll()
                .TakeUntilDisable(this)
                .Subscribe(_ => {
                    foreach (Transform child in _weaponStatusIconsContainer) {
                        Destroy(child.gameObject);
                    }

                    foreach (WeaponStateModel weaponStateModel in _playerCharacterModel.Weapons) {
                        GameObject itemStatusViewGo = Instantiate(_itemStatusViewPrefab, _weaponStatusIconsContainer, false);
                        ItemStatusView itemStatusView = itemStatusViewGo.GetComponent<ItemStatusView>();
                        itemStatusView.Icon.sprite = weaponStateModel.Definition.Icon;
                    }
                });

            _playerCharacterModel.PassiveItems.ObserveAll()
                .TakeUntilDisable(this)
                .Subscribe(_ => {
                    foreach (Transform child in _passiveItemStatusIconsContainer) {
                        Destroy(child.gameObject);
                    }

                    foreach (PassiveItemStateModel passiveItemStateModel in _playerCharacterModel.PassiveItems) {
                        GameObject itemStatusViewGo = Instantiate(_itemStatusViewPrefab, _passiveItemStatusIconsContainer, false);
                        ItemStatusView itemStatusView = itemStatusViewGo.GetComponent<ItemStatusView>();
                        itemStatusView.Icon.sprite = passiveItemStateModel.Definition.Icon;
                    }
                });
        }

        private void FixedUpdate() {
            if (!_playerStatsText.gameObject.activeInHierarchy)
                return;

            string text =
                "Stats:\n" +
                $"  Level: {_playerCharacterModel.Level.Value}\n" +
                $"  Exp/NextLvlExp: {_playerCharacterModel.Experience.Value}/{_playerCharacterModel.ExperienceForNextLevel.Value}\n" +
                $"  Max Health: {_playerCharacterModel.MaxHealth.Value}%\n" +
                $"  Armor: {_playerCharacterModel.Armor.Value}\n" +
                $"  Recovery: {_playerCharacterModel.HealthRecoveryPerSecond.Value}% / s\n" +
                $"  Might: {_playerCharacterModel.DamageMultiplier.Value * 100f}% / s\n" +
                $"  MoveSpeed: {(_playerCharacterModel.MovementSpeedMultiplier.Value - 1) * 100f}%\n" +
                $"  Amount: {_playerCharacterModel.ExtraProjectilesPerAttack.Value}\n" +
                $"  Cooldown: {(-(1 - _playerCharacterModel.WeaponCooldownMultiplier.Value)) * 100f}%\n" +
                $"  Magnet: {(_playerCharacterModel.PickupItemRangeMultiplier.Value) * 100f}%\n" +
                $"  Area: {(_playerCharacterModel.AttackAreaMultiplier.Value) * 100f}%\n" +
                $"  ProjectileSpeed: {(_playerCharacterModel.ProjectileSpeedMultiplier.Value - 1) * 100f}%\n";

            text += "\n";

            text += "Weapons:\n";
            foreach (WeaponStateModel weapon in _playerCharacterModel.Weapons) {
                text += $"  {weapon.Definition.Name}: Level {weapon.Level}, Damage {weapon.Damage}, CD: {weapon.CooldownTimer:F1}\n";
            }

            text += "\n";

            text += "Items:\n";
            foreach (PassiveItemStateModel passiveItem in _playerCharacterModel.PassiveItems) {
                text += $"  {passiveItem.Definition.Name}: Level {passiveItem.Level}\n";
            }

            _playerStatsText.text = text;
        }
    }
}
