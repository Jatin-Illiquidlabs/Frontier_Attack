using UnityEngine;

namespace WerewolfBearer {
    public class LightningRingProjectile : PlayerProjectile {
        [SerializeField]
        private float _damageRadar = 2f;

        public void Set(float areaMultiplier) {
            SetDamageToEnemiesInRadar(_damageRadar * areaMultiplier);
        }
    }
}
