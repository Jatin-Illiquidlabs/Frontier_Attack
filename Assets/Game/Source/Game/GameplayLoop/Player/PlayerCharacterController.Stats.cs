using System;
using UnityEngine;

namespace WerewolfBearer {
    public partial class PlayerCharacterController {
        private void UpdateStats() {
            CharacterStats stats = CharacterStats.Default();

            ApplyCharacterInitialStats(ref stats);
            ApplyCharacterDynamicStats(ref stats);
            ApplyPassiveItemsStats(ref stats);
            ApplyPowerUpStats(ref stats);

            stats.Cooldown = Mathf.Max(0.1f, stats.Cooldown);

            _model.ApplyStats(stats);
        }

        private void ApplyCharacterDynamicStats(ref CharacterStats stats) {
            // FIXME: move to data?
            switch (_characterDefinition.Id) {
                case CharacterId.Antonio: {
                    // Gains 10% more damage every 10 levels (max +50%).
                    int steps = Mathf.Min(_model.Level.Value / 10, 5);
                    stats.Might += 0.1f * steps;
                    break;
                }
                case CharacterId.Arca: {
                    // gains -5% Cooldown every 10 levels until level 30. The maximum Cooldown reduction gained this way is -15%.
                    int steps = Mathf.Min(_model.Level.Value / 10, 3);
                    stats.Cooldown += 0.05f * steps;
                    break;
                }
                case CharacterId.Gennaro:
                    break;
                case CharacterId.Imelda: {
                    // Imelda gains +10% Growth every 5 levels until level 15. The maximum Growth gained this way is +30%.
                    int steps = Mathf.Min(_model.Level.Value / 5, 3);
                    stats.Growth += 0.1f * steps;
                    break;
                }
                case CharacterId.Lama: {
                    // Gains another 5% Might, 5% MoveSpeed and 5% Curse every 10 levels, for a max increase of +20% or +30% in total.
                    int steps = Mathf.Min(_model.Level.Value / 10, 4);
                    stats.Might += 0.05f * steps;
                    stats.MoveSpeed += 0.05f * steps;
                    stats.Curse += 0.05f * steps;
                    break;
                }
                case CharacterId.Pasqualina: {
                    // Gains +10% Speed every 5 levels until level 15.
                    int steps = Mathf.Min(_model.Level.Value / 5, 3);
                    stats.MoveSpeed += 0.1f * steps;
                    break;
                }
                case CharacterId.Porta: {
                    // Porta starts with -90% Cooldown bonus, which decreases by 30% on every level up until negated upon reaching level 4
                    stats.Cooldown -= Mathf.Max(4 - _model.Level.Value, 0) * 0.3f;
                    break;
                }
                case CharacterId.Undefined:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ApplyCharacterInitialStats(ref CharacterStats stats) {
            foreach (CharacterDefinition.StatModifierData initialStat in _characterDefinition.InitialStats) {
                switch (initialStat.Stat) {
                    case CharacterStatId.MaxHealth:
                        stats.MaxHealth *= (1f + initialStat.Value);
                        break;
                    case CharacterStatId.Recovery:
                        stats.Recovery += initialStat.Value;
                        break;
                    case CharacterStatId.Armor:
                        stats.Armor += initialStat.Value;
                        break;
                    case CharacterStatId.MoveSpeed:
                        stats.MoveSpeed += initialStat.Value;
                        break;
                    case CharacterStatId.Might:
                        stats.Might += initialStat.Value;
                        break;
                    case CharacterStatId.Area:
                        stats.Area += initialStat.Value;
                        break;
                    case CharacterStatId.ProjectileSpeed:
                        stats.ProjectileSpeed += initialStat.Value;
                        break;
                    case CharacterStatId.Duration:
                        stats.Duration += initialStat.Value;
                        break;
                    case CharacterStatId.Amount:
                        stats.Amount += Mathf.RoundToInt(initialStat.Value);
                        break;
                    case CharacterStatId.Cooldown:
                        stats.Cooldown += initialStat.Value;
                        break;
                    case CharacterStatId.Luck:
                        stats.Luck += initialStat.Value;
                        break;
                    case CharacterStatId.Growth:
                        stats.Growth += initialStat.Value;
                        break;
                    case CharacterStatId.Greed:
                        stats.Greed += initialStat.Value;
                        break;
                    case CharacterStatId.Curse:
                        stats.Curse += initialStat.Value;
                        break;
                    case CharacterStatId.Magnet:
                        stats.Magnet += initialStat.Value;
                        break;
                    case CharacterStatId.Revival:
                        stats.Revival += Mathf.RoundToInt(initialStat.Value);
                        break;
                    case CharacterStatId.Reroll:
                        stats.Reroll += Mathf.RoundToInt(initialStat.Value);
                        break;
                    case CharacterStatId.Skip:
                        stats.Skip += Mathf.RoundToInt(initialStat.Value);
                        break;
                    case CharacterStatId.Banish:
                        stats.Banish += Mathf.RoundToInt(initialStat.Value);
                        break;
                    case CharacterStatId.Undefined:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void ApplyPassiveItemsStats(ref CharacterStats stats) {
            foreach (PassiveItemStateModel passiveItem in _model.PassiveItems) {
                switch (passiveItem.Definition.Id) {
                    case PassiveItemId.Armor:
                        stats.Armor += passiveItem.Level;
                        break;
                    case PassiveItemId.Spinach:
                        stats.Might += passiveItem.Level * 0.1f;
                        break;
                    case PassiveItemId.HollowHeart:
                        for (int i = 0; i < passiveItem.Level; i++) {
                            stats.MaxHealth *= 1.2f;
                        }

                        break;
                    case PassiveItemId.Pummarola:
                        stats.Recovery += passiveItem.Level * 0.2f;
                        break;
                    case PassiveItemId.Wings:
                        stats.MoveSpeed += passiveItem.Level * 0.1f;
                        break;
                    case PassiveItemId.Duplicator:
                        stats.Amount += passiveItem.Level;
                        break;
                    case PassiveItemId.EmptyTome:
                        stats.Cooldown -= passiveItem.Level * 0.08f;
                        break;
                    case PassiveItemId.Bracer:
                        stats.ProjectileSpeed += passiveItem.Level * 0.1f;
                        break;
                    case PassiveItemId.Tiragisu:
                        stats.Revival += passiveItem.Level;
                        break;
                    case PassiveItemId.Candelabrador:
                        stats.Area += passiveItem.Level * 0.1f;
                        break;
                    case PassiveItemId.Spellbinder:
                        stats.Duration += passiveItem.Level * 0.1f;
                        break;
                    case PassiveItemId.TorronasBox:
                        stats.Might += passiveItem.Level * 0.04f;
                        stats.ProjectileSpeed += passiveItem.Level * 0.04f;
                        stats.Duration += passiveItem.Level * 0.04f;
                        stats.Area += passiveItem.Level * 0.04f;
                        break;
                    case PassiveItemId.Attractorb:
                        stats.Magnet *= passiveItem.Level switch {
                            1 => 1.5f,
                            2 => 1.995f,
                            3 => 2.49375f,
                            4 => 2.9925f,
                            5 => 3.980025f,
                            _ => throw new ArgumentOutOfRangeException(nameof(passiveItem.Level), passiveItem.Level, "")
                        };
                        break;
                    case PassiveItemId.StoneMask:
                        stats.Greed += passiveItem.Level * 0.1f;
                        break;
                    case PassiveItemId.Crown:
                        stats.Growth += passiveItem.Level * 0.1f;
                        break;
                    case PassiveItemId.SkullOManiac:
                        stats.Curse += passiveItem.Level * 0.1f;
                        break;
                    case PassiveItemId.Undefined:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void ApplyPowerUpStats(ref CharacterStats stats)
        {
            foreach (PowerUpDefinition powerUp in PowerUpDatabaseHolder.Instance.powerUpDatabase.PowerUps)
            {
                int level = PowerUpDatabaseHolder.Instance.currentLevel >= powerUp.MaxLevel ? powerUp.MaxLevel : PowerUpDatabaseHolder.Instance.currentLevel;
                switch (powerUp.Id)
                {
                    case PowerUpId.Armor:
                        level = PowerUpDatabaseHolder.Instance.armorLevel >= powerUp.MaxLevel ? powerUp.MaxLevel : PowerUpDatabaseHolder.Instance.armorLevel;
                        stats.Armor += level;
                        break;
                    case PowerUpId.Might:
                        level = PowerUpDatabaseHolder.Instance.mightLevel >= powerUp.MaxLevel ? powerUp.MaxLevel : PowerUpDatabaseHolder.Instance.mightLevel;
                        stats.Might += level * 0.1f;
                        break;
                    case PowerUpId.MaxHealth:
                        level = PowerUpDatabaseHolder.Instance.maxHealthLevel >= powerUp.MaxLevel ? powerUp.MaxLevel : PowerUpDatabaseHolder.Instance.maxHealthLevel;
                        for (int i = 0; i < level; i++)
                        {
                            stats.MaxHealth *= 1.2f;
                        }

                        break;
                    case PowerUpId.Recovery:
                        level = PowerUpDatabaseHolder.Instance.recoveryLevel >= powerUp.MaxLevel ? powerUp.MaxLevel : PowerUpDatabaseHolder.Instance.recoveryLevel;
                        stats.Recovery += level * 0.2f;
                        break;
                    case PowerUpId.MoveSpeed:
                        level = PowerUpDatabaseHolder.Instance.moveSpeedLevel >= powerUp.MaxLevel ? powerUp.MaxLevel : PowerUpDatabaseHolder.Instance.moveSpeedLevel;
                        stats.MoveSpeed += level * 0.1f;
                        break;
                    case PowerUpId.Amount:
                        level = PowerUpDatabaseHolder.Instance.amountLevel >= powerUp.MaxLevel ? powerUp.MaxLevel : PowerUpDatabaseHolder.Instance.amountLevel;
                        stats.Amount += level;
                        break;
                    case PowerUpId.Cooldown:
                        level = PowerUpDatabaseHolder.Instance.cooldownLevel >= powerUp.MaxLevel ? powerUp.MaxLevel : PowerUpDatabaseHolder.Instance.cooldownLevel;
                        stats.Cooldown -= level * 0.08f;
                        break;
                    case PowerUpId.Speed:
                        level = PowerUpDatabaseHolder.Instance.speedLevel >= powerUp.MaxLevel ? powerUp.MaxLevel : PowerUpDatabaseHolder.Instance.speedLevel;
                        stats.ProjectileSpeed += level * 0.1f;
                        break;
                    case PowerUpId.Revival:
                        level = PowerUpDatabaseHolder.Instance.revivalLevel >= powerUp.MaxLevel ? powerUp.MaxLevel : PowerUpDatabaseHolder.Instance.revivalLevel;
                        stats.Revival += level;
                        break;
                    case PowerUpId.Area:
                        level = PowerUpDatabaseHolder.Instance.areaLevel >= powerUp.MaxLevel ? powerUp.MaxLevel : PowerUpDatabaseHolder.Instance.areaLevel;
                        stats.Area += level * 0.1f;
                        break;
                    case PowerUpId.Duration:
                        level = PowerUpDatabaseHolder.Instance.durationLevel >= powerUp.MaxLevel ? powerUp.MaxLevel : PowerUpDatabaseHolder.Instance.durationLevel;
                        stats.Duration += level * 0.1f;
                        break;
                    case PowerUpId.Magnet:
                        level = PowerUpDatabaseHolder.Instance.magnetLevel >= powerUp.MaxLevel ? powerUp.MaxLevel : PowerUpDatabaseHolder.Instance.magnetLevel;
                        stats.Magnet *= level switch
                        {
                            0 => 0f,
                            1 => 1.5f,
                            2 => 1.995f,
                            3 => 2.49375f,
                            4 => 2.9925f,
                            5 => 3.980025f,
                            _ => throw new ArgumentOutOfRangeException(nameof(level), level, "")
                        };

                        break;
                    case PowerUpId.Greed:
                        level = PowerUpDatabaseHolder.Instance.greedLevel >= powerUp.MaxLevel ? powerUp.MaxLevel : PowerUpDatabaseHolder.Instance.greedLevel;
                        stats.Greed += level * 0.1f;
                        break;
                    case PowerUpId.Growth:
                        level = PowerUpDatabaseHolder.Instance.growthLevel >= powerUp.MaxLevel ? powerUp.MaxLevel : PowerUpDatabaseHolder.Instance.growthLevel;
                        stats.Growth += level * 0.1f;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

    }
}
