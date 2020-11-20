using InfinityScript;
using System;
using System.Collections.Generic;

namespace ExtensionScript
{
    class BadWeapons
    {
        private HashSet<string> weapons;
        private HashSet<string> ks;
        public BadWeapons()
        {
            weapons = Constructor();
            ks = Constructor2();
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

            if (weapon.Contains("m320") || weapon.Contains("gl") || weapon.Contains("gp25"))       
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
    }
}
