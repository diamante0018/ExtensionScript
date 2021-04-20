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
using System.IO;

namespace ExtensionScript
{
    public class ChatAlias
    {
        private Dictionary<string, string> playerAliases = new Dictionary<string, string>();
        private string currentPath;
        private bool loaded;

        public bool Loaded
        {
            get { return loaded; }
        }

        public ChatAlias()
        {
            loaded = false;
            CreateDirectory();
            currentPath = Directory.GetCurrentDirectory() + "\\PlayerChat\\Aliases.txt";
        }

        public void Update(string HWID, string alias)
        {
            playerAliases[HWID] = alias;
            Save();
        }

        public bool Remove(string HWID)
        {
            bool result = playerAliases.Remove(HWID);
            if (result)
                Save();
            return result;
        }

        /// <summary>function <c>CheckAlias</c> Checks if the player has an alias. Returns null if not found</summary>
        public string CheckAlias(Entity player)
        {
            if (playerAliases.TryGetValue(player.HWID.Substring(0, 16), out string alias))
                return alias;

            return null;
        }

        /// <summary>function <c>Load</c> Loads the aliases from the text file. Expected format is HWID;ALIAS.</summary>
        public void Load()
        {
            if (!File.Exists(currentPath))
            {
                // Create a file to read from
                using (var sw = File.CreateText(currentPath))
                {
                    Utilities.PrintToConsole($"File {currentPath} did not exist");
                }

                return;
            }

            loaded = true;
            using (var reader = File.OpenText(currentPath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] tokens = line.Split(';');
                    playerAliases[tokens[0]] = tokens[1];
                }
            }
        }

        /// <summary>function <c>Save</c> Saves the whole dictionary.</summary>
        public void Save()
        {
            using (var sw = File.CreateText(currentPath))
            {
                string line;
                foreach (KeyValuePair<string, string> alias in playerAliases)
                {
                    line = $"{alias.Key};{alias.Value}";
                    sw.WriteLine(line);
                }
            }
        }

        /// <summary>function <c>CreateDirectory</c> Creates the directory.</summary>
        private void CreateDirectory()
        {
            try
            {
                var di = Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\PlayerChat");
            }

            catch (Exception e)
            {
                Log.Write(LogLevel.Error, $"The process failed to create directory PlayerChat: {e}");
            }
        }
    }
}