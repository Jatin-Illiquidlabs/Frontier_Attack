using System;
using UniRx;
using UnityEngine;
using UnityEngine.Scripting;

namespace WerewolfBearer {
    [Preserve]
    public class PlayerCharacterModel {
        // Stats
        public FloatReactiveProperty MaxHealth { get; } = new();
        public FloatReactiveProperty HealthRecoveryPerSecond { get; } = new();
        public FloatReactiveProperty Armor { get; } = new();
        public FloatReactiveProperty MovementSpeedMultiplier { get; } = new();
        public FloatReactiveProperty DamageMultiplier { get; } = new();
        public FloatReactiveProperty AttackAreaMultiplier { get; } = new();
        public FloatReactiveProperty ProjectileSpeedMultiplier { get; } = new();
        public FloatReactiveProperty AttackDurationMultiplier { get; } = new();
        public IntReactiveProperty ExtraProjectilesPerAttack { get; } = new();
        public FloatReactiveProperty WeaponCooldownMultiplier { get; } = new();
        public FloatReactiveProperty Luck { get; } = new();
        public FloatReactiveProperty ExperiencePickupsMultiplier { get; } = new();
        public FloatReactiveProperty CoinPickupsMultiplier { get; } = new();
        public FloatReactiveProperty EnemyBuffMultiplier { get; } = new();
        public FloatReactiveProperty PickupItemRangeMultiplier { get; } = new();
        public FloatReactiveProperty MaxRevivals { get; } = new();
        public IntReactiveProperty MaxRewardRerolls { get; } = new();
        public IntReactiveProperty MaxRewardSkips { get; } = new();
        public IntReactiveProperty MaxItemBanishes { get; } = new();

        // State
        public Vector2ReactiveProperty Position { get; } = new();
        public Vector2ReactiveProperty MoveDirection { get; } = new();
        public FloatReactiveProperty ReceivedDamageCooldown { get; } = new();
        public FloatReactiveProperty Health { get; } = new();
        public IntReactiveProperty RevivedTimesCount { get; } = new();
        public IntReactiveProperty Experience { get; } = new();
        public IntReactiveProperty ExperienceForCurrentLevel { get; } = new();
        public IntReactiveProperty ExperienceForNextLevel { get; } = new();
        public IntReactiveProperty Coins { get; } = new();
        public IntReactiveProperty Kills { get; } = new();
        public IntReactiveProperty Level { get; } = new();

        public ReactiveCollection<PassiveItemStateModel> PassiveItems { get; } = new();
        public ReactiveCollection<WeaponStateModel> Weapons { get; } = new();

        public IObservable<bool> IsDead { get; }

        public ReactiveCommand<float> TakeDamage { get; } = new();

        public bool FaceRight = true;
        public float ShieldCountdownTimer;

        public PlayerCharacterModel() {
            Reset();
            IsDead = Health.Select(h => h <= 0).DistinctUntilChanged();
        }

        public void Reset() {
            ApplyStats(CharacterStats.Default());

            Position.Value = Vector2.zero;
            MoveDirection.Value = Vector2.left;
            ReceivedDamageCooldown.Value = 0;
            Health.Value = MaxHealth.Value;
            RevivedTimesCount.Value = 0;
            Experience.Value = 0;
            Coins.Value = 0;
            Kills.Value = 0;
            Level.Value = 1;
            PassiveItems.Clear();
            Weapons.Clear();

            FaceRight = true;
        }

        public void ApplyStats(in CharacterStats stats) {
            MaxHealth.Value = stats.MaxHealth;
            HealthRecoveryPerSecond.Value = stats.Recovery;
            Armor.Value = stats.Armor;
            MovementSpeedMultiplier.Value = stats.MoveSpeed;
            DamageMultiplier.Value = stats.Might;
            AttackAreaMultiplier.Value = stats.Area;
            ProjectileSpeedMultiplier.Value = stats.ProjectileSpeed;
            AttackDurationMultiplier.Value = stats.Duration;
            ExtraProjectilesPerAttack.Value = stats.Amount;
            WeaponCooldownMultiplier.Value = stats.Cooldown;
            Luck.Value = stats.Luck;
            ExperiencePickupsMultiplier.Value = stats.Growth;
            CoinPickupsMultiplier.Value = stats.Greed;
            EnemyBuffMultiplier.Value = stats.Curse;
            PickupItemRangeMultiplier.Value = stats.Magnet;
            MaxRevivals.Value = stats.Revival;
            MaxRewardRerolls.Value = stats.Reroll;
            MaxRewardSkips.Value = stats.Skip;
            MaxItemBanishes.Value = stats.Banish;
        }

        public void AddCoin(int coin) {
            Coins.Value += coin;
        }

        public void AddPlayerXp(int xp) {
            Experience.Value += xp;
        }

        public void AddHealth(float health) {
            Health.Value += health;
        }
    }
}
