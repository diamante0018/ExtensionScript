// ==================== ExtensionScript ===================
// Admin Manager via Rcon. It is recommended you
// Use this script with IW4M 
// Project: https://github.com/diamante0018/ExtensionScript
// Author: Diavolo (https://github.com/diamante0018)
// License: GNU GPL v3.0
// ========================================================
using InfinityScript;
using System;
using System.Linq;
using static InfinityScript.GSCFunctions;

namespace ExtensionScript
{
    public class LoadoutName
    {
        private static readonly Random random = new Random(GetTime());
        private Entity player;
        public LoadoutName(Entity player, bool isRandom = true)
        {
            this.player = player;
            switch (isRandom)
            {
                case true:
                    ChangeLoadOutNameRandom();
                    break;
                case false: //TODO
                    break;
                default:
                    break;
            }
        }

        /// <summary>function <c>ChangeLoadOutNameRandom</c> Adds some color to the class loadout text. Text is random.</summary>
        public void ChangeLoadOutNameRandom()
        {
            for (int i = 0; i < 10; i++)
            {
                player.SetPlayerData("customClasses", i, "name", string.Format("^{0}{1}", i, RandomString(10)));
                player.SetPlayerData("customClasses", i, "inUse", true);
            }
        }

        public static string RandomString(int length, bool ultraSpam = false)
        {
            string chars = ultraSpam ? "^^^^^^^ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz012345678901234567890123456789" : "^^^^^^^ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz012345678901234567890123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}