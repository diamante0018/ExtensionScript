// ==================== ExtensionScript ===================
// Admin Manager via Rcon. It is recommended you
// use this script with IW4M 
// Project: https://github.com/diamante0018/ExtensionScript
// Author: Diavolo (https://github.com/diamante0018)
// License: GNU GPL v3.0
// ========================================================
using InfinityScript;
using System;
using System.Collections.Generic;

namespace ExtensionScript
{
    class BadWeapons
    {
        private HashSet<string> weapons;
        private HashSet<string> ks;
        private int DefaultKnifeAddress;
        private unsafe int* KnifeRange;
        private unsafe int* ZeroAddress;

        public BadWeapons()
        {
            weapons = Constructor();
            ks = Constructor2();
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

            if (!weapon.Contains("desert") && (weapon.Contains("m320") || weapon.Contains("gp25")))
                player.Health += Math.Abs(damage - 3);
        }

        private HashSet<string> Constructor()
        {
            HashSet<string> weapons = new HashSet<string>();
            weapons.Add("semtex_mp");
            weapons.Add("c4death_mp");
            weapons.Add("frag_grenade_mp");
            weapons.Add("rpg_mp");
            weapons.Add("xm25_mp");
            weapons.Add("m320_mp");
            weapons.Add("claymore_mp");
            weapons.Add("iw5_smaw_mp");
            weapons.Add("gl_mp");
            weapons.Add("javelin_mp");
            weapons.Add("bouncingbetty_mp");
            weapons.Add("killstreak_precision_airstrike_mp");
            return weapons;
        }

        private HashSet<string> Constructor2()
        {
            HashSet<string> weapons = new HashSet<string>();
            weapons.Add("stealth_bomb_mp");
            weapons.Add("frag_grenade_short_mp");
            weapons.Add("killstreak_remote_turret_mp");
            weapons.Add("killstreak_stealth_airstrike_mp");
            weapons.Add("cobra_20mm_mp");
            weapons.Add("littlebird_guard_minigun_mp");
            weapons.Add("pavelow_minigun_mp");
            weapons.Add("ac130_25mm_mp");
            weapons.Add("ac130_40mm_mp");
            weapons.Add("osprey_minigun_mp");
            weapons.Add("osprey_player_minigun_mp");
            weapons.Add("ims_projectile_mp");
            weapons.Add("killstreak_ims_mp");
            weapons.Add("manned_minigun_turret_mp");
            weapons.Add("manned_gl_turret_mp");
            weapons.Add("ugv_turret_mp");
            weapons.Add("ugv_gl_turret_mp");
            weapons.Add("remote_turret_mp");
            return weapons;
        }

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

        public unsafe void DisableKnife() => *KnifeRange = (int)ZeroAddress;
    }
}