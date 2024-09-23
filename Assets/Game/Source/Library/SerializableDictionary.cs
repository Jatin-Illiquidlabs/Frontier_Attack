using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace WerewolfBearer {
    //http://answers.unity3d.com/answers/809221/view.html
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
        [FormerlySerializedAs("_pairs")]
        [SerializeField]
        List<Pair> _serializedPairs = new();

        public SerializableDictionary() {
        }

        public SerializableDictionary(IDictionary<TKey, TValue> input) : base(input) {
        }

        public void OnBeforeSerialize() {
            _serializedPairs.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this) {
                if (pair.Key == null)
                    continue;

                _serializedPairs.Add(new Pair(pair.Key, pair.Value));
            }
        }

        public void OnAfterDeserialize() {
            Clear();

            for (int i = 0; i < _serializedPairs.Count; i++) {
                Pair pair = _serializedPairs[i];
                Add(pair.Key, pair.Value);
            }
        }

        [Serializable]
        private class Pair {
            public TKey Key;
            public TValue Value;

            public Pair() {
            }

            public Pair(TKey key, TValue value) {
                Key = key;
                Value = value;
            }
        }
    }
}
