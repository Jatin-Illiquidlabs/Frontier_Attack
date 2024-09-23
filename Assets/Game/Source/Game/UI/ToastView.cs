using UnityEngine;
using TMPro;
using DG.Tweening;

namespace WerewolfBearer {
    public class ToastView : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI _messageText;

        private bool _isToastAppear;
        private float _toastVisibilityTime = 3.0f;
        private float _toastDisappearCountdown;

        private void Update() {
            if (_isToastAppear) {
                _toastDisappearCountdown += Time.deltaTime;

                if (_toastDisappearCountdown >= _toastVisibilityTime) {
                    _isToastAppear = false;
                    ToastDisappear();
                }
            }
        }

        public void ToastAppearWithMessage(string message) {
            _toastDisappearCountdown = 0;
            _messageText.text = message;
            RectTransform toastRectTransform = GetComponent<RectTransform>();
            toastRectTransform.anchoredPosition = new Vector2(0, -100);
            toastRectTransform.DOAnchorPosY(100, 0.5f).SetEase(Ease.OutFlash).OnComplete(() => {
                _isToastAppear = true;
            });
        }

        private void ToastDisappear() {
            RectTransform toastRectTransform = GetComponent<RectTransform>();
            toastRectTransform.anchoredPosition = new Vector2(0, 100);
            toastRectTransform.DOAnchorPosY(-100, 0.5f).SetEase(Ease.InFlash);
        }
    }
}