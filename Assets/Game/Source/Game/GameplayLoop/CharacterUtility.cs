using UnityEngine;

namespace WerewolfBearer {
    public static class CharacterUtility {
        public static Quaternion UpdateBobbingRotation(float time, float bobMaxAngle, float bobFrequency) {
            return Quaternion.Euler(0, 0, Mathf.Sin(time * bobFrequency * Mathf.PI) * bobMaxAngle);
        }
    }
}
