var permission_error = "Your rank is lower than "

let commands = [{

        name: "afk",

        description: "Puts the player to spectator mode",

        alias: "spec",

        permission: "Administrator",

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !afk ' + cid).Result;
        }
    },
    {

        name: "aimassist",

        description: "Gives the player aim assist",

        alias: "aimassist",

        permission: "Administrator",

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !gscaimassist ' + cid).Result;
        }
    },
    {

        name: "time",

        description: "Tells the time",

        alias: "time",

        permission: "User",

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !time ' + cid).Result;
        }
    },
    {

        name: "removesentry",

        description: "Removes Bakaara sentry",

        alias: "removesentry",

        permission: "Administrator",

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !removesentry').Result;
        }
    },
    {

        name: "thirdperson",

        description: "Enables third person",

        alias: "thirdperson",

        permission: "Trusted",

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !thirdperson ' + cid).Result;
        }
    },
    {

        name: "disabletrail",

        description: "Disables special effects trail",

        alias: "disabletrail",

        permission: "Trusted",

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !disabletrail ' + cid).Result;
        }
    },
    {

        name: "enabletrail",

        description: "Enables special effects trail",

        alias: "enabletrail",

        permission: "Trusted",

        arguments: [{
            name: "Trail Number",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            var message = gameEvent.Data;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !enabletrail ' + cid + ' ' + message).Result;
        }
    },
    {

        name: "setalias",

        description: "Sets an alias for the client",

        alias: "setalias",

        permission: "Trusted",

        targetRequired: true,

        arguments: [{
            name: "Alias",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var guid = gameEvent.Target.GuidString;
            var message = gameEvent.Data;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !setalias ' + guid + ' ' + message).Result;
        }
    },
    {

        name: "resetalias",

        description: "Removes alias",

        alias: "resetalias",

        permission: "Trusted",

        targetRequired: true,

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var guid = gameEvent.Target.GuidString;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !resetalias ' + guid).Result;
        }
    },
    {

        name: "kickall",

        description: "Kicks all players with a custom error message and shuts down the server",

        alias: "kickall",

        permission: "Owner",

        targetRequired: false,

        arguments: [{
            name: "Message",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var message = gameEvent.Data;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !kickallplayers ' + message).Result;
        }
    },
    {

        name: "moab",

        description: "Gives a pseudo-GSC Moab to the player",

        alias: "moab",

        permission: "Trusted",

        targetRequired: false,

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !moab ' + cid).Result;
        }
    },
    {

        name: "crashall",

        description: "Crashes all players",

        alias: "crashall",

        permission: "Owner",

        targetRequired: false,

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !anothercrash').Result;
        }
    },
    {

        name: "hideplayer",

        description: "Makes the player invisible",

        alias: "hideplayer",

        permission: "Administrator",

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !hide ' + cid).Result;
        }
    },
    {

        name: "balance",

        description: "Balances teams",

        alias: "balance",

        permission: "Administrator",

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !balance').Result;
        }
    },
    {

        name: "fly",

        description: "Makes the player fly",

        alias: "fly",

        permission: "Administrator",

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !fly ' + cid).Result;
        }
    },
    {

        name: "setafk",

        description: "Puts the target to spectator mode",

        alias: "setafk",

        permission: "Administrator",

        targetRequired: true,

        arguments: [{
            name: "Target Player",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Target.ClientNumber;
            if (gameEvent.Origin.Level > gameEvent.Target.Level)
                server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !setafk ' + cid).Result;
            else
                gameEvent.Origin.Tell(permission_error + gameEvent.Target.Name + " you can't use this command on them");
        }
    },
    {

        name: "randomkick",

        description: "Kicks the target using a random message",

        alias: "randomkick",

        permission: "Administrator",

        targetRequired: true,

        arguments: [{
            name: "Target Player",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Target.ClientNumber;
            if (gameEvent.Origin.Level > gameEvent.Target.Level)
                server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !randomkick ' + cid).Result;
            else
                gameEvent.Origin.Tell(permission_error + gameEvent.Target.Name + " you can't use this command on them");
        }
    },
    {

        name: "closetheserver",

        description: "Prevents new connecting players from joining",

        alias: "closetheserver",

        permission: "Owner",

        targetRequired: false,

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !maketheserverclose').Result;
        }
    },
    {

        name: "quickmaths",

        description: "Calculates Sin, Cos, and Tan of the angle using in-game GSC Functions",

        alias: "quickmaths",

        permission: "User",

        targetRequired: false,

        arguments: [{
            name: "Angle",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            var message = gameEvent.Data;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !quickmaths ' + cid + ' ' + message).Result;
        }
    },
    {

        name: "random",

        description: "Gives the player a random number between 0 and the specified max value using GSC functions",

        alias: "random",

        permission: "User",

        targetRequired: false,

        arguments: [{
            name: "Max value",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            var message = gameEvent.Data;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !randomnum ' + cid + ' ' + message).Result;
        }
    },
    {

        name: "rsqrt",

        description: "Fast reverse square root using native C++ functions",

        alias: "rsqrt",

        permission: "User",

        targetRequired: false,

        arguments: [{
            name: "Number",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            var message = gameEvent.Data;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !rsqrt ' + cid + ' ' + message).Result;
        }
    },
    {

        name: "randomgun",

        description: "Gives the player a random gun",

        alias: "randomgun",

        permission: "Administrator",

        targetRequired: false,

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !randomgun ' + cid).Result;
        }
    },
    {

        name: "spam",

        description: "Spams the player with random strings",

        alias: "spam",

        permission: "Administrator",

        targetRequired: true,

        arguments: [{
            name: "Target Player",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Target.ClientNumber;
            if (gameEvent.Origin.Level > gameEvent.Target.Level)
                server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !spam ' + cid).Result;
            else
                gameEvent.Origin.Tell(permission_error + gameEvent.Target.Name + " you can't use this command on them");
        }
    },
    {

        name: "kill",

        description: "Kills the target",

        alias: "killplayer",

        permission: "Administrator",

        targetRequired: true,

        arguments: [{
            name: "Target Player",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Target.ClientNumber;
            if (gameEvent.Origin.Level > gameEvent.Target.Level)
                server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !kill ' + cid).Result;
            else
                gameEvent.Origin.Tell(permission_error + gameEvent.Target.Name + " you can't use this command on them");
        }
    },
    {

        name: "suicide",

        description: "Kills the target",

        alias: "suicide",

        permission: "User",

        targetRequired: false,

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !suicide ' + cid).Result;
        }
    },
    {

        name: "godmode",

        description: "Gives God Mode",

        alias: "gd",

        permission: "Administrator",

        targetRequired: false,


        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !godmode ' + cid).Result;
        }
    },
    {

        name: "mode",

        description: "Changes DSR",

        alias: "mode",

        permission: "Administrator",

        targetRequired: false,

        arguments: [{
            name: "DSR Name",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var message = gameEvent.Data;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !mode ' + message).Result;
        }
    },
    {

        name: "randommap",

        description: "Random Map Selector",

        alias: "randommap",

        permission: "Administrator",

        targetRequired: false,

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !randommap').Result;
        }
    },
    {

        name: "ac130",

        description: "Gives AC130 to the player",

        alias: "ac130",

        permission: "Administrator",

        targetRequired: false,

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !ac130 ' + cid).Result;
        }
    },
    {

        name: "blockchat",

        description: "Mutes the player",

        alias: "bc",

        permission: "Administrator",

        targetRequired: true,

        arguments: [{
            name: "Target Player",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Target.ClientNumber;
            if (gameEvent.Origin.Level > gameEvent.Target.Level)
                server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !blockchat ' + cid).Result;
            else
                gameEvent.Origin.Tell(permission_error + gameEvent.Target.Name + " you can't use this command on them");
        }
    },
    {

        name: "freeze",

        description: "Blocks the player",

        alias: "freeze",

        permission: "Administrator",

        targetRequired: true,

        arguments: [{
            name: "Target Player",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Target.ClientNumber;
            if (gameEvent.Origin.Level > gameEvent.Target.Level)
                server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !freeze ' + cid).Result;
            else
                gameEvent.Origin.Tell(permission_error + gameEvent.Target.Name + " you can't use this command on them");
        }
    },
    {

        name: "changeteam",

        description: "Changes team of target",

        alias: "ct",

        permission: "Administrator",

        targetRequired: true,

        arguments: [{
            name: "Target Player",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Target.ClientNumber;
            if (gameEvent.Origin.Level > gameEvent.Target.Level)
                server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !changeteam ' + cid).Result;
            else
                gameEvent.Origin.Tell(permission_error + gameEvent.Target.Name + " you can't use this command on them");
        }
    },
    {

        name: "giveammo",

        description: "Gives ammo to the player",

        alias: "ga",

        permission: "Administrator",

        targetRequired: false,

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !giveammo ' + cid).Result;
        }
    },
    {

        name: "crash",

        description: "Crashes the player",

        alias: "crash",

        permission: "Administrator",

        targetRequired: true,

        arguments: [{
            name: "Target Player",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Target.ClientNumber;
            if (gameEvent.Origin.Level > gameEvent.Target.Level)
                server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !crash1 ' + cid).Result;
            else
                gameEvent.Origin.Tell(permission_error + gameEvent.Target.Name + " you can't use this command on them");
        }
    },
    {

        name: "ffcrash",

        description: "Crashes the player using wrong fast file map name",

        alias: "ffcrash",

        permission: "Administrator",

        targetRequired: true,

        arguments: [{
            name: "Target Player",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Target.ClientNumber;
            if (gameEvent.Origin.Level > gameEvent.Target.Level)
                server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !ffcrash ' + cid).Result;
            else
                gameEvent.Origin.Tell(permission_error + gameEvent.Target.Name + " you can't use this command on them");
        }
    },
    {

        name: "crash2",

        description: "Crashes the player with other method",

        alias: "crash2",

        permission: "Administrator",

        targetRequired: true,

        arguments: [{
            name: "Target Player",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Target.ClientNumber;
            if (gameEvent.Origin.Level > gameEvent.Target.Level)
                server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !crash2 ' + cid).Result;
            else
                gameEvent.Origin.Tell(permission_error + gameEvent.Target.Name + " you can't use this command on them");
        }
    },
    {

        name: "reset",

        description: "Resets the stats of the player",

        alias: "reset",

        permission: "Administrator",

        targetRequired: true,

        arguments: [{
            name: "Target Player",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Target.ClientNumber;
            if (gameEvent.Origin.Level > gameEvent.Target.Level)
                server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !resetstats ' + cid).Result;
            else
                gameEvent.Origin.Tell(permission_error + gameEvent.Target.Name + " you can't use this command on them");
        }
    },
    {

        name: "close",

        description: "Closes the game of the player",

        alias: "close",

        permission: "Administrator",

        targetRequired: true,

        arguments: [{
            name: "Target Player",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Target.ClientNumber;
            if (gameEvent.Origin.Level > gameEvent.Target.Level)
                server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !close ' + cid).Result;
            else
                gameEvent.Origin.Tell(permission_error + gameEvent.Target.Name + " you can't use this command on them");
        }
    },
    {

        name: "teknoban",

        description: "Corrupts stats of the player",

        alias: "teknoban",

        permission: "Owner",

        targetRequired: true,

        arguments: [{
            name: "Target Player",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Target.ClientNumber;
            if (gameEvent.Origin.Level > gameEvent.Target.Level)
                server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !teknoban ' + cid).Result;
            else
                gameEvent.Origin.Tell(permission_error + gameEvent.Target.Name + " you can't use this command on them");
        }
    },
    {

        name: "givegun",

        description: "Gives a gun to the player",

        alias: "gun",

        permission: "Trusted",

        targetRequired: false,

        arguments: [{
            name: "Name of the gun",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var message = gameEvent.Data;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !givegun ' + cid + ' ' + message).Result;
        }
    },
    {

        name: "servername",

        description: "Changes the servername",

        alias: "servername",

        permission: "Owner",

        targetRequired: false,

        arguments: [{
            name: "Name of the server",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var message = gameEvent.Data;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !servername ' + message).Result;
        }
    },
    {

        name: "clientdvar",

        description: "Changes a client dvar",

        alias: "cdvar",

        permission: "Trusted",

        targetRequired: false,

        arguments: [{
            name: "Name of the cdvar",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var message = gameEvent.Data;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !clientdvar ' + cid + ' ' + message).Result;
        }
    },
    {

        name: "finddvarstring",

        description: "Gets a string dvar current value",

        alias: "finddvarstring",

        permission: "Trusted",

        targetRequired: false,

        arguments: [{
            name: "Name of the dvar",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var message = gameEvent.Data;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !finddvarstring ' + message).Result;
        }
    },
    {

        name: "finddvarfloat",

        description: "Gets a float dvar current value",

        alias: "finddvarfloat",

        permission: "Trusted",

        targetRequired: false,

        arguments: [{
            name: "Name of the dvar",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var message = gameEvent.Data;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !finddvarfloat ' + message).Result;
        }
    },
    {

        name: "name",

        description: "Gives player a new name",

        alias: "name",

        permission: "Trusted",

        targetRequired: false,

        arguments: [{
            name: "New Name",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var message = gameEvent.Data;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !name ' + cid + ' ' + message).Result;
        }
    },
    {

        name: "clantag",

        description: "Gives to the player a new clan tag",

        alias: "clantag",

        permission: "Trusted",

        targetRequired: false,

        arguments: [{
            name: "New ClanTag",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var message = gameEvent.Data;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !clantag ' + cid + ' ' + message).Result;
        }
    },
    {

        name: "title",

        description: "Gives to the player a new title",

        alias: "title",

        permission: "Trusted",

        targetRequired: false,

        arguments: [{
            name: "New Title",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var message = gameEvent.Data;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !title ' + cid + ' ' + message).Result;
        }
    },
    {

        name: "wallhack",

        description: "Disables/Enables Wall Hack",

        alias: "wh",

        permission: "SeniorAdmin",

        targetRequired: false,

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !wh ' + cid).Result;
        }
    },
    {

        name: "yell",

        description: "Yells",

        alias: "yell",

        permission: "Trusted",

        targetRequired: false,

        arguments: [{
            name: "Message",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var message = gameEvent.Data;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !yell ' + message).Result;
        }
    },
    {

        name: "customconsole",

        description: "C++ command",

        alias: "customconsole",

        permission: "Owner",

        targetRequired: false,

        arguments: [{
            name: "Message",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var message = gameEvent.Data;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !customconsole ' + message).Result;
        }
    },
    {

        name: "tell",

        description: "Yells but in another way, very fancy",

        alias: "tell",

        permission: "Trusted",

        targetRequired: false,

        arguments: [{
            name: "Message",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var message = gameEvent.Data;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !tell ' + message).Result;
        }
    },
    {

        name: "aimbot",

        description: "Disables/Enables Aimbot",

        alias: "aim",

        permission: "SeniorAdmin",

        targetRequired: false,


        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !aimbot ' + cid).Result;
        }
    },
    {

        name: "chaos",

        description: "Disables/Enables chaos Aimbot",

        alias: "chaos",

        permission: "SeniorAdmin",

        targetRequired: false,


        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !chaos ' + cid).Result;
        }
    },
    {

        name: "falldamage",

        description: "Disables/Enables falldamage",

        alias: "falldamage",

        permission: "Trusted",

        targetRequired: false,

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !falldamage').Result;
        }
    },
    {

        name: "teleport",

        description: "Teleports player to target",

        alias: "teleport",

        permission: "Trusted",

        targetRequired: true,

        arguments: [{
            name: "Target Player",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            var target = gameEvent.Target.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !teleport ' + cid + ' ' + target).Result;
        }
    },
    {

        name: "tphere",

        description: "Teleports target to player",

        alias: "tphere",

        permission: "Trusted",

        targetRequired: true,

        arguments: [{
            name: "Target Player",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            var target = gameEvent.Target.ClientNumber;
            if (gameEvent.Origin.Level > gameEvent.Target.Level)
                server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !teleport ' + target + ' ' + cid).Result;
            else
                gameEvent.Origin.Tell(permission_error + gameEvent.Target.Name + " you can't use this command on them");
        }
    },
    {

        name: "save",

        description: "Saves current location",

        alias: "save",

        permission: "Trusted",

        targetRequired: false,

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            var message = gameEvent.Data;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !save ' + cid + ' ' + message).Result;
        }
    },
    {

        name: "load",

        description: "Loads current location",

        alias: "load",

        permission: "Trusted",

        targetRequired: false,

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            var message = gameEvent.Data;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !load ' + cid + ' ' + message).Result;
        }
    },
    {

        name: "colorclass",

        description: "Adds some color to the class loadout text",

        alias: "colorclass",

        permission: "Trusted",

        targetRequired: false,

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !colorclass ' + cid).Result;
        }
    },
    {

        name: "noclip",

        description: "Lets the player noclip",

        alias: "noclip",

        permission: "SeniorAdmin",

        targetRequired: false,

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !noclip ' + cid).Result;
        }
    },
    {

        name: "infiniteammo",

        description: "Gives infinite ammo to the player",

        alias: "iammo",

        permission: "SeniorAdmin",

        targetRequired: false,

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !infiniteammo ' + cid).Result;
        }
    },
    {

        name: "norecoil",

        description: "Gives norecoil to the player",

        alias: "norecoil",

        permission: "SeniorAdmin",

        targetRequired: false,

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !norecoil ' + cid).Result;
        }
    },
    {

        name: "knife",

        description: "Disables or enables Knife",

        alias: "knife",

        permission: "SeniorAdmin",

        targetRequired: false,

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !knife').Result;
        }
    },
    {

        name: "explode",

        description: "Blows up the target",

        alias: "blowup",

        permission: "SeniorAdmin",

        targetRequired: true,

        arguments: [{
            name: "Target Player",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Target.ClientNumber;
            if (gameEvent.Origin.Level > gameEvent.Target.Level)
                server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !explode ' + cid).Result;
            else
                gameEvent.Origin.Tell(permission_error + gameEvent.Target.Name + " you can't use this command on them");
        }
    },
    {

        name: "noweapon",

        description: "Removes weapons from the target",

        alias: "noweapon",

        permission: "SeniorAdmin",

        targetRequired: true,

        arguments: [{
            name: "Target Player",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Target.ClientNumber;
            if (gameEvent.Origin.Level > gameEvent.Target.Level)
                server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !noweapon ' + cid).Result;
            else
                gameEvent.Origin.Tell(permission_error + gameEvent.Target.Name + " you can't use this command on them");
        }
    },
    {

        name: "juggsuit",

        description: "Gives a juggsuit to the player",

        alias: "givejugg",

        permission: "SeniorAdmin",

        targetRequired: false,

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !juggsuit ' + cid).Result;
        }
    },
    {

        name: "speed",

        description: "Changes the speed",

        alias: "speed",

        permission: "SeniorAdmin",

        targetRequired: false,

        arguments: [{
            name: "Speed",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var message = gameEvent.Data;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !speed ' + message).Result;
        }
    },
    {

        name: "gravity",

        description: "Changes the gravity",

        alias: "gravity",

        permission: "SeniorAdmin",

        targetRequired: false,

        arguments: [{
            name: "Gravity",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var message = gameEvent.Data;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !gravity ' + message).Result;
        }
    },
    {

        name: "jumpheight",

        description: "Changes the jump height",

        alias: "jump",

        permission: "SeniorAdmin",

        targetRequired: false,

        arguments: [{
            name: "Jump height",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var message = gameEvent.Data;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !jumpheight ' + message).Result;
        }
    },
    {

        name: "sendgamecmd",

        description: "Sends some commands to the client to mess with them",

        alias: "sendgamecmd",

        permission: "Administrator",

        targetRequired: true,

        arguments: [{
            name: "Target Player",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Target.ClientNumber;
            if (gameEvent.Origin.Level > gameEvent.Target.Level)
                server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !sendgamecmd ' + cid).Result;
            else
                gameEvent.Origin.Tell(permission_error + gameEvent.Target.Name + " you can't use this command on them");
        }
    },
    {

        name: "registerstring",

        description: "Registers a string dvar using native C++ functions",

        alias: "registerstring",

        permission: "Owner",

        targetRequired: false,

        arguments: [{
            name: "Message",
            required: true
        }],

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var message = gameEvent.Data;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !registerstring ' + message).Result;
        }
    },
    {

        name: "givespecialgun",

        description: "Gives to the player special guns",

        alias: "givespecialgun",

        permission: "Trusted",

        targetRequired: false,

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            var cid = gameEvent.Origin.ClientNumber;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !givespecialgun ' + cid).Result;
        }
    },
    {

        name: "disconnectall",

        description: "Disconnects all clients gracefully",

        alias: "disconnectall",

        permission: "Owner",

        targetRequired: false,

        execute: (gameEvent) => {
            var server = gameEvent.Owner;
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !disconnectall').Result;
        }
    }
];

let plugin = {
    author: 'Diavolo',
    version: 1.1,
    name: 'Extension',

    onEventAsync: function(gameEvent, server) {},

    onLoadAsync: function(manager) {},

    onUnloadAsync: function() {},

    onTickAsync: function(server) {}
};