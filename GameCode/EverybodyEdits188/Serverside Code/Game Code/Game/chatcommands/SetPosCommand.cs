using System;

namespace EverybodyEdits.Game.ChatCommands
{
    public class SetPosCommand : ChatCommand
    {
        public SetPosCommand(EverybodyEdits game)
            : base(game)
        {
        }

        public override void resolve(Player player, string[] commandInput)
        {
            var _player = player;
            var yCoord = (int)_player.y;
            var xCoord = (int)_player.x;

            if (commandInput.Length == 4) // assume that x and y are at 2 and 3
            {
                try
                {
                    xCoord = (int.Parse(commandInput[2]) - 1) * Config.TILE_WIDTH; // coords are in tiles
                    yCoord = (int.Parse(commandInput[3]) - 1) * Config.TILE_HEIGHT; // coords are in tiles
                }
                catch (Exception)
                {
                }
            }

            if (xCoord < 1 || yCoord < 1 ||
                xCoord > (this._game.BaseWorld.width - 2) * Config.TILE_WIDTH ||
                yCoord > (this._game.BaseWorld.height - 2) * Config.TILE_HEIGHT)
            {
                _player.Send("info", "I can't do that", "You cannot teleport players outside of the world");
                return;
            }

            foreach (var p in this._game.Players)
            {
                if (p.name.ToLower() == commandInput[1].ToLower())
                {
                    if (!p.isgod && !p.ismod) /* If the player isn't moderator or god */
                    {
                        this._game.Broadcast("teleport", p.Id, xCoord, yCoord);
                        return;
                    }
                    return;
                }
            }
        }
    }
}