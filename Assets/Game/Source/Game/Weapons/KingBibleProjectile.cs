using UnityEngine;

namespace WerewolfBearer {
    public class KingBibleProjectile : PlayerProjectile {
        [SerializeField]
        private float _rotationSpeed = 350f;

        private Transform _playerTransform;
        private float _rotationSpeedMultiplier;

        private void Update() {
            Transform.Rotate(_rotationSpeed * _rotationSpeedMultiplier * Time.deltaTime * new Vector3(0, 0, -1));
        }

        public void Set(float angle, Transform playerTransform, float projectileSpeedMultiplier) {
            _playerTransform = playerTransform;
            _rotationSpeedMultiplier = projectileSpeedMultiplier;

            Transform.SetParent(_playerTransform);
            Transform.localPosition = Vector3.zero;
            Transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
