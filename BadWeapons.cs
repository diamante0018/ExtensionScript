// ==================== ExtensionScript ===================
// Admin Manager via Rcon. It is recommended you
// Use this script with IW4M 
// Project: https://github.com/diamante0018/ExtensionScript
// Author: Diavolo (https://github.com/diamante0018)
// License: GNU GPL v3.0
// ========================================================
using InfinityScript;
using System;
using System.Collections.Generic;

namespace ExtensionScript
{
    public class BadWeapons
    {
        private readonly HashSet<string> weapons;
        private readonly HashSet<string> ks;
        private readonly HashSet<string> nukeWeapons;
        private readonly Random rng = new Random();
        private int DefaultKnifeAddress;
        private unsafe int* KnifeRange;
        private unsafe int* ZeroAddress;
        private readonly string[] specialGuns = { "iw5_mk14_mp_xmags_rof_camo11", "iw5_barrett_mp", "iw5_barrett_mp_xmags_rof_camo11", "uav_strike_marker_mp" , "airdrop_escort_marker_mp",
        "defaultweapon_mp", "iw5_usp45jugg_mp_akimbo", "iw5_m60jugg_mp", "iw5_mp412jugg_mp" , "iw5_m60jugg_mp_camo08", "iw5_m60jugg_mp_thermal_silencer_camo07" };

        public BadWeapons()
        {
            weapons = Constructor();
            ks = Constructor2();
            nukeWeapons = Constructor3();
            SetupKnife();
        }

        public void GiveHealthBack(Entity player, string weapon, int damage)
        {
            if (!player.IsPlayer)
                return;
            /*
             * Nerf Vests
             */
            if (player.Health > 100)
                player.Health = 100;

            if (weapons.Contains(weapon))
                player.Health += Math.Abs(damage - 13);


            if (ks.Contains(weapon))
                player.Health += Math.Abs(damage - 3);

            // If it's not a desert eagle *NOTE THE GL IN EAGLE* check if it's a noob tube if yes give health back
            if (!weapon.Contains("desert") && (weapon.Contains("m320") || weapon.Contains("gl") || weapon.Contains("gp25")))
                player.Health += Math.Abs(damage - 3);
        }

        private HashSet<string> Constructor()
        {
            HashSet<string> weapons = new HashSet<string>
            {
                "semtex_mp",
                "c4death_mp",
                "frag_grenade_mp",
                "rpg_mp",
                "xm25_mp",
                "m320_mp",
                "claymore_mp",
                "iw5_smaw_mp",
                "gl_mp",
                "javelin_mp",
                "bouncingbetty_mp",
                "killstreak_precision_airstrike_mp"
            };
            return weapons;
        }

        private HashSet<string> Constructor2()
        {
            HashSet<string> weapons = new HashSet<string>
            {
                "stealth_bomb_mp",
                "frag_grenade_short_mp",
                "killstreak_remote_turret_mp",
                "killstreak_stealth_airstrike_mp",
                "cobra_20mm_mp",
                "littlebird_guard_minigun_mp",
                "pavelow_minigun_mp",
                "ac130_25mm_mp",
                "ac130_40mm_mp",
                "ac130_105mm_mp",
                "osprey_minigun_mp",
                "osprey_player_minigun_mp",
                "ims_projectile_mp",
                "killstreak_ims_mp",
                "manned_minigun_turret_mp",
                "manned_gl_turret_mp",
                "ugv_turret_mp",
                "ugv_gl_turret_mp",
                "remote_turret_mp"
            };
            return weapons;
        }

        private HashSet<string> Constructor3()
        {
            HashSet<string> weapons = new HashSet<string>
            {
                "cobra_player_minigun_mp",
                "artillery_mp",
                "stealth_bomb_mp",
                "pavelow_minigun_mp",
                "sentry_minigun_mp",
                "harrier_20mm_mp",
                "ac130_25mm_mp",
                "ac130_40mm_mp",
                "ac130_105mm_mp",
                "remotemissile_projectile_mp",
                "cobra_20mm_mp",
                "nuke_mp",
                "apache_minigun_mp",
                "littlebird_guard_minigun_mp",
                "uav_strike_marker_mp",
                "osprey_minigun_mp",
                "strike_marker_mp",
                "a10_30mm_mp",
                "manned_minigun_turret_mp",
                "manned_gl_turret_mp",
                "airdrop_trap_explosive_mp",
                "uav_strike_projectile_mp",
                "remote_mortar_missile_mp",
                "manned_littlebird_sniper_mp",
                "iw5_m60jugg_mp",
                "iw5_mp412jugg_mp",
                "iw5_riotshieldjugg_mp",
                "iw5_usp45jugg_mp",
                "remote_turret_mp",
                "osprey_player_minigun_mp",
                "deployable_vest_marker_mp",
                "ugv_turret_mp",
                "ugv_gl_turret_mp" ,
                "remote_tank_projectile_mp",
                "uav_remote_mp"
            };
            return weapons;
        }

        public bool IsWeaponNukeScriptRelated(string weapon) => nukeWeapons.Contains(weapon);

        /// <summary>function <c>FindMem</c> From DG Admin I have no idea what it does.</summary>
        private unsafe int FindMem(byte?[] search, int num = 1, int start = 16777216, int end = 63963136)
        {
            try
            {
                int num1 = 0;
                for (int index1 = start; index1 < end; ++index1)
                {
                    byte* numPtr = (byte*)index1;
                    bool flag = false;
                    for (int index2 = 0; index2 < search.Length; ++index2)
                    {
                        if (search[index2].HasValue)
                        {
                            int num2 = *numPtr;
                            byte? nullable = search[index2];
                            if ((num2 != nullable.GetValueOrDefault() ? 1 : (!nullable.HasValue ? 1 : 0)) != 0)
                                break;
                        }
                        if (index2 == search.Length - 1)
                        {
                            if (num == 1)
                            {
                                flag = true;
                            }
                            else
                            {
                                ++num1;
                                if (num1 == num)
                                    flag = true;
                            }
                        }
                        else
                            ++numPtr;
                    }
                    if (flag)
                        return index1;
                }
            }
            catch (Exception ex)
            {
                Utilities.PrintToConsole("Error in FindMem: " + ex.Message);
            }
            return 0;
        }

        /// <summary>function <c>SetupKnife</c> From DG Admin I have no idea what it does.</summary>
        public unsafe void SetupKnife()
        {
            try
            {
                byte?[] search1 = new byte?[23]
                {
                  new byte?(139),
                  new byte?(),
                  new byte?(),
                  new byte?(),
                  new byte?(131),
                  new byte?(),
                  new byte?(4),
                  new byte?(),
                  new byte?(131),
                  new byte?(),
                  new byte?(12),
                  new byte?(217),
                  new byte?(),
                  new byte?(),
                  new byte?(),
                  new byte?(139),
                  new byte?(),
                  new byte?(217),
                  new byte?(),
                  new byte?(),
                  new byte?(),
                  new byte?(217),
                  new byte?(5)
                };
                KnifeRange = (int*)(FindMem(search1, 1, 4194304, 5242880) + search1.Length);
                if ((int)KnifeRange == search1.Length)
                {
                    byte?[] search2 = new byte?[25]
                    {
                        new byte?(139),
                        new byte?(),
                        new byte?(),
                        new byte?(),
                        new byte?(131),
                        new byte?(),
                        new byte?(24),
                        new byte?(),
                        new byte?(131),
                        new byte?(),
                        new byte?(12),
                        new byte?(217),
                        new byte?(),
                        new byte?(),
                        new byte?(),
                        new byte?(141),
                        new byte?(),
                        new byte?(),
                        new byte?(),
                        new byte?(217),
                        new byte?(),
                        new byte?(),
                        new byte?(),
                        new byte?(217),
                        new byte?(5)
                    };
                    KnifeRange = (int*)(FindMem(search2, 1, 4194304, 5242880) + search2.Length);
                    if ((int)KnifeRange == search2.Length)
                        KnifeRange = null;
                }
                DefaultKnifeAddress = *KnifeRange;
                byte?[] search3 = new byte?[24]
                {
                  new byte?(217),
                  new byte?(92),
                  new byte?(),
                  new byte?(),
                  new byte?(216),
                  new byte?(),
                  new byte?(),
                  new byte?(216),
                  new byte?(),
                  new byte?(),
                  new byte?(217),
                  new byte?(92),
                  new byte?(),
                  new byte?(),
                  new byte?(131),
                  new byte?(),
                  new byte?(1),
                  new byte?(15),
                  new byte?(134),
                  new byte?(),
                  new byte?(0),
                  new byte?(0),
                  new byte?(0),
                  new byte?(217)
                };
                ZeroAddress = (int*)(FindMem(search3, 1, 4194304, 5242880) + search3.Length + 2);

                if (!((int)KnifeRange != 0 && DefaultKnifeAddress != 0 && (int)ZeroAddress != 0))
                    Utilities.PrintToConsole("Error finding address: NoKnife");
            }
            catch (Exception ex)
            {
                Utilities.PrintToConsole("Error in NoKnife Plugin.");
                Utilities.PrintToConsole(ex.ToString());
            }
        }

        public bool IsKillstreakWeapon(string weapon) => weapon.Contains("ac130") || weapon.Contains("remote") || weapon.Contains("minigun");

        public unsafe void DisableKnife() => *KnifeRange = (int)ZeroAddress;

        public string GetRandomGun() => specialGuns[rng.Next(specialGuns.Length)];
    }
}