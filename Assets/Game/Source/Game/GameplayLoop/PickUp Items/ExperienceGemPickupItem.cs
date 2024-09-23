using UnityEngine;

namespace WerewolfBearer {
    public class ExperienceGemPickupItem : PickupItem {
        protected override void CollectItemUnchecked(PlayerCharacterModel playerCharacterModel) {
            playerCharacterModel.AddPlayerXp(Mathf.RoundToInt(1 * playerCharacterModel.ExperiencePickupsMultiplier.Value));
        }
    }
}
