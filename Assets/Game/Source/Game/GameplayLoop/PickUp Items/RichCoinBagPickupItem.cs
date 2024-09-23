using UnityEngine;

namespace WerewolfBearer {
    public class RichCoinBagPickupItem : PickupItem {

        protected override void CollectItemUnchecked(PlayerCharacterModel playerCharacterModel) {
            playerCharacterModel.AddCoin(Mathf.RoundToInt(100 * playerCharacterModel.CoinPickupsMultiplier.Value));
        }
    }
}
