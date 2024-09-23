using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace WerewolfBearer {
    public class PauseMenuPresenter : MonoBehaviour {
        [SerializeField]
        private GameObject _pauseScreenContainer;

        [Inject]
        private GameStateModel _gameStateModel;

        private GameplayState _stateBeforePaused;

        private void OnEnable() {
            _gameStateModel.State
                .TakeUntilDisable(this)
                .Subscribe(state => _pauseScreenContainer.SetActive(state == GameplayState.PauseMenu));
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                _stateBeforePaused = _gameStateModel.State.Value;
                _gameStateModel.State.Value = GameplayState.PauseMenu;
            }
        }

        public void OnMainMenuClicked() {
            SceneManager.UnloadSceneAsync(SRScenes.Game.name);
            SceneManager.LoadSceneAsync(SRScenes.Menu.name, LoadSceneMode.Additive);

            Time.timeScale = 1f;
        }

        public void OnResumeClicked() {
            _gameStateModel.State.Value = _stateBeforePaused;
        }
    }
}
