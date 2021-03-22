// ==================== ExtensionScript ===================
// Admin Manager via Rcon. It is recommended you
// Use this script with IW4M 
// Project: https://github.com/diamante0018/ExtensionScript
// Author: Diavolo (https://github.com/diamante0018)
// License: GNU GPL v3.0
// ========================================================
using InfinityScript;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using static InfinityScript.GSCFunctions;

namespace ExtensionScript
{
    public static class Mem
    {
        public static unsafe string ReadString(int address, int maxlen = 0)
        {
            string ret = "";
            maxlen = (maxlen == 0) ? int.MaxValue : maxlen;

            for (; address < address + maxlen && *(byte*)address != 0; address++)
            {
                ret += Encoding.ASCII.GetString(new byte[] { *(byte*)address });
            }

            return ret;
        }

        public static unsafe void WriteString(int address, string str)
        {
            byte[] strarr = Encoding.ASCII.GetBytes(str);

            foreach (byte ch in strarr)
            {
                *(byte*)address = ch;
                address++;
            }

            *(byte*)address = 0;
        }
    }

    public class ServerUtils
    {
        public Dictionary<string, bool> GenerateServerDvarsDict()
        {
            var dvars = new Dictionary<string, bool>
            {
                ["sv_Bounce"] = GetDvarInt("sv_Bounce") == 1,
                ["sv_KnifeEnabled"] = GetDvarInt("sv_KnifeEnabled") == 1,
                ["sv_UndoRCE"] = GetDvarInt("sv_UndoRCE") == 1,
                ["sv_RemoveBakaaraSentry"] = GetDvarInt("sv_RemoveBakaaraSentry") == 1,
                ["sv_hideCommands"] = GetDvarInt("sv_hideCommands") != 0,
                ["sv_AntiHardScope"] = GetDvarInt("sv_AntiHardScope") == 1,
                ["sv_AntiCamp"] = GetDvarInt("sv_AntiCamp") == 1,
                ["sv_autoBalance"] = GetDvarInt("sv_autoBalance") == 1,
                ["sv_LastStand"] = GetDvarInt("sv_LastStand") == 0,
                ["sv_NerfGuns"] = GetDvarInt("sv_NerfGuns") == 1,
                ["sv_ExplosivePrank"] = GetDvarInt("sv_ExplosivePrank") == 1,
                ["sv_DisableAkimbo"] = GetDvarInt("sv_DisableAkimbo") == 1,
                ["sv_AllPerks"] = GetDvarInt("sv_AllPerks") == 1,
                ["sv_LocalizedStr"] = GetDvarInt("sv_LocalizedStr") == 1,
                ["sv_AntiRQ"] = GetDvarInt("sv_AntiRQ") == 1,
                ["sv_MaxAmmoFillsClip"] = GetDvarInt("sv_MaxAmmoFillsClip") == 1
            };

            return dvars;
        }

        public Dictionary<string, string> GenerateKeyWordsDict()
        {
            string ball = "cardicon_8ball";
            string face = "facebook";

            var keyWords = new Dictionary<string, string>()
            {
                { "null", "\0" },
                { "controldel",  "\x7F" },
                { "control3", "\u0080\u009F\u001F" },
                { "controlone", "\x01\x01\x01" },
                { "xp", "^OOxp" },
                { "crash", "\x5e\x01\xCC\xCC\x0Ashellshock" },
                { "weird", "� ^������" },
                { "weird2", "^ÿÿÿÿ" },
                { "8ball", $"\x5E\x01\x3F\x3F\x0E{ball}" },
                { "face", $"\x5E\x01\x3F\x3F\x0F{face}" },
                { "cod2", "\x5E\x33\x5E\x01\x7F\x2F\x09\x6C\x6F\x67\x6F\x5F\x63\x6F\x64\x32\x5E\x01\x32\x2F\x07\x75\x69\x5F\x68\x6F\x73\x74\x5E\x01\x40" }
            };

            return keyWords;
        }

        public CultureInfo TryGetColture()
        {
            CultureInfo culture;

            try
            {
                string dvarCulture = GetDvar("sv_serverCulture");

                if (string.IsNullOrWhiteSpace(dvarCulture))
                    dvarCulture = "en-GB";

                culture = new CultureInfo(dvarCulture, false);
            }

            catch (CultureNotFoundException)
            {
                InfinityScript.Log.Write(LogLevel.Error, "Invalid Culture: Set correctly sv_serverCulture string dvar");
                culture = new CultureInfo("en-GB", false);
            }

            return culture;
        }
    }
}