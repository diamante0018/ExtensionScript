# ExtensionScript
Admin manager script for Tekno MW3. All done via Rcon.
Put Extension.js in your IW4M Plugins folder to make use of this script. Or else, you can use the command ```!rcon set sv_b3Execute !commandname args``` to execute the commands.
You must use this version of IS: [InfinityScript](https://github.com/diamante0018/InfinityScript)
It is also shipped with every release of this script.
Sorry if this README isn't that helpful to get you started, it is recommended that you read the source code to understand what is going on.
I don't have a fancy pdf with all the feautures. I don't have time for it.
Most commands work now so the js plugin should contain all of them.
In addition it is now possible to put this dll [RemoveTeknoChecks](https://github.com/diamante0018/RemoveTeknoChecks) in the game folder (the same as IS) so that we can nop a few checks here and there to make the server more 'fun' like using weird clantags and titles.
A few things to know:
In the server.cfg set these dvars with the values you wish or they will be set to their default value described below
- sv_balanceInterval 15
- sv_autoBalance 1
- sv_Bounce 1
- sv_NopAddresses 0 //Disable if using Wine to run your server
- sv_KnifeEnabled
- sv_hideCommands 1 //Hides messages that start with ! or @
- sv_gmotd ^:Welcome to ^4DIA ^:servers. https://discord.com/invite/
- sv_forceSmoke 1 //Forces rendering of smoke on clients, bypassable but not for the average user
- sv_objText ^7Join our Discord Server now! ^1https://discord.com/invite/
- sv_clientDvars 1
- sv_killStreakCounter 1
- sv_scrollingHud 1
- sv_scrollingSpeed 30
- sv_UndoRCE 1 //Removes SteamAuth RCE one line 'patch' (restores one address to it's original states allowing buffer overflow)