using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WerewolfBearer {
    [CreateAssetMenu(menuName = "WerewolfBearer/Weapon Database")]
    public class WeaponDatabase : SingletonResourcesScriptableObject<WeaponDatabase> {
        [SerializeField]
        [InlineEditor]
        private WeaponDefinition[] _weapons;

        public WeaponDefinition[] Weapons => _weapons;

        public WeaponDefinition GetWeaponById(WeaponId id) {
            return _weapons.FirstOrDefault(w => w.Id == id);
        }

        public static WeaponDatabase Instance {
            get {
                if (_instance == null) {
                    Load("WeaponDatabase");
                }

                return _instance;
            }
        }
    }
}
