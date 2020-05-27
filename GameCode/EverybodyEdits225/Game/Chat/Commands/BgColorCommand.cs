using System.Globalization;
using System;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class BgColorCommand : ChatCommand
    {
        public BgColorCommand(EverybodyEdits game)
            : base(game, CommandAccess.CrewMember, "bgcolor", "bgcolour")
        {
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            var color = (uint) ((0 << 24) | (0 << 16) | (0 << 8) | (0 << 0));

            if (commandInput.Length >= 2 && commandInput[1].ToLower() != "none" && commandInput[1].ToLower() != "remove")
            {
                var colorString = commandInput[1];

                if (!colorString.Contains("#"))
                {
                    this.SendSystemMessage(player, "Value not formatted correctly. Missing #");
                    return;
                }
                if (colorString.Length < 7)
                {
                    this.SendSystemMessage(player, "Value not formatted correctly. Too short.");
                    return;
                }

                try
                {
                    //#RRGGBB
                    var r = int.Parse(colorString.Substring(1, 2), NumberStyles.AllowHexSpecifier);
                    var g = int.Parse(colorString.Substring(3, 2), NumberStyles.AllowHexSpecifier);
                    var b = int.Parse(colorString.Substring(5, 2), NumberStyles.AllowHexSpecifier);
                    color = (uint) ((255 << 24) | (r << 16) | (g << 8) | (b << 0));
                }
                catch (Exception exception)
                {
                    this.SendSystemMessage(player, "Value was not a color. " + exception.Message);
                    return;
                }
            }

            this.Game.BaseWorld.BackgroundColor = color;
            this.Game.BroadcastMessage("backgroundColor", color);
        }
    }
}