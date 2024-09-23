using System;
using UnityEngine;

namespace WerewolfBearer {
    public class TransformMove : MonoBehaviour {
        [SerializeField]
        private Vector3 _speed;

        private void Update() {
            transform.position += _speed * Time.deltaTime;
        }
    }
}
