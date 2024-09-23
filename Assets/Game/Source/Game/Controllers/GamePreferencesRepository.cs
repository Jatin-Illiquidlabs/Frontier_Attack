using System.Collections.Generic;
using UnityEngine.Scripting;
using Zenject;

namespace WerewolfBearer {
    [Preserve]
    public class GamePreferencesRepository {
        public IntPreference CoinsCollected { get; }
        public IReadOnlyDictionary<StageId, BooleanPreference> StageUnlocked { get; }
        public IReadOnlyDictionary<CharacterId, BooleanPreference> CharacterUnlocked { get; }

        public IntPreference CheatTimerTimeScale { get; }
        public BooleanPreference CheatGodMode { get; }
        public IntPreference CheatEnemyDamageScale { get; }

        [Inject]
        public GamePreferencesRepository(IPreferencesRepository preferencesRepository) {
            CoinsCollected = new IntPreference(preferencesRepository, "CoinsCollected", 0);

            Dictionary<StageId, BooleanPreference> stageUnlocked = new();
            for (int i = (int) StageId.Stage2; i <= (int) StageId.Stage4; i++) {
                stageUnlocked[(StageId) i] = new BooleanPreference(preferencesRepository, $"Stage_{i}_Unlocked", false);
            }

            StageUnlocked = stageUnlocked;

            Dictionary<CharacterId, BooleanPreference> characterUnlocked = new();
            foreach (CharacterDefinition character in CharacterDatabase.Instance.Characters) {
                if (character.AlwaysUnlocked)
                    continue;

                characterUnlocked[character.Id] = new BooleanPreference(preferencesRepository, $"Character_{(int) character.Id}_Unlocked", false);
            }

            CharacterUnlocked = characterUnlocked;

            CheatTimerTimeScale = new IntPreference(preferencesRepository, nameof(CheatTimerTimeScale), 1);
            CheatGodMode = new BooleanPreference(preferencesRepository, nameof(CheatGodMode), false);
            CheatEnemyDamageScale = new IntPreference(preferencesRepository, nameof(CheatEnemyDamageScale), 1);
        }
    }
}
