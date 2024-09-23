using UnityEngine;

namespace WerewolfBearer {
    public class RunetracerProjectile : PlayerProjectile {
        private const float ReflectDelay = 0.6f;

        [SerializeField]
        private float _speed = 5.0f;

        [SerializeField]
        private TrailRenderer _trailRenderer;

        private Camera _camera;
        private Transform _cameraTransform;
        private Vector2 _cameraSize;

        private Vector2 _velocity;
        private float _reflectAllowedTimer;

        private void Update() {
            _reflectAllowedTimer += Time.deltaTime;
            if (_reflectAllowedTimer <= ReflectDelay)
                return;

            Vector2 movementDirection = Transform.position - _cameraTransform.position;
            if (movementDirection.x >= _cameraSize.x / 2 || movementDirection.x <= -_cameraSize.x / 2) {
                // Touch to boundaries
                _velocity.x *= -1;
                Rigidbody2D.velocity = _velocity;
                _reflectAllowedTimer = 0;
            } else if (movementDirection.y >= _cameraSize.y / 2 || movementDirection.y <= -_cameraSize.y / 2) {
                _velocity.y *= -1;
                Rigidbody2D.velocity = _velocity;
                _reflectAllowedTimer = 0;
            }

            Rigidbody2D.rotation = Mathf.Atan2(_velocity.y, _velocity.x) * Mathf.Rad2Deg;
            AttackData.KnockbackDirection = _velocity;
        }

        public void Set(Vector3 direction, Camera camera, float projectileSpeedMultiplier) {
            _camera = camera;
            _cameraSize = CameraUtility.GetCameraViewportSize(camera);
            _cameraTransform = camera.transform;
            _velocity = direction * projectileSpeedMultiplier * _speed;

            Rigidbody2D.velocity = _velocity;
            AttackData.KnockbackDirection = _velocity;
            _reflectAllowedTimer = ReflectDelay;

            _trailRenderer.Clear();
        }
    }
}
