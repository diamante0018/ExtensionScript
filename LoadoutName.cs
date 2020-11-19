using InfinityScript;
using System;
using System.Linq;

namespace ExtensionScript
{
    class LoadoutName
    {
        private static Random random = new Random();
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

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
