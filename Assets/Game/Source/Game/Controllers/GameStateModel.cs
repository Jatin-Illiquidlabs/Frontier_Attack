using System.Collections.Generic;
using UniRx;
using UnityEngine.Scripting;

namespace WerewolfBearer {
    [Preserve]
    public class GameStateModel {
        public ReactiveProperty<GameplayState> State = new(GameplayState.Gameplay);
        public ReactiveProperty<float> Timer = new(0);

        public ReactiveCommand StartNewGame = new();
        public bool NextLevelUnlockChecked;

        public List<SpawnedEnemy> Enemies = new();
        public bool IsBossFight;

        public void Reset() {
            Timer.Value = 0;
            NextLevelUnlockChecked = false;
            Enemies.Clear();
            IsBossFight = false;
        }
    }
}
