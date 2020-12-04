// ==================== ExtensionScript ===================
// Admin Manager via Rcon. It is recommended you
// use this script with IW4M 
// Project: https://github.com/diamante0018/ExtensionScript
// Author: Diavolo (https://github.com/diamante0018)
// License: GNU GPL v3.0
// ========================================================
using InfinityScript;
using System.Collections.Generic;
using System.Text;

namespace ExtensionScript
{
    public static class ExtFunc
    {
        private static Dictionary<string, Dictionary<string, int>> fields = new Dictionary<string, Dictionary<string, int>>();

        /// <summary>function <c>SetClanTag</c> Sets the clantag of the player. A clantag must be already present (active) for it to be changed. It resets after UAV is called, team is changed or map rotates.</summary>
        public static unsafe void SetClanTag(this Entity player, string tag)
        {
            if (player == null || !player.IsPlayer || tag.Length > 8)
                return;

            int address = 0x38A4 * player.EntRef + 0x01AC5564;

            for (int i = 0; i < tag.Length; i++)
                *(byte*)(address + i) = (byte)tag[i];

            *(byte*)(address + tag.Length) = 0;
        }

        /// <summary>function <c>GetClanTag</c> Gets the clantag of the player.</summary>
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

        /// <summary>function <c>SetName</c> Sets the nickname of the player. Same conditions of SetClanTag apply.</summary>
        public static unsafe string SetName(this Entity player, string name)
        {
            if (player == null || !player.IsPlayer || name.Length > 15)
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
            byte set = 0x01;
            if (player.HasNoClip())
                set = 0x00;

            int address = 0x38A4 * player.EntRef + 0x01AC56C0;
            *(byte*)address = set;
        }

        public static unsafe bool HasNoClip(this Entity player) => *(byte*)(0x38A4 * player.EntRef + 0x01AC56C0) == 1;

        public static void MyGiveMaxAmmo(this Entity player, bool feedback = true)
        {
            string gun = player.GetCurrentWeapon();
            player.GiveStartAmmo(gun);
            player.GiveMaxAmmo(gun);
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

        /// <summary>function <c>ChangeTeam</c> Changes the team of the player.</summary>
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
                    player.ChangeTeam("allies");
                    break;
                case "axis":
                    player.ChangeTeam("axis");
                    break;
                default:
                    break;
            }
        }

        public static void TellPlayer(this Entity player, string text = "^5Welcome")
        {
            HudElem welcomer = HudElem.CreateFontString(player, HudElem.Fonts.Objective, 1.8f);
            welcomer.SetPoint("CENTER", "CENTER", 0, -110);
            welcomer.SetText(text);
            welcomer.GlowAlpha = 1f;
            welcomer.SetPulseFX(100, 0x1b58, 600);
            welcomer.HideWhenInMenu = true;
        }

        public static void MySetField(this Entity player, string field, int value)
        {
            if (!player.IsPlayer)
                return;
            if (!fields.ContainsKey(player.HWID))
                fields.Add(player.HWID, new Dictionary<string, int>());

            if (!MyHasField(player, field))
                fields[player.HWID].Add(field, value);
            else
                fields[player.HWID][field] = value;
        }

        public static int MyGetField(this Entity player, string field)
        {
            if (!player.IsPlayer)
                return int.MinValue;
            if (!MyHasField(player, field))
                return int.MinValue;
            return fields[player.HWID][field];
        }

        public static void MyRemoveField(this Entity player) => fields.Remove(player.HWID);
    }
}
