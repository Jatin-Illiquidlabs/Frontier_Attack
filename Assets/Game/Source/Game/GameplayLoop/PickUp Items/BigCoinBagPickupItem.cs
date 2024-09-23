using UnityEngine;

namespace WerewolfBearer {
    public class BigCoinBagPickupItem : PickupItem {
        protected override void CollectItemUnchecked(PlayerCharacterModel playerCharacterModel) {
            playerCharacterModel.AddCoin(Mathf.RoundToInt(25 * playerCharacterModel.CoinPickupsMultiplier.Value));
        }
    }
}
