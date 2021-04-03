using InfinityScript;
using System;
using System.Collections.Generic;

namespace ExtensionScript
{
    public class Connection
    {
        private HashSet<string> playerDat;

        public Connection()
        {
            playerDat = new HashSet<string>();
        }

        /// <summary>function <c>GetHWID</c> Gets the first token from playerHWID string.</summary>
        public string GetHWID(string playerHWID)
        {
            string[] kv = playerHWID.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            return kv[0];
        }

        public bool CheckPlayerData(string playerConString)
        {
            string playerHWID = GetHWID(playerConString);
            string playerXUID = GetValueForKey(playerConString, "xuid");
            string playerxnaddr = GetValueForKey(playerConString, "xnaddr");
            string playerSteamID = GetValueForKey(playerConString, "steamid");
            string playerName = GetValueForKey(playerConString, "name");
            string IP = GetValueForKey(playerConString, "IP-string");

            if (playerDat.Contains($"{playerXUID}.{playerxnaddr}") || playerDat.Contains($"{playerXUID}.{playerSteamID}"))
            {
                InsertBan.WriteIPBan(IP, playerName, "Attempted to use illegal tools to kick online players");
                return true;
            }

            foreach (var player in ExtensionScript.onlinePlayers)
            {
                if (player.HWID.ToLower() == playerHWID || player.GetXUID() == playerXUID || playerName == player.Name)
                {
                    InsertBan.WriteIPBan(IP, playerName, "Attempted to use illegal tools to kick online players");
                    return true;
                }
            }

            Utilities.PrintToConsole($"{playerName} is allowed to connect because no other player shares his data");
            playerDat.Add($"{playerXUID}.{playerxnaddr}");
            playerDat.Add($"{playerXUID}.{playerSteamID}");

            return false;
        }

        public void RemovePlayerDat(Entity player)
        {
            playerDat.RemoveWhere(s => s.Contains(player.GetXUID()));
        }

        /// <summary>function <c>MyGetValueForKey</c> Another implementation for GetValueForKey. If no value for key is found key is returned.</summary>
        public string GetValueForKey(string longString, string key)
        {
            string[] kv = longString.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            int index = Array.FindIndex(kv, x => x == key);
            return (index + 1 < kv.Length) ? kv[index + 1] : kv[index];
        }
    }
}
