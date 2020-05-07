using System.Collections.Generic;

namespace EverybodyEdits.Game.ChatCommands
{
    public class HelpCommand : ChatCommand
    {
        protected Dictionary<string, string> helpTexts = new Dictionary<string, string>();

        public HelpCommand(EverybodyEdits game)
            : base(game)
        {
            this.populateHelpTexts();
        }

        public override void resolve(Player player, string[] commandInput)
        {
            var _player = player;
            if (commandInput.Length == 2)
            {
                _player.Send("write", this._game.SystemName, commandInput[1] + "\n" + this.GetHelpText(commandInput[1]));
            }
            else
            {
                _player.Send("write", this._game.SystemName, this.GetHelpText(commandInput[0]));
            }
        }

        public string GetHelpText(string commandString)
        {
            var helpText = "";
            try
            {
                helpText = this.helpTexts[commandString];
            }
            catch (KeyNotFoundException)
            {
                helpText = "Is not a valid command";
            }
            return helpText;
        }

        protected void populateHelpTexts()
        {
            string[] commandNames =
            {
                "/loadlevel",
                "/reset",
                "/respawnall",
                "/giveedit",
                "/removeedit",
                "/kick",
                "/kill",
                "/killemall",
                "/teleport",
                "/potionson",
                "/potionsoff",
                "/visible",
                "/getpos",
                "/listportals",
                "/reportabuse",
                "/kickguests",
                "/mute",
                "/unmute",
                "/pm"
            };

            this.helpTexts["/help"] = "\nType '/help' followed by command name.\nCommand names:\n" +
                                      string.Join("\n", commandNames);

            this.helpTexts["/loadlevel"] = "Reloads the world and respawns all players at their spawn point.";
            this.helpTexts["/reset"] = "Resets all players.";
            this.helpTexts["/respawnall"] = "Respawns all players at their last touched checkpoint.";
            this.helpTexts["/giveedit"] =
                "Gives edit rights to playername. Use this command followed by a space and the user name you wish to give edit right to. World owners always have editing rights and these cannot be changed.";
            this.helpTexts["/removeedit"] =
                "Removes editing right from a user. Use this command followed by a space and the user name you wish to give edit right to. World owners always have editing rights and these cannot be changed.";
            this.helpTexts["/kick"] =
                "Kicks a player from a world. Use this command followed by a space then the username of the player you wish to kick out. If you wish to specify a reason you enter it after another space character.";
            this.helpTexts["/kill"] =
                "Kills the specified player and respawns the player at a world spawn point or the last activated checkpoint.";
            this.helpTexts["/killemall"] =
                "Kills all players in a world and respawns them at a world spawn point or the last activated checkpoint.";
            this.helpTexts["/teleport"] =
                "Teleports playername to the (x,y) world coordinate (specified in tiles). If x and y are omitted, the player is teleported to the position of the command issuer.";
            this.helpTexts["/potionson"] =
                "Set a list of potions to enabled or disabled. The id's are listed in the potion description. Note, that the world setting 'Potions on/off' will overrule these specific potion settings. In order for this to have effect, potions must be 'on'.";
            this.helpTexts["/potionsoff"] =
                "Set a list of potions to enabled or disabled. The id's are listed in the potion description. Note, that the world setting 'Potions on/off' will overrule these specific potion settings. In order for this to have effect, potions must be 'on'.";
            this.helpTexts["/visible"] =
                "Hides or shows a world in the lobby. type '/visible' followed by either true or false to toggle visibility of a world.";
            this.helpTexts["/getpos"] =
                "Returns the position (in tiles) of the 'playername' parameter. Type this command followed by a space and the playername of the player you want the current position of.";
            this.helpTexts["/listportals"] =
                "Returns a list of all visible and invisible portals in the world on the form: 'id,target,x,y,type'.";
            this.helpTexts["/repostabuse"] =
                "Notifies moderators about abusive behaviour. The command requires a playername and a reason, in the form: /reportabuse #playername# #reason#";
            this.helpTexts["/kickguests"] = "Kicks all guests.";
            this.helpTexts["/mute"] =
                "Prevents the player with the specified username from sending you messages in the chat. Others will still be able to see messages from the specified user. Note: /mute is local to a game session.";
            this.helpTexts["/unmute"] = "Removes the player with the specified username from the muted list.";
            this.helpTexts["/pm"] = "Sends a private message to another player.";
        }
    }
}