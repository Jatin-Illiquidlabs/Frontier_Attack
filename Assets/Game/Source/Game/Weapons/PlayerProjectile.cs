using System.Collections.Generic;
using UnityEngine;

namespace WerewolfBearer {
    public abstract class PlayerProjectile : MonoBehaviour {
        private Transform _transform;
        private Rigidbody2D _rigidbody2D;
        private AttackData _attackData;

        public Transform Transform => _transform;

        public Rigidbody2D Rigidbody2D => _rigidbody2D;

        public AttackData AttackData => _attackData;

        private void Awake() {
            _transform = transform;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _attackData = GetComponent<AttackData>();
        }

        protected void SetDamageToEnemiesInRadar(float radius) {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(_transform.position, radius, SRLayerMask.Enemy);
            List<EnemyController> enemiesInRadar = new List<EnemyController>();
            for (int i = 0; i < enemies.Length; i++) {
                EnemyController enemyController = enemies[i].GetComponent<EnemyController>();
                enemyController.EventsListener.OnTouchedPlayerWeapon(enemyController, _attackData);
            }
        }
    }
}
