using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class GenCouponCommand : ChatCommand
    {
        public GenCouponCommand(EverybodyEdits game)
            : base(game, CommandAccess.Admin, "gencoupon")
        {
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            var allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var pass = "";

            int amount;
            if (!int.TryParse(commandInput[1], out amount))
            {
                amount = 10;
            }

            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    pass += allowedChars[this.Game.Random.Next(0, allowedChars.Length)];
                }
                pass += i < 3 ? "-" : "";
            }

            var dbo = new DatabaseObject();
            dbo.Set("Gems", amount);

            this.Game.PlayerIO.BigDB.CreateObject("Coupons", pass, dbo,
                value => this.SendSystemMessage(player, "Coupon added! Code: " + pass));
        }
    }
}