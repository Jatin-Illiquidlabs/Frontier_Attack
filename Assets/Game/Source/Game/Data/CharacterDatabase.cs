using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WerewolfBearer {
    [CreateAssetMenu(menuName = "WerewolfBearer/Character Database")]
    public class CharacterDatabase : SingletonResourcesScriptableObject<CharacterDatabase> {
        [SerializeField]
        [InlineEditor]
        private CharacterDefinition[] _characters;

        public CharacterDefinition[] Characters => _characters;

        public CharacterDefinition GetCharacterById(CharacterId id) {
            return _characters.First(w => w.Id == id);
        }

        public static CharacterDatabase Instance {
            get {
                if (_instance == null) {
                    Load("CharacterDatabase");
                }

                return _instance;
            }
        }
    }
}
