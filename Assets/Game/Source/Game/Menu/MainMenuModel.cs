using UniRx;
using UnityEngine.Scripting;

namespace WerewolfBearer {
    [Preserve]
    public class MainMenuModel {
        public ReactiveProperty<MainMenuState> State { get; } = new(MainMenuState.Splash);

        public ReactiveProperty<StageId> StageId { get; } = new();
        public ReactiveProperty<CharacterId> CharacterId { get; } = new();

        public ReactiveCommand StartGamePressed { get; } = new();
        public ReactiveCommand<StageId> StageIdSelected { get; } = new();
        public ReactiveCommand<CharacterId> CharacterIdSelected { get; } = new();
    }
}
