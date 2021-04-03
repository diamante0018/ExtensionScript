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
using System.Text;
using static InfinityScript.GSCFunctions;
using static InfinityScript.HudElem;

namespace ExtensionScript
{
    public static class ExtFunc
    {
        private static Dictionary<string, Dictionary<string, Parameter>> fields = new Dictionary<string, Dictionary<string, Parameter>>();
        private static readonly byte[] utility = { 0x00, 0x01 };

        /// <summary>function <c>SetClanTag</c> Sets the clantag of the player. A clantag must already be present for this function to work (active). It resets to its original state after some unknown in-game actions.</summary>
        public static unsafe void SetClanTag(this Entity player, string tag)
        {
            if (player == null || !player.IsPlayer)
                return;

            int address = 0x38A4 * player.EntRef + 0x01AC5564;

            for (int i = 0; i < tag.Length; i++)
                *(byte*)(address + i) = (byte)tag[i];

            *(byte*)(address + tag.Length) = 0;
        }

        /// <summary>function <c>SetPlayerTitle</c> Sets the player title of the player. A title must already be present for this function to work (active). It resets to its original state after some unknown in-game actions.</summary>
        public static unsafe void SetPlayerTitle(this Entity player, string title)
        {
            if (player == null || !player.IsPlayer || title.Length > 25)
                return;

            int address = 0x38A4 * player.EntRef + 0x01AC5548;

            for (int i = 0; i < title.Length; i++)
                *(byte*)(address + i) = (byte)title[i];

            *(byte*)(address + title.Length) = 0;
        }

        /// <summary>function <c>GetPlayerTitle</c> Gets the player title from the player card if present.</summary>
        public static unsafe string GetPlayerTitle(this Entity player)
        {
            if (player == null || !player.IsPlayer)
                return null;

            int address = 0x38A4 * player.EntRef + 0x01AC5548;

            StringBuilder result = new StringBuilder();

            for (; address < address + 8 && *(byte*)address != 0; address++)
                result.Append(Encoding.ASCII.GetString(new byte[] { *(byte*)address }));

            return result.ToString();
        }

        /// <summary>function <c>GetClanTag</c> Gets the clantag of the player if present.</summary>
        public static unsafe string GetClanTag(this Entity player)
        {
            if (player == null || !player.IsPlayer)
                return null;

            int address = 0x38A4 * player.EntRef + 0x01AC5564;

            StringBuilder result = new StringBuilder();

            for (; address < address + 8 && *(byte*)address != 0; address++)
                result.Append(Encoding.ASCII.GetString(new byte[] { *(byte*)address }));

            return result.ToString();
        }

        /// <summary>function <c>SetName</c> Sets the nickname of the player. Same conditions of SetClanTag/SetPlayerTitle apply.</summary>
        public static unsafe string SetName(this Entity player, string name)
        {
            if (player == null || !player.IsPlayer)
                return null;

            for (int i = 0; i < name.Length; i++)
            {
                *(byte*)((0x38A4 * player.EntRef + 0x1AC5490) + i) = (byte)name[i];
                *(byte*)((0x38A4 * player.EntRef + 0x1AC5508) + i) = (byte)name[i];
            }

            return name;
        }

        /// <summary>function <c>NoClip</c> Makes the player noclip.</summary>
        public static unsafe void NoClip(this Entity player)
        {
            byte set = player.HasNoClip() ? utility[0] : utility[1];

            int address = 0x38A4 * player.EntRef + 0x01AC56C0;
            *(byte*)address = set;
        }

        /// <summary>function <c>HasNoClip</c> Check if the player is already no-clipping.</summary>
        public static unsafe bool HasNoClip(this Entity player) => *(byte*)(0x38A4 * player.EntRef + 0x01AC56C0) == 1;

        public static void MyGiveMaxAmmo(this Entity player, bool feedback = true, bool fillClip = true)
        {
            string gun = player.GetCurrentWeapon();
            player.GiveMaxAmmo(gun);

            if (fillClip)
            {
                player.GiveStartAmmo(gun);
            }

            if (feedback)
            {
                player.PlayLocalSound("mp_suitcase_pickup");
                player.IPrintLnBold("^1Wow^0! ^3You have ^7received ^1Ammunition");
            }
        }

        public static bool MyHasField(this Entity player, string field)
        {
            if (!player.IsPlayer)
                return false;
            if (fields.ContainsKey(player.HWID))
                return fields[player.HWID].ContainsKey(field);
            return false;
        }

        /// <summary>
        /// Changes the team of the player
        /// </summary>
        /// <param name="team">Team</param>
        public static void ChangeTeam(this Entity player, string team)
        {
            player.SessionTeam = team;
            player.Notify("menuresponse", "team_marinesopfor", team);
        }

        public static void ChangeTeam(this Entity player)
        {
            switch (player.SessionTeam)
            {
                case "allies":
                    player.ChangeTeam("axis");
                    break;
                case "axis":
                    player.ChangeTeam("allies");
                    break;
                default:
                    break;
            }
        }

        public static void TellPlayer(this Entity player, string text = "^5Welcome")
        {
            HudElem welcomer = CreateFontString(player, Fonts.Objective, 1.8f);
            welcomer.SetPoint("CENTER", "CENTER", 0, -110);
            welcomer.SetText(text);
            welcomer.GlowAlpha = 1f;
            welcomer.SetPulseFX(100, 0x1b58, 600);
            welcomer.HideWhenInMenu = true;
        }

        public static void MySetField(this Entity player, string field, Parameter value)
        {
            if (!player.IsPlayer)
                return;
            if (!fields.ContainsKey(player.HWID))
                fields.Add(player.HWID, new Dictionary<string, Parameter>());

            fields[player.HWID][field] = value;
        }

        public static Parameter MyGetField(this Entity player, string field)
        {
            if (!player.IsPlayer)
                return new Parameter(int.MinValue);
            if (!MyHasField(player, field))
                return new Parameter(int.MinValue);
            return fields[player.HWID][field];
        }

        public static void MyRemoveField(this Entity player) => fields.Remove(player.HWID);

        public static HudElem CreateTemplateOverlay(this Entity player, string shader = "")
        {
            HudElem overlay = NewClientHudElem(player);
            overlay.X = 0;
            overlay.Y = 0;
            overlay.AlignX = XAlignments.Left;
            overlay.AlignY = YAlignments.Top;
            overlay.HorzAlign = HorzAlignments.Fullscreen;
            overlay.VertAlign = VertAlignments.Fullscreen;
            overlay.SetShader(shader, 640, 480);
            overlay.Sort = -10;
            overlay.Alpha = 1;
            return overlay;
        }

        public static void GiveJuggSuit(this Entity player)
        {
            if (player.SessionTeam == "spectator")
                return;

            player.DetachAll();
            player.ShowAllParts();
            player.SetViewModel("viewhands_juggernaut_opforce");
            player.SetModel("mp_fullbody_opforce_juggernaut");
            HudElem element = player.CreateTemplateOverlay("goggles_overlay");
            player.MySetField("juggernaut", element);
            player.Health += 2500;
            player.EnableWeaponPickup();
            player.TellPlayer("^2You ^7Have Been ^6Given ^7a ^1Jugg ^0Suit");
        }

        public static void ExplodePlayer(this Entity player, bool tellThem = true)
        {
            if (player.SessionTeam == "spectator")
                return;

            Vector3 offset1 = player.Origin;
            Vector3 offset2 = player.Origin;
            offset1.Z -= 1000f;
            offset2.Z += 6000f;
            MagicBullet("uav_strike_projectile_mp", offset2, offset1, player);
            offset2.X += 2000f;
            MagicBullet("uav_strike_projectile_mp", offset2, offset1, player);
            offset2.X -= 4000f;
            MagicBullet("uav_strike_projectile_mp", offset2, offset1, player);

            if (tellThem)
                player.TellPlayer("You have been ^1Killed ^7in a ^2very ^6Fancy ^7Way^0!");
        }

        /// <summary>function <c>CheckLocalized</c> If the player title starts with @, the @ is removed.</summary>
        public static void CheckLocalized(this Entity player)
        {
            if (player.GetPlayerTitle().StartsWith("@"))
                player.SetPlayerTitle(player.GetPlayerTitle().Substring(1));
        }

        /// <summary>function <c>NoWeaponEnable</c> Takes away all weapons from the player and disables weapon pickup.</summary>
        public static void NoWeaponEnable(this Entity player)
        {
            player.TakeAllWeapons();
            player.DisableWeaponSwitch();
            player.DisableWeaponPickup();
            player.DisableWeapons();
        }

        /// <summary>function <c>NoWeaponDisable</c> Restores player's ability to pickup weapons and gives him a USP45.</summary>
        public static void NoWeaponDisable(this Entity player)
        {
            player.EnableWeaponSwitch();
            player.EnableWeaponPickup();
            player.EnableWeapons();
            player.GiveWeapon("iw5_usp45_mp");
            player.SwitchToWeaponImmediate("iw5_usp45_mp");
        }

        /// <summary>function <c>Suicide</c> Kills the player and displays a death message.</summary>
        public static void Suicide(this Entity player, string deathMsg)
        {
            player.TellPlayer(deathMsg);
            player.Suicide();
        }

        /// <summary>function <c>GiveAC130</c> Gives AC130 to the player.</summary>
        public static void GiveAC130(this Entity player)
        {
            player.TakeAllWeapons();
            player.GiveWeapon("ac130_105mm_mp");
            player.GiveWeapon("ac130_40mm_mp");
            player.GiveWeapon("ac130_25mm_mp");
            player.SwitchToWeaponImmediate("ac130_25mm_mp");
        }

        /// <summary>function <c>ThirdPerson</c> Enables third person.</summary>
        public static void ThirdPerson(this Entity player)
        {
            player.SetClientDvar("cg_thirdperson", true);
            player.SetClientDvar("cg_thirdPersonRange", 170f);
        }

        /// <summary>function <c>FirstPerson</c> Enables first person.</summary>
        public static void FirstPerson(this Entity player)
        {
            player.SetClientDvar("cg_thirdperson", false);
        }

        /// <summary>function <c>GiveAllPerks</c> Gives all perks.</summary>
        public static void GiveAllPerks(this Entity player)
        {
            player.SetPerk("specialty_longersprint", true, false);
            player.SetPerk("specialty_fastreload", true, false);
            player.SetPerk("specialty_scavenger", true, false);
            player.SetPerk("specialty_blindeye", true, false);
            player.SetPerk("specialty_paint", true, false);
            player.SetPerk("specialty_hardline", true, false);
            player.SetPerk("specialty_coldblooded", true, false);
            player.SetPerk("specialty_quickdraw", true, false);
            player.SetPerk("specialty_twoprimaries", true, false);
            player.SetPerk("specialty_assists", true, false);
            player.SetPerk("specialty_blastshield", true, false);
            player.SetPerk("specialty_detectexplosive", true, false);
            player.SetPerk("specialty_autospot", true, false);
            player.SetPerk("specialty_bulletaccuracy", true, false);
            player.SetPerk("specialty_quieter", true, false);
            player.SetPerk("specialty_stalker", true, false);
            player.SetPerk("specialty_falldamage", true, false);
        }

        /// <summary>function <c>GiveSpecialGuns</c> Gives special guns.</summary>
        public static void GiveSpecialGuns(this Entity player)
        {
            player.TakeAllWeapons();
            player.GiveWeapon("at4_mp");
            player.GiveWeapon("uav_strike_marker_mp");
            player.SwitchToWeapon("at4_mp");
        }

        /// <summary>function <c>SVClientDvars</c> Sets some useful dvars.</summary>
        public static void SVClientDvars(this Entity player)
        {
            player.SetClientDvar("cg_objectiveText", GetDvar("sv_objText"));
            player.SetClientDvar("sys_lockThreads", "all");
            player.SetClientDvar("com_maxFrameTime", 1000);
            player.SetClientDvars("snaps", 30, "rate", GetDvar("sv_rate"));
            player.SetClientDvars("g_teamicon_allies", "weapon_missing_image", "g_teamicon_MyAllies", "weapon_missing_image", "g_teamicon_EnemyAllies", "weapon_missing_image");
            player.SetClientDvars("g_teamicon_axis", "weapon_missing_image", "g_teamicon_MyAxis", "weapon_missing_image", "g_teamicon_EnemyAxis", "weapon_missing_image");
        }

        /// <summary>function <c>ClosePlayerMeny</c> Closes player menu.</summary>
        public static void ClosePlayerMenu(this Entity player)
        {
            player.ClosePopUpMenu("");
            player.CloseInGameMenu();
        }

        /// <summary>function <c>GetIPInt</c> Gets the unsigned integer representing the IP of the player.</summary>
        public static uint GetIPInt(this Entity player)
        {
            var address = player.IP.Address;
            byte[] bytes = address.GetAddressBytes();

            // Flip big-endian (network order) to little-endian
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return BitConverter.ToUInt32(bytes, 0);
        }
    }
}