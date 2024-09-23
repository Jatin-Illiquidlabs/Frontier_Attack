namespace WerewolfBearer {
    public partial class WeaponStateModel {
        private static void ApplyDynamicStats(ref WeaponModifiableStats stats, WeaponId weaponId, int level) {
            switch (weaponId) {
                case WeaponId.Knife:
                case WeaponId.XKnife_BigAndSlow:
                    for (int i = 2; i <= level; i++) {
                        switch (i) {
                            case 2:
                                stats.Amount++;
                                break;
                            case 3:
                                stats.Amount++;
                                stats.BaseDamage += 5;
                                break;
                            case 4:
                                stats.Amount++;
                                break;
                            case 5:
                                stats.Pierce++;
                                break;
                            case 6:
                                stats.Amount++;
                                break;
                            case 7:
                                stats.Amount++;
                                stats.BaseDamage += 5;
                                break;
                            case 8:
                                stats.Pierce++;
                                break;
                        }
                    }

                    break;
                case WeaponId.MagicWand:
                case WeaponId.HolyWand:
                    for (int i = 2; i <= level; i++) {
                        switch (i) {
                            case 2:
                                stats.Amount++;
                                break;
                            case 3:
                                stats.Cooldown -= 0.2f;
                                break;
                            case 4:
                                stats.Amount++;
                                break;
                            case 5:
                                stats.BaseDamage += 10;
                                break;
                            case 6:
                                stats.Amount++;
                                break;
                            case 7:
                                stats.Pierce++;
                                break;
                            case 8:
                                stats.BaseDamage += 10;
                                break;
                        }
                    }

                    break;
                case WeaponId.Cross:
                case WeaponId.CrossEvo:
                    for (int i = 2; i <= level; i++) {
                        switch (i) {
                            case 2:
                                stats.BaseDamage += 10;
                                break;
                            case 3:
                                stats.ProjectileSpeed *= 1.25f;
                                stats.Area *= 1.1f;
                                break;
                            case 4:
                                stats.Amount++;
                                break;
                            case 5:
                                stats.BaseDamage += 10;
                                break;
                            case 6:
                                stats.ProjectileSpeed *= 1.25f;
                                stats.Area *= 1.1f;
                                break;
                            case 7:
                                stats.Amount++;
                                break;
                            case 8:
                                stats.BaseDamage += 10;
                                break;
                        }
                    }

                    break;
                case WeaponId.KingBible:
                case WeaponId.KingBibleEvo:
                case WeaponId.XKingBible_BiggerRadius:
                    for (int i = 2; i <= level; i++) {
                        switch (i) {
                            case 2:
                                stats.Amount++;
                                break;
                            case 3:
                                stats.ProjectileSpeed *= 1.3f;
                                stats.Area *= 1.25f;
                                break;
                            case 4:
                                stats.Duration += 0.5f;
                                stats.BaseDamage += 10;
                                break;
                            case 5:
                                stats.Amount++;
                                break;
                            case 6:
                                stats.ProjectileSpeed *= 1.3f;
                                stats.Area *= 1.25f;
                                break;
                            case 7:
                                stats.Duration += 0.5f;
                                stats.BaseDamage += 10;
                                break;
                            case 8:
                                stats.Amount++;
                                break;
                        }
                    }

                    break;
                case WeaponId.FireWand:
                case WeaponId.HellFire:
                    for (int i = 2; i <= level; i++) {
                        switch (i) {
                            case 2:
                                stats.BaseDamage += 10;
                                break;
                            case 3:
                                stats.ProjectileSpeed *= 1.2f;
                                stats.BaseDamage += 10;
                                break;
                            case 4:
                                stats.BaseDamage += 10;
                                break;
                            case 5:
                                stats.ProjectileSpeed *= 1.2f;
                                stats.BaseDamage += 10;
                                break;
                            case 6:
                                stats.BaseDamage += 10;
                                break;
                            case 7:
                                stats.ProjectileSpeed *= 1.2f;
                                stats.BaseDamage += 10;
                                break;
                            case 8:
                                stats.BaseDamage += 10;
                                break;
                        }
                    }

                    break;
                case WeaponId.Garlic:
                    for (int i = 2; i <= level; i++) {
                        switch (i) {
                            case 2:
                                stats.Area *= 1.4f;
                                stats.BaseDamage += 2;
                                break;
                            case 3:
                                stats.Cooldown -= 0.1f;
                                stats.BaseDamage += 1;
                                break;
                            case 4:
                                stats.Area *= 1.2f;
                                stats.BaseDamage += 1;
                                break;
                            case 5:
                                stats.Cooldown -= 0.1f;
                                stats.BaseDamage += 1;
                                break;
                            case 6:
                                stats.Area *= 1.2f;
                                stats.BaseDamage += 1;
                                break;
                            case 7:
                                stats.Cooldown -= 0.1f;
                                stats.BaseDamage += 1;
                                break;
                            case 8:
                                stats.Area *= 1.2f;
                                stats.BaseDamage += 1;
                                break;
                        }
                    }

                    break;
                case WeaponId.SantaWater:
                    for (int i = 2; i <= level; i++) {
                        switch (i) {
                            case 2:
                                stats.Amount++;
                                stats.Area *= 1.2f;
                                break;
                            case 3:
                                stats.BaseDamage += 10;
                                stats.Duration += 0.5f;
                                break;
                            case 4:
                                stats.Amount++;
                                stats.Area *= 1.2f;
                                break;
                            case 5:
                                stats.BaseDamage += 10;
                                stats.Duration += 0.3f;
                                break;
                            case 6:
                                stats.Amount++;
                                stats.Area *= 1.2f;
                                break;
                            case 7:
                                stats.BaseDamage += 5;
                                stats.Duration += 0.3f;
                                break;
                            case 8:
                                stats.BaseDamage += 5;
                                stats.Area *= 1.2f;
                                break;
                        }
                    }

                    break;
                case WeaponId.Whip:
                    stats.BaseDamage *= level;
                    stats.Amount = level > 1 ? 1 : 0;
                    if (level >= 4) {
                        stats.Area += 0.1f;
                    }

                    if (level >= 6) {
                        stats.Area += 0.1f;
                    }

                    break;
                case WeaponId.Laurel:
                case WeaponId.LaurelEvo:
                    stats.Cooldown -= level * 0.7f;
                    stats.Duration += level * 0.45f;
                    break;
                case WeaponId.Carrello:
                    stats.BaseDamage += level * 2.5f;
                    stats.Area += level * 0.2f;
                    break;
                default:
                    // For weapons with no custom progression
                    stats.Amount = (level - 1) / 2;
                    stats.BaseDamage += 0.05f * (level - 1);
                    stats.Area += 0.05f * (level - 1);
                    break;
            }
        }
    }
}
