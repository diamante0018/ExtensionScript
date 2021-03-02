// ==================== ExtensionScript ===================
// Admin Manager via Rcon. It is recommended you
// Use this script with IW4M 
// Project: https://github.com/diamante0018/ExtensionScript
// Author: Diavolo (https://github.com/diamante0018)
// License: GNU GPL v3.0
// ========================================================
using InfinityScript;
using System.Collections.Generic;

namespace ExtensionScript
{
    class Teleporter
    {
        private Dictionary<long, Vector3> locations = new Dictionary<long, Vector3>();

        public void Teleport2Players(Entity player, Entity target) => player.SetOrigin(target.Origin);

        public void Save(string playerToParse, string locationToParse)
        {
            Entity player = GetPlayer(playerToParse);

            if (long.TryParse(locationToParse, out long locNum))
                Utilities.SayTo(player, $"^2Parsed Number ^7{locNum}");

            else
            {
                Utilities.SayTo(player, $"^1Ivalid Data ^2Couldn't parse input: ^1{locationToParse}");
                return;
            }

            if (locations.ContainsKey(locNum))
            {
                locations[locNum] = player.Origin;
                Utilities.SayTo(player, $"^2Location was replaced! ^0{locNum}");
                return;
            }

            locations.Add(locNum, player.Origin);
            Utilities.SayTo(player, $"^2Added Location! ^0{locNum} ^7coordinates: ^2{player.Origin}");
        }

        public void Load(string playerToParse, string locationToParse)
        {
            Entity player = GetPlayer(playerToParse);

            if (long.TryParse(locationToParse, out long locNum))
                Utilities.SayTo(player, $"^2Parsed Number ^7{locNum}");

            else
            {
                Utilities.SayTo(player, $"^1Ivalid Data ^2Couldn't parse input: ^1{locationToParse}");
                return;
            }

            if (!locations.ContainsKey(locNum))
            {
                Utilities.SayTo(player, $"^1No location was found under that number: ^0{locNum}");
                return;
            }

            player.SetOrigin(locations[locNum]);
            Utilities.SayTo(player, $"^2Teleporting to ^1{locNum} ^7{locations[locNum]}");
        }

        private Entity GetPlayer(string entRef)
        {
            int.TryParse(entRef, out int IntegerentRef);
            return Entity.GetEntity(IntegerentRef);
        }
    }
}