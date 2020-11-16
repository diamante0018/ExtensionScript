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
    }
},
{

    name: "kill",

    description: "Kills the target",

    alias: "kill",

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
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !crash ' + cid).Result;
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
            server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !reset ' + cid).Result;
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

    description: "Gives player a new clan tag",

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

    name: "wallhack",

    description: "Disables/Enables Wall Hack",

    alias: "wh",

    permission: "SeniorAdmin",

    targetRequired: false,

    arguments: [{
        name: "Target Player",
        required: true
    }],

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

    permission: "SeniorAdmin",

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


    execute: (gameEvent) => {


        var server = gameEvent.Owner;
        var cid = gameEvent.Origin.ClientNumber;
        var target = gameEvent.Target.ClientNumber;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !teleport ' + cid + ' ' + target).Result;
    }
}
];

let plugin = {
    author: 'Diavolo',
    version: 1.0,
    name: 'Extension',

    onEventAsync: function (gameEvent, server) {
    },

    onLoadAsync: function (manager) {
    },

    onUnloadAsync: function () {
    },

    onTickAsync: function (server) {
    }
};