﻿// ==================== ExtensionScript ===================
// Admin Manager via Rcon. It is recommended you
// Use this script with IW4M 
// Project: https://github.com/diamante0018/ExtensionScript
// Author: Diavolo (https://github.com/diamante0018)
// License: GNU GPL v3.0
// ========================================================
using System;
using System.Collections.Generic;

namespace ExtensionScript
{
    public class RandomMap
    {
        private readonly Random rng = new Random();
        private readonly List<string> AllMapList = new List<string>() { "mp_alpha", "mp_bootleg", "mp_bravo", "mp_carbon", "mp_dome",
        "mp_exchange", "mp_hardhat", "mp_interchange", "mp_lambeth", "mp_mogadishu", "mp_paris", "mp_plaza2",
        "mp_radar", "mp_seatown", "mp_underground", "mp_village", "mp_italy", "mp_park", "mp_morningwood", "mp_overwatch", "mp_aground_ss",
        "mp_courtyard_ss", "mp_cement", "mp_hillside_ss", "mp_meteora", "mp_qadeem", "mp_restrepo_ss", "mp_terminal_cls", "mp_crosswalk_ss",
        "mp_six_ss", "mp_burn_ss", "mp_shipbreaker", "mp_roughneck", "mp_nola", "mp_moab" };

        /// <summary>function <c>GetRandomMap</c> Gets a random map to rotate to.</summary>
        public string GetRandomMap() => AllMapList[rng.Next(AllMapList.Count)];

        /// <summary>function <c>IsValidMap</c> Checks if map is in the map list.</summary>
        public bool IsValidMap(string map) => AllMapList.Contains(map);
    }
}