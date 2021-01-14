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
using static InfinityScript.GSCFunctions;
using static InfinityScript.ThreadScript;

namespace ExtensionScript
{
    public class Nuke : BaseScript
    {
        public static Nuke NukeFuncs;
        private int[] effects = new int[3];
        private int nukeTimer = 10;
        private int cancelMode;
        private int nukeEmpTimeout = 60;
        private bool nukeIncoming;
        private Entity nukeInfo;
        private bool isTeamBased = true;
        private int nukeEmpTimeRemaining;
        private int killsToNuke = 25;
        private bool nukeChainsKills;
        private bool destroyExplosives;
        private bool explosivesDestroyed;
        private readonly Entity level = Entity.GetEntity(2046);
        private BadWeapons weapons = new BadWeapons();

        public Nuke()
        {
            NukeFuncs = this;

            effects[0] = LoadFX("explosions/player_death_nuke");
            effects[1] = LoadFX("explosions/player_death_nuke_flash");
            effects[2] = LoadFX("dust/nuke_aftermath_mp");
            cancelMode = 0;

            SetDvarIfUninitialized("scr_killsToNuke", 25);
            SetDvarIfUninitialized("scr_killstreaksChainToNuke", 0);
            SetDvarIfUninitialized("scr_nukeDestroysExplosives", 0);

            nukeInfo = Spawn("script_model", Vector3.Zero);
            level.MySetField("teamNukeEMPed_axis", 0);
            level.MySetField("teamNukeEMPed_allies", 0);
            level.MySetField("teamNukeEMPed_none", 0);

            killsToNuke = GetDvarInt("scr_killsToNuke");
            nukeChainsKills = GetDvarInt("scr_killstreaksChainToNuke") == 1;
            destroyExplosives = GetDvarInt("scr_nukeDestroysExplosives") == 1;

            string gametype = GetDvar("g_gametype");
            if (gametype == "dm" || gametype == "gun" || gametype == "oic" || gametype == "jugg")
                isTeamBased = false;

            PlayerConnected += OnPlayerConnected;
            Notified += OnNotified;
        }

        public void OnNotified(int arg1, string arg2, Parameter[] arg3)
        {
            switch (arg2)
            {
                case "weapon_change":
                    OnPlayerWeaponChange(arg1, arg3);
                    break;
                case "nuke_death":
                    SetSlowMotion(0.25f, 1, 2f);
                    VisionSetNaked("aftermath", 5);
                    VisionSetPain("aftermath");
                    break;
                default:
                    break;
            }
        }

        public void OnPlayerConnected(Entity player)
        {
            player.MySetField("hasNuke", 0);
            player.MySetField("hasFauxNuke", 0);
            player.MySetField("killstreak", 0);

            if (level.MyHasField("nukeDetonated"))
                player.VisionSetNakedForPlayer("aftermath", 0);

            Thread(OnPlayerSpawned(player), (entRef, notify, paras) =>
            {
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
                AfterDelay(50, () =>
                {
                    if (NukeFuncs.level.MyGetField("teamNukeEMPed_" + player.SessionTeam).As<int>() == 1)
                    {
                        if (NukeFuncs.isTeamBased)
                            player.SetEMPJammed(true);

                        else if (NukeFuncs.nukeInfo.MyHasField("player") && !player.Equals(Entity.GetEntity(NukeFuncs.nukeInfo.MyGetField("player").As<int>())) && NukeFuncs.nukeEmpTimeRemaining > 0)
                            player.SetEMPJammed(true);
                    }

                    if (player.MyGetField("hasNuke").As<int>() > 0)
                    {
                        NukeFuncs.GiveNuke(player, false);
                    }
                });

                player.MySetField("killstreak", 0);
                if (NukeFuncs.level.MyHasField("nukeDetonated"))
                    player.VisionSetNakedForPlayer("aftermath", 0);
            }
        }

        private void OnPlayerWeaponChange(int entRef, Parameter[] myParams)
        {
            if (entRef < 0 || entRef > 17)
                return;

            Entity player = Entity.GetEntity(entRef);
            string weapon = myParams[0].As<string>();

            if (NukeFuncs.MayDropWeapon(weapon))
                player.MySetField("lastDroppableWeapon", weapon);

            if (weapon == "killstreak_emp_mp")
            {
                player.SwitchToWeapon(player.MyGetField("lastDroppableWeapon").As<string>());

                if (player.MyGetField("hasFauxNuke").As<int>() > 0 || player.MyGetField("hasNuke").As<int>() > 0)
                {
                    if (!NukeFuncs.TryUseNuke(player))
                    {
                        Utilities.PrintToConsole($"Nuke could not be Called in for {player.Name}!");
                    }

                    if (player.MyGetField("hasFauxNuke").As<int>() > 0)
                    {
                        player.MySetField("hasFauxNuke", player.MyGetField("hasFauxNuke").As<int>() - 1);
                    }

                    if (player.MyGetField("hasNuke").As<int>() < 1)
                    {
                        AfterDelay(1000, () => player.TakeWeapon("killstreak_emp_mp"));
                        player.SetPlayerData("killstreaksState", "icons", 0, "");
                        player.SetPlayerData("killstreaksState", "hasStreak", 0, false);

                    }

                }
            }

            if (player.MyGetField("hasFauxNuke").As<int>() > 0 && player.MyGetField("hasNuke").As<int>() > 0)
            {
                player.GiveWeapon("killstreak_emp_mp", 0, false);
                player.SetActionSlot(4, "weapon", "killstreak_emp_mp");
            }


        }

        private void CancelNukeOnDeath(Entity player)
        {
            OnInterval(50, () =>
            {
                if (player.IsAlive && player.IsPlayer)
                    return nukeIncoming;

                SetDvar("ui_bomb_timer", 0);
                nukeIncoming = false;
                Notify("nuke_cancelled");
                return false;
            });
        }

        private void DelayThreadNuke(int delay, Action function)
        {
            AfterDelay(delay, function);
        }

        private void DestroyDestructables()
        {
            if (explosivesDestroyed)
                return;

            Entity currentLevel = !nukeInfo.MyHasField("player") ? null : Entity.GetEntity(nukeInfo.MyGetField("player").As<int>());

            for (int i = 18; i < 2047; i++)
            {
                Entity levelEntity = Entity.GetEntity(i);
                if (levelEntity == null)
                    continue;

                string field = levelEntity.TargetName;
                string field2 = levelEntity.Model;

                switch (field)
                {
                    default:
                        if (field2 != "vehicle_hummer_destructible")
                            continue;
                        break;
                    case "destructable":
                    case "destructible":
                    case "explodable_barrel":
                        break;
                }

                if (currentLevel == null)
                    currentLevel = levelEntity;

                levelEntity.Notify("damage", 99999, currentLevel, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), "MOD_EXPLOSIVE", "", "", "", 0, "frag_grenade_mp");
            }
            explosivesDestroyed = true;
        }

        private void DoNuke(Entity player, bool allowCancel, bool instant)
        {
            nukeInfo.MySetField("player", player.EntRef);
            nukeInfo.MySetField("team", player.SessionTeam);
            nukeIncoming = true;
            SetDvar("ui_bomb_timer", 4);

            if (isTeamBased)
                TeamPlayerCardSplash("used_nuke", player);

            else
                player.IPrintLnBold("Friendly M.O.A.B. inbound!");

            if (instant)
            {
                NukeSoundExplosion();
                NukeSloMo();
                NukeEffects();
                DelayThreadNuke(250, NukeVision);
                DelayThreadNuke(1500, NukeDeath);

                if (destroyExplosives && !explosivesDestroyed)
                    DelayThreadNuke(1600, DestroyDestructables);

                NukeAftermathEffect();
                return;
            }

            DelayThreadNuke(nukeTimer * 1000 - 3300, NukeSoundIncoming);
            DelayThreadNuke(nukeTimer * 1000, NukeSoundExplosion);
            DelayThreadNuke(nukeTimer * 1000, NukeSloMo);
            DelayThreadNuke(nukeTimer * 1000, NukeEffects);
            DelayThreadNuke(nukeTimer * 1000 + 250, NukeVision);
            DelayThreadNuke(nukeTimer * 1000 + 1500, NukeDeath);

            if (destroyExplosives && !explosivesDestroyed)
                DelayThreadNuke(nukeTimer * 1000 + 1600, DestroyDestructables);

            NukeAftermathEffect();
            UpdateUITimers();

            if (cancelMode != 0 && allowCancel)
                CancelNukeOnDeath(player);

            int nukeTimerLoc = nukeTimer;

            OnInterval(1000, () =>
            {
                if (nukeTimerLoc > 0)
                {
                    PlaySoundAtPos(Vector3.Zero, "ui_mp_nukebomb_timer");
                    nukeTimerLoc--;
                    return true;
                }
                return false;
            });
        }

        private int GetKillstreakIndex(string streakName) => TableLookupRowNum("mp/killstreakTable.csv", 1, streakName) - 1;

        private string GetTeamPrefix(Entity player)
        {
            string alliesChar = GetMapCustom("allieschar");
            string axisChar = GetMapCustom("axischar");

            if (isTeamBased && player.SessionTeam == "allies")
                return TableLookup("mp/factiontable.csv", 0, alliesChar, 7);


            if (!isTeamBased || player.SessionTeam != "axis")
                return "US_";

            return TableLookup("mp/factiontable.csv", 0, axisChar, 7);
        }

        public void GiveNuke(Entity player, bool persistant = true)
        {
            player.SetPlayerData("killstreaksState", "icons", 0, GetKillstreakIndex("nuke"));
            player.GiveWeapon("killstreak_emp_mp", 0, false);
            player.SetActionSlot(4, "weapon", "killstreak_emp_mp");
            player.SetPlayerData("killstreaksState", "hasStreak", 0, true);

            int field = player.MyGetField("killstreak").As<int>();

            if (killsToNuke == 0)
                player.ShowHudSplash("nuke", 0, field);

            else
                player.ShowHudSplash("nuke", 0, killsToNuke);

            player.PlayLocalSound(GetTeamPrefix(player) + "1mc_achieve_moab");

            if (!persistant)
                player.MySetField("hasFauxNuke", 1);

            else
            {
                player.MySetField("hasNuke", player.MyGetField("hasNuke").As<int>() + 1);
                player.MySetField("hasFauxNuke", 0);
            }
        }

        private void KeepNukeEMPTimeRemaining()
        {
            Notify("keepNukeEMPTimeRemaining");
            nukeEmpTimeRemaining = nukeEmpTimeout;

            OnInterval(1000, () =>
            {
                nukeEmpTimeRemaining--;
                return nukeEmpTimeRemaining > 0;
            });
        }

        private bool MayDropWeapon(string weapon)
        {
            if (weapon == "none")
                return false;

            if (weapon.Contains("ac130"))
                return false;

            if (weapon.Contains("killstreak"))
                return false;

            return WeaponInventoryType(weapon) == "primary";
        }
        private void NukeEMPJam()
        {
            if (!isTeamBased)
                Notify("EMP_JamPlayers");

            else
            {
                Notify("EMP_JamTeamaxis");
                Notify("EMP_JamTeamallies");
            }

            Notify("nuke_EMPJam");

            if (isTeamBased)
                level.MySetField("teamNukeEMPed_" + (nukeInfo.MyGetField("team").As<string>() != "allies" ? "axis" : "axis"), 1);

            else
            {
                string team1 = nukeInfo.MyGetField("team").As<string>() != "allies" ? "axis" : "axis";
                level.MySetField("teamNukeEMPed_" + nukeInfo.MyGetField("team").As<string>(), 1);
                level.MySetField("teamNukeEMPed_" + team1, 1);
            }

            Notify("nuke_emp_update");
            KeepNukeEMPTimeRemaining();

            AfterDelay(nukeEmpTimeout * 1000, () =>
            {
                if (isTeamBased)
                    level.MySetField("teamNukeEMPed_" + (nukeInfo.MyGetField("team").As<string>() != "allies" ? "axis" : "axis"), 0);

                else
                {
                    string team2 = nukeInfo.SessionTeam != "allies" ? "axis" : "axis";
                    level.MySetField("teamNukeEMPed_" + nukeInfo.MyGetField("team").As<string>(), 0);
                    level.MySetField("teamNukeEMPed_" + team2, 0);
                }

                foreach (Entity player in Players)
                {
                    if (!isTeamBased || player.SessionTeam != nukeInfo.MyGetField("team").As<string>())
                    {
                        player.MySetField("nuked", 0);
                        player.SetEMPJammed(false);
                    }
                }

                Notify("nuke_emp_ended");
            });
        }

        public void NukeEMPTeamTracker()
        {
            foreach (Entity player in Players)
            {
                if (!player.IsPlayer || player.SessionTeam == "spectator")
                    continue;

                if (isTeamBased)
                    if (nukeInfo.MyHasField("team") && player.SessionTeam == nukeInfo.MyGetField("team").As<string>())
                        continue;

                    else if (nukeInfo.MyHasField("player") && player == Entity.GetEntity(nukeInfo.MyGetField("player").As<int>()))
                        continue;

                player.SetEMPJammed(level.MyGetField("teamNukeEMPed_" + player.SessionTeam).As<int>() != 0);
            }
        }

        private void NukeAftermathEffect()
        {
            Entity entity = GetEnt("mp_global_intermission", "classname");
            Vector3 angles = entity.Angles;
            Vector3 anglesUp = AnglesToUp(angles);
            Vector3 anglesRight = AnglesToRight(angles);
            PlayFX(effects[2], entity.Origin, anglesUp, anglesRight);
        }

        private void NukeDeath()
        {
            Notify("nuke_death");
            AmbientStop(1);

            foreach (Entity player in Players)
            {
                if (!player.IsPlayer)
                    continue;

                if (isTeamBased)
                {
                    if (nukeInfo.MyHasField("team") && player.SessionTeam == nukeInfo.MyGetField("team").As<string>())
                        continue;
                }

                else
                {
                    if (nukeInfo.MyHasField("player") && player == Entity.GetEntity(nukeInfo.MyGetField("player").As<int>()))
                        continue;
                }

                player.MySetField("nuked", 1);
                Entity owner = Entity.GetEntity(nukeInfo.MyGetField("player").As<int>());
                if (player.IsAlive)
                    player.FinishPlayerDamage(owner, owner, 999999, 0, "MOD_EXPLOSIVE", "nuke_mp", player.Origin, player.Origin, "none", 0);
            }

            NukeEMPJam();
            nukeIncoming = false;
        }

        private void NukeEffect(Entity nukeEnt, Entity player) => AfterDelay(50, () => PlayFXOnTagForClients(effects[1], nukeEnt, "tag_origin", player));

        private void NukeEffects()
        {
            SetDvar("ui_bomb_timer", 0);
            level.MySetField("nukeDetonated", 1);

            foreach (Entity player in Players)
            {
                if (player.IsPlayer)
                {
                    Vector3 angles = player.Angles;
                    Vector3 toFoward = AnglesToForward(angles);
                    toFoward = new Vector3(toFoward.X, toFoward.Y, 0f);
                    toFoward.Normalize();
                    Entity entity = Spawn("script_model", player.Origin + (toFoward * 5000f));
                    entity.SetModel("tag_origin");
                    entity.Angles = new Vector3(0f, angles.Y + 180f, 90f);
                    NukeEffect(entity, player);
                }
            }
        }

        private void NukeSloMo() => SetSlowMotion(1f, 0.25f, 0.5f);

        private void NukeSoundExplosion()
        {
            foreach (Entity player in Players)
            {
                if (player.IsPlayer)
                {
                    player.PlayLocalSound("nuke_explosion");
                    player.PlayLocalSound("nuke_wave");
                }
            }
        }

        private void NukeSoundIncoming()
        {
            foreach (Entity player in Players)
                if (player.IsPlayer)
                    player.PlayLocalSound("nuke_incoming");
        }

        private void NukeVision()
        {
            level.MySetField("nukeVisionInProgress", 1);
            VisionSetNaked("mpnuke", 3);
        }

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            AfterDelay(100, () =>
            {
                if (attacker.IsPlayer && attacker != player && (nukeChainsKills || weapons.IsWeaponNukeScriptRelated(weapon)))
                {
                    if (isTeamBased && player.SessionTeam != attacker.SessionTeam)
                        attacker.MySetField("killstreak", attacker.MyGetField("killstreak").As<int>() + 1);

                    else if (!isTeamBased)
                        attacker.MySetField("killstreak", attacker.MyGetField("killstreak").As<int>() + 1);

                    bool hasHardline = attacker.HasPerk("specialty_hardline");

                    if (hasHardline && (attacker.MyGetField("killstreak").As<int>() == killsToNuke - 1 && killsToNuke > 1))
                        GiveNuke(attacker, true);
                    else if (hasHardline && attacker.MyGetField("killstreak").As<int>() == killsToNuke && killsToNuke == 1)
                        GiveNuke(attacker, true);
                    else if (!hasHardline && attacker.MyGetField("killstreak").As<int>() == killsToNuke)
                        GiveNuke(attacker, true);
                }
            });
        }

        private void TeamPlayerCardSplash(string splash, Entity owner)
        {
            foreach (Entity player in Players)
            {
                if (player.IsPlayer)
                {
                    player.SetCardDisplaySlot(owner, 5);
                    player.ShowHudSplash(splash, 1);

                    if (isTeamBased && player.SessionTeam == owner.SessionTeam)
                        player.PlayLocalSound(GetTeamPrefix(player) + "1mc_use_moab");

                    else if (isTeamBased && player.SessionTeam != owner.SessionTeam)
                        player.PlayLocalSound(GetTeamPrefix(player) + "1mc_enemy_moab");
                }
            }
        }

        public bool TryUseNuke(Entity player, bool allowCancel = false)
        {
            if (!player.IsPlayer)
            {
                InfinityScript.Log.Write(LogLevel.Error, "Nuke attempted to call in from a non-player entity!");
                return false;
            }

            if (nukeIncoming)
            {
                player.IPrintLnBold("M.O.A.B. already inbound!");
                return false;
            }

            if (nukeEmpTimeRemaining > 0 && level.MyGetField("teamNukeEMPed_" + player.SessionTeam).As<int>() == 1 && isTeamBased)
            {
                player.IPrintLnBold($"M.O.A.B. fallout still active for {nukeEmpTimeRemaining} seconds.");
                return false;
            }

            if (!isTeamBased && nukeEmpTimeRemaining > 0 && nukeInfo.MyHasField("player") && Entity.GetEntity(nukeInfo.MyGetField("player").As<int>()) != player)
            {
                player.IPrintLnBold($"M.O.A.B. fallout still active for {nukeEmpTimeRemaining} seconds.");
                return false;
            }

            DoNuke(player, allowCancel, false);

            player.MySetField("hasNuke", player.MyGetField("hasNuke").As<int>() - 1);

            if (player.MyGetField("hasNuke").As<int>() < 0)
                player.MySetField("hasNuke", 0);


            player.Notify("used_nuke");
            return true;
        }

        public bool TryUseNukeImmediate(Entity player)
        {
            if (!player.IsPlayer)
            {
                InfinityScript.Log.Write(LogLevel.Error, "Nuke attempted to call in from a non-player entity!");
                return false;
            }

            DoNuke(player, false, true);

            player.MySetField("hasNuke", player.MyGetField("hasNuke").As<int>() - 1);

            if (player.MyGetField("hasNuke").As<int>() < 0)
                player.MySetField("hasNuke", 0);

            player.Notify("used_nuke");
            return true;
        }

        private void UpdateUITimers() => SetDvar("ui_nuke_end_milliseconds", nukeTimer * 1000 + GetTime());
    }
}