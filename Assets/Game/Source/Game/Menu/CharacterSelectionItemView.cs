using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace WerewolfBearer {
    public class CharacterSelectionItemView : MonoBehaviour {
        public ReactiveCommand SelectClicked { get; } = new();
        public ReactiveCommand UnlockClicked { get; } = new();

        public Image AvatarImage;
        public Image WeaponImage;
        public TextMeshProUGUI Name;
        public TextMeshProUGUI Description;
        public GameObject LockedOverlayContainer;
        public TextMeshProUGUI LockedOverlayUnlockText;

        public void OnSelectClickedHandler() {
            SelectClicked.Execute();
        }

        public void OnUnlockClickedHandler() {
            UnlockClicked.Execute();
        }
    }
}
