// ==================== ExtensionScript ===================
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
    public static class KickMSG
    {
        private static readonly Random rng = new Random();
        private static readonly List<string> msg = new List<string>() { "@PLATFORM_STEAM_CONNECT_FAIL", "@EXE_ERR_BANNED_TEMP", "@EXE_ERR_BANNED_PERM", "@PLATFORM_STEAM_KICK_CHEAT", "@PLATFORM_STEAM_AUTH_DENIED", "@MENU_NO_CLAN_DESCRIPTION",
        "@EXE_TRANSMITERROR", "@EXE_SERVERISFULL", "@EXE_SV_INFO_SERVERNAME", "@EXE_MATCHENDED", "@MPUI_PARTY_LOST_HOST", "@MENU_VAULT_NOGUESTACCOUNTS", "@MP_BUILDEXPIRED", "@MP_HOST_CHANGING_SETTING",
        "@MP_AUTH_NOT_ENTITLED", "@MENU_ONLINEVAULT_RESTRICTED", "@MENU_ONLINEVAULT_MPNOTALLOWED", "@EXE_SV_INFO_MAXRATE", "@MPUI_MAKEHOSTFAILED", "@MPUI_MAKEHOSTTIMEOUT", "@MP_BETACLOSED",
        "@MP_NOGOODHOST", "@MP_BANNED", "@MENU_VAULT_MUSTLOGIN", "@MP_ENDED_GAME_MIGRATION_FAILED", "@EXE_BAD_CHALLENGE", "@EXE_ERR_UNREGISTERED_CONNECTION", "@EXE_MIGRATION_IN_PROGRESS", "@MPUI_ONLINE_STATS_WARNING",
        "@MENU_MENU_COULDNT_BE_FOUND", "@XBOXLIVE_MUSTLOGIN", "@MENU_STRICTHINT", "@MENU_IWNET_MUSTLOGIN", "@MENU_IWNET_LOGIN", "@MPUI_SERVER_INFO_FAILED", "@EXE_UNPURECLIENTDETECTED", "@PLATFORM_STEAM_DISCONNECTED", "@PLATFORM_STEAM_JOIN_FAIL",
        "@PLATFORM_MISSINGMAP", "@PLATFORM_INVITE_SYSTEM_ERROR", "@EXE_DEMONWARE_DISCONNECT", "@EXE_SERVERKILLED", "@EXE_ERR_UNAUTHORIZED_IP", "@EXE_SERVER_IS_DIFFERENT_VER", "@EXE_MIGRATIONABORTED", "@EXE_MIGRATIONABORTED_BACKOUT" };

        public static string GetRandomMSG() => msg[rng.Next(msg.Count)];
    }
}