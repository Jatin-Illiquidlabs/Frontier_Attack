using UnityEngine;

namespace WerewolfBearer {
    public class GarlicProjectile : PlayerProjectile {
        [SerializeField]
        private float _attackDelay = 0.2f;

        [SerializeField]
        private float _damageRadar = 1.5f;

        private float _intervalCounter;
        private float _areaMultiplier;

        private void Update() {
            _intervalCounter += Time.deltaTime;
            if (_intervalCounter >= _attackDelay) {
                _intervalCounter = 0;
                SetDamageToEnemiesInRadar(_damageRadar * _areaMultiplier);
            }
        }

        public void Set(Transform playerTransform, float areaMultiplier) {
            _areaMultiplier = areaMultiplier;

            Transform.SetParent(playerTransform);
            Transform.localPosition = Vector3.zero;
        }
    }
}
