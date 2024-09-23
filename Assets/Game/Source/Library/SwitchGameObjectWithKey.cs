using System;
using UnityEngine;

namespace WerewolfBearer {
    public class SwitchGameObjectWithKey : MonoBehaviour {
        [SerializeField]
        private KeyCode _key = KeyCode.I;

        [SerializeField]
        private bool _initialState;

        [SerializeField]
        private GameObject[] _gameObjects;

        private bool _state;

        private void OnEnable() {
            _state = _initialState;
            Set();
        }

        private void Update() {
            if (Input.GetKeyDown(_key)) {
                _state = !_state;
                Set();
            }
        }

        private void Set() {
            foreach (GameObject go in _gameObjects) {
                if (go == null)
                    continue;

                go.SetActive(_state);
            }
        }
    }
}
