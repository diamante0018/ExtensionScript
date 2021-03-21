# ExtensionScript
Admin manager-script made for Tekno MW3. All done via Rcon.
Put Extension.js in your IW4M Plugins folder to make use of this script. Or else, you can use the command ```!rcon set sv_b3Execute !commandname args``` to execute the commands.
You must use this version of IS: [InfinityScript](https://github.com/diamante0018/InfinityScript)
It is also shipped with every release of this script.
Sorry if this README isn't that helpful to get you started, it is recommended that you read the source code to understand what is going on.
I don't have a fancy pdf with all the features. I don't have time for it.
Most commands work now, so the js plugin should contain all of them.
Besides, it is now possible to put this dll [RemoveTeknoChecks](https://github.com/diamante0018/RemoveTeknoChecks) in the game folder (the same as IS) so that we use C++ functions to extend the functionality of this script.
A few things to know:
In the server.cfg set these dvars with the values you wish, or they will be set to their default value described below
- sv_balanceInterval 15
- sv_autoBalance 1 //Balances players using killstreak data to avoid ruining killstreaks.
- sv_Bounce 1
- sv_NopAddresses 0 //Disable if using wine to run your server
- sv_KnifeEnabled 0 //If set to 0 the knife will not work
- sv_hideCommands 1 //Hides messages that start with ! or @
- sv_gmotd ^:Welcome to ^4DIA ^:servers. https://discord.com/invite/
- sv_forceSmoke 1 //Forces rendering of smoke on clients, bypassable but not for the average user
- sv_objText ^7Join our Discord Server now! ^1https://discord.com/invite/
- sv_clientDvars 1
- sv_killStreakCounter 1
- sv_scrollingHud 1
- sv_scrollingSpeed 30
- sv_UndoRCE 0 //If set to 1 it removes SteamAuth RCE one line 'patch' (restores one address to it's original states allowing buffer overflow)
- sv_LocalizedStr 1 //If set to 0 it will disable localized strings placed in the player card such as @MENU_FACEBOOK_LEGAL
- sv_AntiCamp 1 //If set to 1 it will hurt campers that stay in the same place and don't move
- sv_AntiHardScope 0 //If set to 1 it hill prevent hard scoping and drop shots
- sv_LastStand 0 //If set to 0 players on last stand will be pranked
- sv_RemoveBakaaraSentry 0 //If set to 1 it will delete all entities that have class name misc_turret
- sv_MyMapName WeirdMap
- sv_MyGameMode WeirdGameMode
- sv_allowedClan1 AG //Optional
- sv_allowedClan2 AU //Optional
- sv_playerChatAlias 1 //If set to 1 it will enable aliases for the chat
- sv_serverCulture "en-GB" //Definies local culture
- sv_NerfGuns 1 //If set to 1 it will nerfs damage done by killstreaks
- sv_ExplosivePrank 1 //If set to 1 it will delete some explosives
- sv_DisableAkimbo 1 //If set to 1 it will disable akimbo guns by replacing them with a USP45

Native C++ functions:
- NopTheFuckOut Called if sv_NopAddresses is 1. It uses hooks to get player data needed for other C++ functions
- PrintErrorToConsole Shuts down the server (until it's manually restarted) and displays a reason to all the users
- DvarFindDvar Tries to look for a string dvar if not found, it will return <undefined>
- SendGameCommand I only have a few examples on how to use this, but basically, you can force the client to do anything
- CrashAll Crashes all clients at once, the game is closed but the server stays open
- DvarRegisterString Registers a string dvar. Similar to SetDvar. Max length is somewhere between 100-150 characters
- NET_Print Arguments are netadr_t struct, duplicate packets, entity reference and the message itself. I suspect duplicate packets are necessary because we are using the UDP protocol
- Q_rsqrt This function calculates the reverse square root of a given float number
- DvarModifyMaxClients Modifies the dvar sv_maxclients to whatever value is specified within 0-512. It may cause crashes
- GetValueForKey Uses the server built-in string tokenizer to get values from a string that uses a backslash as the separator between key-value
- GetConnectedString Gets the cleaned connected string generated when a player last joined
- RemoveConnectedString Removes the cleaned connected string

General Trivia:
- com_printDebug bool dvar is not used
- Rekt RCE patch is a one-line NOP that doesn't address the real issue (easily undone)
- NetSendPacket and QueryInfo NOP undoing doesn't do anything
- sv_kickBanTime is a float dvar (even though it stores time expressed in seconds), and its DvarValues are modified by TeknoMW3S.dll
- sv_kickBanTime new DvarValues are: current 3600f * 3f, max 60f * 60f * 24f * 7f, default 3600f * 3f, min 0.0f

Special thanks to [S3VDITO](https://github.com/S3VDITO)