using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace WerewolfBearer {
    public class EnemyController : MonoBehaviour {
        [FormerlySerializedAs("_view")]
        [SerializeField]
        private EnemyVisualView _visualView;

        [FormerlySerializedAs("_view")]
        [SerializeField]
        private EnemyPhysicsView _physicsView;

        [SerializeField]
        private EnemyDefinition _enemyDefinition;

        public EnemyVisualView VisualView => _visualView;

        public EnemyPhysicsView PhysicsView => _physicsView;

        public EnemyDefinition EnemyDefinition => _enemyDefinition;

        public TouchEnemyModel Model { get; } = new();

        public IEnemyEventsListener EventsListener { get; set; }

        public Action Destroyed;

        private void OnTriggerStay2D(Collider2D other) {
            HandleCollision(other);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            HandleCollision(other);
        }

        private void HandleCollision(Collider2D other) {
            if (!other.isActiveAndEnabled)
                return;

            if (other.gameObject.layer == SRLayers.Player) {
                EventsListener.OnTouchedPlayer(this);
            } else if (other.gameObject.layer == SRLayers.PlayerWeapon) {
                AttackData attackData = other.gameObject.GetComponentInParent<AttackData>();
                if (attackData == null) {
                    Assert.IsNotNull(attackData, other.gameObject.name);
                }

                EventsListener.OnTouchedPlayerWeapon(this, attackData);
            }
        }
    }
}
