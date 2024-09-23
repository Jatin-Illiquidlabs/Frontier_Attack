using System.Globalization;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace WerewolfBearer {
    public class GameOverPresenter : MonoBehaviour {
        [SerializeField]
        private GameObject _gameOverUIContainer;

        [SerializeField]
        private CanvasGroup _gameOverUICanvasGroup;

        [SerializeField]
        private TextMeshProUGUI _restartCountdownText;

        [Inject]
        private GameStateModel _gameStateModel;

        private void OnEnable() {
            _gameOverUIContainer.SetActive(false);
            _gameStateModel.State
                .TakeUntilDisable(this)
                .Subscribe(HandleStateChanged);
        }

        private void HandleStateChanged(GameplayState gameplayState) {
            if (gameplayState != GameplayState.GameOver) {
                return;
            }

            _gameOverUIContainer.SetActive(true);
            _gameOverUICanvasGroup.alpha = 0;
            _gameOverUICanvasGroup.DOFade(1f, 0.5f);

            const float countdownDuration = 3f;
            float countdownTimer = countdownDuration + 1f;
            DOTween.To(
                    () => countdownTimer,
                    value => countdownTimer = value,
                    0f,
                    countdownDuration
                ).OnUpdate(() => {
                    _restartCountdownText.text = ((int)countdownTimer).ToString(CultureInfo.InvariantCulture);
                })
                .OnComplete(() => {
                    _gameOverUICanvasGroup.DOFade(0f, 0.5f);
                    _gameStateModel.State.Value = GameplayState.Gameplay;
                    _gameStateModel.StartNewGame.Execute();
                });
        }
    }
}
