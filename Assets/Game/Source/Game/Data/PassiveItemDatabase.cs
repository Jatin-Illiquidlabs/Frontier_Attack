using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WerewolfBearer {
    [CreateAssetMenu(menuName = "WerewolfBearer/PassiveItemDatabase")]
    public class PassiveItemDatabase : SingletonResourcesScriptableObject<PassiveItemDatabase> {
        [SerializeField]
        [InlineEditor]
        private PassiveItemDefinition[] _passiveItems;

        public PassiveItemDefinition[] PassiveItems => _passiveItems;

        public PassiveItemDefinition GetPassiveItemById(PassiveItemId id) {
            return _passiveItems.First(w => w.Id == id);
        }

        public static PassiveItemDatabase Instance {
            get {
                if (_instance == null) {
                    Load("PassiveItemDatabase");
                }

                return _instance;
            }
        }
    }
}
