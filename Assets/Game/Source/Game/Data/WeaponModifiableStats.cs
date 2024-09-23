namespace WerewolfBearer {
    public struct WeaponModifiableStats {
        public float BaseDamage;
        public float ProjectileSpeed;
        public float Duration;
        public float Cooldown;
        public float HitboxDelay;
        public float CritMultiplier;
        public float Area;
        public int Amount;
        public int Pierce;
        public float Knockback;
        public float Chance;

        public WeaponModifiableStats(
            float baseDamage,
            float projectileSpeed,
            float duration,
            float cooldown,
            float hitboxDelay,
            float critMultiplier,
            float area,
            int amount,
            int pierce,
            float knockback,
            float chance
        ) {
            BaseDamage = baseDamage;
            ProjectileSpeed = projectileSpeed;
            Duration = duration;
            Cooldown = cooldown;
            HitboxDelay = hitboxDelay;
            CritMultiplier = critMultiplier;
            Area = area;
            Amount = amount;
            Pierce = pierce;
            Knockback = knockback;
            Chance = chance;
        }
    }
}
