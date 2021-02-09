using InfinityScript;
using System;
using System.Collections.Generic;
using System.IO;

namespace ExtensionScript
{
    public class ChatAlias
    {
        private Dictionary<string, string> playerAlias = new Dictionary<string, string>();
        private string currentPath;

        public ChatAlias()
        {
            CreateDirectory();
            currentPath = Directory.GetCurrentDirectory() + $"\\PlayerChat\\Aliases.txt";
        }

        public void Update(string HWID, string alias)
        {
            playerAlias[HWID] = alias;
            Save();
        }

        public bool Remove(string HWID)
        {
            bool result = playerAlias.Remove(HWID);
            if (result)
                Save();
            return result;
        }

        /// <summary>function <c>CheckAlias</c> Checks if the player has an alias. Returns null if not found</summary>
        public string CheckAlias(Entity player)
        {
            if (playerAlias.TryGetValue(player.HWID, out string alias))
                return alias;

            return null;
        }

        /// <summary>function <c>Load</c> Loads the aliases from the text file. Expected format is HWID;ALIAS.</summary>
        public void Load()
        {
            if (!File.Exists(currentPath))
            {
                // Create a file to write to.
                File.CreateText(currentPath).Close();
                return;
            }

            StreamReader reader = File.OpenText(currentPath);
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                string[] tokens = line.Split(';');
                playerAlias[tokens[0]] = tokens[1];
            }

            reader.Close();
        }

        /// <summary>function <c>Save</c> Saves the whole dictionary.</summary>
        public void Save()
        {
            if (!File.Exists(currentPath))
            {
                // Create a file to write to.
                File.CreateText(currentPath).Close();
            }

            StreamWriter sw = File.CreateText(currentPath);
            string line;

            foreach (KeyValuePair<string, string> aliases in playerAlias)
            {
                line = $"{aliases.Key};{aliases.Value}";
                sw.WriteLine(line);
            }

            sw.Close();
        }

        /// <summary>function <c>CreateDirectory</c> Creates the directory.</summary>
        private void CreateDirectory()
        {
            try
            {
                DirectoryInfo di = Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\PlayerChat");
            }

            catch (Exception e)
            {
                Log.Write(LogLevel.Error, $"The process failed: {e}");
            }
        }
    }
}
