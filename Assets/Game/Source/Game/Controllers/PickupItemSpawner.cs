using DG.Tweening;
using Lean.Pool;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace WerewolfBearer {
    public class PickupItemSpawner : MonoBehaviour {
        private const float PickupItemSpawnChance = 0.7f;
        private const float PickupItemRichCoinWeight = 0.3f;
        private const float PickupItemFloorChickenWeight = 0.3f;
        private const float PickupItemBigCoinBagWeight = 0.6f;
        private const float PickupItemCoinBagWeight = 0.8f;
        private const float PickupItemLittleHeartWeight = 1.0f;
        private const float PickupItemGoldCoinWeight = 0.5f;
        private const float PickupItemExperienceGemWeight = 10.0f;

        [Inject]
        private GameplayPools _gameplayPools;

        [Inject]
        private PlayerCharacterModel _playerCharacterModel;

        private WeightedRandom<LeanGameObjectPool> _weightedRandom;

        private void Awake() {
            _weightedRandom = new( new []{
                    (PickupItemRichCoinWeight, _gameplayPools.RichCoinPickupItem),
                    (PickupItemFloorChickenWeight, _gameplayPools.FloorChickenPickupItem),
                    (PickupItemBigCoinBagWeight, _gameplayPools.BigCoinBagPickupItem),
                    (PickupItemCoinBagWeight, _gameplayPools.CoinBagPickupItem),
                    (PickupItemLittleHeartWeight, _gameplayPools.LittleHeartPickupItem),
                    (PickupItemGoldCoinWeight, _gameplayPools.GoldCoinPickupItem),
                    (PickupItemExperienceGemWeight, _gameplayPools.ExperienceGemPickupItem),
                }
            );
        }

        public void HandleNewGame() {

        }

        public void SpawnRandomPickupItems(Vector3 spawnPosition) {
            void SpawnPickup(LeanGameObjectPool itemPool) {
                GameObject pickupGo = itemPool.Spawn(spawnPosition, Quaternion.identity);
                IPickupItem pickupItem = pickupGo.GetComponent<IPickupItem>();

                void OnItemCollected() {
                    pickupItem.ItemCollected -= OnItemCollected;

                    Transform pickupTransform = pickupGo.transform;
                    Vector3 initialPosition = pickupTransform.position;
                    float lerp = 0f;
                    DOTween
                        .To(
                            () => lerp,
                            value => {
                                lerp = value;
                                pickupTransform.position = Vector3.LerpUnclamped(initialPosition, _playerCharacterModel.Position.Value, value);
                            },
                            1f, 0.5f
                        )
                        .SetEase(Ease.InBack, 3)
                        .OnComplete(() => itemPool.Despawn(pickupGo));
                }

                pickupItem.ItemCollected += OnItemCollected;
            }

            // Check if pickup will spawn at all
            if (Random.value < PickupItemSpawnChance)
                return;

            LeanGameObjectPool pickupPool = _weightedRandom.GetRandomItem();
            SpawnPickup(pickupPool);
        }
    }
}
