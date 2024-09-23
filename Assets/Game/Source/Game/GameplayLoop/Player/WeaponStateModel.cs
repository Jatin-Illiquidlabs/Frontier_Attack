namespace WerewolfBearer {
    public partial class WeaponStateModel {
        public WeaponDefinition Definition { get; }
        public int Level { get; set; }
        public float Damage { get; private set; }
        public float Speed { get; private set; }
        public float Area { get; private set; }
        public float ProjectileLifetime { get; private set; }
        public float Cooldown { get; private set; }
        public int ExtraProjectilesPerAttack { get; private set; }
        public int ProjectilePierce { get; private set; }
        public int FireCounter { get; private set; }

        public float CooldownTimer;

        public WeaponStateModel(WeaponDefinition definition) {
            Definition = definition;

            Level = 1;
            UpdateStats();

            CooldownTimer = Cooldown * 0.8f;
        }

        public void Update(float deltaTime) {
            CooldownTimer += deltaTime;
            UpdateStats();
        }

        public void IncrementFireCounter() {
            FireCounter++;
        }

        private void UpdateStats() {
            WeaponModifiableStats stats = new(
                Definition.Damage,
                float.PositiveInfinity,
                Definition.AttackLifetime,
                Definition.AttackPeriod,
                float.PositiveInfinity,
                1f,
                1f,
                1,
                Definition.ProjectilePierce,
                0f,
                0
            );

            ApplyDynamicStats(ref stats, Definition.Id, Level);

            Area = stats.Area;
            Damage = stats.BaseDamage;
            ExtraProjectilesPerAttack = stats.Amount;
            Cooldown = stats.Cooldown;
            ProjectileLifetime = stats.Duration;
        }

        /*public void UpgradeWeapon() {
            WeaponDefinition.LevelUpgradeData levelUpgrade = WeaponUpgreads[Level.Value - 1];

            for (int i = 0; i < levelUpgrade.Upgrades.Count; i++) {
                WeaponDefinition.UpgradeData upgrade = levelUpgrade.Upgrades[i];
                switch (upgrade.UpgradeType) {
                    case WeaponUpgradeType.AddProjectile:
                        NumberOfProjectile += upgrade.UpgradeValue;
                        break;
                    case WeaponUpgradeType.AddNumberOfEnemiesThroughProjectileCanPass:
                        NumberOfEnemiesThroughProjectileCanPass += upgrade.UpgradeValue;
                        break;
                    case WeaponUpgradeType.AddProjectileLifetime:
                        break;
                    case WeaponUpgradeType.BaseAreaUp:
                        BaseAreaMultiplier += upgrade.UpgradeValue / 100.0f;
                        break;
                    case WeaponUpgradeType.BaseDamageUp:
                        BaseDamage += upgrade.UpgradeValue;
                        break;
                    case WeaponUpgradeType.BaseSpeedUp:
                        BaseSpeedMultiplier += upgrade.UpgradeValue / 100.0f;
                        break;
                }
            }
        }*/
    }
}
