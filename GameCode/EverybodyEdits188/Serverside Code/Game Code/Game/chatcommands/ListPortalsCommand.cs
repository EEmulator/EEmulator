using System;

namespace EverybodyEdits.Game.ChatCommands
{
    internal class ListPortalsCommand : ChatCommand
    {
        public ListPortalsCommand(EverybodyEdits game)
            : base(game)
        {
        }

        public override void resolve(Player player, string[] commandInput)
        {
            var _player = player;

            var output = String.Empty;

            var portals = this._game.BaseWorld.getPortals();
            foreach (var item in portals)
            {
                var portal = (BrickPortal)this._game.BaseWorld.getBrick(0, item.x, item.y);
                output += portal.id + "," + portal.target + "," + item.x + "," + item.y + ",Portal\n";
            }

            var invisiblePortals = this._game.BaseWorld.getInvisiblePortals();
            foreach (var item in invisiblePortals)
            {
                var portal = (BrickPortal)this._game.BaseWorld.getBrick(0, item.x, item.y);
                output += portal.id + "," + portal.target + "," + item.x + "," + item.y + ",PortalInvisible\n";
            }

            if (output == String.Empty)
            {
                output = "No portals found";
            }

            _player.Send("write", this._game.SystemName, output);
        }
    }
}