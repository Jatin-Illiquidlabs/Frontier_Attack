using System;
using CodeStage.AdvancedFPSCounter;
using UnityEngine;

namespace WerewolfBearer {
    public class FPSLimitSetter : MonoBehaviour {
        [SerializeField]
        private KeyCode Key = KeyCode.P;

        [SerializeField]
        private AFPSCounter _afpsCounter;

        private void Update() {
            if (Input.GetKeyDown(Key)) {
                if (_afpsCounter.ForceFrameRate) {
                    _afpsCounter.ForceFrameRate = false;
                    _afpsCounter.ForcedFrameRate = -1;
                } else {
                    _afpsCounter.ForceFrameRate = true;
                    _afpsCounter.ForcedFrameRate = 3000;
                }
            }
        }
    }
}
