using System.Collections.Generic;
using System.Linq;
using Lean.Pool;
using UnityEngine;

namespace WerewolfBearer {
    public class GameplayPools : MonoBehaviour {
        [SerializeField]
        private GameObject[] _enemyExplosionsPrefabs;

        [SerializeField]
        private GameObject _experienceGemPickupItemPrefab;

        [SerializeField]
        private GameObject _bigCoinBagPickupItemPrefab;

        [SerializeField]
        private GameObject _coinBagPickupItemPrefab;

        [SerializeField]
        private GameObject _floorChickenPickupItemPrefab;

        [SerializeField]
        private GameObject _goldCoinPickupItemPrefab;

        [SerializeField]
        private GameObject _littleHeartPickupItemPrefab;

        [SerializeField]
        private GameObject _richCoinPickupItemPrefab;

        [SerializeField]
        private GameObject _hitIndicatorPrefab;

        [SerializeField]
        private GameObject _bloodParticlesPrefab;

        private Dictionary<GameObject, LeanGameObjectPool> _genericPool = new();

        public LeanGameObjectPool[] EnemyExplosions { get; private set; }
        public LeanGameObjectPool ExperienceGemPickupItem { get; private set; }
        public LeanGameObjectPool BigCoinBagPickupItem { get; private set; }
        public LeanGameObjectPool CoinBagPickupItem { get; private set; }
        public LeanGameObjectPool FloorChickenPickupItem { get; private set; }
        public LeanGameObjectPool GoldCoinPickupItem { get; private set; }
        public LeanGameObjectPool LittleHeartPickupItem { get; private set; }
        public LeanGameObjectPool RichCoinPickupItem { get; private set; }
        public LeanGameObjectPool HitIndicator { get; private set; }
        public LeanGameObjectPool BloodParticles { get; private set; }

        private void Awake() {
            EnemyExplosions = _enemyExplosionsPrefabs.Select(prefab => CreatePool(prefab)).ToArray();
            ExperienceGemPickupItem = CreatePool(_experienceGemPickupItemPrefab);
            BigCoinBagPickupItem = CreatePool(_bigCoinBagPickupItemPrefab);
            CoinBagPickupItem = CreatePool(_coinBagPickupItemPrefab);
            FloorChickenPickupItem = CreatePool(_floorChickenPickupItemPrefab);
            GoldCoinPickupItem = CreatePool(_goldCoinPickupItemPrefab);
            LittleHeartPickupItem = CreatePool(_littleHeartPickupItemPrefab);
            RichCoinPickupItem = CreatePool(_richCoinPickupItemPrefab);
            HitIndicator = CreatePool(_hitIndicatorPrefab, 10, 15);
            BloodParticles = CreatePool(_bloodParticlesPrefab, 3);
        }

        public void DespawnAll() {
            // FIXME: how do we not forget anything later?
            foreach (LeanGameObjectPool explosionPool in EnemyExplosions) {
                explosionPool.DespawnAll();
            }

            ExperienceGemPickupItem.DespawnAll();
            BigCoinBagPickupItem.DespawnAll();
            CoinBagPickupItem.DespawnAll();
            FloorChickenPickupItem.DespawnAll();
            GoldCoinPickupItem.DespawnAll();
            LittleHeartPickupItem.DespawnAll();
            RichCoinPickupItem.DespawnAll();
            HitIndicator.DespawnAll();
            BloodParticles.DespawnAll();

            foreach (LeanGameObjectPool genericPool in _genericPool.Values) {
                genericPool.DespawnAll();
                Destroy(genericPool.gameObject);
            }

            _genericPool.Clear();
        }

        public LeanGameObjectPool GetGenericPool(GameObject prefab, int preload = 10, int? capacity = null) {
            if (!_genericPool.TryGetValue(prefab, out LeanGameObjectPool pool)) {
                pool = CreatePool(transform, prefab, preload, capacity);
                _genericPool[prefab] = pool;
            }

            return pool;
        }

        public LeanGameObjectPool CreatePool(GameObject prefab, int preload = 10, int? capacity = null) {
            return CreatePool(transform, prefab, preload);
        }

        private static LeanGameObjectPool CreatePool(Transform poolParent, GameObject prefab, int preload = 10, int? capacity = null) {
            GameObject poolGameObject = new($"Pool_{prefab.name}");
            poolGameObject.transform.SetParent(poolParent);
            poolGameObject.transform.position = Vector3.zero;

            LeanGameObjectPool leanGameObjectPool = poolGameObject.AddComponent<LeanGameObjectPool>();
            leanGameObjectPool.Notification = LeanGameObjectPool.NotificationType.None;
            leanGameObjectPool.Prefab = prefab;
            leanGameObjectPool.Preload = preload;

            if (capacity != null) {
                leanGameObjectPool.Capacity = capacity.Value;
            }

            leanGameObjectPool.PreloadAll();

            return leanGameObjectPool;
        }
    }
}
