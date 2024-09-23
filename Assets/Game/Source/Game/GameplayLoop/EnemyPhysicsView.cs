using UnityEngine;

namespace WerewolfBearer {
    public class EnemyPhysicsView : MonoBehaviour {
        [SerializeField]
        private Rigidbody2D _rigidbody2D;
        public Rigidbody2D Rigidbody2D => _rigidbody2D;

        private void Reset() {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }
    }
}