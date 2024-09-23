using UnityEngine;

namespace WerewolfBearer {
    [RequireComponent(typeof(Camera))]
    public class CameraFollow2D : MonoBehaviour {
        [SerializeField]
        private Transform _followedTransform;

        public Transform FollowedTransform {
            get => _followedTransform;
            set => _followedTransform = value;
        }

        private Camera _camera;

        private void OnEnable() {
            _camera = GetComponent<Camera>();
        }

        private void Update() {
            if (_followedTransform == null)
                return;

            Vector3 cameraPosition = _followedTransform.transform.position;
            cameraPosition.z = _camera.transform.position.z;
            _camera.transform.position = cameraPosition;
        }
    }
}
