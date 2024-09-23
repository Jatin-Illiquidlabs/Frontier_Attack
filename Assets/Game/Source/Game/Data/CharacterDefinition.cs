using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace WerewolfBearer {
    [CreateAssetMenu(menuName = "WerewolfBearer/Character")]
    public class CharacterDefinition : ScriptableObject {
        [SerializeField]
        [Required]
        private GameObject _prefab;

        [SerializeField]
        [Required]
        private CharacterId _id;

        [SerializeField]
        [Required]
        private string _name;

        [SerializeField]
        [Required]
        private string _shortName;

        [SerializeField]
        [Required, Multiline(3)]
        private string _description;

        [SerializeField]
        //[Required, AssetsOnly, PreviewField(200, ObjectFieldAlignment.Left)]
        [Required, AssetsOnly]
        private Sprite _avatar;

        [SerializeField]
        private WeaponId _startingWeapon;

        [SerializeField]
        private bool _alwaysUnlocked;

        [SerializeField]
        private int _baseUnlockCost;

        [FormerlySerializedAs("_statModifiers")]
        [SerializeField]
        private StatModifierData[] _initialStats;

        [Header("Note")]
        [SerializeField]
        [Multiline(3), HideLabel]
        private string _note;

        public GameObject Prefab => _prefab;

        public CharacterId Id => _id;

        public string Name => _name;

        public string ShortName => _shortName;

        public string Description => _description;

        public Sprite Avatar => _avatar;

        public WeaponId StartingWeapon => _startingWeapon;

        public bool AlwaysUnlocked => _alwaysUnlocked;

        public int BaseUnlockCost => _baseUnlockCost;

        public StatModifierData[] InitialStats => _initialStats;

        [Serializable]
        public struct StatModifierData {
            [SerializeField]
            private CharacterStatId _stat;

            [SerializeField]
            private float _value;

            public CharacterStatId Stat => _stat;
            public float Value => _value;
        }
    }
}
