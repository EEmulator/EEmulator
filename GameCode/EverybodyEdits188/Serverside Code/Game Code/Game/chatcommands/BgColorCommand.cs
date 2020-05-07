using System;
using System.Globalization;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.ChatCommands
{
    public class BgColorCommand : ChatCommand
    {
        public BgColorCommand(EverybodyEdits game)
            : base(game)
        {
        }

        public override void resolve(Player player, string[] commandInput)
        {
            if (commandInput.Length < 2 || commandInput[1].ToLower() == "none" || commandInput[1].ToLower() == "remove")
            {
                var noColor = (uint)((0 << 24) | (0 << 16) | (0 << 8) | (0 << 0));
                this._game.BaseWorld.backgroundColor = noColor;
                this._game.BroadcastMessage(Message.Create("backgroundColor", noColor));
            }
            else
            {
                var colorString = commandInput[1];

                if (!colorString.Contains("#"))
                {
                    player.Send("write", this._game.SystemName, "Value not formatted correct. Missing #");
                    return;
                }
                if (colorString.Length < 7)
                {
                    player.Send("write", this._game.SystemName, "Value not formatted correct. Too short.");
                    return;
                }

                try
                {
                    //#RRGGBB
                    var a = 255;
                    var r = int.Parse(colorString.Substring(1, 2), NumberStyles.AllowHexSpecifier);
                    var g = int.Parse(colorString.Substring(3, 2), NumberStyles.AllowHexSpecifier);
                    var b = int.Parse(colorString.Substring(5, 2), NumberStyles.AllowHexSpecifier);
                    var bgColor = (uint)((a << 24) | (r << 16) | (g << 8) | (b << 0));

                    this._game.BaseWorld.backgroundColor = bgColor;
                    this._game.BroadcastMessage(Message.Create("backgroundColor", bgColor));
                    Console.WriteLine("Set bg to " + bgColor);
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Error " + exception.Message);
                    player.Send("write", this._game.SystemName, "Value was not a color. " + exception.Message);
                }
            }
        }
    }
}