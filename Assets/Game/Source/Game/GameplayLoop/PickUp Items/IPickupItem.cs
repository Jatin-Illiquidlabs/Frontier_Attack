using System;

namespace WerewolfBearer {
    public interface IPickupItem {
        event Action ItemCollected;

        void CollectItem(PlayerCharacterModel playerCharacterModel);
    }
}
