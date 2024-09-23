using System.Linq;
using UnityEngine;

namespace WerewolfBearer {
    [CreateAssetMenu(menuName = "WerewolfBearer/StageDatabase")]
    public class StageDatabase : SingletonResourcesScriptableObject<StageDatabase> {
        [SerializeField]
        private StageDefinition[] _stages;

        public StageDefinition[] Stages => _stages;

        public StageDefinition GetStageById(StageId id) {
            return _stages.First(w => w.StageId == id);
        }

        public static StageDatabase Instance {
            get {
                if (_instance == null) {
                    Load("StageDatabase");
                }

                return _instance;
            }
        }
    }
}
