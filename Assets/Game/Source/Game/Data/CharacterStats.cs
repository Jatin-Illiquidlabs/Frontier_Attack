namespace WerewolfBearer {
    public struct CharacterStats {
        public float MaxHealth;
        public float Recovery;
        public float Armor;
        public float MoveSpeed;
        public float Might;
        public float Area;
        public float ProjectileSpeed;
        public float Duration;
        public int Amount;
        public float Cooldown;
        public float Luck;
        public float Growth;
        public float Greed;
        public float Curse;
        public float Magnet;
        public int Revival;
        public int Reroll;
        public int Skip;
        public int Banish;

        public static CharacterStats Default() {
            return new CharacterStats {
                MaxHealth = 100,
                Recovery = 0,
                Armor = 0,
                MoveSpeed = 1,
                Might = 1,
                Area = 1,
                ProjectileSpeed = 1,
                Duration = 1,
                Amount = 0,
                Cooldown = 1,
                Luck = 1,
                Growth = 1,
                Greed = 1,
                Curse = 1,
                Magnet = 1,
                Revival = 0,
                Reroll = 0,
                Skip = 0,
                Banish = 0,
            };
        }
    }
}
