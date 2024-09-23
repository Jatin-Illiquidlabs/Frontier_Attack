using UnityEngine;

namespace WerewolfBearer {
    public static class CameraUtility {
        public static Vector2 GetCameraViewportSize(Camera camera) {
            return new Vector2(
                camera.aspect * camera.orthographicSize * 2f,
                camera.orthographicSize * 2f
            );
        }
    }
}