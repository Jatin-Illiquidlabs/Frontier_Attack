using UnityEngine;

namespace WerewolfBearer {
    public class FlamesOfMisspellProjectile : PlayerProjectile {
        [SerializeField]
        private float _velocityDecay = 1f;

        [SerializeField]
        private float _movementSpeed = 5f;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private Animator _animator;

        private Vector2 _velocity;

        private void Update() {
            _velocity = Vector2.Lerp(_velocity, Vector2.zero, _velocityDecay * Time.deltaTime);
            Rigidbody2D.velocity = _velocity;
        }

        public void Set(Vector2 direction, float projectileSpeedMultiplier, Color color) {
            _velocity = direction * _movementSpeed * projectileSpeedMultiplier;

            _spriteRenderer.color = color;
            _animator.Play("FlamesAttack", -1, Random.value);
        }
    }
}
