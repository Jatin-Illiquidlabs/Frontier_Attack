using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace WerewolfBearer {
    [CreateAssetMenu(fileName = "WeaponDefinition", menuName = "WerewolfBearer/Weapon", order = 1)]
    public class WeaponDefinition : ScriptableObject {
        [SerializeField]
        private WeaponId _id;

        [SerializeField]
        [FormerlySerializedAs("_weaponName")]
        private string _name;

        [SerializeField]
        [Required, Multiline(3)]
        private string _description;

        [SerializeField]
        //[PreviewField(ObjectFieldAlignment.Left)]
        private Sprite _icon;

        [SerializeField]
        private int _maxLevel;

        [SerializeField]
        [FormerlySerializedAs("WeaponPrefab")]
        private GameObject _weaponPrefab;

        [SerializeField]
        private float _attackPeriod;

        [SerializeField]
        [FormerlySerializedAs("_attackLifeTime")]
        private float _attackLifetime;

        [SerializeField]
        private int _damage;

        [SerializeField]
        private float _knockbackScale = 1f;

        [SerializeField]
        [Min(1)]
        private int _projectilePierce = 1;

        [SerializeField]
        [Min(1)]
        private WeaponId _evolutionFrom = WeaponId.Undefined;

        [SerializeField]
        [Min(1)]
        private WeaponId[] _unionFrom = new WeaponId[0];

        /*
        [SerializeField]
        [FormerlySerializedAs("WeaponUpgread")]
        private List<LevelUpgradeData> _upgrades = new();
        */

        [Header("Note")]
        [SerializeField]
        [Multiline(3), HideLabel]
        private string _note;

        public WeaponId Id => _id;

        public string Name => _name;

        public string Description => _description;

        public Sprite Icon => _icon;

        public int MaxLevel => _maxLevel;

        public GameObject WeaponPrefab => _weaponPrefab;


        public float AttackPeriod => _attackPeriod;

        public float AttackLifetime => _attackLifetime;

        public int Damage => _damage;

        public float KnockbackScale => _knockbackScale;

        public int ProjectilePierce => _projectilePierce;

        public WeaponId EvolutionFrom => _evolutionFrom;

        public WeaponId[] UnionFrom => _unionFrom;

        //public List<LevelUpgradeData> Upgrades => _upgrades;

        [Serializable]
        public struct LevelUpgradeData {
            [FormerlySerializedAs("UpgreadDatas")]
            private List<UpgradeData> _upgrades;

            public List<UpgradeData> Upgrades => _upgrades;
        }

        [Serializable]
        public struct UpgradeData {
            [SerializeField]
            [FormerlySerializedAs("UpgreadStat")]
            private WeaponUpgradeType _upgradeType;

            [SerializeField]
            [FormerlySerializedAs("UpgreadValue")]
            private int _upgradeValue;

            public WeaponUpgradeType UpgradeType => _upgradeType;

            public int UpgradeValue => _upgradeValue;
        }
    }
}
