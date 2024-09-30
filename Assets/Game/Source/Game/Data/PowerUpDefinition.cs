using Sirenix.OdinInspector;
using UnityEngine;

namespace WerewolfBearer
{

    [CreateAssetMenu(menuName = "WerewolfBearer/PowerUp")]
    public class PowerUpDefinition : ScriptableObject
    {
        [SerializeField]
        [Required]
        private PowerUpId _id;

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
        //[Required]
        private Sprite _icon;

        [SerializeField]
        [Tooltip("Change in percentage or direct numbers per upgrade")]
        [Required]
        private float _changePerRank;

        [SerializeField]
        [Required]
        private int _pricePerLevel;
    }
}
