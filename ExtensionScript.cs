﻿// ==================== ExtensionScript ===================
// Admin Manager via Rcon. It is recommended you
// use this script with IW4M 
// Project: https://github.com/diamante0018/ExtensionScript
// Author: Diavolo (https://github.com/diamante0018)
// License: GNU GPL v3.0
// ========================================================
using InfinityScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static InfinityScript.GSCFunctions;
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
        volatile string MapRotation = "";
        public static bool activeUnlimitedAmmo = false;
        private Kicker proKicker = new Kicker();
        private Teleporter teleport = new Teleporter();
        private BadWeapons weapons = new BadWeapons();
        private LoadoutName load;
        private bool fallDamage = false;
        private List<Entity> onlinePlayers = new List<Entity>();
        //private string DSRName = ""; //private Regex rx = new Regex(@"^[\w\-. ]+\.dsr$");

        public ExtensionScript()
        {
            IPrintLn("^1I am Diavolo and I lost my Mind. ^7DIA Script for 1.5 IS");
            InfinityScript.Log.Write(LogLevel.Info, "^1I am Diavolo and I lost my Mind.");
            SetDvarIfUninitialized("sv_hideCommands", "1");
            SetDvarIfUninitialized("sv_gmotd", "^:Welcome to ^4DIA ^:servers. https://discord.com/invite/");
            SetDvarIfUninitialized("sv_forceSmoke", "1");
            SetDvarIfUninitialized("sv_objText", "^7Join our Discord Server now! ^1https://discord.com/invite/");
            SetDvarIfUninitialized("sv_clientDvars", "1");
            SetDvarIfUninitialized("sv_rate", "210000");
            SetDvarIfUninitialized("sv_serverDvars", "1");
            SetDvarIfUninitialized("sv_killStreakCounter", "1");
            SetDvarIfUninitialized("sv_hudEnable", "1");
            //SetDvarIfUninitialized("sv_hudTop", "^1TOP Message");
            SetDvarIfUninitialized("sv_hudBottom", "^1Press ^7'Vote Yes' ^1for max ammo! ^7Discord: ^5https://discord.com/invite/");
            //SetDvarIfUninitialized("sv_hudRight", "^1Right Message");
            //SetDvarIfUninitialized("sv_hudLeft", "^1Left Message");
            SetDvarIfUninitialized("sv_scrollingSpeed", "30");
            SetDvarIfUninitialized("sv_scrollingHud", "1");
            SetDvarIfUninitialized("sv_b3Execute", "null");

            //Loading Server Dvars.
            ServerDvars();

            //HudElem For Information
            InformationHuds();

            //Assigning things.
            PlayerConnected += OnPlayerConnect;

            OnInterval(50, () =>
            {
                if (GetDvar("sv_b3Execute") != "null")
                {
                    string content = GetDvar("sv_b3Execute");
                    ProcessCommand(content);
                    SetDvar("sv_b3Execute", "null");
                }
                return true;
            });
            //Bounce and Bunny Hop related code
            unsafe
            {
                int[] addr = { 0x0422AB6, 0x0422AAF, 0x041E00C, 0x0414127, 0x04141b4, 0x0414e027, 0x0414b126, 0x041416d, 0x041417c };

                byte nop = 0x90;
                for (int i = 0; i < 7; ++i)
                {

                    *((byte*)addr[7] + i) = nop;
                    *((byte*)addr[8] + i) = nop;
                    *((byte*)addr[i]) = nop;
                    *((byte*)(addr[i] + 1)) = nop;
                }
            }

            Notified += OnNotified;

            OnInterval(60000, () =>
            {
                BalanceTeams();
                return true;
            });

            /*OnInterval(50, () =>
            {
                DSRName = ExtUtil.GetDSRName();
                if (!DSRName.Contains(".dsr"))
                    return true;
                return false;
            });
            */
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
                    Entity player = GetPlayer(arg1);
                    if (player.MyGetField("infiniteammo") == 1)
                        player.MyGiveMaxAmmo(false);
                    if (player.MyGetField("norecoil") == 1)
                        player.Player_RecoilScaleOn(0);
                    break;
                default:
                    break;
            }
        }

        public void ServerDvars()
        {
            if (GetDvarInt("sv_serverDvars") != 0)
            {
                SetDevDvar("sv_network_fps", 200);
                SetDvar("sv_hugeSnapshotSize", 10000);
                SetDvar("sv_hugeSnapshotDelay", 100);
                SetDvar("sv_pingDegradation", 0);
                SetDvar("sv_pingDegradationLimit", 9999);
                SetDvar("sv_acceptableRateThrottle", 9999);
                SetDvar("sv_newRateThrottling", 2);
                SetDvar("sv_minPingClamp", 50);
                SetDvar("sv_cumulThinkTime", 1000);
                SetDvar("sys_lockThreads", "all");
                SetDvar("com_maxFrameTime", 1000);
                SetDvar("com_maxFps", 0);
                SetDvar("sv_voiceQuality", 9);
                SetDvar("maxVoicePacketsPerSec", 1000);
                SetDvar("maxVoicePacketsPerSecForServer", 200);
                SetDvar("cg_everyoneHearsEveryone", 1);
                MakeDvarServerInfo("motd", GetDvar("sv_gmotd"));
                MakeDvarServerInfo("didyouknow", GetDvar("sv_gmotd"));
            }
        }

        public void InformationHuds()
        {
            if (GetDvarInt("sv_hudEnable") != 0)
            {
                if (GetDvar("sv_hudTop") != "null")
                {
                    top = HudElem.CreateServerFontString(HudElem.Fonts.HudBig, 0.5f);
                    top.SetPoint("TOPCENTER", "TOPCENTER", 0, 5);
                    top.HideWhenInMenu = true;
                    top.SetText(GetDvar("sv_hudTop"));
                }
                if (GetDvar("sv_hudRight") != "null")
                {
                    right = HudElem.CreateServerFontString(HudElem.Fonts.HudBig, 0.5f);
                    right.SetPoint("TOPRIGHT", "TOPRIGHT", -5, 5);
                    right.HideWhenInMenu = true;
                    right.SetText(GetDvar("sv_hudRight"));
                }
                if (GetDvar("sv_hudRight") != "null")
                {
                    left = HudElem.CreateServerFontString(HudElem.Fonts.HudBig, 0.5f);
                    left.SetPoint("TOPLEFT", "TOPLEFT", 6, 105);
                    left.HideWhenInMenu = true;
                    left.SetText(GetDvar("sv_hudLeft"));
                }
                if ((GetDvar("sv_hudBottom") != "null") && (GetDvarInt("sv_scrollingHud") != 0) && (GetDvarInt("sv_scrollingSpeed") != 0))
                {
                    bottom = HudElem.CreateServerFontString(HudElem.Fonts.HudBig, 0.4f);
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
                    bottom = HudElem.CreateServerFontString(HudElem.Fonts.HudBig, 0.5f);
                    bottom.SetPoint("BOTTOMCENTER", "BOTTOMCENTER", 0, -5);
                    bottom.HideWhenInMenu = true;
                    bottom.SetText(GetDvar("sv_hudBottom"));
                }
            }

        }

        public void OnPlayerConnect(Entity player)
        {
            player.MySetField("playerKillStreak", 0);
            onlinePlayers.Add(player);

            if (GetDvarInt("sv_clientDvars") != 0)
            {
                player.SetClientDvar("cg_objectiveText", GetDvar("sv_objText"));
                player.SetClientDvar("sys_lockThreads", "all");
                player.SetClientDvar("com_maxFrameTime", "1000");
                player.SetClientDvar("rate ", GetDvar("sv_rate"));

            }
            if (GetDvarInt("sv_forceSmoke") != 0)
                player.SetClientDvar("fx_draw", "1");

            //Killstreak Related Code
            var killstreakHud = HudElem.CreateFontString(player, HudElem.Fonts.HudSmall, 0.8f);
            killstreakHud?.SetPoint("TOP", "TOP", -9, 2);
            killstreakHud?.SetText("^5Killstreak: ");
            killstreakHud.HideWhenInMenu = true;

            var noKills = HudElem.CreateFontString(player, HudElem.Fonts.HudSmall, 0.8f);
            noKills?.SetPoint("TOP", "TOP", 39, 2);
            noKills?.SetText("^20");
            noKills.HideWhenInMenu = true;

            KillStreakHud[GetEntityNumber(player)] = killstreakHud;
            NoKillsHudElem[GetEntityNumber(player)] = noKills;

            //ID Related Code
            HudElem elem = HudElem.CreateFontString(player, HudElem.Fonts.HudBig, 0.5f);
            elem.SetPoint("BOTTOMCENTER", "BOTTOMCENTER", 0, -5);
            elem.SetText(string.Format("^0| ^5NAME ^0| ^7{0} ^0| ^5ID ^0| ^7{1}", player.Name, player.EntRef));
            elem.HideWhenInMenu = true;
            elem.GlowAlpha = 0f;

            //Welcomer Related Code
            AfterDelay(5000, () => player.TellPlayer("^5Welcome ^7to ^3DIA ^1Servers^0! ^7Vote Yes for ^2Ammo"));

            //Give Ammo Related Code
            player.NotifyOnPlayerCommand("giveammo", "vote yes"); ;

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
        }

        /// <summary>function <c>OnPlayerVoteYes</c> Co-routines function.</summary>
        private static IEnumerator OnPlayerVoteYes(Entity player)
        {
            while (true)
            {
                yield return player.WaitTill("giveammo");
                player.MyGiveMaxAmmo();
            }
        }

        /// <summary>function <c>OnPlayerSpawned</c> Co-routines function.</summary>
        private static IEnumerator OnPlayerSpawned(Entity player)
        {
            while (true)
            {
                yield return player.WaitTill("spawned_player");
                player.SetClientDvar("cg_objectiveText", GetDvar("sv_objText"));
                player.MyGiveMaxAmmo(false);

                if (player.MyGetField("wallhack") == 1)
                    player.ThermalVisionFOFOverlayOn();

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
            }
        }

        public void ProcessCommand(string message)
        {
            try
            {
                string[] msg = message.Split(' ');
                msg[0] = msg[0].ToLowerInvariant();

                if (msg[0].StartsWith("!afk"))
                {
                    Entity player = GetPlayer(msg[1]);
                    player.ChangeTeam("spectator");
                }
                else if (msg[0].StartsWith("!setafk"))
                {
                    Entity player = GetPlayer(msg[1]);
                    player.ChangeTeam("spectator");
                }
                else if (msg[0].StartsWith("!kill"))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (player.MyGetField("fly") != 1 && player.SessionTeam != "spectator")
                    {
                        player.Suicide();
                        Utilities.RawSayAll($"{player.Name} was killed");
                    }
                    else
                        Utilities.RawSayAll($"{player.Name} can't be killed since he is flying/spectator");
                }
                else if (msg[0].StartsWith("!suicide"))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (player.MyGetField("fly") != 1 && player.SessionTeam != "spectator")
                    {
                        player.Suicide();
                        Utilities.SayTo(player, "You commited suicide");
                    }
                    else
                        Utilities.SayTo(player, "You can't commit suicide");
                }
                else if (msg[0].StartsWith("!godmode"))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!player.MyHasField("godmodeon"))
                    {
                        player.MySetField("godmodeon", 0);
                    }
                    if (player.MyGetField("godmodeon") == 1)
                    {
                        player.Health = 100;
                        player.MySetField("godmodeon", 0);
                        Utilities.SayTo(player, "^1GodMode has been deactivated.");
                    }
                    else if (player.MyGetField("godmodeon") == 0)
                    {
                        player.Health = -1;
                        player.MySetField("godmodeon", 1);
                        Utilities.SayTo(player, "^1GodMode has been activated.");
                    }
                }
                else if (msg[0].StartsWith("!teleport"))
                {
                    Entity teleporter = GetPlayer(msg[1]);
                    Entity reciever = GetPlayer(msg[2]);
                    teleport.Teleport2Players(teleporter, reciever);
                }
                else if (msg[0].StartsWith("!mode"))
                {
                    if (!System.IO.File.Exists($@"admin\{msg[1]}.dsr"))
                    {
                        Utilities.RawSayAll("^1DSR not found.");
                        return;
                    }
                    Mode(msg[1]);
                }
                else if (msg[0].StartsWith("!gametype"))
                {
                    if (!System.IO.File.Exists($@"admin\{msg[1]}.dsr"))
                    {
                        Utilities.RawSayAll("^1DSR not found.");
                        return;
                    }
                    string newMap = msg[2];
                    Mode(msg[1], newMap);
                }
                else if (msg[0].StartsWith("!ac130"))
                {
                    if (msg[1].StartsWith("*all*"))
                    {
                        AC130All();
                        return;
                    }

                    Entity player = GetPlayer(msg[1]);
                    AfterDelay(500, () =>
                    {
                        player.TakeAllWeapons();
                        player.GiveWeapon("ac130_105mm_mp");
                        player.GiveWeapon("ac130_40mm_mp");
                        player.GiveWeapon("ac130_25mm_mp");
                        player.SwitchToWeaponImmediate("ac130_25mm_mp");
                    });
                }
                else if (msg[0].StartsWith("!blockchat"))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!player.MyHasField("muted"))
                    {
                        player.MySetField("muted", 0);
                    }
                    if (player.MyGetField("muted") == 1)
                    {
                        player.MySetField("muted", 0);
                        Utilities.RawSayAll($"{player.Name} ^1chat has been unblocked.");
                    }
                    else if (player.MyGetField("muted") == 0)
                    {
                        player.MySetField("muted", 1);
                        Utilities.RawSayAll($"{player.Name} ^1chat has been blocked.");
                    }
                }
                else if (msg[0].StartsWith("!freeze"))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!player.MyHasField("frozen"))
                    {
                        player.MySetField("frozen", 0);
                    }
                    if (player.MyGetField("frozen") == 1)
                    {
                        player.FreezeControls(false);
                        player.MySetField("frozen", 0);
                        Utilities.RawSayAll($"{player.Name} ^1has been unfrozen.");
                    }
                    else if (player.MyGetField("frozen") == 0)
                    {
                        player.FreezeControls(true);
                        player.MySetField("frozen", 1);
                        Utilities.RawSayAll($"{player.Name} ^1has been frozen.");
                    }
                }
                else if (msg[0].StartsWith("!changeteam"))
                {
                    Entity player = GetPlayer(msg[1]);
                    player.ChangeTeam();
                }
                else if (msg[0].StartsWith("!giveammo"))
                {
                    Entity player = GetPlayer(msg[1]);
                    player.MyGiveMaxAmmo();
                }
                else if (msg[0].StartsWith("!crash"))
                {
                    Entity player = GetPlayer(msg[1]);
                    proKicker.Crasher(player);
                    IPrintLn(string.Format("^1{0}'s game has been crashed", player.Name));
                }
                else if (msg[0].StartsWith("!reset"))
                {
                    Entity player = GetPlayer(msg[1]);
                    proKicker.Reset(player);
                }
                else if (msg[0].StartsWith("!close"))
                {
                    Entity player = GetPlayer(msg[1]);
                    proKicker.Close(player);
                }
                else if (msg[0].StartsWith("!teknoban"))
                {
                    Entity player = GetPlayer(msg[1]);
                    proKicker.Teknoban(player);
                    AfterDelay(3000, () => Utilities.ExecuteCommand($"dropclient {player.EntRef} You have been ^1permanently banned ^7from ^2Tekno^7MW3"));
                }
                else if (msg[0].StartsWith("!givegun"))
                {
                    Entity player = GetPlayer(msg[1]);
                    string gun = msg[2];
                    player.GiveWeapon(gun);
                    player.SwitchToWeaponImmediate(gun);
                }
                else if (msg[0].StartsWith("!servername"))
                {
                    Utilities.ExecuteCommand(string.Format("set sv_hostname {0}", msg[1]));
                }
                else if (msg[0].StartsWith("!clientdvar"))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (msg.Length < 4)
                    {
                        Utilities.RawSayAll($"^1{player.Name} ^7Client dvar can't be changed. Not enough arguments supplied: cdvar value.");
                        return;
                    }
                    player.SetClientDvar(msg[2], msg[3]);
                }
                else if (msg[0].StartsWith("!dvar"))
                {
                    if (msg.Length < 3)
                    {
                        Utilities.RawSayAll($"^1Dvar can't be changed. Not enough arguments supplied: dvar value.");
                        return;
                    }
                    Utilities.ExecuteCommand(string.Format("set {0} {1}", msg[1], msg[2]));
                }
                else if (msg[0].StartsWith("!name"))
                {
                    Entity player = GetPlayer(msg[1]);
                    player.SetName(CalculateString(msg[2]));
                }
                else if (msg[0].StartsWith("!clantag"))
                {
                    Entity player = GetPlayer(msg[1]);
                    player.SetClanTag(CalculateString(msg[2]));
                }
                else if (msg[0].StartsWith("!speed"))
                {
                    if (int.TryParse(msg[1], out int speed))
                        Utilities.Speed = speed;
                }
                else if (msg[0].StartsWith("!gravity"))
                {
                    if (int.TryParse(msg[1], out int gravity))
                        Utilities.Gravity = gravity;
                }
                else if (msg[0].StartsWith("!falldamage"))
                {
                    fallDamage = !fallDamage;
                    Utilities.FallDamage = fallDamage;
                }
                else if (msg[0].StartsWith("!jumpheight"))
                {
                    if (float.TryParse(msg[1], out float height))
                        Utilities.JumpHeight = height;
                }
                else if (msg[0].StartsWith("!knife"))
                {
                    if (IsKnifeEnabled())
                        DisableKnife();
                    else
                        EnableKnife();
                }
                else if (msg[0].StartsWith("!moab"))
                {
                    //Entity player = GetPlayer(msg[1]);
                    //TODO
                }
                else if (msg[0].StartsWith("!wh"))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!player.MyHasField("wallhack"))
                    {
                        player.MySetField("wallhack", 0);
                    }
                    if (player.MyGetField("wallhack") == 1)
                    {
                        player.ThermalVisionFOFOverlayOff();
                        player.MySetField("wallhack", 0);
                        Utilities.RawSayTo(player, "Wallhack is switched off");
                    }
                    else if (player.MyGetField("wallhack") == 0)
                    {
                        player.ThermalVisionFOFOverlayOn();
                        player.MySetField("wallhack", 1);
                        Utilities.RawSayTo(player, "Wallhack is switched on");
                    }
                }
                else if (msg[0].StartsWith("!aimbot"))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!player.MyHasField("aimbot"))
                    {
                        player.MySetField("aimbot", 0);
                    }
                    if (player.MyGetField("aimbot") == 1)
                    {
                        player.MySetField("aimbot", 0);
                        Utilities.RawSayTo(player, "Aimbot is switched off");
                    }
                    else if (player.MyGetField("aimbot") == 0)
                    {
                        GiveAimBot(player);
                        player.MySetField("aimbot", 1);
                        Utilities.RawSayTo(player, "Aimbot is switched on");
                    }
                }
                else if (msg[0].StartsWith("!norecoil"))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!player.MyHasField("norecoil"))
                    {
                        player.MySetField("norecoil", 0);
                    }
                    if (player.MyGetField("norecoil") == 1)
                    {
                        player.MySetField("norecoil", 0);
                        Utilities.RawSayTo(player, "No Recoil is switched off");
                    }
                    else if (player.MyGetField("norecoil") == 0)
                    {
                        player.Player_RecoilScaleOff();
                        player.MySetField("norecoil", 1);
                        Utilities.RawSayTo(player, "No Recoil is switched on");
                    }
                }
                else if (msg[0].StartsWith("!infiniteammo"))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!player.MyHasField("infiniteammo"))
                    {
                        player.MySetField("infiniteammo", 0);
                    }
                    if (player.MyGetField("infiniteammo") == 1)
                    {
                        player.MySetField("infiniteammo", 0);
                        Utilities.RawSayTo(player, "Infiniteammo is switched off");
                    }
                    else if (player.MyGetField("infiniteammo") == 0)
                    {
                        player.MySetField("infiniteammo", 1);
                        Utilities.RawSayTo(player, "Infiniteammo is switched on");
                    }
                }
                else if (msg[0].StartsWith("!hide"))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!player.MyHasField("hide"))
                    {
                        player.MySetField("hide", 0);
                    }
                    if (player.MyGetField("hide") == 1)
                    {
                        player.Show();
                        player.MySetField("hide", 0);
                        Utilities.RawSayTo(player, "You are not hidden");
                    }
                    else if (player.MyGetField("hide") == 0)
                    {
                        player.Hide();
                        player.MySetField("hide", 1);
                        Utilities.RawSayTo(player, "You are hidden");
                    }
                }
                else if (msg[0].StartsWith("!noclip"))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (player.MyGetField("fly") != 1 && player.SessionTeam != "spectator")
                        player.NoClip();
                    else
                        Utilities.SayTo(player, "You can't noclip as a spectator or when you are flying");
                }
                else if (msg[0].StartsWith("!colorclass"))
                {
                    Entity player = GetPlayer(msg[1]);
                    load = new LoadoutName(player);
                }
                else if (msg[0].StartsWith("!yell"))
                {
                    if (msg.Length < 2)
                        return;

                    string yell = "";
                    for (int i = 1; i < msg.Length; i++)
                        yell = yell + " " + msg[i];

                    IPrintLnBold(yell);
                }
                else if (msg[0].StartsWith("!tell"))
                {
                    if (msg.Length < 2)
                        return;

                    string tell = "";
                    for (int i = 1; i < msg.Length; i++)
                        tell = tell + " " + msg[i];

                    foreach (Entity player in Players)
                        player.TellPlayer(tell);
                }
                else if (msg[0].StartsWith("!save"))
                {
                    teleport.Save(msg[1], msg[2]);
                }
                else if (msg[0].StartsWith("!load"))
                {
                    teleport.Load(msg[1], msg[2]);
                }
                else if (msg[0].StartsWith("!fly"))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!player.MyHasField("fly"))
                    {
                        player.MySetField("fly", 0);
                    }
                    if (player.MyGetField("fly") == 1)
                    {
                        player.AllowSpectateTeam("freelook", false);
                        player.SessionState = "playing";
                        player.SetContents(100);
                        player.MySetField("fly", 0);
                        Utilities.RawSayTo(player, "You are not flying");
                    }
                    else if (player.MyGetField("fly") == 0)
                    {
                        if (player.SessionTeam == "spectator")
                        {
                            Utilities.SayTo(player, "You can't fly as a spectator");
                            return;
                        }

                        player.AllowSpectateTeam("freelook", true);
                        player.SessionState = "spectator";
                        player.SetContents(0);
                        player.MySetField("fly", 1);
                        Utilities.RawSayTo(player, "You are flying");
                    }
                }
                else if (msg[0].StartsWith("!explode"))
                {
                    foreach (Entity player in Players)
                    {
                        if (player.SessionTeam == "spectator")
                            continue;
                        Vector3 offset1 = player.Origin;
                        Vector3 offset2 = player.Origin;
                        offset1.Z -= 1000f;
                        offset2.Z += 6000f;
                        MagicBullet("uav_strike_projectile_mp", offset2, offset1, player);
                        offset2.X += 2000f;
                        MagicBullet("uav_strike_projectile_mp", offset2, offset1, player);
                        offset2.X -= 4000f;
                        MagicBullet("uav_strike_projectile_mp", offset2, offset1, player);
                        AfterDelay(3000, () =>
                        {
                            if (player.IsAlive)
                                player.Suicide();
                        });
                        player.TellPlayer("You have been ^1Killed ^7in a ^2very ^6Fancy ^7Way^0!");
                    }
                }
                else if (msg[0].StartsWith("!noweapon"))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!player.MyHasField("noweapon"))
                    {
                        player.MySetField("noweapon", 0);
                    }
                    if (player.MyGetField("noweapon") == 1)
                    {
                        player.EnableWeaponSwitch();
                        player.EnableWeaponPickup();
                        player.EnableWeapons();
                        player.GiveWeapon("rpg_mp");
                        player.MySetField("noweapon", 0);
                        Utilities.RawSayAll($"{player.Name} can fight back now");
                    }
                    else if (player.MyGetField("noweapon") == 0)
                    {
                        player.TakeWeapon(player.CurrentWeapon);
                        player.DisableWeaponSwitch();
                        player.DisableWeaponPickup();
                        player.DisableWeapons();
                        player.MySetField("noweapon", 1);
                        Utilities.RawSayAll($"{player.Name} weapons have been taken away from him");
                    }
                }
                else if (msg[0].StartsWith("!juggsuit"))
                {
                    if (msg[1].StartsWith("*all*"))
                    {
                        JuggSuitAll();
                        return;
                    }

                    Entity player = GetPlayer(msg[1]);
                    player.DetachAll();
                    player.ShowAllParts();
                    player.SetViewModel("viewhands_juggernaut_opforce");
                    player.SetModel("mp_fullbody_opforce_juggernaut");
                    player.Health += 2500;
                    player.TellPlayer("^2You ^7Have Been ^6Given ^7a ^1Jugg ^0Suit");
                }
            }
            catch (Exception e)
            {
                InfinityScript.Log.Write(LogLevel.Error, "Error in Command Processing. Error:" + e.Message + e.StackTrace);
            }
        }

        /// <summary>function <c>GiveAimBot</c> Gives 'aimbot' to the player. The loop that changes the player view calculates with each iteration what is the closest entity to lock on to.</summary>
        public void GiveAimBot(Entity player)
        {
            OnInterval(150, () =>
            {
                if (!player.IsPlayer || player.MyGetField("aimbot") != 1)
                    return false;

                Entity[] victims = SortByDistance(CleanOnlinePlayerList(player).ToArray(), player);
                if (victims.Length > 0)
                    player.SetPlayerAngles(VectorToAngles(victims[0].GetEye() - player.GetEye()));
                return true;
            });
        }

        /// <summary>function <c>AC130All</c> Gives to all players a toy AC130.</summary>
        public void AC130All()
        {
            foreach (Entity player in Players)
            {
                player.TakeAllWeapons();
                player.GiveWeapon("ac130_105mm_mp");
                player.GiveWeapon("ac130_40mm_mp");
                player.GiveWeapon("ac130_25mm_mp");
                player.SwitchToWeaponImmediate("ac130_25mm_mp");
            }
        }

        /// <summary>function <c>JuggSuitAll</c> Gives to all players a Jugg Suit.</summary>
        public void JuggSuitAll()
        {
            foreach (Entity player in Players)
            {
                player.DetachAll();
                player.ShowAllParts();
                player.SetViewModel("viewhands_juggernaut_opforce");
                player.SetModel("mp_fullbody_opforce_juggernaut");
                player.Health += 2500;
                player.TellPlayer("^2You ^7Have Been ^6Given ^7a ^1Jugg ^0Suit");
            }
        }

        private List<Entity> CleanOnlinePlayerList(Entity aimbotter)
        {
            List<Entity> cleanedList = onlinePlayers;
            cleanedList.Remove(aimbotter);
            foreach (Entity player in cleanedList)
            {
                if (player.Equals(aimbotter) || !player.IsPlayer)
                    cleanedList.Remove(player);
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
            using (System.IO.StreamWriter DSPLStream = new System.IO.StreamWriter("admin\\default.dspl"))
            {
                DSPLStream.WriteLine(map + "," + dsrname + ",1000");
            }
            MapRotation = GetDvar("sv_maprotation");
            OnExitLevel();
            Utilities.ExecuteCommand("sv_maprotation default");
            Utilities.ExecuteCommand("map_rotate");
            Utilities.ExecuteCommand("sv_maprotation " + MapRotation);
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
            if (!player.MyHasField("playerKillStreak") || !player.MyHasField("playerKillStreak"))
                return;
            try
            {
                if (player != attacker) //Suicide Alert!
                {
                    attacker.MySetField("playerKillStreak", attacker.MyGetField("playerKillStreak") + 1);
                }
                player.MySetField("playerKillStreak", 0);
                var attackerNoKills = NoKillsHudElem[GetEntityNumber(attacker)];
                if (attackerNoKills == null)
                {
                    throw new Exception($"AttackerNoKills is null. Attacker: {attacker.Name}");
                }
                attackerNoKills.SetText("^2" + attacker.MyGetField("playerKillStreak"));
                NoKillsHudElem[GetEntityNumber(attacker)] = attackerNoKills;

                var victimNoKills = NoKillsHudElem[GetEntityNumber(player)];
                if (victimNoKills == null)
                {
                    throw new Exception($"VictimNoKills is null. Victim: {player.Name}");
                }
                victimNoKills.SetText("0");
                NoKillsHudElem[GetEntityNumber(player)] = victimNoKills;
            }
            catch (Exception ex)
            {
                InfinityScript.Log.Write(LogLevel.Error, $"Error in Killstreak: {ex.Message} {ex.StackTrace}");
                return;
            }
        }

        public override void OnPlayerDisconnect(Entity player)
        {
            player.MyRemoveField();
            onlinePlayers.Remove(player);
        }

        /// <summary>function <c>OnPlayerDamage</c> If the player is damaged by a 'bad' weapon his health is restored.</summary>
        public override void OnPlayerDamage(Entity player, Entity inflictor, Entity attacker, int damage, int dFlags, string mod, string weapon, Vector3 point, Vector3 dir, string hitLoc) => weapons.GiveHealthBack(player, weapon, damage);

        /// <summary>function <c>OnSay2</c> If the player is muted or the message starts with ! or @ the message will be censored and it will not be seen by other players.</summary>
        public override EventEat OnSay2(Entity player, string name, string message)
        {

            message = message.ToLower();
            if ((message.StartsWith("!")) || (message.StartsWith("@")))
            {
                if (GetDvarInt("sv_hideCommands") != 0)
                    return EventEat.EatGame;
            }

            if (player.MyGetField("muted") == 1)
            {
                return EventEat.EatGame;
            }
            return EventEat.EatNone;
        }

        /// <summary>function <c>BalanceTeams</c> Balances teams. It makes sure that if you are on a high killstreak you won't be balanced.</summary>
        public void BalanceTeams()
        {
            if (GetDvar("g_gametype") == "ffa" || GetDvar("g_gametype") == "gg")
                return;

            List<Entity> axis = new List<Entity>();
            List<Entity> allies = new List<Entity>();
            foreach (Entity player in Players)
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

            int difference = Convert.ToInt32(Math.Abs(axis.Count - allies.Count) / 2.0);

            if (difference > 0)
            {
                IEnumerable<Entity> sortByKillstreak = (axis.Count <= allies.Count) ? allies.OrderBy((Entity player) => player.MyGetField("playerKillStreak")).Take(difference).ToList() : axis.OrderBy((Entity player) => player.MyGetField("playerKillStreak")).Take(difference).ToList();
                foreach (Entity player in sortByKillstreak)
                {
                    player.ChangeTeam();
                    player.IPrintLnBold("You have been balanced");
                }
            }
        }

        /// <summary>function <c>CalculateString</c> Takes as input what the player typed when using !clantag or !name and check if it matches a keyword, it then returns a new string.</summary>
        private string CalculateString(string input)
        {
            switch (input)
            {
                case "null":
                    input = "\0";
                    break;
                case "controldel":
                    input = "\u007F";
                    break;
                case "control3":
                    input = "\u0080\u009F\u001F";
                    break;
                case "controlblank":
                    input = "\u0000";
                    break;
                case "xp":
                    input = "^OOxp";
                    break;
                default:
                    break;
            }
            return input;
        }

        /// <summary>function <c>DisableKnife</c> Disables knifes, useful for iSnipe.</summary>
        private unsafe void DisableKnife() => *(float*)95880920 = 0f;

        /// <summary>function <c>EnableKnife</c> Enables knifes.</summary>
        private unsafe void EnableKnife() => *(float*)95880920 = 64f;

        private unsafe bool IsKnifeEnabled() => *(float*)95880920 == 64f;
    }
}