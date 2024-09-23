using UnityEngine;
using UnityEngine.Assertions;

namespace WerewolfBearer {
    public abstract class SingletonResourcesScriptableObject<T> : ScriptableObject where T : ScriptableObject {
        protected static T _instance;

        protected static void Load(string path) {
            Assert.IsTrue(_instance == null);
            _instance = Resources.Load<T>(path);
            Assert.IsFalse(_instance == null);
        }
    }
}
