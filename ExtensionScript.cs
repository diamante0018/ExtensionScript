// ==================== ExtensionScript ===================
// Admin Manager via Rcon. It is recommended you
// Use this script with IW4M 
// Project: https://github.com/diamante0018/ExtensionScript
// Author: Diavolo (https://github.com/diamante0018)
// License: GNU GPL v3.0
// ========================================================
using InfinityScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using static InfinityScript.GSCFunctions;
using static InfinityScript.HudElem;
using static InfinityScript.ThreadScript;

namespace ExtensionScript
{
    public class ExtensionScript : BaseScript
    {
        private static HudElem[] KillStreakHud = new HudElem[18];
        private static HudElem[] NoKillsHudElem = new HudElem[18];
        private HudElem top;
        private HudElem bottom;
        private HudElem right;
        private HudElem left;
        private string MapRotation = "";
        private Kicker proKicker = new Kicker();
        private Teleporter teleport = new Teleporter();
        private Weapons weapons = new Weapons();
        private Server sv = new Server();
        private ChatAlias chat = new ChatAlias();
        private RandomMap map;
        private LoadoutName load;
        private bool fallDamage = true;
        private bool closedServer = false;
        private int sv_balanceInterval;
        private int lastPlayerDamaged;
        private List<Entity> onlinePlayers = new List<Entity>();
        private Dictionary<string, string> keyWords;
        private static Dictionary<string, bool> dvars;
        private CultureInfo culture;

        public ExtensionScript()
        {
            IPrintLn("^1I am Diavolo and I lost my Mind. ^7DIA Script for 1.5 IS");
            InfinityScript.Log.Write(LogLevel.Info, "^1I am Diavolo and I lost my Mind.");
            sv.ScriptDvars();
            sv.ServerTitle(GetDvar("sv_MyMapName"), GetDvar("sv_MyGameMode"));
            InitClassFields();
            //sv.MaxClients(69); // May cause crashes

            //Loading Server Dvars.
            sv.ServerDvars();

            //HudElem For Information
            InformationHuds();

            //Assigning things.
            PlayerConnected += OnPlayerConnect;

            OnInterval(50, () =>
            {
                if (GetDvar("sv_b3Execute") != "undefined")
                {
                    string content = GetDvar("sv_b3Execute");
                    ProcessCommand(content);
                    SetDvar("sv_b3Execute", "undefined");
                }
                return true;
            });

            unsafe
            {
                if (dvars["sv_Bounce"])
                {
                    int[] addr = { 0x0422AB6, 0x0422AAF, 0x041E00C, 0x0414127, 0x04141B4, 0x0414E027, 0x0414B126, 0x041416B, 0x041417C };

                    byte nop = 0x90;
                    for (int i = 0; i < 7; ++i)
                    {
                        *((byte*)addr[7] + i) = nop;
                        *((byte*)addr[8] + i) = nop;
                        *(byte*)addr[i] = nop;
                        *(byte*)(addr[i] + 1) = nop;
                    }
                }

                if (dvars["sv_UndoRCE"])
                {
                    int addr = 0x04E6170;
                    *(byte*)addr = 0x81;
                    Utilities.PrintToConsole("You undid the RCE Patch");
                }

                //Undo something else pesky Tekno devs did.
                int userInfo = 0x4E7490;
                *(byte*)userInfo = 0xA1;

                int QueryInfo = 0x04EBCF4;
                *(byte*)QueryInfo = 0xE8;
                *(byte*)(QueryInfo + 1) = 0x97;
                *(byte*)(QueryInfo + 2) = 0x87;
                *(byte*)(QueryInfo + 3) = 0xFF;
                *(byte*)(QueryInfo + 4) = 0xFF;

                int NetSendPacket = 0x526443;
                *(byte*)NetSendPacket = 0xE8;
                *(byte*)(NetSendPacket + 1) = 0x18;
                *(byte*)(NetSendPacket + 2) = 0x66;
                *(byte*)(NetSendPacket + 3) = 0xFA;
                *(byte*)(NetSendPacket + 4) = 0xFF;

                int ProcessUICommand = 0x04800B0;
                *(byte*)ProcessUICommand = 0x81;
                *(byte*)(ProcessUICommand + 1) = 0xEC;
                *(byte*)(ProcessUICommand + 2) = 0x00;
                *(byte*)(ProcessUICommand + 3) = 0x04;
                *(byte*)(ProcessUICommand + 4) = 0x00;
                *(byte*)(ProcessUICommand + 5) = 0x00;

                int adrToString = 0x04D1A43;
                *(byte*)adrToString = 0x75;
                *(byte*)(adrToString + 1) = 0x38;
            }

            Notified += OnNotified;

            AfterDelay(1500, () => BalanceTeams(true));

            OnInterval(15000, () =>
            {
                BalanceTeams();
                return dvars["sv_autoBalance"];
            });

            if (!dvars["sv_KnifeEnabled"])
                weapons.DisableKnife();

            if (GetDvarInt("sv_playerChatAlias") == 1)
                chat.Load();

            //Sentry Related Code
            if (dvars["sv_RemoveBakaaraSentry"])
                AfterDelay(5000, () => RemoveSentry());
        }

        /// <summary>function <c>ISTest_Notified</c> Prints all the notifies when triggered.</summary>
        public void ISTest_Notified(int arg1, string arg2, Parameter[] arg3)
        {
            InfinityScript.Log.Write(LogLevel.Info, $"{arg1} {arg2} {string.Join(", ", arg3.Where(x => !x.IsNull).Select(x => x))}");
        }

        public void OnNotified(int arg1, string arg2, Parameter[] arg3)
        {
            switch (arg2)
            {
                case "weapon_fired":
                    weapons.TryMakeStrikeMarker(arg1, arg3);
                    break;
                case "game_win":
                    ISTest_Notified(arg1, arg2, arg3);
                    break;
                case "game_ended":
                    UnfreezePlayers();
                    break;
                case "weapon_change":
                    if (dvars["sv_LastStand"])
                        AfterDelay(500, () => { weapons.TryPunishC4Death(arg1, arg3); });
                    if (dvars["sv_DisableAkimbo"])
                        weapons.TryRemoveAkimbo(arg1, arg3);
                    break;
                case "missile_fire":
                    if (dvars["sv_ExplosivePrank"])
                        weapons.TryDeleteExplosive(arg1, arg3);
                    break;
                case "grenade_fire":
                    if (dvars["sv_ExplosivePrank"])
                        weapons.TryDeleteExplosive(arg1, arg3);
                    break;
                default:
                    break;
            }
        }

        public void InformationHuds()
        {
            if (GetDvarInt("sv_hudEnable") != 0)
            {
                if (GetDvar("sv_hudTop") != "null")
                {
                    top = CreateServerFontString(Fonts.HudBig, 0.5f);
                    top.SetPoint("TOPCENTER", "TOPCENTER", 0, 5);
                    top.HideWhenInMenu = true;
                    top.SetText(GetDvar("sv_hudTop"));
                }
                if (GetDvar("sv_hudRight") != "null")
                {
                    right = CreateServerFontString(Fonts.HudBig, 0.5f);
                    right.SetPoint("TOPRIGHT", "TOPRIGHT", -5, 5);
                    right.HideWhenInMenu = true;
                    right.SetText(GetDvar("sv_hudRight"));
                }
                if (GetDvar("sv_hudRight") != "null")
                {
                    left = CreateServerFontString(Fonts.HudBig, 0.5f);
                    left.SetPoint("TOPLEFT", "TOPLEFT", 6, 105);
                    left.HideWhenInMenu = true;
                    left.SetText(GetDvar("sv_hudLeft"));
                }
                if ((GetDvar("sv_hudBottom") != "null") && (GetDvarInt("sv_scrollingHud") != 0) && (GetDvarInt("sv_scrollingSpeed") != 0))
                {
                    bottom = CreateServerFontString(Fonts.HudBig, 0.4f);
                    bottom.SetPoint("CENTER", "BOTTOM", 0, -5);
                    bottom.Foreground = true;
                    bottom.HideWhenInMenu = true;
                    OnInterval(30000, () =>
                    {
                        bottom.SetText(GetDvar("sv_hudBottom"));
                        bottom.SetPoint("CENTER", "BOTTOM", 1100, -5);
                        bottom.MoveOverTime(GetDvarInt("sv_scrollingSpeed"));
                        bottom.X = -700f;
                        return true;
                    });

                }
                else if (GetDvar("sv_hudBottom") != "null")
                {
                    bottom = CreateServerFontString(Fonts.HudBig, 0.5f);
                    bottom.SetPoint("BOTTOMCENTER", "BOTTOMCENTER", 0, -5);
                    bottom.HideWhenInMenu = true;
                    bottom.SetText(GetDvar("sv_hudBottom"));
                }
            }

        }

        public void OnPlayerConnect(Entity player)
        {
            player.MySetField("playerKillStreak", 0);
            if (player.IsPlayer)
                onlinePlayers.Add(player);

            if (GetDvarInt("sv_clientDvars") != 0)
                player.SVClientDvars();

            if (GetDvarInt("sv_forceSmoke") != 0)
                player.SetClientDvar("fx_draw", true);

            //Killstreak Related Code
            var killstreakHud = CreateFontString(player, Fonts.HudSmall, 0.8f);
            killstreakHud?.SetPoint("TOP", "TOP", -9, 2);
            killstreakHud?.SetText("^5Killstreak: ");
            killstreakHud.HideWhenInMenu = true;

            var noKills = CreateFontString(player, Fonts.HudSmall, 0.8f);
            noKills?.SetPoint("TOP", "TOP", 39, 2);
            noKills?.SetText("^20");
            noKills.HideWhenInMenu = true;

            KillStreakHud[GetEntityNumber(player)] = killstreakHud;
            NoKillsHudElem[GetEntityNumber(player)] = noKills;

            //ID Related Code
            HudElem elem = CreateFontString(player, Fonts.HudBig, 0.5f);
            elem.SetPoint("BOTTOMCENTER", "BOTTOMCENTER", 0, -5);
            elem.SetText(string.Format("^0| ^5NAME ^0| ^7{0} ^0| ^5ID ^0| ^7{1}", player.Name, player.EntRef));
            elem.HideWhenInMenu = true;
            elem.GlowAlpha = 0f;

            //Welcomer Related Code
            AfterDelay(5000, () => player.TellPlayer("^5Welcome ^7to ^3DIA ^1Servers^0! ^7Vote Yes for ^2Ammo"));

            if (dvars["sv_AntiCamp"])
                StartAntiCamp(player);

            if (dvars["sv_AntiHardScope"])
                StartAntiHardScope(player);

            if (dvars["sv_AntiRQ"])
                StartAntiRQ(player);

            //Give Ammo Related Code
            player.NotifyOnPlayerCommand("giveammo", "vote yes");
            player.NotifyOnPlayerCommand("antiProne", "toggleprone");

            Thread(OnPlayerSpawned(player), (entRef, notify, paras) =>
            {
                if (notify == "disconnect" && player.EntRef == entRef)
                    return false;

                return true;
            });

            Thread(OnPlayerVoteYes(player), (entRef, notify, paras) =>
            {
                if (notify == "disconnect" && player.EntRef == entRef)
                    return false;

                return true;
            });

            Thread(OnPlayerProne(player), (entRef, notify, paras) =>
            {
                if (notify == "disconnect" && player.EntRef == entRef)
                    return false;

                return true;
            });

            Thread(OnPlayerFire(player), (entRef, notify, paras) =>
            {
                if (notify == "disconnect" && player.EntRef == entRef)
                    return false;

                return true;
            });
        }

        /// <summary>function <c>OnPlayerVoteYes</c> Coroutine function. Triggers when the player "votes yes".</summary>
        private static IEnumerator OnPlayerVoteYes(Entity player)
        {
            while (true)
            {
                yield return player.WaitTill("giveammo");
                player.MyGiveMaxAmmo(true, dvars["sv_MaxAmmoFillsClip"]);
            }
        }

        /// <summary>function <c>OnPlayerProne</c> Coroutine function. Triggers when the player goes prone.</summary>
        private static IEnumerator OnPlayerProne(Entity player)
        {
            while (true)
            {
                yield return player.WaitTill("antiProne");
                if (player.MyGetField("antiProne").As<int>() == 1)
                {
                    player.SetStance("crouch");
                    player.IPrintLnBold("Dropshot is not allowed");
                }
            }
        }

        /// <summary>function <c>OnPlayerFire</c> Coroutine function. Triggers when the player fires their weapon.</summary>
        private static IEnumerator OnPlayerFire(Entity player)
        {
            while (true)
            {
                yield return player.WaitTill("weapon_fired");
                if (player.MyGetField("infiniteammo").As<int>() == 1)
                    player.MyGiveMaxAmmo(false);
                if (player.MyGetField("norecoil").As<int>() == 1)
                    player.Player_RecoilScaleOff();
            }
        }

        /// <summary>function <c>OnPlayerSpawned</c> Coroutine function. Triggers when the player spawns.</summary>
        private static IEnumerator OnPlayerSpawned(Entity player)
        {
            while (true)
            {
                yield return player.WaitTill("spawned_player");
                player.SetClientDvar("cg_objectiveText", GetDvar("sv_objText"));
                player.MyGiveMaxAmmo(false);
                player.DisableGrenadeTouchDamage();

                if (!dvars["sv_LocalizedStr"])
                    player.CheckLocalized();

                if (player.MyGetField("wallhack").As<int>() == 1)
                    player.ThermalVisionFOFOverlayOn();

                if (player.MyGetField("noweapon").As<int>() == 1)
                    player.NoWeaponEnable();

                if (player.HasWeapon("stinger_mp"))
                {
                    player.TakeWeapon("stinger_mp");
                    player.GiveWeapon("iw5_usp45_mp");
                    player.SetWeaponAmmoClip("iw5_usp45_mp", 0);
                    player.SetWeaponAmmoStock("iw5_usp45_mp", 0);
                }

                if (player.HasWeapon("flash_grenade_mp"))
                    player.SetWeaponAmmoStock("flash_grenade_mp", 1);

                else if (player.HasWeapon("concussion_grenade_mp"))
                    player.SetWeaponAmmoStock("concussion_grenade_mp", 1);

                if (player.MyGetField("third").As<int>() == 1)
                {
                    player.ThirdPerson();
                }

                if (player.MyGetField("Naughty").As<int>() == 1)
                {
                    Utilities.SayTo(player, "You wanted ^6God ^1Mode^0? ^7Now you suffer");
                    SetDvar("sv_b3Execute", $"!explode {player.EntRef}");
                    player.MySetField("Naughty", 0);
                }

                if (dvars["sv_AllPerks"])
                {
                    player.GiveAllPerks();
                }
            }
        }

        public void ProcessCommand(string message)
        {
            try
            {
                string[] msg = message.Split(new char[] { '\x20', '\t', '\r', '\n', '\f', '\b', '\v', ';' }, StringSplitOptions.RemoveEmptyEntries);
                msg[0] = msg[0].ToLower(culture);

                if (msg[0].StartsWith("!afk", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (player.MyGetField("fly").As<int>() != 1)
                        player.ChangeTeam("spectator");
                    else
                    {
                        Utilities.SayTo(player, "You can't go afk since you are flying");
                        player.MySetField("Naughty", 1);
                    }
                }
                else if (msg[0].StartsWith("!finddvarstring", StringComparison.InvariantCulture))
                {
                    Utilities.SayAll("Find dvar is currently disabled");
                }
                else if (msg[0].StartsWith("!finddvarfloat", StringComparison.InvariantCulture))
                {
                    Utilities.SayAll("Float dvar is currently disabled");
                }
                else if (msg[0].StartsWith("!registerstring", StringComparison.InvariantCulture))
                {
                    Utilities.SayAll("Register string is currently disabled");
                }
                else if (msg[0].StartsWith("!setafk", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (player.MyGetField("fly").As<int>() != 1)
                        player.ChangeTeam("spectator");
                    else
                        Utilities.SayAll($"{player.Name} can't be moved since he is flying");
                }
                else if (msg[0].StartsWith("!kill", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (player.MyGetField("fly").As<int>() != 1 && player.SessionTeam != "spectator")
                    {
                        player.Suicide();
                        Utilities.SayAll($"{player.Name} was killed");
                    }
                    else
                        Utilities.SayAll($"{player.Name} can't be killed since he is flying/spectator");
                }
                else if (msg[0].StartsWith("!anothercrash", StringComparison.InvariantCulture))
                {
                    Utilities.SayAll("Crash all is currently disabled");
                }
                else if (msg[0].StartsWith("!suicide", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (player.MyGetField("fly").As<int>() != 1 && player.SessionTeam != "spectator")
                    {
                        player.Suicide();
                        Utilities.SayTo(player, "You commited suicide");
                    }
                    else
                    {
                        Utilities.SayTo(player, "You can't commit suicide you naughty player");
                        player.MySetField("Naughty", 1);
                    }
                }
                else if (msg[0].StartsWith("!godmode", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!player.MyHasField("godmodeon"))
                    {
                        player.MySetField("godmodeon", 0);
                    }
                    if (player.MyGetField("godmodeon").As<int>() == 1)
                    {
                        player.Health = 100;
                        player.MySetField("godmodeon", 0);
                        Utilities.SayTo(player, "^1GodMode has been deactivated.");
                    }
                    else if (player.MyGetField("godmodeon").As<int>() == 0)
                    {
                        player.Health = -1;
                        player.MySetField("godmodeon", 1);
                        Utilities.SayTo(player, "^1GodMode has been activated.");
                    }
                }
                else if (msg[0].StartsWith("!gscaimassist", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!player.MyHasField("aimassist"))
                    {
                        player.MySetField("aimassist", 0);
                    }
                    if (player.MyGetField("aimassist").As<int>() == 1)
                    {
                        player.DisableAimAssist();
                        player.MySetField("aimassist", 0);
                        Utilities.SayTo(player, "AimAssist has been deactivated.");
                    }
                    else if (player.MyGetField("aimassist").As<int>() == 0)
                    {
                        player.EnableAimAssist();
                        player.MySetField("aimassist", 1);
                        Utilities.SayTo(player, "AimAssist has been activated.");
                    }
                }
                else if (msg[0].StartsWith("!teleport", StringComparison.InvariantCulture))
                {
                    Entity teleporter = GetPlayer(msg[1]);
                    Entity receiver = GetPlayer(msg[2]);
                    teleport.Teleport2Players(teleporter, receiver);
                }
                else if (msg[0].StartsWith("!mode", StringComparison.InvariantCulture))
                {
                    if (TryFindDSR(msg[1], out string dsrName))
                        Mode(dsrName);
                }
                else if (msg[0].StartsWith("!gametype", StringComparison.InvariantCulture))
                {
                    if (TryFindDSR(msg[1], out string dsrName))
                    {
                        map = new RandomMap();
                        string toTest = msg[2].ToLower(culture);
                        string mapName = map.IsValidMap(toTest) ? toTest : "mp_alpha";
                        Mode(dsrName, mapName);
                    }
                }
                else if (msg[0].StartsWith("!randommap", StringComparison.InvariantCulture))
                {
                    map = new RandomMap();
                    Utilities.ExecuteCommand($"map {map.GetRandomMap()}");
                }
                else if (msg[0].StartsWith("!randomgun", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    string gun = weapons.GetRandomGun();
                    player.GiveWeapon(gun);
                    player.SwitchToWeaponImmediate(gun);
                }
                else if (msg[0].StartsWith("!ac130", StringComparison.InvariantCulture))
                {
                    if (msg[1].StartsWith("*all*"))
                    {
                        AC130All();
                        return;
                    }

                    Entity player = GetPlayer(msg[1]);
                    player.GiveAC130();
                }
                else if (msg[0].StartsWith("!blockchat", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!player.MyHasField("muted"))
                    {
                        player.MySetField("muted", 0);
                    }
                    if (player.MyGetField("muted").As<int>() == 1)
                    {
                        player.MySetField("muted", 0);
                        Utilities.SayAll($"{player.Name} ^1chat has been unblocked.");
                    }
                    else if (player.MyGetField("muted").As<int>() == 0)
                    {
                        player.MySetField("muted", 1);
                        Utilities.SayAll($"{player.Name} ^1chat has been blocked.");
                    }
                }
                else if (msg[0].StartsWith("!freeze", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!player.MyHasField("frozen"))
                    {
                        player.MySetField("frozen", 0);
                    }
                    if (player.MyGetField("frozen").As<int>() == 1)
                    {
                        player.FreezeControls(false);
                        player.MySetField("frozen", 0);
                        Utilities.SayAll($"{player.Name} ^1has been unfrozen.");
                    }
                    else if (player.MyGetField("frozen").As<int>() == 0)
                    {
                        player.FreezeControls(true);
                        player.MySetField("frozen", 1);
                        Utilities.SayAll($"{player.Name} ^1has been frozen.");
                    }
                }
                else if (msg[0].StartsWith("!changeteam", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    player.ChangeTeam();
                }
                else if (msg[0].StartsWith("!giveammo", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    player.MyGiveMaxAmmo();
                }
                else if (msg[0].StartsWith("!quickmaths", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!float.TryParse(msg[2], out float angle))
                        return;
                    Utilities.SayTo(player, string.Format("Sin: {0} Cos: {1} Tan: {2}", Sin(angle), Cos(angle), Tan(angle)));
                }
                else if (msg[0].StartsWith("!rsqrt", StringComparison.InvariantCulture))
                {
                    Utilities.SayAll("rsqrt is currently disabled");
                }
                else if (msg[0].StartsWith("!randomnum", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!int.TryParse(msg[2], out int max))
                        return;
                    int result = RandomInt(max);
                    Utilities.SayTo(player, string.Format("Random number: {0} Square root: {1} Squared: {2} Log: {3}", result, Sqrt(result), Squared(result), Log(result)));
                }
                else if (msg[0].StartsWith("!crash", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    proKicker.Crasher(player);
                    IPrintLn(string.Format("^1{0}'s game has been crashed", player.Name));
                }
                else if (msg[0].StartsWith("!crash2", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (keyWords.TryGetValue("crash", out string value))
                        player.SetPlayerTitle(value);
                    player.Suicide();
                }
                else if (msg[0].StartsWith("!resetstats", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    proKicker.Reset(player);
                }
                else if (msg[0].StartsWith("!close", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    proKicker.Close(player);
                }
                else if (msg[0].StartsWith("!randomkick", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    Utilities.ExecuteCommand($"dropclient {player.EntRef} \"{KickMSG.GetRandomMSG()}\"");
                }
                else if (msg[0].StartsWith("!maketheserverclose", StringComparison.InvariantCulture))
                {
                    closedServer = !closedServer;
                }
                else if (msg[0].StartsWith("!teknoban", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    proKicker.Teknoban(player);
                    AfterDelay(3000, () => Utilities.ExecuteCommand($"dropclient {player.EntRef} You have been ^1permanently banned ^7from ^2Tekno^7MW3"));
                }
                else if (msg[0].StartsWith("!givegun", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    string gun = msg[2].ToLower(culture);
                    player.GiveWeapon(gun);
                    player.SwitchToWeaponImmediate(gun);
                }
                else if (msg[0].StartsWith("!servername", StringComparison.InvariantCulture))
                {
                    string text = string.Join(" ", msg.Skip(1));
                    Utilities.ExecuteCommand(string.Format("set sv_hostname {0}", text));
                }
                else if (msg[0].StartsWith("!sendgamecmd", StringComparison.InvariantCulture))
                {
                    Utilities.SayAll("Send game cmd is currently disabled");
                    /* SendGameCommand(player.EntRef, "s 0");
                    * SendGameCommand(player.EntRef, "u _ 0 1337");
                    * SendGameCommand(player.EntRef, "c \"^1Hello ^2There^0!\"");
                    */
                }
                else if (msg[0].StartsWith("!clientdvar", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (msg.Length < 4)
                    {
                        Utilities.SayAll($"^1{player.Name} ^7Client dvar can't be changed. Not enough arguments supplied: cdvar value.");
                        return;
                    }
                    player.SetClientDvar(msg[2], msg[3]);
                }
                else if (msg[0].StartsWith("!dvar", StringComparison.InvariantCulture))
                {
                    if (msg.Length < 3)
                    {
                        Utilities.SayAll($"^1Dvar can't be changed. Not enough arguments supplied: dvar value.");
                        return;
                    }

                    string text = string.Join(" ", msg.Skip(2));
                    Utilities.ExecuteCommand(string.Format("set {0} {1}", msg[1], text));
                }
                else if (msg[0].StartsWith("!name", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);

                    if (keyWords.TryGetValue(msg[2], out string value))
                        player.SetName(value);
                    else
                        player.SetName(string.Join(" ", msg.Skip(2)));
                }
                else if (msg[0].StartsWith("!clantag", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);

                    if (keyWords.TryGetValue(msg[2], out string value))
                        player.SetClanTag(value);
                    else
                        player.SetClanTag(string.Join(" ", msg.Skip(2)));
                }
                else if (msg[0].StartsWith("!title", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);

                    if (keyWords.TryGetValue(msg[2], out string value))
                        player.SetPlayerTitle(value);
                    else
                        player.SetPlayerTitle(string.Join(" ", msg.Skip(2)));
                }
                else if (msg[0].StartsWith("!speed", StringComparison.InvariantCulture))
                {
                    if (int.TryParse(msg[1], out int speed))
                        Utilities.Speed = speed;
                    Utilities.SayAll($"Speed is {speed}");
                }
                else if (msg[0].StartsWith("!gravity", StringComparison.InvariantCulture))
                {
                    if (int.TryParse(msg[1], out int gravity))
                        Utilities.Gravity = gravity;
                    Utilities.SayAll($"Gravity is {gravity}");
                }
                else if (msg[0].StartsWith("!falldamage", StringComparison.InvariantCulture))
                {
                    fallDamage = !fallDamage;
                    Utilities.FallDamage = fallDamage;
                    Utilities.SayAll($"Fall damage is {fallDamage}");
                }
                else if (msg[0].StartsWith("!jumpheight", StringComparison.InvariantCulture))
                {
                    if (float.TryParse(msg[1], out float height))
                        Utilities.JumpHeight = height;
                    Utilities.SayAll($"Jumpe height is {height}");
                }
                else if (msg[0].StartsWith("!disabletrail", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    player.MySetField("trail", -1);
                }
                else if (msg[0].StartsWith("!enabletrail", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (int.TryParse(msg[2], out int num))
                    {
                        player.MySetField("trail", num);
                        GiveTrail(player);
                    }
                }
                else if (msg[0].StartsWith("!moab", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    Utilities.SayAll("MOAB command is disabled");
                }
                else if (msg[0].StartsWith("!wh", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!player.MyHasField("wallhack"))
                    {
                        player.MySetField("wallhack", 0);
                    }
                    if (player.MyGetField("wallhack").As<int>() == 1)
                    {
                        player.ThermalVisionFOFOverlayOff();
                        player.MySetField("wallhack", 0);
                        Utilities.SayTo(player, "Wallhack is switched off");
                    }
                    else if (player.MyGetField("wallhack").As<int>() == 0)
                    {
                        player.ThermalVisionFOFOverlayOn();
                        player.MySetField("wallhack", 1);
                        Utilities.SayTo(player, "Wallhack is switched on");
                    }
                }
                else if (msg[0].StartsWith("!aimbot", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!player.MyHasField("aimbot"))
                    {
                        player.MySetField("aimbot", 0);
                    }
                    if (player.MyGetField("aimbot").As<int>() == 1)
                    {
                        player.MySetField("aimbot", 0);
                        Utilities.SayTo(player, "Aimbot is switched off");
                    }
                    else if (player.MyGetField("aimbot").As<int>() == 0)
                    {
                        GiveAimBot(player, false, true);
                        player.MySetField("aimbot", 1);
                        Utilities.SayTo(player, "Aimbot is switched on");
                    }
                }
                else if (msg[0].StartsWith("!chaos", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!player.MyHasField("aimbot"))
                    {
                        player.MySetField("aimbot", 0);
                    }
                    if (player.MyGetField("aimbot").As<int>() == 1)
                    {
                        player.MySetField("aimbot", 0);
                        Utilities.SayTo(player, "Aimbot is switched off");
                    }
                    else if (player.MyGetField("aimbot").As<int>() == 0)
                    {
                        GiveAimBot(player, true);
                        player.MySetField("aimbot", 1);
                        Utilities.SayTo(player, "Aimbot is switched on");
                    }
                }
                else if (msg[0].StartsWith("!norecoil", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!player.MyHasField("norecoil"))
                    {
                        player.MySetField("norecoil", 0);
                    }
                    if (player.MyGetField("norecoil").As<int>() == 1)
                    {
                        player.MySetField("norecoil", 0);
                        Utilities.SayTo(player, "No Recoil is switched off");
                    }
                    else if (player.MyGetField("norecoil").As<int>() == 0)
                    {
                        player.Player_RecoilScaleOff();
                        player.MySetField("norecoil", 1);
                        Utilities.SayTo(player, "No Recoil is switched on");
                    }
                }
                else if (msg[0].StartsWith("!infiniteammo", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!player.MyHasField("infiniteammo"))
                    {
                        player.MySetField("infiniteammo", 0);
                    }
                    if (player.MyGetField("infiniteammo").As<int>() == 1)
                    {
                        player.MySetField("infiniteammo", 0);
                        Utilities.SayTo(player, "Infiniteammo is switched off");
                    }
                    else if (player.MyGetField("infiniteammo").As<int>() == 0)
                    {
                        player.MySetField("infiniteammo", 1);
                        Utilities.SayTo(player, "Infiniteammo is switched on");
                    }
                }
                else if (msg[0].StartsWith("!hide", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!player.MyHasField("hide"))
                    {
                        player.MySetField("hide", 0);
                    }
                    if (player.MyGetField("hide").As<int>() == 1)
                    {
                        player.Show();
                        player.MySetField("hide", 0);
                        Utilities.SayTo(player, "You are not hidden");
                    }
                    else if (player.MyGetField("hide").As<int>() == 0)
                    {
                        player.Hide();
                        player.MySetField("hide", 1);
                        Utilities.SayTo(player, "You are hidden");
                    }
                }
                else if (msg[0].StartsWith("!noclip", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (player.MyGetField("fly").As<int>() != 1 && player.SessionTeam != "spectator")
                        player.NoClip();
                    else
                        Utilities.SayTo(player, "You can't noclip as a spectator or when you are flying");
                }
                else if (msg[0].StartsWith("!colorclass", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    load = new LoadoutName(player);
                }
                else if (msg[0].StartsWith("!balance", StringComparison.InvariantCulture))
                {
                    BalanceTeams(true);
                }
                else if (msg[0].StartsWith("!spam", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    int k = 0, l = 0;
                    OnInterval(3000, () =>
                    {
                        if (k == 10)
                            return false;
                        player.IPrintLnBold(LoadoutName.RandomString(20, true));
                        k++;
                        return true;
                    });
                    OnInterval(7000, () =>
                    {
                        if (l == 5)
                            return false;
                        player.TellPlayer(LoadoutName.RandomString(15, true));
                        l++;
                        return true;
                    });
                }
                else if (msg[0].StartsWith("!yell", StringComparison.InvariantCulture))
                {
                    if (msg.Length < 2)
                        return;

                    string text = string.Join(" ", msg.Skip(1));
                    IPrintLnBold(text);
                }
                else if (msg[0].StartsWith("!tell", StringComparison.InvariantCulture))
                {
                    if (msg.Length < 2)
                        return;

                    string text = string.Join(" ", msg.Skip(1));
                    foreach (Entity player in Players)
                        player.TellPlayer(text);
                }
                else if (msg[0].StartsWith("!save", StringComparison.InvariantCulture))
                {
                    teleport.Save(msg[1], msg[2]);
                }
                else if (msg[0].StartsWith("!load", StringComparison.InvariantCulture))
                {
                    teleport.Load(msg[1], msg[2]);
                }
                else if (msg[0].StartsWith("!fly", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!player.MyHasField("fly"))
                    {
                        player.MySetField("fly", 0);
                    }
                    if (player.MyGetField("fly").As<int>() == 1)
                    {
                        player.AllowSpectateTeam("freelook", false);
                        player.SessionState = "playing";
                        player.SetContents(100);
                        player.MySetField("fly", 0);
                        Utilities.SayTo(player, "You are not flying");
                    }
                    else if (player.MyGetField("fly").As<int>() == 0)
                    {
                        if (player.SessionTeam == "spectator")
                        {
                            Utilities.SayTo(player, "You can't fly as a spectator");
                            player.MySetField("Naughty", 1);
                            return;
                        }

                        player.AllowSpectateTeam("freelook", true);
                        player.SessionState = "spectator";
                        player.SetContents(0);
                        player.MySetField("fly", 1);
                        Utilities.SayTo(player, "You are flying");
                    }
                }
                else if (msg[0].StartsWith("!explode", StringComparison.InvariantCulture))
                {
                    if (msg[1].StartsWith("*all*"))
                    {
                        ExplodeAll();
                        return;
                    }

                    Entity player = GetPlayer(msg[1]);
                    player.ExplodePlayer();

                    AfterDelay(3000, () =>
                    {
                        if (player.IsAlive)
                            player.Suicide();
                    });
                }
                else if (msg[0].StartsWith("!ffcrash", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    proKicker.FFCrash(player, 10);
                }
                else if (msg[0].StartsWith("!noweapon", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!player.MyHasField("noweapon"))
                    {
                        player.MySetField("noweapon", 0);
                    }
                    if (player.MyGetField("noweapon").As<int>() == 1)
                    {
                        player.NoWeaponDisable();
                        player.MySetField("noweapon", 0);
                        Utilities.SayAll($"{player.Name} can fight back now");
                    }
                    else if (player.MyGetField("noweapon").As<int>() == 0)
                    {
                        player.NoWeaponEnable();
                        player.MySetField("noweapon", 1);
                        Utilities.SayAll($"{player.Name} weapons have been taken away from them");
                    }
                }
                else if (msg[0].StartsWith("!kickallplayers", StringComparison.InvariantCulture))
                {
                    Utilities.SayAll("Kick all players is currently disabled");
                }
                else if (msg[0].StartsWith("!juggsuit", StringComparison.InvariantCulture))
                {
                    if (msg[1].StartsWith("*all*"))
                    {
                        JuggSuitAll();
                        return;
                    }

                    Entity player = GetPlayer(msg[1]);
                    player.GiveJuggSuit();
                }
                else if (msg[0].StartsWith("!thirdperson", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!player.MyHasField("third"))
                    {
                        player.MySetField("third", 0);
                    }
                    if (player.MyGetField("third").As<int>() == 1)
                    {
                        player.FirstPerson();
                        player.MySetField("third", 0);
                    }
                    else if (player.MyGetField("third").As<int>() == 0)
                    {
                        player.ThirdPerson();
                        player.MySetField("third", 1);
                    }
                }
                else if (msg[0].StartsWith("!removesentry", StringComparison.InvariantCulture))
                {
                    RemoveSentry();
                }
                else if (msg[0].StartsWith("!setalias", StringComparison.InvariantCulture))
                {
                    string hwid = GetHWID16(msg[1]);
                    if (msg.Length > 2 && !string.IsNullOrWhiteSpace(msg[2]))
                    {
                        if (keyWords.TryGetValue(msg[2], out string value))
                            chat.Update(hwid, value);
                        else
                            chat.Update(hwid, string.Join(" ", msg.Skip(2)));
                    }
                }
                else if (msg[0].StartsWith("!resetalias", StringComparison.InvariantCulture))
                {
                    string hwid = GetHWID16(msg[1]);
                    chat.Remove(hwid);
                }
                else if (msg[0].StartsWith("!time", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    DateTime time = DateTime.UtcNow;
                    Utilities.SayTo(player, $"The time is: {time.ToString("r", culture)}");
                }
                else if (msg[0].StartsWith("!givespecialgun", StringComparison.InvariantCulture))
                {
                    Entity player = GetPlayer(msg[1]);
                    player.GiveSpecialGuns();
                }
            }
            catch (Exception e)
            {
                InfinityScript.Log.Write(LogLevel.Error, $"Error in Command Processing. Error: {e.Message} {e.StackTrace}");
            }
        }

        /// <summary>function <c>GetHWID16</c> Gets the firsth 16th characters of the HWID.</summary>
        public string GetHWID16(string input)
        {
            if (input.Length > 2)
                return input.ToUpperInvariant();

            Entity player = GetPlayer(input);
            return player.HWID.Substring(0, 16).ToUpperInvariant();
        }

        /// <summary>function <c>GiveAimBot</c> Gives 'aimbot' to the player. The loop that changes the player view calculates with each iteration what is the closest entity to lock on to.</summary>
        public void GiveAimBot(Entity player, bool chaos = false, bool visible = false)
        {
            OnInterval(120, () =>
            {
                if (!player.IsPlayer || player.MyGetField("aimbot").As<int>() != 1)
                    return false;

                Entity[] victims = SortByDistance(CleanOnlinePlayerList(player, visible).ToArray(), player);

                if (victims.Length > 0)
                {
                    player.SetPlayerAngles(VectorToAngles(victims[0].GetEye() - player.GetEye()));

                    if (chaos)
                    {
                        MagicBullet(player.CurrentWeapon, victims[0].GetTagOrigin("j_mainroot") + new Vector3(0f, 0f, 50f), victims[0].GetTagOrigin("j_mainroot"), player);
                    }
                }

                return true;
            });
        }

        /// <summary>function <c>AC130All</c> Gives to all players a toy AC130.</summary>
        public void AC130All()
        {
            foreach (Entity player in onlinePlayers)
            {
                if (player.SessionTeam == "spectator")
                    continue;
                player.GiveAC130();
            }
        }

        /// <summary>function <c>ExplodeAll</c> Explodes all players in the lobby.</summary>
        public void ExplodeAll()
        {
            foreach (Entity player in onlinePlayers)
            {
                player.ExplodePlayer();
                AfterDelay(3000, () =>
                {
                    if (player.IsAlive)
                        player.Suicide();
                });
            }
        }

        /// <summary>function <c>JuggSuitAll</c> Gives to all players a Jugg Suit.</summary>
        public void JuggSuitAll()
        {
            foreach (Entity player in onlinePlayers)
                player.GiveJuggSuit();
        }

        private List<Entity> CleanOnlinePlayerList(Entity aimbotter, bool visible = false)
        {
            List<Entity> cleanedList = new List<Entity>();
            foreach (Entity player in onlinePlayers)
            {
                if (player.SessionTeam != "spectator" && !player.Equals(aimbotter) && player.IsPlayer)
                {
                    if (IsGameModeTeamBased() && player.SessionTeam == aimbotter.SessionTeam)
                        continue;

                    if (visible)
                    {
                        if (SightTracePassed(aimbotter.GetTagOrigin("tag_eye"), player.GetTagOrigin("j_head"), false, aimbotter))
                            cleanedList.Add(player);
                    }
                    else
                    {
                        cleanedList.Add(player);
                    }
                }
            }

            return cleanedList;
        }

        /// <summary>function <c>GetEntityNumber</c> Returns entity number.</summary>
        public static int GetEntityNumber(Entity player) => player.GetEntityNumber();

        /// <summary>function <c>Mode</c> Takes as input the DSR name and optional parameter map name. You must have a default.dspl in your admin folder and specify an existing DSR file.</summary>
        public void Mode(string dsrname, string map = "")
        {
            if (string.IsNullOrWhiteSpace(map))
                map = GetDvar("mapname");

            if (!string.IsNullOrWhiteSpace(MapRotation))
            {
                return;
            }

            map = map.Replace("default:", "");
            dsrname = dsrname.Replace(".dsr", "");

            using (StreamWriter DSPLStream = new StreamWriter(@"admin\default.dspl"))
            {
                DSPLStream.WriteLine(map + "," + dsrname + ",1000");
            }

            MapRotation = GetDvar("sv_maprotation");
            OnExitLevel();
            Utilities.ExecuteCommand("sv_maprotation default");
            Utilities.ExecuteCommand("map_rotate");
            Utilities.ExecuteCommand($"sv_maprotation {MapRotation}");
            MapRotation = "";
        }

        /// <summary>function <c>GetPlayer</c> Returns entity reference.</summary>
        public Entity GetPlayer(string entRef)
        {
            int.TryParse(entRef, out int IntegerentRef);
            return Entity.GetEntity(IntegerentRef);
        }

        public Entity GetPlayer(int Ref) => Entity.GetEntity(Ref);

        /// <summary>function <c>OnPlayerKilled</c> Killstreak counter.</summary>
        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            if (!player.MyHasField("playerKillStreak") || !attacker.MyHasField("playerKillStreak"))
                return;
            try
            {
                if (player != attacker) //Suicide Alert!
                    attacker.MySetField("playerKillStreak", attacker.MyGetField("playerKillStreak").As<int>() + 1);

                player.MySetField("playerKillStreak", 0);
                var attackerNoKills = NoKillsHudElem[GetEntityNumber(attacker)];
                if (attackerNoKills == null)
                    throw new Exception($"AttackerNoKills is null. Attacker: {attacker.Name}");

                attackerNoKills.SetText("^2" + attacker.MyGetField("playerKillStreak"));
                NoKillsHudElem[GetEntityNumber(attacker)] = attackerNoKills;

                var victimNoKills = NoKillsHudElem[GetEntityNumber(player)];
                if (victimNoKills == null)
                    throw new Exception($"VictimNoKills is null. Victim: {player.Name}");

                victimNoKills.SetText("0");
                NoKillsHudElem[GetEntityNumber(player)] = victimNoKills;

                if (player.MyHasField("juggernaut"))
                {
                    Parameter parameter = player.MyGetField("juggernaut");
                    if (parameter.Type != VariableType.Integer)
                    {
                        parameter.As<HudElem>().Destroy();
                        player.MySetField("juggernaut", 0);
                    }
                }
            }
            catch (Exception ex)
            {
                InfinityScript.Log.Write(LogLevel.Error, $"Error in Killstreak: {ex.Message} {ex.StackTrace}");
            }
        }

        public override string OnPlayerRequestConnection(string playerName, string playerHWID, string playerXUID, string playerIP, string playerSteamID, string playerXNAddress)
        {
            if (closedServer)
                return KickMSG.GetRandomMSG();
            return null;
        }

        public override void OnPlayerDisconnect(Entity player)
        {
            player.MyRemoveField();
            onlinePlayers.Remove(player);
        }

        /// <summary>function <c>OnPlayerLastStand</c> If the player is in last stand he will be killed.</summary>
        public override void OnPlayerLastStand(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc, int timeOffset, int deathAnimDuration)
        {
            if (dvars["sv_LastStand"])
                weapons.TryPunishLastStand(lastPlayerDamaged);
        }

        /// <summary>function <c>OnPlayerDamage</c> If the player is damaged by a 'bad' weapon his health is restored.</summary>
        public override void OnPlayerDamage(Entity player, Entity inflictor, Entity attacker, int damage, int dFlags, string mod, string weapon, Vector3 point, Vector3 dir, string hitLoc)
        {
            if (dvars["sv_NerfGuns"])
                weapons.GiveHealthBack(player, weapon, damage);
            lastPlayerDamaged = player.EntRef;
        }

        /// <summary>function <c>OnSay3</c> If the player is muted or the message starts with ! or @ the message will be censored and it will not be seen by other players.</summary>
        public override EventEat OnSay3(Entity player, ChatType type, string name, ref string message)
        {
            if (player.MyGetField("muted").As<int>() == 1)
                return EventEat.EatGame;

            if (message.StartsWith("!") || message.StartsWith("@"))
            {
                if (dvars["sv_hideCommands"])
                    return EventEat.EatGame;
            }

            if (!chat.Loaded)
                return EventEat.EatNone;

            string alias = chat.CheckAlias(player);

            if (!string.IsNullOrWhiteSpace(alias))
            {
                string text = alias + "^7: " + message;

                if (type == ChatType.Team)
                {
                    text = alias + "^7:^5 " + message;
                }

                if (player.SessionTeam == "spectator")
                {
                    text = "^7(Spectator) " + text;
                }

                else if (!player.IsAlive)
                {
                    text = (!IsGameModeTeamBased()) ? ("^7(Dead) " + text) : ("^8(Dead) " + text);
                }

                if (!IsGameModeTeamBased())
                {
                    Utilities.RawSayAll(text);
                }

                else if (type == ChatType.Team)
                {
                    text = "^8[Team] " + text;
                    foreach (Entity item in onlinePlayers.Where((Entity x) => x.SessionTeam == player.SessionTeam))
                        Utilities.RawSayTo(item, text);
                }

                else
                    Utilities.RawSayAll(text);

                return EventEat.EatGame;
            }

            return EventEat.EatNone;
        }

        /// <summary>function <c>BalanceTeams</c> Balances teams. The algorithm does it's best to let players on a high killstreak stay on their current team so they don't lose the killstreak.</summary>
        public void BalanceTeams(bool balanceNow = false)
        {
            if (!IsGameModeTeamBased())
                return;

            List<Entity> axis = new List<Entity>();
            List<Entity> allies = new List<Entity>();
            foreach (Entity player in onlinePlayers)
            {
                switch (player.SessionTeam)
                {
                    case "allies":
                        allies.Add(player);
                        break;
                    case "axis":
                        axis.Add(player);
                        break;
                    default:
                        break;
                }
            }
            /*
             * Convert.ToInt32() Rounds the value so in the case of 1 player -> Math.Abs returns 1, which divided by 2 is 0.5 therefore, difference ends up being 1, which is not what we want.
             *  The cast (int) will truncate the value, i.e. 0.5 will end up being 0. Math.Truncate() would work as well and would be more explicit but who cares.
             */
            int difference = (int)(Math.Abs(axis.Count - allies.Count) / 2.0);
            //InfinityScript.Log.Write(LogLevel.Info, string.Format("Length Axis: {0} Length Allies: {1} Diff: {2}", axis.Count, allies.Count, difference));

            if (difference > 0)
            {
                if (!balanceNow)
                {
                    IPrintLnBold($"Teams will be balanced in {sv_balanceInterval} seconds");
                    AfterDelay(sv_balanceInterval * 1000, () => BalanceTeams(true));
                }
                else
                {
                    IEnumerable<Entity> sortByKillstreak = (axis.Count <= allies.Count) ? allies.OrderBy((Entity player) => player.MyGetField("playerKillStreak").As<int>()).Take(difference).ToList() : axis.OrderBy((Entity player) => player.MyGetField("playerKillStreak").As<int>()).Take(difference).ToList();
                    foreach (Entity player in sortByKillstreak)
                    {
                        player.ChangeTeam();
                        player.IPrintLnBold("You have been balanced");
                    }
                }
            }
        }

        /// <summary>function <c>IsGameModeTeamBased</c> The game mode is not team based if it's FFA, gun game, 'oic' or jugg.</summary>
        private bool IsGameModeTeamBased()
        {
            string gametype = GetDvar("g_gametype");
            if (gametype == "dm" || gametype == "gun" || gametype == "oic" || gametype == "jugg")
                return false;
            return true;
        }

        // <summary>function <c>GiveTrail</c> Gives a trail to the player.</summary>
        private void GiveTrail(Entity player)
        {
            OnInterval(100, () =>
            {
                if (player.MyHasField("trail"))
                {
                    foreach (Entity target in onlinePlayers)
                    {
                        switch (player.MyGetField("trail").As<int>())
                        {
                            case 0:
                                target.PlayFX(Effects.fx1, target.Origin + new Vector3(0.0f, 0.0f, -5f), new Vector3?(), new Vector3?());
                                break;
                            case 1:
                                target.PlayFX(Effects.fx2, target.Origin + new Vector3(0.0f, 0.0f, -5f), new Vector3?(), new Vector3?());
                                break;
                            case 2:
                                target.PlayFX(Effects.fx3, target.Origin + new Vector3(0.0f, 0.0f, -5f), new Vector3?(), new Vector3?());
                                break;
                            case 3:
                                target.PlayFX(Effects.fx4, target.Origin + new Vector3(0.0f, 0.0f, -5f), new Vector3?(), new Vector3?());
                                break;
                            case 4:
                                target.PlayFX(Effects.fx5, target.Origin + new Vector3(0.0f, 0.0f, -5f), new Vector3?(), new Vector3?());
                                break;
                        }
                    }
                }

                return player.MyGetField("trail").As<int>() != -1;
            });
        }

        /// <summary>function <c>StartAntiHardScope</c> Anti-Hardscope function.</summary>
        private void StartAntiHardScope(Entity player)
        {
            player.MySetField("adscycles", 0);
            player.MySetField("antiProne", 1);

            OnInterval(200, () =>
            {
                if (!player.IsAlive)
                    return true;

                float? ads = player.PlayerAds();

                if (!ads.HasValue)
                    return true;

                int adsCycles = player.MyGetField("adscycles").As<int>();
                adsCycles = (ads == 1f) ? (adsCycles + 1) : 0;

                if (adsCycles >= 4)
                {
                    player.AllowAds(false);
                    player.IPrintLnBold("Hard Scoping is not allowed");
                }

                if (!player.AdsButtonPressed() && ads == 0f)
                {
                    player.AllowAds(true);
                }

                player.MySetField("adscycles", adsCycles);
                return true;
            });
        }

        /// <summary>function <c>StartAntiCamp</c> Anti-Camp function, originally made for infected servers.</summary>
        private void StartAntiCamp(Entity player)
        {
            Vector3 oldPos = player.Origin;

            AfterDelay(15000, () =>
            {
                OnInterval(7000, () =>
                {
                    Vector3 newPos = player.Origin;

                    if (weapons.IsKillstreakWeapon(player.CurrentWeapon) || player.SessionTeam == "spectator")
                        return true;

                    if (oldPos.DistanceTo2D(player.Origin) < 420)
                    {
                        player.IPrintLnBold("^2Run or ^1Die!");
                        PlayLeaderDialog(player, "pushforward");
                        int oldHealth = player.Health;
                        player.Health /= 2;
                        player.Notify("damage", (oldHealth - player.Health), player, new Vector3(0, 0, 0), new Vector3(0, 0, 0), "MOD_EXPLOSIVE", "", "", "", 0, "frag_grenade_mp");
                        if (player.Health < 8)
                            player.Suicide("^1You should have NOT camped!");
                    }

                    AfterDelay(250, () => oldPos = player.Origin);

                    return player.IsPlayer;
                });
            });
        }

        /// <summary>function <c>StartAntiRQ</c> Anti-RQ function, originally made for infected servers.</summary>
        public void StartAntiRQ(Entity player)
        {
            player.ClosePlayerMenu();
            player.Notify("menuresponse", "team_marinesopfor", "axis");

            Thread(OnPlayerJoinTeam(player), (entRef, notify, paras) =>
            {
                if (notify == "disconnect" && player.EntRef == entRef)
                    return false;

                return true;
            });

            OnInterval(100, () =>
            {
                player.ClosePlayerMenu();
                return true;
            });
        }

        /// <summary>function <c>OnPlayerJoinTeam</c> Coroutine function. Triggers when the player joins a team.</summary>
        private static IEnumerator OnPlayerJoinTeam(Entity player)
        {
            while (true)
            {
                yield return player.WaitTill("joined_team");
                AfterDelay(500, () => { player.Notify("menuresponse", "changeclass", "class1"); });
            }
        }

        /// <summary>
        /// Play leader dialog for player
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="sound">Sound</param>
        public void PlayLeaderDialog(Entity player, string sound)
        {
            if (player.SessionTeam == "allies")
                player.PlayLocalSound(GetTeamVoicePrefix(GetMapCustom("allieschar")) + "1mc_" + sound);
            else
                player.PlayLocalSound(GetTeamVoicePrefix(GetMapCustom("axischar")) + "1mc_" + sound);
        }

        public string GetTeamVoicePrefix(string teamRef) => TableLookup("mp/factionTable.csv", 0, teamRef, 7);

        /// <summary>function <c>RemoveSentry</c> Removes entity "misc_turret" from the map.</summary>
        public void RemoveSentry()
        {
            for (int i = 18; i < 2048; i++)
            {
                Entity entity = GetEntByNum(i);
                if (entity != null)
                    if (!StriCmp(entity.Classname, "misc_turret"))
                        entity.Delete();
            }
        }

        /// <summary>function <c>TryFindDSR</c> Looks for the DSR in the admin folder. If it is missing it returns an empty string.</summary>
        public bool TryFindDSR(string name, out string dsrName)
        {
            if (!File.Exists($@"admin\{name}.dsr"))
            {
                Utilities.SayAll($"Could not find DSR *{name}*, the name is case sensitive");
                dsrName = "";
                return false;
            }

            dsrName = $"{name}.dsr";
            return true;
        }

        /// <summary>function <c>UnFreezePlayers</c> Unfreezes players.</summary>
        public void UnfreezePlayers()
        {
            foreach (Entity player in onlinePlayers)
                player.FreezeControls(false);
        }

        /// <summary>function <c>InitClassFields</c> Sets the fields of this class.</summary>
        private void InitClassFields()
        {
            dvars = new Dictionary<string, bool>
            {
                ["sv_Bounce"] = GetDvarInt("sv_Bounce") == 1,
                ["sv_KnifeEnabled"] = GetDvarInt("sv_KnifeEnabled") == 1,
                ["sv_UndoRCE"] = GetDvarInt("sv_UndoRCE") == 1,
                ["sv_RemoveBakaaraSentry"] = GetDvarInt("sv_RemoveBakaaraSentry") == 1,
                ["sv_hideCommands"] = GetDvarInt("sv_hideCommands") != 0,
                ["sv_AntiHardScope"] = GetDvarInt("sv_AntiHardScope") == 1,
                ["sv_AntiCamp"] = GetDvarInt("sv_AntiCamp") == 1,
                ["sv_autoBalance"] = GetDvarInt("sv_autoBalance") == 1,
                ["sv_LastStand"] = GetDvarInt("sv_LastStand") == 0,
                ["sv_NerfGuns"] = GetDvarInt("sv_NerfGuns") == 1,
                ["sv_ExplosivePrank"] = GetDvarInt("sv_ExplosivePrank") == 1,
                ["sv_DisableAkimbo"] = GetDvarInt("sv_DisableAkimbo") == 1,
                ["sv_AllPerks"] = GetDvarInt("sv_AllPerks") == 1,
                ["sv_LocalizedStr"] = GetDvarInt("sv_LocalizedStr") == 1,
                ["sv_AntiRQ"] = GetDvarInt("sv_AntiRQ") == 1,
                ["sv_MaxAmmoFillsClip"] = GetDvarInt("sv_MaxAmmoFillsClip") == 1
            };

            sv_balanceInterval = GetDvarInt("sv_balanceInterval");
            keyWords = GenerateKeyWords();

            try
            {
                string dvarCulture = GetDvar("sv_serverCulture");

                if (string.IsNullOrWhiteSpace(dvarCulture))
                    dvarCulture = "en-GB";

                culture = new CultureInfo(dvarCulture, false);
            }

            catch (CultureNotFoundException)
            {
                InfinityScript.Log.Write(LogLevel.Error, "Invalid Culture: Set correctly sv_serverCulture string dvar");
                culture = new CultureInfo("en-GB", false);
            }
        }

        private Dictionary<string, string> GenerateKeyWords()
        {
            string ball = "cardicon_8ball";
            string face = "facebook";

            var dict = new Dictionary<string, string>()
            {
                { "null", "\0" },
                { "controldel",  "\x7F" },
                { "control3", "\u0080\u009F\u001F" },
                { "controlone", "\x01\x01\x01" },
                { "xp", "^OOxp" },
                { "crash", "\x5e\x01\xCC\xCC\x0Ashellshock" },
                { "weird", "� ^������" },
                { "weird2", "^ÿÿÿÿ" },
                { "8ball", $"\x5E\x01\x3F\x3F\x0E{ball}" },
                { "face", $"\x5E\x01\x3F\x3F\x0F{face}" },
                { "cod2", "\x5E\x33\x5E\x01\x7F\x2F\x09\x6C\x6F\x67\x6F\x5F\x63\x6F\x64\x32\x5E\x01\x32\x2F\x07\x75\x69\x5F\x68\x6F\x73\x74\x5E\x01\x40" }
            };

            return dict;
        }
    }
}