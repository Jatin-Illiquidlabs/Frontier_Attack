namespace WerewolfBearer {
    public class LittleHeartPickupItem : PickupItem {
        protected override void CollectItemUnchecked(PlayerCharacterModel playerCharacterModel) {
            playerCharacterModel.AddHealth(1);
        }
    }
}
