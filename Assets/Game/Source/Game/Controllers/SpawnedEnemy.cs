using Lean.Pool;
using UnityEngine;

namespace WerewolfBearer {
    public readonly struct SpawnedEnemy {
        public readonly GameObject GameObject;
        public readonly EnemyController Controller;
        public readonly LeanGameObjectPool Pool;

        public SpawnedEnemy(GameObject gameObject, EnemyController controller, LeanGameObjectPool pool) {
            GameObject = gameObject;
            Controller = controller;
            Pool = pool;
        }
    }
}
