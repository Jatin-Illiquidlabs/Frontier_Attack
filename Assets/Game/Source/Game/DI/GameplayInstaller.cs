using UnityEngine;
using Zenject;

namespace WerewolfBearer {
    public class GameplayInstaller : MonoInstaller {
        [SerializeField]
        private ToastView _toastView;

        [SerializeField]
        private GameplayPools _gameplayPools;

        public override void InstallBindings() {
            Container.Bind<GameStateModel>().AsSingle();
            Container.Bind<PlayerCharacterModel>().AsSingle();
            Container.Bind<ToastView>().FromInstance(_toastView);
            Container.Bind<GameplayPools>().FromInstance(_gameplayPools);
        }
    }
}
