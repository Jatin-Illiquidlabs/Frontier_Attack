using Sirenix.OdinInspector;
using UnityEngine;

namespace WerewolfBearer {
    [CreateAssetMenu(menuName = "WerewolfBearer/PassiveItem")]
    public class PassiveItemDefinition : ScriptableObject {
        [SerializeField]
        [Required]
        private PassiveItemId _id;

        [SerializeField]
        [Required]
        private string _name;

        [SerializeField]
        [Required, Multiline(3)]
        private string _description;

        [SerializeField]
        [Required]
        private int _maxLevel;

        [SerializeField]
        [Required]
        private Sprite _icon;

        [Header("Note")]
        [SerializeField]
        [Multiline(3), HideLabel]
        private string _note;

        public PassiveItemId Id => _id;

        public string Name => _name;

        public string Description => _description;

        public int MaxLevel => _maxLevel;

        [PreviewField(ObjectFieldAlignment.Left)]
        public Sprite Icon => _icon;
    }
}
