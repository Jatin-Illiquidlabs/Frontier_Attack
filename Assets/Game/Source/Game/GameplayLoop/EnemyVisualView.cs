using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WerewolfBearer {
    public class EnemyVisualView : MonoBehaviour {
        [SerializeField]
        private Transform _transform;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private SpriteRenderer _shadowSpriteRenderer;

        [SerializeField]
        private SpriteRenderer _overlaySpriteRenderer;

        private float _movementTimer;
        private Tween _fadeOutTween;

        public SpriteRenderer SpriteRenderer => _spriteRenderer;
        public Transform Transform => _transform;

        private void Awake() {
            if (_overlaySpriteRenderer == null) {
                GameObject spriteOverlayGo = new GameObject("SpriteOverlay");
                spriteOverlayGo.transform.SetParent(_transform);
                spriteOverlayGo.transform.localPosition = Vector3.zero;
                spriteOverlayGo.transform.localScale = _spriteRenderer.transform.localScale;

                SpriteRenderer overlaySpriteRenderer = spriteOverlayGo.AddComponent<SpriteRenderer>();
                overlaySpriteRenderer.sprite = _spriteRenderer.sprite;
                overlaySpriteRenderer.material = SRResources.SpritesConstantColorOverlay.Load();
                overlaySpriteRenderer.sortingOrder = 1;

                _overlaySpriteRenderer = overlaySpriteRenderer;
            }
        }

        private void OnEnable() {
            _movementTimer = Random.value * 3f;
            _spriteRenderer.color = Color.white;
            _shadowSpriteRenderer.color = new Color32(0, 0, 0, 150);
            _overlaySpriteRenderer.enabled = false;
        }

        private void OnDisable() {
            _fadeOutTween?.Kill();
        }

        public void UpdateView(bool faceRight) {
            _spriteRenderer.flipX = _shadowSpriteRenderer.flipX = _overlaySpriteRenderer.flipX = !faceRight;

            _movementTimer += Time.deltaTime;
            Transform.rotation = CharacterUtility.UpdateBobbingRotation(_movementTimer, 5f, 2f);
        }

        public void StartTakeDamageAnimation(bool isDead) {
            float fadeDuration = isDead ? 0.1f : 0.5f;

            _overlaySpriteRenderer.enabled = true;
            _overlaySpriteRenderer.color = Color.white;
            _overlaySpriteRenderer
                .DOColor(Color.white.WithA(0), fadeDuration)
                .OnComplete(() => _overlaySpriteRenderer.enabled = false);
        }

        public void StartDeathAnimation() {
            const float fadeDuration = 0.1f;

            _fadeOutTween?.Kill();
            Sequence sequence = DOTween.Sequence();
            sequence.Insert(
                0,
                _spriteRenderer
                    .DOFade(0f, fadeDuration)
                    .OnKill(() => _fadeOutTween = null)
            );
            sequence.Insert(
                0,
                _shadowSpriteRenderer
                    .DOFade(0f, fadeDuration)
                    .OnKill(() => _fadeOutTween = null)
            );
            _fadeOutTween = sequence;
        }
    }
}
