using System;
using UnityEngine;
using Zenject;
using UniRx;

namespace WerewolfBearer {
    public class GameController : MonoBehaviour {
        [SerializeField]
        private Transform _sceneRoot;

        [SerializeField]
        private SerializableDictionary<StageId, BackgroundGenerator> _backgroundGeneratorPrefabs;

        [Inject]
        private GameStartData _gameStartData;

        [Inject]
        private GameStateModel _gameStateModel;

        private BackgroundGenerator _backgroundGenerator;

        private void OnEnable() {
            _gameStateModel.StartNewGame
                .TakeUntilDisable(this)
                .Subscribe(_ => HandleNewGame());

            _gameStateModel.State
                .TakeUntilDisable(this)
                .Subscribe(OnStateChanged);
        }

        private void Start() {
            _gameStateModel.StartNewGame.Execute();
        }

        private void HandleNewGame() {
            SetupGameplayState();
        }

        private void OnStateChanged(GameplayState state) {
            Time.timeScale =
                state is GameplayState.LevelUpReward or GameplayState.PauseMenu ?
                    0 : 1f;
            switch (state) {
                case GameplayState.Gameplay:
                    break;
                case GameplayState.LevelUpReward:
                    break;
                case GameplayState.PauseMenu:
                    break;
                case GameplayState.GameOver:
                    break;
                case GameplayState.Undefined:
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void SetupGameplayState() {
            if (_backgroundGenerator != null) {
                Destroy(_backgroundGenerator);
            }

            BackgroundGenerator backgroundGeneratorPrefab = _backgroundGeneratorPrefabs[_gameStartData.StageId];
            BackgroundGenerator backgroundGenerator = Instantiate(backgroundGeneratorPrefab, _sceneRoot, false);
            _backgroundGenerator = backgroundGenerator;

            _backgroundGenerator.PerlinSeed = new Vector2(UnityEngine.Random.Range(100, 1000), UnityEngine.Random.Range(100, 1000));
            _backgroundGenerator.Clear();
            _backgroundGenerator.Initialize();
        }
    }
}
