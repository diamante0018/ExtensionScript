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
sv_balanceInterval 15
sv_autoBalance 1
sv_Bounce 1
