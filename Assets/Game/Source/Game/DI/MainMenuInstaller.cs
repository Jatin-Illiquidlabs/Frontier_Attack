using UnityEngine;
using Zenject;

namespace WerewolfBearer {
    public class MainMenuInstaller : MonoInstaller {
        [SerializeField]
        private ToastView _toastView;

        public override void InstallBindings() {
            Container.Bind<MainMenuModel>().AsSingle();
            Container.Bind<ToastView>().FromInstance(_toastView);
        }
    }
}
