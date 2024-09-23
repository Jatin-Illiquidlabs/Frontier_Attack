using System;
using System.Collections.Generic;
using System.Linq;
using Lean.Pool;
using UnityEngine;

namespace WerewolfBearer {
    public class PoolManager : MonoBehaviour {
        [SerializeField]
        private SerializableDictionary<GameObject, PoolDefinition> _poolDefinitions = new();

        private readonly Dictionary<GameObject, LeanGameObjectPool> _pools = new();

        private void Awake() {
            foreach (KeyValuePair<GameObject, PoolDefinition> poolDefinitionPair in _poolDefinitions) {
                GameObject poolGameObject = new($"Pool_{poolDefinitionPair.Key.name}");
                poolGameObject.transform.SetParent(transform);
                poolGameObject.transform.position = Vector3.zero;

                LeanGameObjectPool leanGameObjectPool = poolGameObject.AddComponent<LeanGameObjectPool>();
                leanGameObjectPool.Prefab = poolDefinitionPair.Key;
                leanGameObjectPool.Preload = poolDefinitionPair.Value.Preload;

                _pools[poolDefinitionPair.Key] = leanGameObjectPool;
            }
        }

        public LeanGameObjectPool GetPoolForPrefab(GameObject prefab) {
            if (!_pools.TryGetValue(prefab, out LeanGameObjectPool pool))
                throw new Exception($"No pool found for prefab '{prefab}'");

            return pool;
        }

#if UNITY_EDITOR
        private void Reset() {
            _poolDefinitions.Clear();

            LeanGameObjectPool[] leanGameObjectPools =
                FindObjectsOfType<LeanGameObjectPool>()
                    .OrderBy(pool => UnityEditor.AssetDatabase.GetAssetPath(pool.Prefab).ToLowerInvariant())
                    .ToArray();

            foreach (LeanGameObjectPool pool in leanGameObjectPools) {
                _poolDefinitions[pool.Prefab] = new PoolDefinition {
                    Preload = pool.Preload
                };
            }
        }
#endif

        [Serializable]
        public class PoolDefinition {
            public int Preload = 10;
        }
    }
}
