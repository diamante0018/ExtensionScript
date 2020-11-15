let commands = [{
    // required
    name: "afk",
    // required
    description: "Puts the player to spectator mode",
    // required
    alias: "spec",
    // required
    permission: "Administrator",
    // required
    execute: (gameEvent) => {
        //var onlinePlayers = gameEvent.Owner.GetClientsAsList();

        var server = gameEvent.Owner;
        var cid = gameEvent.Origin.ClientNumber;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !afk ' + cid).Result;
    }
},
{
    // required
    name: "setafk",
    // required
    description: "Puts the target to spectator mode",
    // required
    alias: "setafk",
    // required
    permission: "Administrator",

    targetRequired: true,

    arguments: [{
        name: "Target Player",
        required: true
    }],
    // required
    execute: (gameEvent) => {
        //var onlinePlayers = gameEvent.Owner.GetClientsAsList();

        var server = gameEvent.Owner;
        var cid = gameEvent.Origin.ClientNumber;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !setafk ' + cid).Result;
    }
},
{
    // required
    name: "kill",
    // required
    description: "Kills the target",
    // required
    alias: "kill",
    // required
    permission: "Administrator",

    targetRequired: true,

    arguments: [{
        name: "Target Player",
        required: true
    }],
    // required
    execute: (gameEvent) => {
        //var onlinePlayers = gameEvent.Owner.GetClientsAsList();

        var server = gameEvent.Owner;
        var cid = gameEvent.Origin.ClientNumber;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !kill ' + cid).Result;
    }
},
{
    // required
    name: "godmode",
    // required
    description: "Gives God Mode",
    // required
    alias: "gd",
    // required
    permission: "Administrator",

    targetRequired: false,

    // required
    execute: (gameEvent) => {
        //var onlinePlayers = gameEvent.Owner.GetClientsAsList();

        var server = gameEvent.Owner;
        var cid = gameEvent.Origin.ClientNumber;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !godmode ' + cid).Result;
    }
},
{
    // required
    name: "mode",
    // required
    description: "Changes DSR",
    // required
    alias: "mode",
    // required
    permission: "Administrator",

    targetRequired: false,
    arguments: [{
        name: "DSR Name",
        required: true
    }],
    // required
    execute: (gameEvent) => {
        //var onlinePlayers = gameEvent.Owner.GetClientsAsList();

        var server = gameEvent.Owner;
        var message = gameEvent.Data;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !mode ' + message).Result;
    }
},
{
    // required
    name: "ac130",
    // required
    description: "Gives AC130 to the player",
    // required
    alias: "ac130",
    // required
    permission: "Administrator",

    targetRequired: false,
    // required
    execute: (gameEvent) => {
        //var onlinePlayers = gameEvent.Owner.GetClientsAsList();

        var server = gameEvent.Owner;
        var cid = gameEvent.Origin.ClientNumber;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !ac130 ' + cid).Result;
    }
},
{
    // required
    name: "blockchat",
    // required
    description: "Mutes the player",
    // required
    alias: "bc",
    // required
    permission: "Administrator",

    targetRequired: true,

    arguments: [{
        name: "Target Player",
        required: true
    }],
    // required
    execute: (gameEvent) => {
        //var onlinePlayers = gameEvent.Owner.GetClientsAsList();

        var server = gameEvent.Owner;
        var cid = gameEvent.Origin.ClientNumber;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !blockchat ' + cid).Result;
    }
},
{
    // required
    name: "freeze",
    // required
    description: "Blocks the player",
    // required
    alias: "freeze",
    // required
    permission: "Administrator",

    targetRequired: true,

    arguments: [{
        name: "Target Player",
        required: true
    }],
    // required
    execute: (gameEvent) => {
        //var onlinePlayers = gameEvent.Owner.GetClientsAsList();

        var server = gameEvent.Owner;
        var cid = gameEvent.Origin.ClientNumber;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !freeze ' + cid).Result;
    }
},
{
    // required
    name: "changeteam",
    // required
    description: "Changes team of target",
    // required
    alias: "ct",
    // required
    permission: "Administrator",

    targetRequired: true,

    arguments: [{
        name: "Target Player",
        required: true
    }],
    // required
    execute: (gameEvent) => {
        //var onlinePlayers = gameEvent.Owner.GetClientsAsList();

        var server = gameEvent.Owner;
        var cid = gameEvent.Origin.ClientNumber;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !changeteam ' + cid).Result;
    }
},
{
    // required
    name: "giveammo",
    // required
    description: "Gives ammo to the player",
    // required
    alias: "ga",
    // required
    permission: "Administrator",

    targetRequired: false,
    
    // required
    execute: (gameEvent) => {
        //var onlinePlayers = gameEvent.Owner.GetClientsAsList();

        var server = gameEvent.Owner;
        var cid = gameEvent.Origin.ClientNumber;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !giveammo ' + cid).Result;
    }
},
{
    // required
    name: "crash",
    // required
    description: "Crashes the player",
    // required
    alias: "crash",
    // required
    permission: "Administrator",

    targetRequired: true,

    arguments: [{
        name: "Target Player",
        required: true
    }],
    // required
    execute: (gameEvent) => {
        //var onlinePlayers = gameEvent.Owner.GetClientsAsList();

        var server = gameEvent.Owner;
        var cid = gameEvent.Origin.ClientNumber;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !crash ' + cid).Result;
    }
},
{
    // required
    name: "reset",
    // required
    description: "Resets the stats of the player",
    // required
    alias: "reset",
    // required
    permission: "Administrator",

    targetRequired: true,

    arguments: [{
        name: "Target Player",
        required: true
    }],
    // required
    execute: (gameEvent) => {
        //var onlinePlayers = gameEvent.Owner.GetClientsAsList();

        var server = gameEvent.Owner;
        var cid = gameEvent.Origin.ClientNumber;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !reset ' + cid).Result;
    }
},
{
    // required
    name: "close",
    // required
    description: "Closes the game of the player",
    // required
    alias: "close",
    // required
    permission: "Administrator",

    targetRequired: true,

    arguments: [{
        name: "Target Player",
        required: true
    }],
    // required
    execute: (gameEvent) => {
        //var onlinePlayers = gameEvent.Owner.GetClientsAsList();

        var server = gameEvent.Owner;
        var cid = gameEvent.Origin.ClientNumber;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !close ' + cid).Result;
    }
},
{
    // required
    name: "teknoban",
    // required
    description: "Corrupts stats of the player",
    // required
    alias: "teknoban",
    // required
    permission: "Owner",

    targetRequired: true,

    arguments: [{
        name: "Target Player",
        required: true
    }],
    // required
    execute: (gameEvent) => {
        //var onlinePlayers = gameEvent.Owner.GetClientsAsList();

        var server = gameEvent.Owner;
        var cid = gameEvent.Origin.ClientNumber;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !teknoban ' + cid).Result;
    }
},
{
    // required
    name: "givegun",
    // required
    description: "Gives a gun to the player",
    // required
    alias: "gun",
    // required
    permission: "Trusted",

    targetRequired: false,

    arguments: [{
        name: "Name of the gun",
        required: true
    }],
    // required
    execute: (gameEvent) => {
        //var onlinePlayers = gameEvent.Owner.GetClientsAsList();

        var server = gameEvent.Owner;
        var message = gameEvent.Data;
        var cid = gameEvent.Origin.ClientNumber;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !givegun ' + cid + ' ' + message).Result;
    }
},
{
    // required
    name: "servername",
    // required
    description: "Changes the servername",
    // required
    alias: "servername",
    // required
    permission: "Owner",

    targetRequired: false,

    arguments: [{
        name: "Name of the server",
        required: true
    }],
    // required
    execute: (gameEvent) => {
        //var onlinePlayers = gameEvent.Owner.GetClientsAsList();

        var server = gameEvent.Owner;
        var message = gameEvent.Data;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !servername ' + message).Result;
    }
},
{
    // required
    name: "clientdvar",
    // required
    description: "Changes a client dvar",
    // required
    alias: "cdvar",
    // required
    permission: "Trusted",

    targetRequired: false,

    arguments: [{
        name: "Name of the cdvar",
        required: true
    }],
    // required
    execute: (gameEvent) => {
        //var onlinePlayers = gameEvent.Owner.GetClientsAsList();

        var server = gameEvent.Owner;
        var message = gameEvent.Data;
        var cid = gameEvent.Origin.ClientNumber;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !clientdvar ' + cid + ' ' + message).Result;
    }
},
{
    // required
    name: "name",
    // required
    description: "Gives player a new name",
    // required
    alias: "name",
    // required
    permission: "Trusted",

    targetRequired: false,

    arguments: [{
        name: "New Name",
        required: true
    }],
    // required
    execute: (gameEvent) => {
        //var onlinePlayers = gameEvent.Owner.GetClientsAsList();

        var server = gameEvent.Owner;
        var message = gameEvent.Data;
        var cid = gameEvent.Origin.ClientNumber;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !name ' + cid + ' ' + message).Result;
    }
},
{
    // required
    name: "clantag",
    // required
    description: "Gives player a new clan tag",
    // required
    alias: "clantag",
    // required
    permission: "Trusted",

    targetRequired: false,

    arguments: [{
        name: "New ClanTag",
        required: true
    }],
    // required
    execute: (gameEvent) => {
        //var onlinePlayers = gameEvent.Owner.GetClientsAsList();

        var server = gameEvent.Owner;
        var message = gameEvent.Data;
        var cid = gameEvent.Origin.ClientNumber;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !clantag ' + cid + ' ' + message).Result;
    }
},
{
    // required
    name: "wallhack",
    // required
    description: "Disables/Enables Wall Hack",
    // required
    alias: "wh",
    // required
    permission: "Owner",

    targetRequired: true,

    arguments: [{
        name: "Target Player",
        required: true
    }],
    // required
    execute: (gameEvent) => {
        //var onlinePlayers = gameEvent.Owner.GetClientsAsList();

        var server = gameEvent.Owner;
        var cid = gameEvent.Origin.ClientNumber;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !wh ' + cid).Result;
    }
},
{
    // required
    name: "yell",
    // required
    description: "Disables/Enables Wall Hack",
    // required
    alias: "yell",
    // required
    permission: "Trusted",

    targetRequired: false,

    arguments: [{
        name: "Message",
        required: true
    }],
    // required
    execute: (gameEvent) => {
        //var onlinePlayers = gameEvent.Owner.GetClientsAsList();

        var server = gameEvent.Owner;
        var message = gameEvent.Data;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !yell ' + message).Result;
    }
},
{
    // required
    name: "aimbot",
    // required
    description: "Disables/Enables Aimbot",
    // required
    alias: "aim",
    // required
    permission: "Owner",

    targetRequired: false,

    // required
    execute: (gameEvent) => {
        //var onlinePlayers = gameEvent.Owner.GetClientsAsList();

        var server = gameEvent.Owner;
        var cid = gameEvent.Origin.ClientNumber;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !aimbot ' + cid).Result;
    }
},
{
    // required
    name: "falldamage",
    // required
    description: "Disables/Enables falldamage",
    // required
    alias: "falldamage",
    // required
    permission: "Trusted",

    targetRequired: false,

    // required
    execute: (gameEvent) => {
        //var onlinePlayers = gameEvent.Owner.GetClientsAsList();

        var server = gameEvent.Owner;
        server.RconParser.ExecuteCommandAsync(server.RemoteConnection, 'set sv_b3Execute !falldamage').Result;
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