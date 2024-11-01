using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WerewolfBearer
{
    [CreateAssetMenu(menuName = "WerewolfBearer/PowerUp Database")]
    public class PowerUpDatabase : SingletonResourcesScriptableObject<PowerUpDatabase>
    {
        [SerializeField]
        [InlineEditor]
        private PowerUpDefinition[] _powerUps;

        public PowerUpDefinition[] PowerUps => _powerUps;

        public PowerUpDefinition GetPowerUpById(PowerUpId id)
        {
            return _powerUps.FirstOrDefault(w => w.Id == id);
        }

        public static PowerUpDatabase Instance
        {
            get
            {
                if (_instance == null)
                {
                    Load("PowerUpDatabase");
                }

                return _instance;
            }
        }

    }
}
