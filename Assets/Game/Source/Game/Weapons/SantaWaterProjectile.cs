using UnityEngine;
using DG.Tweening;

namespace WerewolfBearer {
    public class SantaWaterProjectile : PlayerProjectile {
        [SerializeField]
        private GameObject _santaWaterTail;

        [SerializeField]
        private GameObject _santaWaterEffect;

        [SerializeField]
        private float _damageRadar = 2.5f;

        [SerializeField]
        private float _attackInterval = 0.2f;

        [SerializeField]
        private float _travelToTargetTime = 0.3f;

        private bool _allowToSetDamage;
        private float _attackTimer;
        private float _areaMultiplier;

        private void Update() {
            if (!_allowToSetDamage)
                return;

            _attackTimer -= Time.deltaTime;
            if (_attackTimer <= 0) {
                _attackTimer = _attackInterval;
                SetDamageToEnemiesInRadar(_damageRadar * _areaMultiplier);
            }
        }

        public void Set(Vector3 targetPosition, float areaMultiplier) {
            _areaMultiplier = areaMultiplier;

            _santaWaterTail.SetActive(true);
            _santaWaterEffect.SetActive(false);

            _attackTimer = 0;
            _allowToSetDamage = false;
            Transform
                .DOMove(targetPosition, _travelToTargetTime)
                .SetEase(Ease.OutCirc)
                .OnComplete(() => {
                    _santaWaterTail.SetActive(false);
                    _santaWaterEffect.SetActive(true);
                    _allowToSetDamage = true;
                });
        }
    }
}
