using System;
using UnityEngine;

namespace WerewolfBearer {
    public class AttackData : MonoBehaviour {
        public float Damage { get; private set; }
        public int Pierce { get; private set; }
        public int PierceRemaining { get; private set; }
        public Vector2 KnockbackDirection { get; set; }
        public float KnockbackScale { get; set; }
        public bool DestroyOnPierceEnd = true;

        public event Action Destroyed;

        public void Set(float damage, int pierce, float knockbackScale) {
            Damage = damage;
            Pierce = pierce;
            PierceRemaining = pierce;
            KnockbackScale = knockbackScale;
        }

        public void DecrementRemainingPierce() {
            PierceRemaining--;
            if (PierceRemaining <= 0 && DestroyOnPierceEnd) {
                InvokeDestroyed();
            }
        }

        public void InvokeDestroyed() {
            Destroyed?.Invoke();
        }
    }
}
