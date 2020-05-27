using System.Collections.Generic;
using System.Threading;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class HelpCommand : ChatCommand
    {
        private readonly Dictionary<string, string> helpTexts = new Dictionary<string, string>();

        private readonly string[] moderatorCommandNames =
        {
            "/appear",
            "/ban",
            "/banip",
            "/checkip",
            "/clear",
            "/kick",
            "/killroom",
            "/save",
            "/tempban",
            "/vanish"
        };

        private readonly string[] ownerCommandNames =
        {
            "/bgcolor",
            "/clear",
            "/cleareffects,/ce",
            "/forcefly",
            "/forgive",
            "/givecrown",
            "/giveedit,/gedit,/ge",
            "/givegod",
            "/hidelobby",
            "/kick",
            "/kill",
            "/killall",
            "/listportals",
            "/loadlevel",
            "/name",
            "/removecrown",
            "/removeedit,/redit,/re",
            "/removegod",
            "/reset",
            "/resetall",
            "/resetswitches",
            "/respawnall",
            "/setteam",
            "/teleport",
            "/visible"
        };

        private readonly string[] playerCommandNames =
        {
            "/clearchat",
            "/getpos",
            "/inspect",
            "/mute",
            "/pm",
            "/reportabuse,/report,/rep",
            "/roomid",
            "/spectate,/spec",
            "/unmute"
        };

        public HelpCommand(EverybodyEdits game)
            : base(game, CommandAccess.Public, "help")
        {
            this.PopulateHelpTexts();
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            if (commandInput.Length == 2)
            {
                this.SendSystemMessage(player, commandInput[1] + "\n" + this.GetHelpText(commandInput[1], player));
            }
            else
            {
                this.SendSystemMessage(player, this.GetHelpText(commandInput[0], player));
            }
        }

        private string GetHelpText(string commandString, Player player)
        {
            var helpText = "";
            try
            {
                if (commandString == "help")
                {
                    helpText = "\nType '/help' followed by command name.\nCommand names:\n";

                    helpText += string.Join("\n", playerCommandNames);

                    if (player.IsModerator || player.IsAdmin)
                    {
                        helpText += "\n\nModerator command names:\n" + string.Join("\n", moderatorCommandNames);
                    }

                    if (player.Owner)
                    {
                        helpText += "\n\nOwner command names:\n" + string.Join("\n", ownerCommandNames);
                    }

                    return helpText;
                }
                helpText = this.helpTexts[commandString];
            }
            catch (KeyNotFoundException)
            {
                helpText += "Is not a valid command";
            }
            return helpText;
        }

        private void PopulateHelpTexts()
        {
            this.helpTexts["/bgcolor"] =
                "Changes the world's background color. [/bgcolor #hex#] (Use none as #hex# to set default background)";
            this.helpTexts["/clear"] = "Clears the world.";
            this.helpTexts["/clearchat"] = "Clears the chat. Affects only you.";
            this.helpTexts["/cleareffects"] =
                this.helpTexts["/ce"] = "Clears all effects of the specified player. [/cleareffects #username#]";
            this.helpTexts["/forcefly"] = "Turns godmode on or off for the player. [/forcefly #username# true/false]";
            this.helpTexts["/getpos"] = "Returns the position (in tiles) of the player. [/getpos #username#]";
            this.helpTexts["/givecrown"] = "Gives the crown to the player. [/givecrown #username#]";
            this.helpTexts["/giveedit"] =
                this.helpTexts["/gedit"] =
                    this.helpTexts["/ge"] =
                        "Gives edit rights to the player. World owners always have editing rights and these cannot be changed. [/giveedit #username#]";
            this.helpTexts["/giveeffect"] =
                this.helpTexts["/geffect"] = "Gives the specified effect to the specified user. [/giveeffect #name# #effect id/name# #argument#]";
            this.helpTexts["/givegod"] =
                "Gives the player the ability to enable/disable godmode by pressing G, without edit rights. [/givegod #username#]";
            this.helpTexts["/hidelobby"] =
                "Hides this world in the lobby and profile, but allow other playes to join. [/hidelobby true/false]";
            this.helpTexts["/inspect"] =
                "When enabled, hover over a block to see who placed it. Placement history is wiped after a world is empty.";
            this.helpTexts["/kick"] = "Kicks the player from the world. [/kick #username# #reason#]";
            this.helpTexts["/kill"] = "Kills the player. [/kill #username#]";
            this.helpTexts["/killall"] = "Kills all players in the world.";
            this.helpTexts["/listportals"] =
                "Returns a list of all visible and invisible portals in the world on the form: 'id,target,x,y,type'.";
            this.helpTexts["/loadlevel"] = "Reloads the world and respawns all players at their spawn point.";
            this.helpTexts["/mute"] =
                "Prevents the player from sending you messages in the chat. Others will still be able to see messages from the specified user. Note: /mute is local to a game session. [/mute #username#]";
            this.helpTexts["/name"] = "Sets the world name [/name #newname#]";
            this.helpTexts["/pm"] = "Sends a private message to another player. [/pm #username# #message#]";
            this.helpTexts["/r"] = "Reply to the last PM you received. [/r #message#]";
            this.helpTexts["/removecrown"] = "Takes the crown from the player that currently has it.";
            this.helpTexts["/removeedit"] =
                this.helpTexts["/redit"] =
                    this.helpTexts["/re"] =
                        "Removes editing right from a user. World owners always have editing rights and these cannot be changed. [/removeedit #username#]";
            this.helpTexts["/removeeffect"] =
                this.helpTexts["/reffect"] = "Takes the specified effect from the specified user. [/removeeffect #name# #effect id/name#]";
            this.helpTexts["/removegod"] =
                "Removes the player's ability to enable/disable godmode by pressing G, without edit rights. [/removegod #username#]";
            this.helpTexts["/report"] =
                this.helpTexts["/reportabuse"] =
                    this.helpTexts["/rep"] =
                        "Report a player for breaking the rules. Warning: abusing this command can result in a ban for you. [/report #playername# #reason#]";
            this.helpTexts["/reset"] = "Resets yourself or specified player. [/reset #playername#]";
            this.helpTexts["/resetall"] = "Resets all FilteredPlayers.";
            this.helpTexts["/resetswitches"] = "Resets all global switches to an off-state.";
            this.helpTexts["/respawnall"] = "Respawns all players at their last touched checkpoint.";
            this.helpTexts["/roomid"] = "Returns ID of the room.";
            this.helpTexts["/setteam"] =
                "Sets the team of specified player. (Must buy team blocks to use) [/setteam #username# #team#]";
            this.helpTexts["/spectate"] =
                this.helpTexts["/spec"] =
                    "Spectate another player by moving the camera to their smiley. [/spec #username#]";
            this.helpTexts["/teleport"] =
                this.helpTexts["/tp"] =
                    "Teleports playername to the (x,y) world coordinate (specified in tiles). If x and y are omitted, the player is teleported to the position of the command issuer. [/teleport #x# #y#]";
            this.helpTexts["/unmute"] = "Removes the player from the muted list. [/unmute #username#]";
            this.helpTexts["/visible"] = "Hides your world from the lobby and your profile. [/visible true/false]";
        }
    }
}