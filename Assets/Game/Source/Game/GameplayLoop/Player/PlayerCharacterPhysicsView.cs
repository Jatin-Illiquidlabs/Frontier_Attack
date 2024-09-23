using UnityEngine;

namespace WerewolfBearer {
    public class PlayerCharacterPhysicsView : MonoBehaviour {
        [SerializeField]
        private PlayerCharacterController _playerCharacterController;

        public void OnTriggerEnter2D(Collider2D other) {
            _playerCharacterController.OnTriggerEnter2D(other);
        }

        public void OnTriggerStay2D(Collider2D other) {
            _playerCharacterController.OnTriggerStay2D(other);
        }
    }
}
