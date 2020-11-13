using InfinityScript;
using System;
using System.Collections;
using System.Collections.Generic;
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
        private Dictionary<string, Dictionary<string, int>> fields = new Dictionary<string, Dictionary<string, int>>();


        public ExtensionScript()
        {
            IPrintLn("^1I am Diavolo and I lost my Mind. ^7AG Script for 1.5.0 IS");
            InfinityScript.Log.Write(LogLevel.Info, "^1I am Diavolo and I lost my Mind.");

            //Making and Settings dvars if they are unused and have value
            SetDvarIfUninitialized("sv_hideCommands", "1");
            SetDvarIfUninitialized("sv_gmotd", "^:Welcome to ^4AG ^:servers. https://discord.com/invite/TaZEWrRh");
            SetDvarIfUninitialized("sv_forceSmoke", "1");
            SetDvarIfUninitialized("sv_objText", "^7 Join our Discord Server now! ^1https://discord.com/invite/TaZEWrRh");
            SetDvarIfUninitialized("sv_clientDvars", "1");
            SetDvarIfUninitialized("sv_rate", "210000");
            SetDvarIfUninitialized("sv_serverDvars", "1");
            SetDvarIfUninitialized("sv_killStreakCounter", "1");
            SetDvarIfUninitialized("sv_hudEnable", "1");
            //SetDvarIfUninitialized("sv_hudTop", "^1TOP Message");
            //SetDvarIfUninitialized("sv_hudBottom", "^1Bottom Message");
            //SetDvarIfUninitialized("sv_hudRight", "^1Right Message");
            //SetDvarIfUninitialized("sv_hudLeft", "^1Left Message");
            SetDvarIfUninitialized("sv_scrollingSpeed", "0");
            SetDvarIfUninitialized("sv_scrollingHud", "0");
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
                //Function.Call("makedvarserverinfo", "motd", Call<string>("getDvar", "sv_gmotd"));
                //Function.Call("makedvarserverinfo", "didyouknow", Call<string>("getDvar", "sv_gmotd"));
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
            MySetField(player, "playerKillStreak",0);

            if (GetDvarInt("sv_clientDvars") != 0)
            {
                player.SetClientDvar("cg_objectiveText", GetDvar("sv_objText"));
                player.SetClientDvar("sys_lockThreads", "all");
                player.SetClientDvar("com_maxFrameTime", "1000");
                player.SetClientDvar("rate ", GetDvar("sv_rate"));

            }
            if (GetDvarInt("sv_forceSmoke") != 0)
            {
                player.SetClientDvar("fx_draw", "1");
            }

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

            Thread(OnPlayerSpawned(player), (entRef, notify, paras) =>
            {
                // This is endon function
                // If that delegate return false then this coroutine will not be processed
                if (notify == "disconnect" && player.EntRef == entRef)
                    return false;

                return true;
            });  
        }

        private static IEnumerator OnPlayerSpawned(Entity player)
        {
            while (true)
            {
                yield return player.WaitTill("spawned_player");
                player.SetClientDvar("cg_objectiveText", GetDvar("sv_objText"));
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
                    ChangeTeam(player, "spectator");
                }
                if (msg[0].StartsWith("!setafk"))
                {
                    Entity player = GetPlayer(msg[1]);
                    ChangeTeam(player, "spectator");
                }
                if (msg[0].StartsWith("!kill"))
                {
                    Entity player = GetPlayer(msg[1]);
                    player.Suicide();
                }
                if (msg[0].StartsWith("!suicide"))
                {
                    Entity player = GetPlayer(msg[1]);
                    player.Suicide();
                }
                if (msg[0].StartsWith("!godmode"))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!MyHasField(player, "godmodeon"))
                    {
                        MySetField(player, "godmodeon", 0);
                    }
                    if (MyGetField(player, "godmodeon") == 1)
                    {
                        player.Health = 30;
                        MySetField(player, "godmodeon", 0);
                        Utilities.SayTo(player, "^1GodMode has been deactivated.");
                    }
                    else if (MyGetField(player, "godmodeon") == 0)
                    {
                        player.Health = -1;
                        MySetField(player, "godmodeon", 0);
                        Utilities.SayTo(player, "^1GodMode has been activated.");
                    }
                }
                if (msg[0].StartsWith("!teleport"))
                {
                    Entity teleporter = GetPlayer(msg[1]);
                    Entity reciever = GetPlayer(msg[2]);

                    teleporter.SetOrigin(reciever.Origin);
                }
                if (msg[0].StartsWith("!mode"))
                {
                    if (!System.IO.File.Exists($@"admin\{msg[1]}.dsr") && !System.IO.File.Exists($@"players2\{msg[1]}.dsr"))
                    {
                        Utilities.RawSayAll("^1DSR not found.");
                        return;
                    }
                    Mode(msg[1]);
                }
                if (msg[0].StartsWith("!gametype"))
                {
                    if (!System.IO.File.Exists($@"admin\{msg[1]}.dsr") && !System.IO.File.Exists($@"players2\{msg[1]}.dsr"))
                    {
                        Utilities.RawSayAll("^1DSR not found.");
                        return;
                    }
                    string newMap = msg[2];
                    Mode(msg[1], newMap);
                }
                if (msg[0].StartsWith("!ac130"))
                {
                    if (msg[1].StartsWith("*all*"))
                    {
                        AC130All();
                    }
                    else
                    {
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
                }
                if (msg[0].StartsWith("!blockchat"))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!MyHasField(player,"muted"))
                    {
                        MySetField(player, "muted", 0);
                    }
                    if (MyGetField(player,"muted") == 1)
                    {
                        MySetField(player, "muted", 0);
                        Utilities.RawSayAll($"{player.Name} ^1chat has been unblocked.");
                    }
                    else if (MyGetField(player, "muted") == 0)
                    {
                        MySetField(player, "muted", 0);
                        Utilities.RawSayAll($"{player.Name} ^1chat has been blocked.");
                    }
                }
                if (msg[0].StartsWith("!freeze"))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!MyHasField(player, "frozen"))
                    {
                        MySetField(player, "frozen", 0);
                    }
                    if (MyGetField(player,"frozen") == 1)
                    {
                        player.FreezeControls(false);
                        MySetField(player, "frozen", 0);
                        Utilities.RawSayAll($"{player.Name} ^1has been unfrozen.");
                    }
                    else if (MyGetField(player, "frozen") == 0)
                    {
                        player.FreezeControls(true);
                        MySetField(player, "frozen", 1);
                        Utilities.RawSayAll($"{player.Name} ^1has been frozen.");
                    }
                }
                if (msg[0].StartsWith("!changeteam"))
                {
                    Entity player = GetPlayer(msg[1]);
                    string playerteam = player.GetField<string>("sessionteam");

                    switch (playerteam)
                    {
                        case "axis":
                            ChangeTeam(player, "allies");
                            break;
                        case "allies":
                            ChangeTeam(player, "axis");
                            break;
                        case "spectator":
                            Utilities.RawSayAll($"^1{player.Name} team can't be changed because he is already spectator.");
                            break;
                    }
                }
                if (msg[0].StartsWith("!giveammo"))
                {
                    Entity player = GetPlayer(msg[1]);
                    string gun = player.GetCurrentWeapon();
                    player.GiveStartAmmo(gun);
                    player.GiveMaxAmmo(gun);
                }
                if (msg[0].StartsWith("!crash"))
                {
                    Entity player = GetPlayer(msg[1]);
                    proKicker.Crasher(player);
                    IPrintLn(string.Format("^1{0}'s game has been crashed", player.Name));
                }
                if (msg[0].StartsWith("!reset"))
                {
                    Entity player = GetPlayer(msg[1]);
                    proKicker.Reset(player);
                }
                if (msg[0].StartsWith("!close"))
                {
                    Entity player = GetPlayer(msg[1]);
                    proKicker.Close(player);
                }
                if (msg[0].StartsWith("!teknoban"))
                {
                    Entity player = GetPlayer(msg[1]);
                    proKicker.Teknoban(player);
                }
                if (msg[0].StartsWith("!givegun"))
                {
                    Entity player = GetPlayer(msg[1]);
                    string gun = msg[2];
                    player.GiveWeapon(gun);
                    player.SwitchToWeaponImmediate(gun);
                }
                if (msg[0].StartsWith("!servername"))
                {
                    Utilities.ExecuteCommand(string.Format("set sv_hostname {0}", msg[1]));
                }
                if (msg[0].StartsWith("!clientdvar"))
                {
                    Entity player = GetPlayer(msg[1]);
                    if(msg.Length < 4)
                    {
                        Utilities.RawSayAll($"^1{player.Name} ^7Client dvar can't be changed. Not enough arguments supplied: cdvar value.");
                        return;
                    }
                    player.SetClientDvar(msg[2], msg[3]);
                }
                if (msg[0].StartsWith("!dvar"))
                {
                    if (msg.Length < 3)
                    {
                        Utilities.RawSayAll($"^1Dvar can't be changed. Not enough arguments supplied: dvar value.");
                        return;
                    }
                    Utilities.ExecuteCommand(string.Format("set {0} {1}", msg[1], msg[2]));
                }
                if (msg[0].StartsWith("!name"))
                {
                    Entity player = GetPlayer(msg[1]);
                    player.SetName(CalculateString(msg[2]));
                }
                if (msg[0].StartsWith("!clantag"))
                {
                    Entity player = GetPlayer(msg[1]);
                    player.SetClanTag(CalculateString(msg[2]));
                }
                if (msg[0].StartsWith("!speed"))
                {     
                    int.TryParse(msg[1], out int speed);
                    Utilities.Speed = speed;
                }
                if (msg[0].StartsWith("!gravity"))
                {
                    int.TryParse(msg[1], out int gravity);
                    Utilities.Gravity = gravity;
                }
                if (msg[0].StartsWith("!falldamage"))
                {
                    bool gravity = bool.Parse(msg[1]);
                    Utilities.FallDamage = gravity;
                }
                if (msg[0].StartsWith("!jumpheight"))
                {
                    float.TryParse(msg[1], out float height);
                    Utilities.JumpHeight = height;
                }
                if (msg[0].StartsWith("!moab"))
                {
                    Entity player = GetPlayer(msg[1]);
                    //TODO
                }
                if (msg[0].StartsWith("!wh"))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!MyHasField(player, "wallhack"))
                    {
                        MySetField(player, "wallhack", 0);
                    }
                    if (MyGetField(player,"wallhack") == 1)
                    {
                        player.ThermalVisionFOFOverlayOff();
                        MySetField(player, "wallhack", 0);
                        Utilities.RawSayTo(player, "Wallhack is switched off");
                    }
                    else if (MyGetField(player, "wallhack") == 0)
                    {
                        player.ThermalVisionFOFOverlayOn();
                        MySetField(player, "wallhack", 1);
                        Utilities.RawSayTo(player, "Wallhack is switched on");
                    }
                }
                if (msg[0].StartsWith("!aimbot"))
                {
                    Entity player = GetPlayer(msg[1]);
                    if(!MyHasField(player, "aimbot"))
                    {
                        MySetField(player, "aimbot", 0);
                    }
                    if (MyGetField(player, "aimbot") == 1)
                    {
                        MySetField(player, "aimbot", 0);
                        Utilities.RawSayTo(player, "Aimbot is switched off");
                    }
                    else if (MyGetField(player, "aimbot") == 0)
                    {
                        GiveAimBot(player);
                        MySetField(player, "aimbot", 1);
                        Utilities.RawSayTo(player, "Aimbot is switched on");
                    }
                }
                if (msg[0].StartsWith("!fly"))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!MyHasField(player, "fly"))
                    {
                        MySetField(player, "fly", 0);
                    }
                    if (MyGetField(player, "fly") == 1)
                    {
                        player.AllowSpectateTeam("freelook", false);
                        player.SetField("sessionstate", "playing");
                        player.SetContents(100);
                        MySetField(player, "fly", 0);
                    }
                    else if (MyGetField(player, "fly") == 0)
                    {
                        player.AllowSpectateTeam("freelook", true);
                        player.SetField("sessionstate", "spectator");
                        player.SetContents(0);
                        MySetField(player, "fly", 1);
                    }
                }
            }
            catch (Exception e)
            {
                InfinityScript.Log.Write(LogLevel.Error,"Error in Command Processing. Error:" + e.Message + e.StackTrace);
            }
        }

        public void GiveAimBot(Entity player)
        {
            OnInterval(1000, () =>
            {
                if (!player.IsPlayer && !player.HasField("aimbot") && player.GetField<int>("aimbot") != 1)
                    return false;
                Entity[] victims = SortByDistance(Players.ToArray(), player);
                if (victims.Length > 1)
                    player.SetPlayerAngles(VectorToAngles(victims[0].Origin - player.GetEye()));
                return true;
            }); 
        }

        public void AC130All()
        {
            List<Entity> MyPlayers = Players;
            foreach (Entity player in MyPlayers)
            {
                player.TakeAllWeapons();
                player.GiveWeapon("ac130_105mm_mp");
                player.GiveWeapon("ac130_40mm_mp");
                player.GiveWeapon("ac130_25mm_mp");
                player.SwitchToWeaponImmediate("ac130_25mm_mp");
            }
        }

        public static int GetEntityNumber(Entity player)
        {
            return player.GetEntityNumber();
        }

        public void Mode(string dsrname, string map = "")
        {
            if (string.IsNullOrWhiteSpace(map))
                map = GetDvar("mapname");

            if (!string.IsNullOrWhiteSpace(MapRotation))
            {
                return;
            }

            map = map.Replace("default:", "");
            using (System.IO.StreamWriter DSPLStream = new System.IO.StreamWriter("players2\\EX.dspl"))
            {
                DSPLStream.WriteLine(map + "," + dsrname + ",1000");
            }
            MapRotation = GetDvar("sv_maprotation");
            OnExitLevel();
            Utilities.ExecuteCommand("sv_maprotation EX");
            Utilities.ExecuteCommand("map_rotate");
            Utilities.ExecuteCommand("sv_maprotation " + MapRotation);
            MapRotation = "";
        }

        public Entity GetPlayer(string entRef)
        {
            int.TryParse(entRef, out int IntegerentRef);
            return Entity.GetEntity(IntegerentRef);
        }

        public void ChangeTeam(Entity player, string team)
        {
            player.SetField("sessionteam", team);
            player.Notify("menuresponse", "team_marinesopfor", team);
        }

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            if (!MyHasField(player,"playerKillStreak") || !MyHasField(attacker, "playerKillStreak"))
                return;
            try
            {
                if (player != attacker) //Suicide Alert!
                {
                    MySetField(attacker, "playerKillStreak", MyGetField(attacker, "playerKillStreak") + 1);
                }
                MySetField(player, "playerKillStreak", 0);
                var attackerNoKills = NoKillsHudElem[GetEntityNumber(attacker)];
                if (attackerNoKills == null)
                {
                    throw new Exception("AttackerNoKills is null. Attacker: " + attacker.Name);
                }
                attackerNoKills.SetText("^2" + MyGetField(attacker, "playerKillStreak"));
                NoKillsHudElem[GetEntityNumber(attacker)] = attackerNoKills;

                var victimNoKills = NoKillsHudElem[GetEntityNumber(player)];
                if (victimNoKills == null)
                {
                    throw new Exception("VictimNoKills is null. Victim: " + player.Name);
                }
                victimNoKills.SetText("0");
                NoKillsHudElem[GetEntityNumber(player)] = victimNoKills;
            }
            catch (Exception ex)
            {
                InfinityScript.Log.Write(LogLevel.Error, "Error in Killstreak: " + ex.Message + ex.StackTrace);
                return;
            }
        }


        public override EventEat OnSay2(Entity player, string name, string message)
        {

            message = message.ToLower();
            if ((message.StartsWith("!")) || (message.StartsWith("@")))
            {
                if (GetDvarInt("sv_hideCommands") != 0)
                    return EventEat.EatGame;
            }

            if (MyGetField(player,"muted") == 1)
            {
                return EventEat.EatGame;
            }
            return EventEat.EatNone;
        }

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

        private bool MyHasField(Entity player, string field)
        {
            if (fields.ContainsKey(player.Name))
                return fields[player.Name].ContainsKey(field);
            return false;
        }

        private void MySetField(Entity player, string field, int value)
        {
            if (!fields.ContainsKey(player.Name))
                fields.Add(player.Name, new Dictionary<string, int>());

            if (!MyHasField(player, field))
                fields[player.Name].Add(field, value);
            else
                fields[player.Name][field] = value;
        }

        private int MyGetField(Entity player, string field)
        {
            if (!MyHasField(player, field))
                return -1;
            return fields[player.Name][field];
        }
    }
}