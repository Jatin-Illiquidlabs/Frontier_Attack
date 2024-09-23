using System;
using UnityEngine;

namespace WerewolfBearer {
    public class DestroyGameObjectInRelease : MonoBehaviour {
        private void OnEnable() {
#if !DEBUG
            Destroy(gameObject);
#endif
        }
    }
}
