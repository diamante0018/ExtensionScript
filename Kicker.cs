// ==================== ExtensionScript ===================
// Admin Manager via Rcon. It is recommended you
// Use this script with IW4M 
// Project: https://github.com/diamante0018/ExtensionScript
// Author: Diavolo (https://github.com/diamante0018)
// License: GNU GPL v3.0
// ========================================================
using InfinityScript;

namespace ExtensionScript
{
    public class Kicker
    {
        /// <summary>function <c>Reset</c> Resets the stats of the player.</summary>
        public void Reset(Entity player)
        {
            player.SetClientDvar("com_errorMessage", "Your stats have been reset as a result of bad conduct.");
            player.SetClientDvar("com_errorResolveCommand", "defaultStatsInit");
            Utilities.ExecuteCommand($"dropclient {player.EntRef} ^1Your stats have been reset as a result of bad conduct.");
        }

        /// <summary>function <c>Close</c> Closes the game of the player and opens Steam.</summary>
        public void Close(Entity player)
        {
            player.SetClientDvar("com_errorMessage", "You are being redirected to Steam as a result of bad conduct.");
            player.SetClientDvar("com_errorResolveCommand", "startSingleplayer");
        }

        /// <summary>function <c>Silentkick</c> Kicks the player without displaying a reason.</summary>
        public void Silentkick(Entity player)
        {
            Utilities.ExecuteCommand($"dropclient {player.EntRef} \"\"");
        }

        /// <summary>function <c>Teknoban</c> Corrupts the stats of the player.</summary>
        public void Teknoban(Entity player)
        {
            string banner = "^ÿÿÿÿ";

            for (int i = 0; i < 15; i++)
            {
                player.SetPlayerData("customClasses", i, "name", banner);
                player.SetPlayerData("customClasses", i, "inUse", true);
            }

            player.SetPlayerData("experience", int.MaxValue);
            player.SetPlayerData("prestige", 69);
            player.SetPlayerData("level", int.MinValue);
            player.SetPlayerData("kills", -1);
            player.SetPlayerData("playerXuidLow", int.MinValue);
            player.SetPlayerData("playerXuidHigh", int.MaxValue);
        }

        /// <summary>function <c>Crasher</c> Closes the game of the player abruptly.</summary>
        public void Crasher(Entity player)
        {
            player.SetPlayerData("persistentWeaponsUnlocked", "iw5_m60jugg", 1);
            Utilities.ExecuteCommand($"dropclient {player.EntRef} \"\"");
        }
    }
}