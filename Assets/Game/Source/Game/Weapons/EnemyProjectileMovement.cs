using UnityEngine;

namespace WerewolfBearer {
    public class EnemyProjectileMovement : MonoBehaviour {
        private Rigidbody2D _rigidbody2D;
        private Transform _targetTransform;
        private float _movementSpeed;
        private float _rotationSpeed;

        private void Awake() {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate() {
            if (_movementSpeed > 0) {
                Vector2 newDirection = ((Vector2) _targetTransform.position - _rigidbody2D.position).normalized * _movementSpeed;
                newDirection = Vector2.Lerp(_rigidbody2D.velocity, newDirection, _rotationSpeed * Time.deltaTime);
                _rigidbody2D.velocity = newDirection;
            }
        }

        public void Set(Vector2 direction, Transform targetTransform, float movementSpeed, float rotationSpeed) {
            _targetTransform = targetTransform;
            _movementSpeed = movementSpeed;
            _rotationSpeed = rotationSpeed;

            _rigidbody2D.velocity = direction * movementSpeed;
        }
    }
}
