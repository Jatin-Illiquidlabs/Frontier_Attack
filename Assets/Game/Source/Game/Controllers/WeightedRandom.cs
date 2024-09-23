using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace WerewolfBearer {
    public class WeightedRandom<T> {
        public IReadOnlyList<(float weight, T item)> WeightedItems { get; }

        public WeightedRandom(IReadOnlyList<(float weight, T item)> weightedItems) {
            //Debug.Assert(weightedItems.Count != 0);
            WeightedItems = weightedItems;
        }

        public T GetRandomItem() {
            float weightSum = WeightedItems.Sum(x => x.weight);
            float randomValue = Random.value * weightSum;

            float currentProgress = 0;
            foreach ((float weight, T item) weightedItem in WeightedItems) {
                float maxProgress = currentProgress + weightedItem.weight;
                if (randomValue >= currentProgress && randomValue <= maxProgress) {
                    return weightedItem.item;
                }

                currentProgress = maxProgress;
            }

            throw new InvalidOperationException("GetRandomItem failed");
        }
    }
}
