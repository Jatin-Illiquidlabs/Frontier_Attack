using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace WerewolfBearer {
    public class StageSelectionItemView : MonoBehaviour {
        public ReactiveCommand SelectClicked { get; } = new();

        public Image IconImage;
        public TextMeshProUGUI Name;
        public TextMeshProUGUI Description;
        public GameObject LockedOverlayContainer;
        public TextMeshProUGUI LockedOverlayText;

        public void OnSelectClickedHandler() {
            SelectClicked.Execute();
        }
    }
}
