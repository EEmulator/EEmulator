using System;
using System.Collections.Generic;

namespace EverybodyEdits.Game.ChatCommands
{
    public class PotionsOnOffCommand : ChatCommand
    {
        public PotionsOnOffCommand(EverybodyEdits game)
            : base(game)
        {
        }

        public override void resolve(Player player, string[] commandInput)
        {
            var _player = player;
            if (commandInput.Length > 1 && this._game.onChangePotionSettings(_player))
            {
                var enable = (commandInput[0] == "/potionson");

                // Make list of potionids to enable/disable
                var potionids = new List<string>();

                for (var i = 1; i < commandInput.Length; i++)
                {
                    try
                    {
                        var id = Convert.ToInt32(commandInput[i]);
                        if (ItemManager.GetPotion(id) != null)
                        {
                            potionids.Add(commandInput[i]);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("User send something else than a potion id");
                    }
                }
                this._game.BaseWorld.setPotionsEnabled(potionids, enable);
            }

            var potionsdisabled = this._game.BaseWorld.getPotionsEnabled(false);
            potionsdisabled.Sort();
            _player.Send("write", this._game.SystemName,
                "Potions not allowed: " +
                (potionsdisabled.Count == 0 ? "None." : string.Join(",", potionsdisabled.ToArray())));

            this._game.broadcastPotionSettings();

            if (this._game.BaseWorld.allowPotions)
            {
                foreach (var p in this._game.Players)
                {
                    this._game.broadcastActivePotions(p);
                }
            }
        }
    }
}