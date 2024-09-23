using UnityEngine;

namespace WerewolfBearer {
    public class CoinBagPickupItem : PickupItem {
        protected override void CollectItemUnchecked(PlayerCharacterModel playerCharacterModel) {
            playerCharacterModel.AddCoin(Mathf.RoundToInt(10 * playerCharacterModel.CoinPickupsMultiplier.Value));
        }
    }
}
