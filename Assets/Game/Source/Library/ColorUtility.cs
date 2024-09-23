using UnityEngine;

namespace WerewolfBearer {
    public static class ColorUtility {
        public static Color WithA(this Color color, float alpha) {
            color.a = alpha;
            return color;
        }

        public static Color32 WithA(this Color32 color, byte alpha) {
            color.a = alpha;
            return color;
        }
    }
}
