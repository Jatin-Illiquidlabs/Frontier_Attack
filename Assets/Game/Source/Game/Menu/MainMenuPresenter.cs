using System;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace WerewolfBearer {
    public class MainMenuPresenter : MonoBehaviour {
        [SerializeField]
        private GameObject _splashScreenContainer;

        [SerializeField]
        private GameObject _characterSelectContainer;

        [SerializeField]
        private Transform _characterSelectItemsContainer;

        [SerializeField]
        private GameObject _characterSelectItemPrefab;

        [SerializeField]
        private GameObject _stageSelectContainer;

        [SerializeField]
        private Transform _stageSelectItemsContainer;

        [SerializeField]
        private GameObject _stageSelectItemPrefab;

        [SerializeField]
        private GameObject _backButton;

        [SerializeField]
        private TextMeshProUGUI _collectedCoinsText;

        [Inject]
        private MainMenuModel _model;

        [Inject]
        private GameStartData _gameStartData;

        [Inject]
        private GamePreferencesRepository _gamePreferencesRepository;

        [Inject]
        private ToastView _toastView;

        public void OnStartButtonClicked() {
            _model.State.Value = MainMenuState.CharacterSelect;
        }

        public void OnBackButtonClicked() {
            switch (_model.State.Value) {
                case MainMenuState.CharacterSelect:
                    _model.State.Value = MainMenuState.Splash;
                    break;
                case MainMenuState.StageSelect:
                    _model.State.Value = MainMenuState.CharacterSelect;
                    break;
            }
        }

        private void OnEnable() {
            UpdateCoinsText();

            _model.State
                .TakeUntilDisable(this)
                .Subscribe(OnStateChanged);

            _model.StartGamePressed
                .TakeUntilDisable(this)
                .Subscribe(_ => _model.State.Value = MainMenuState.CharacterSelect);

            _model.CharacterIdSelected
                .TakeUntilDisable(this)
                .Subscribe(characterId => {
                    _model.CharacterId.Value = characterId;
                    _model.State.Value = MainMenuState.StageSelect;
                });

            _model.StageIdSelected
                .TakeUntilDisable(this)
                .Subscribe(stageId => {
                    _model.StageId.Value = stageId;
                    StartGame();
                });
        }

        private void StartGame() {
            _gameStartData.CharacterId = _model.CharacterId.Value;
            _gameStartData.StageId = _model.StageId.Value;

            SceneManager.UnloadSceneAsync(SRScenes.Menu.name);
            AsyncOperation loadGameSceneOperation = SceneManager.LoadSceneAsync(SRScenes.Game.name, LoadSceneMode.Additive);
            loadGameSceneOperation.completed += _ => {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(SRScenes.Game.name));
            };
            loadGameSceneOperation.allowSceneActivation = true;
        }

        private void OnStateChanged(MainMenuState state) {
            _backButton.SetActive(false);

            _splashScreenContainer.SetActive(false);
            _characterSelectContainer.SetActive(false);
            _stageSelectContainer.SetActive(false);

            switch (state) {
                case MainMenuState.Splash:
                    _splashScreenContainer.SetActive(true);
                    break;
                case MainMenuState.CharacterSelect:
                    _backButton.SetActive(true);
                    _characterSelectContainer.SetActive(true);
                    PopulateCharacterSelect();
                    break;
                case MainMenuState.StageSelect:
                    _backButton.SetActive(true);
                    _stageSelectContainer.SetActive(true);
                    PopulateStageSelect();
                    break;
                case MainMenuState.Undefined:
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void PopulateStageSelect() {
            foreach (Transform childTransform in _stageSelectItemsContainer) {
                Destroy(childTransform.gameObject);
            }

            void AddStageSelectItem(StageDefinition stage, StageDefinition prevStage) {
                GameObject item = Instantiate(_stageSelectItemPrefab);
                RectTransform rectTransform = item.GetComponent<RectTransform>();
                rectTransform.SetParent(_stageSelectItemsContainer, false);

                StageSelectionItemView view = item.GetComponent<StageSelectionItemView>();

                view.Name.text = stage.Name;
                //view.Description.text = character.Description;
                view.IconImage.sprite = stage.Icon;

                bool unlocked = stage.AlwaysUnlocked || _gamePreferencesRepository.StageUnlocked[stage.StageId].Value;
                view.LockedOverlayContainer.SetActive(!unlocked);
                if (unlocked) {
                    view.SelectClicked.Subscribe(_ => _model.StageIdSelected.Execute(stage.StageId));
                } else  {
                    bool prevUnlocked = prevStage.AlwaysUnlocked || _gamePreferencesRepository.StageUnlocked[prevStage.StageId].Value;
                    if (prevUnlocked) {
                        view.LockedOverlayText.text = $"Survive 30 minutes in {prevStage.Name} to unlock!";
                    } else {
                        view.LockedOverlayText.text = "LOCKED";
                    }
                }
            }

            for (int i = 0; i < StageDatabase.Instance.Stages.Length; i++) {
                StageDefinition stage = StageDatabase.Instance.Stages[i];
                AddStageSelectItem(stage, i == 0 ? null : StageDatabase.Instance.Stages[i - 1]);
            }
        }

        private void PopulateCharacterSelect() {
            foreach (Transform childTransform in _characterSelectItemsContainer) {
                Destroy(childTransform.gameObject);
            }

            CharacterDefinition[] characters = CharacterDatabase.Instance.Characters;
            foreach (CharacterDefinition character in characters) {
                GameObject item = Instantiate(_characterSelectItemPrefab);
                RectTransform rectTransform = item.GetComponent<RectTransform>();
                rectTransform.SetParent(_characterSelectItemsContainer, false);

                CharacterSelectionItemView view = item.GetComponent<CharacterSelectionItemView>();

                view.Name.text = character.Name;
                view.Description.text = character.Description;
                view.AvatarImage.sprite = character.Avatar;
                view.WeaponImage.sprite = WeaponDatabase.Instance.GetWeaponById(character.StartingWeapon)?.Icon;

                bool unlocked = character.AlwaysUnlocked || _gamePreferencesRepository.CharacterUnlocked[character.Id].Value;
                view.LockedOverlayContainer.SetActive(!unlocked);
                view.LockedOverlayUnlockText.text = $"Unlock for {character.BaseUnlockCost} coins";
                view.SelectClicked.Subscribe(_ => _model.CharacterIdSelected.Execute(character.Id));
                view.UnlockClicked.Subscribe(_ => {
                    if (_gamePreferencesRepository.CoinsCollected < character.BaseUnlockCost) {
                        _toastView.ToastAppearWithMessage("Not enough coins!");
                        return;
                    }

                    _toastView.ToastAppearWithMessage($"Unlocked {character.Name}!");

                    int oldCoinsCollected = _gamePreferencesRepository.CoinsCollected;
                    int currentValue = oldCoinsCollected;
                    _gamePreferencesRepository.CoinsCollected.Value -= character.BaseUnlockCost;

                    DOTween
                        .To(() => currentValue, x => currentValue = x, _gamePreferencesRepository.CoinsCollected.Value, 1f)
                        .SetEase(Ease.OutQuad)
                        .OnUpdate(() => {
                            _collectedCoinsText.text = currentValue.ToString();
                        });

                    view.LockedOverlayContainer.SetActive(false);
                    _gamePreferencesRepository.CharacterUnlocked[character.Id].Value = true;
                });
            }
        }

        private void UpdateCoinsText() {
            _collectedCoinsText.text = _gamePreferencesRepository.CoinsCollected.Value.ToString();
        }
    }
}
