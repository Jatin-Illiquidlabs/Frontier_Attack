using DG.Tweening;
using DG.Tweening.Core.Easing;
using UnityEngine;

namespace WerewolfBearer {
    public static class DOTweenUtility {
        public static Vector2 EasedValue(Vector2 from, Vector2 to, float lifetimePercentage, Ease easeType) {
            var ease = EvaluateEase(lifetimePercentage, easeType);
            return Vector2.Lerp(from, to, ease);
        }

        public static Color EasedValue(Color from, Color to, float lifetimePercentage, Ease easeType) {
            var ease = EvaluateEase(lifetimePercentage, easeType);
            return Color.Lerp(from, to, ease);
        }

        public static Vector3 EasedValue(Vector3 from, Vector3 to, float lifetimePercentage, Ease easeType) {
            var ease = EvaluateEase(lifetimePercentage, easeType);
            return Vector3.Lerp(from, to, ease);
        }

        public static float EasedValue(float from, float to, float lifetimePercentage, Ease easeType, EaseFunction customEase = null) {
            var ease = EvaluateEase(lifetimePercentage, easeType, customEase);
            return Mathf.Lerp(from, to, ease);
        }

        public static float EvaluateEase(float lifetimePercentage, Ease easeType, EaseFunction customEase = null) {
            return EaseManager.Evaluate(
                easeType,
                customEase,
                lifetimePercentage,
                1f,
                DOTween.defaultEaseOvershootOrAmplitude,
                DOTween.defaultEasePeriod
            );
        }
    }
}
