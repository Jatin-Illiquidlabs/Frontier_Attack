namespace WerewolfBearer {
    public class FloorChickenPickupItem : PickupItem {
        protected override void CollectItemUnchecked(PlayerCharacterModel playerCharacterModel) {
            playerCharacterModel.AddHealth(30);
        }
    }
}
