using System;
using UnityEngine;

namespace WerewolfBearer {
    public abstract class PickupItem : MonoBehaviour, IPickupItem {
        [SerializeField]
        private GameObject _collider;

        private bool _pickedUp;

        private void OnEnable() {
            _pickedUp = false;
            _collider.SetActive(true);
        }

        public event Action ItemCollected;

        public void CollectItem(PlayerCharacterModel playerCharacterModel) {
            if (_pickedUp)
                return;

            _pickedUp = true;
            _collider.SetActive(false);

            CollectItemUnchecked(playerCharacterModel);
            ItemCollected?.Invoke();
        }

        protected abstract void CollectItemUnchecked(PlayerCharacterModel playerCharacterModel);
    }
}
