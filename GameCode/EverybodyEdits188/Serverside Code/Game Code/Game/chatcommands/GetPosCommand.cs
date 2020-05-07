using System;

namespace EverybodyEdits.Game.ChatCommands
{
    public class GetPosCommand : ChatCommand
    {
        public GetPosCommand(EverybodyEdits game)
            : base(game)
        {
        }

        public override void resolve(Player player, string[] commandInput)
        {
            var _player = player;

            var yCoord = _player.y;
            var xCoord = _player.x;
            var playerName = _player.name;
            if (commandInput.Length == 2)
                // if we have a playername, we display that players coordinates otherwise we return the owners
            {
                foreach (var p in this._game.Players)
                {
                    if (p.name.ToLower() == commandInput[1].ToLower())
                    {
                        if (!p.canbemod || !p.isgod) // If the player isn't moderator or god
                        {
                            xCoord = p.x;
                            yCoord = p.y;
                            playerName = p.name;
                        }
                        break;
                    }
                }
            }

            _player.Send("write", this._game.SystemName,
                "pos," + playerName + "," + Math.Round((xCoord + Config.TILE_WIDTH) / Config.TILE_WIDTH) + "," +
                Math.Round((yCoord + Config.TILE_HEIGHT) / Config.TILE_HEIGHT));
        }
    }
}