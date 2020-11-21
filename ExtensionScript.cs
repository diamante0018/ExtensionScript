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
        private Teleporter teleport = new Teleporter();
        private Welcomer welcome = new Welcomer();
        private BadWeapons weapons = new BadWeapons();
        private LoadoutName load;
        private bool fallDamage = false;
        private List<Entity> onlinePlayers = new List<Entity>();
        //Noclip Related Code
        private static int noClipAddress = 0x01AC56C0;
        private bool noClip = false;

        public ExtensionScript()
        {
            IPrintLn("^1I am Diavolo and I lost my Mind. ^7DIA Script for 1.5 IS");
            InfinityScript.Log.Write(LogLevel.Info, "^1I am Diavolo and I lost my Mind.");

            //Making and Settings dvars if they are unused and have value
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
            MySetField(player, "playerKillStreak", 0);
            onlinePlayers.Add(player);

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

            //Welcomer Related Code
            AfterDelay(5000, () =>
            {
                welcome.TellPlayer(player, "^5Welcome ^7to ^3DIA ^1Servers^0! ^7Vote Yes for ^2Ammo");
            });

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

                if (player.HasWeapon("stinger_mp"))
                {
                    player.TakeWeapon("stinger_mp");
                    player.GiveWeapon("iw5_usp45_mp");
                    player.SetWeaponAmmoClip("iw5_usp45_mp", 0);
                    player.SetWeaponAmmoStock("iw5_usp45_mp", 0);

                }

                if (player.HasWeapon("flash_grenade_mp"))
                {
                    player.SetWeaponAmmoStock("flash_grenade_mp", 1);
                }

                else if (player.HasWeapon("concussion_grenade_mp"))
                {
                    player.SetWeaponAmmoStock("concussion_grenade_mp", 1);
                }
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
                        player.Health = 100;
                        MySetField(player, "godmodeon", 0);
                        Utilities.SayTo(player, "^1GodMode has been deactivated.");
                    }
                    else if (MyGetField(player, "godmodeon") == 0)
                    {
                        player.Health = -1;
                        MySetField(player, "godmodeon", 1);
                        Utilities.SayTo(player, "^1GodMode has been activated.");
                    }
                }
                if (msg[0].StartsWith("!teleport"))
                {
                    Entity teleporter = GetPlayer(msg[1]);
                    Entity reciever = GetPlayer(msg[2]);
                    teleport.Teleport2Players(teleporter, reciever);
                }
                if (msg[0].StartsWith("!mode"))
                {
                    if (!System.IO.File.Exists($@"admin\{msg[1]}.dsr"))
                    {
                        Utilities.RawSayAll("^1DSR not found.");
                        return;
                    }
                    Mode(msg[1]);
                }
                if (msg[0].StartsWith("!gametype"))
                {
                    if (!System.IO.File.Exists($@"admin\{msg[1]}.dsr"))
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
                    if (!MyHasField(player, "muted"))
                    {
                        MySetField(player, "muted", 0);
                    }
                    if (MyGetField(player, "muted") == 1)
                    {
                        MySetField(player, "muted", 0);
                        Utilities.RawSayAll($"{player.Name} ^1chat has been unblocked.");
                    }
                    else if (MyGetField(player, "muted") == 0)
                    {
                        MySetField(player, "muted", 1);
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
                    if (MyGetField(player, "frozen") == 1)
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
                    player.MyGiveMaxAmmo();
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
                    AfterDelay(3000, () => Utilities.ExecuteCommand($"dropclient {player.EntRef} You have been ^1permanently banned ^7from ^2Tekno^7MW3"));
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
                    if (msg.Length < 4)
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
                    if (int.TryParse(msg[1], out int speed))
                        Utilities.Speed = speed;
                }
                if (msg[0].StartsWith("!gravity"))
                {
                    if (int.TryParse(msg[1], out int gravity))
                        Utilities.Gravity = gravity;
                }
                if (msg[0].StartsWith("!falldamage"))
                {
                    fallDamage = !fallDamage;
                    Utilities.FallDamage = fallDamage;
                }
                if (msg[0].StartsWith("!jumpheight"))
                {
                    if (float.TryParse(msg[1], out float height))
                        Utilities.JumpHeight = height;
                }
                if (msg[0].StartsWith("!moab"))
                {
                    //Entity player = GetPlayer(msg[1]);
                    //TODO
                }
                if (msg[0].StartsWith("!wh"))
                {
                    Entity player = GetPlayer(msg[1]);
                    if (!MyHasField(player, "wallhack"))
                    {
                        MySetField(player, "wallhack", 0);
                    }
                    if (MyGetField(player, "wallhack") == 1)
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
                    if (!MyHasField(player, "aimbot"))
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
                if (msg[0].StartsWith("!noclip"))
                {
                    byte set;
                    if (noClip)
                        set = 0x00;
                    else
                        set = 0x01;
                    noClip = !noClip;

                    unsafe
                    {
                        *(byte*)noClipAddress = set;
                    }
                    Utilities.RawSayAll($"^1No clip is ^2{noClip}");
                }
                if (msg[0].StartsWith("!colorclass"))
                {
                    Entity player = GetPlayer(msg[1]);
                    load = new LoadoutName(player);
                }
                if (msg[0].StartsWith("!yell"))
                {
                    if (msg.Length < 2)
                        return;

                    string yell = "";
                    for (int i = 1; i < msg.Length; i++)
                        yell = yell + " " + msg[i];

                    IPrintLnBold(yell);
                }
                if (msg[0].StartsWith("!tell"))
                {
                    if (msg.Length < 2)
                        return;

                    string tell = "";
                    for (int i = 1; i < msg.Length; i++)
                        tell = tell + " " + msg[i];

                    foreach (Entity player in Players)
                        welcome.TellPlayer(player, tell);
                }
                if (msg[0].StartsWith("!save"))
                {
                    teleport.Save(msg[1], msg[2]);
                }
                if (msg[0].StartsWith("!load"))
                {
                    teleport.Load(msg[1], msg[2]);
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
                if (!player.IsPlayer || MyGetField(player, "aimbot") != 1)
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
        public static int GetEntityNumber(Entity player)
        {
            return player.GetEntityNumber();
        }

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

        /// <summary>function <c>ChangeTeam</c> Changes the team of the player. Target team specified in the arguments.</summary>
        public void ChangeTeam(Entity player, string team)
        {
            player.SetField("sessionteam", team);
            player.Notify("menuresponse", "team_marinesopfor", team);
        }

        /// <summary>function <c>OnPlayerKilled</c> Killstreak counter.</summary>
        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            if (!MyHasField(player, "playerKillStreak") || !MyHasField(attacker, "playerKillStreak"))
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

        public override void OnPlayerDisconnect(Entity player)
        {
            fields.Remove(player.HWID);
            onlinePlayers.Remove(player);
        }

        /// <summary>function <c>OnPlayerDamage</c> If the player is damaged by a 'bad' weapon his health is restored.</summary>
        public override void OnPlayerDamage(Entity player, Entity inflictor, Entity attacker, int damage, int dFlags, string mod, string weapon, Vector3 point, Vector3 dir, string hitLoc)
        {
            weapons.GiveHealthBack(player, weapon, damage);
        }

        /// <summary>function <c>OnSay2</c> If the player is muted or the message starts with ! or @ the message will be censored and it will not be seen by other players.</summary>
        public override EventEat OnSay2(Entity player, string name, string message)
        {

            message = message.ToLower();
            if ((message.StartsWith("!")) || (message.StartsWith("@")))
            {
                if (GetDvarInt("sv_hideCommands") != 0)
                    return EventEat.EatGame;
            }

            if (MyGetField(player, "muted") == 1)
            {
                return EventEat.EatGame;
            }
            return EventEat.EatNone;
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

        private bool MyHasField(Entity player, string field)
        {
            if (!player.IsPlayer)
                return false;
            if (fields.ContainsKey(player.HWID))
                return fields[player.HWID].ContainsKey(field);
            return false;
        }

        private void MySetField(Entity player, string field, int value)
        {
            if (!player.IsPlayer)
                return;
            if (!fields.ContainsKey(player.HWID))
                fields.Add(player.HWID, new Dictionary<string, int>());

            if (!MyHasField(player, field))
                fields[player.HWID].Add(field, value);
            else
                fields[player.HWID][field] = value;
        }

        private int MyGetField(Entity player, string field)
        {
            if (!player.IsPlayer)
                return int.MinValue;
            if (!MyHasField(player, field))
                return int.MinValue;
            return fields[player.HWID][field];
        }
    }
}