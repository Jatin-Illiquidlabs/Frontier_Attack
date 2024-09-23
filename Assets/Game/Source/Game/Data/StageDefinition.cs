using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WerewolfBearer {
    [Serializable]
    public class StageDefinition {
        [SerializeField]
        private StageId _stageId;

        [SerializeField]
        private string _name;

        [SerializeField]
        private Sprite _icon;

        [SerializeField]
        private bool _alwaysUnlocked;

        [SerializeField]
        private EnemySpawnData[] _enemies;

        [SerializeField]
        private EnemySpawnData[] _bosses;

        [SerializeField]
        private float _enemySpawnIntervalBase = 3f;

        [SerializeField]
        private float _enemySpawnIntervalMin = 0.25f;

        [SerializeField]
        private float _enemySpawnIntervalDecreasePerPlayerLevel = 0.12f;

        [Header("Note")]
        [SerializeField]
        [Multiline(3), HideLabel]
        private string _note;

        public StageId StageId => _stageId;

        public string Name => _name;

        public Sprite Icon => _icon;

        public bool AlwaysUnlocked => _alwaysUnlocked;

        public EnemySpawnData[] Enemies => _enemies;

        public EnemySpawnData[] Bosses => _bosses;

        public float EnemySpawnIntervalBase => _enemySpawnIntervalBase;

        public float EnemySpawnIntervalMin => _enemySpawnIntervalMin;

        public float EnemySpawnIntervalDecreasePerPlayerLevel => _enemySpawnIntervalDecreasePerPlayerLevel;

        [Serializable]
        public class EnemySpawnData {
            [Required]
            public GameObject EnemyPrefab;
            public float SpawnWeight = 1;
        }
    }
}
