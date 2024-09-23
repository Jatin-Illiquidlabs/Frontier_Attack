using UnityEngine.SceneManagement;
using Zenject;

namespace WerewolfBearer {
    public class CoreInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container.Bind<GameStartData>().AsSingle();
            Container.Bind<IPreferencesRepository>().To<PlayerPrefsPreferencesRepository>().AsSingle();
            Container.Bind<GamePreferencesRepository>().AsSingle();
        }

        public override void Start() {
            Scene menuScene = SceneManager.GetSceneByName(SRScenes.Menu.name);
            if (!menuScene.isLoaded) {
                SceneManager.LoadScene(SRScenes.Menu.name, LoadSceneMode.Additive);
            } else {
                SceneManager.SetActiveScene(menuScene);
            }
        }
    }
}
