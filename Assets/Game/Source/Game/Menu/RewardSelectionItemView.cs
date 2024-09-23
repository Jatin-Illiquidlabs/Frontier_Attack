using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace WerewolfBearer {
    public class RewardSelectionItemView : MonoBehaviour {
        public ReactiveCommand SelectClicked { get; } = new();

        public Image IconImage;
        public TextMeshProUGUI Name;
        public TextMeshProUGUI Description;
        public TextMeshProUGUI Level;

        public void OnSelectClickedHandler() {
            SelectClicked.Execute();
        }
    }
}
