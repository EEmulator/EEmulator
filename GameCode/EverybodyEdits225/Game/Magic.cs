using System;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game
{
    public class Magic
    {
        private readonly EverybodyEdits game;
        private bool enabled;

        public Magic(EverybodyEdits game)
        {
            this.game = game;
            this.game.PlayerIO.BigDB.LoadOrCreate("Config", "magic",
                magicSettings => { this.enabled = magicSettings.GetBool("enabled", true); });
        }

        private double Multiplier
        {
            get
            {
                var multiplier = this.game.Random.NextDouble();
                if (multiplier < .65)
                {
                    multiplier = .80;
                }
                return multiplier;
            }
        }

        public void OnCoin(Player player, bool isBlue, int goldCoins, int blueCoins)
        {
            // Magic is disabled :(
            if (!this.enabled)
            {
                return;
            }

            // Guests can't get magic
            if (player.ConnectUserId == "simpleguest")
            {
                return;
            }

            // Bots can't get magic
            if (player.IsBot ?? true)
            {
                return;
            }

            // Can't get magic twice
            if (!player.CanWinEnergy)
            {
                return;
            }

            // There can't be more than 400 coins
            if (goldCoins + blueCoins > 400)
            {
                return;
            }

            // Don't give magic when player didn't move for more than 15 seconds.
            if ((DateTime.UtcNow - player.LastMove).TotalSeconds > 15)
            {
                return;
            }

            var minSinceLastMagic = (int) (DateTime.UtcNow - player.LastMagic).TotalMinutes;
            // Need to wait at least 1 hour after getting magic
            if (minSinceLastMagic < 60)
            {
                return;
            }

            // There should be at least 15 seconds difference between coins
            if (this.CalculateDifference(player).TotalSeconds > 15)
            {
                return;
            }

            var chance = this.CalculateChance(minSinceLastMagic, goldCoins, blueCoins, player.MaxEnergy);
            var multiplier = this.Multiplier;

            if (this.IsLucky(chance, multiplier))
            {
                this.GivePlayerMagic(player, isBlue);
            }
            else
            {
                // Bonus chance to get smileys or blocks
                chance = this.CalculateChance(minSinceLastMagic, goldCoins, blueCoins);
                if (this.IsLucky(chance, multiplier))
                {
                    this.GivePlayerMagic(player, isBlue, true);
                }
            }
        }

        private void GivePlayerMagic(Player player, bool isBlue, bool isBonus = false)
        {
            var received = false;
            var rand = this.game.Random.Next(0, 100);

            if (rand < 10 || (isBlue && rand < 55))
            {
                received = isBlue
                    ? this.GiveBlock(player)
                    : this.GiveSmiley(player);
            }

            if (!received && !isBonus)
            {
                this.GiveEnergy(player);
                received = true;
            }

            if (received)
            {
                player.SendMessage("magic");
            }
        }

        private void GiveEnergy(Player player)
        {
            var rand = this.game.Random.Next(0, 100);

            if (rand <= 1)
            {
                this.SendInfo(player,
                    "You found a magical coin! Upon touching the coin you were awarded 30 total Energy and a full Energy bar!",
                    player.Name.ToUpper() + " just found a magical coin and was awarded 30 total energy and a full energy bar.");

                this.AwardMaxEnergy(player, 30, true);
            }
            else if (rand <= 7)
            {
                this.SendInfo(player,
                    "You found a magical coin! Upon touching the coin you were awarded 15 total Energy and a full Energy bar!",
                    player.Name.ToUpper() + " just found a magical coin and was awarded 15 total energy and a full energy bar.");

                this.AwardMaxEnergy(player, 15, true);
            }
            else if (rand <= 17)
            {
                this.SendInfo(player,
                    "You found a magical coin! Upon touching the coin you found 10 total Energy and a full Energy bar!",
                    player.Name.ToUpper() + " just found a magical coin and was awarded 10 total energy and a full energy bar.");

                this.AwardMaxEnergy(player, 10, true);
            }
            else if (rand <= 35)
            {
                this.SendInfo(player,
                    "You found a magical coin! Upon touching the coin you found 5 total Energy!",
                    player.Name.ToUpper() + " just found a magical coin and was awarded 5 total energy.");

                this.AwardMaxEnergy(player, 5, false);
            }
            else if (rand <= 58)
            {
                this.SendInfo(player,
                    "You found a magical coin! Upon touching the coin you found 2 total Energy!",
                    player.Name.ToUpper() + " just found a magical coin and was awarded 2 total energy.");

                this.AwardMaxEnergy(player, 2, false);
            }
            else
            {
                this.SendInfo(player,
                    "You found a magical coin! Upon touching the coin you found 1 total Energy!",
                    player.Name.ToUpper() + " just found a magical coin and was awarded 1 total energy.");

                this.AwardMaxEnergy(player, 1, false);
            }
        }

        private bool GiveBlock(Player player)
        {
            for (var i = 1; i <= 6; i++)
            {
                var id = i == 1 ? "" : i.ToString();
                var brickId = "brickmagic" + id;
                if (player.PayVault.Has(brickId))
                {
                    continue;
                }

                player.PayVault.Give(new[] {new BuyItemInfo(brickId)}, () =>
                {
                    this.SendInfo(player,
                        "You found an extra magical coin! Upon touching the coin you found that you got yourself a neat magic brick...",
                        player.Name.ToUpper() + " just got a magic brick!");
                });

                this.DisableRewarding(player);
                player.SendMessage("givemagicbrickpackage", "magic");
                return true;
            }

            return false;
        }

        private bool GiveSmiley(Player player)
        {
            if (!player.PayVault.Has("smileywizard"))
            {
                this.AwardSmiley(player, 22, "smileywizard", () =>
                {
                    this.SendInfo(player,
                        "You found an extra magical coin! Upon touching the coin you found that you had a wizard hat and beard...",
                        player.Name.ToUpper() + " just became a wizard!");
                });
                return true;
            }
            if (!player.PayVault.Has("smileywizard2"))
            {
                this.AwardSmiley(player, 32, "smileywizard2", () =>
                {
                    this.SendInfo(player,
                        "You found an extra magical coin! Upon touching the coin you found that you had a red wizard hat and beard...",
                        player.Name.ToUpper() + " just became a fire wizard!");
                });
                return true;
            }
            if (!player.PayVault.Has("smileywitch"))
            {
                this.AwardSmiley(player, 41, "smileywitch", () =>
                {
                    this.SendInfo(player,
                        "You found an extra magical coin! Upon touching the coin you found that you became a witch...",
                        player.Name.ToUpper() + " just became a witch!");
                });
                return true;
            }
            if (!player.PayVault.Has("smileydarkwizard"))
            {
                this.AwardSmiley(player, 94, "smileydarkwizard", () =>
                {
                    this.SendInfo(player,
                        "You found an extra dark magical coin! Upon touching the coin you found that you became a dark wizard...",
                        player.Name.ToUpper() + " just became a dark wizard!");
                });
                return true;
            }
            if (!player.PayVault.Has("smileylightwizard"))
            {
                this.AwardSmiley(player, 122, "smileylightwizard", () =>
                {
                    this.SendInfo(player,
                        "You found an extra light magical coin! Upon touching the coin you found that you had a white wizard hat and a beard...",
                        player.Name.ToUpper() + " just became a light wizard!");
                });
                return true;
            }

            return false;
        }

        private void AwardMaxEnergy(Player player, int energy, bool refill)
        {
            this.DisableRewarding(player);

            player.RefreshPlayerObject(() =>
            {
                player.MaxEnergy += energy;
                if (refill)
                {
                    player.RefillEnergy();
                }
                player.PlayerObject.Save();
            });
        }

        private void AwardSmiley(Player player, int smileyId, string smileyPayVaultId, Callback callback)
        {
            this.DisableRewarding(player);

            player.PayVault.Give(new[] {new BuyItemInfo(smileyPayVaultId)}, callback);
            this.game.SetPlayerFace(player, smileyId);
            player.SendMessage("givemagicsmiley", smileyPayVaultId);
        }

        private void DisableRewarding(Player player)
        {
            player.LastMagic = DateTime.UtcNow;
            player.CanWinEnergy = false;
        }

        private void SendInfo(Player player, string text, string broadcastText)
        {
            player.SendMessage("info2", "Congratulations!", text);
            this.game.BroadcastMessage("write", "* MAGIC", broadcastText);
        }

        private bool IsLucky(int chance, double multiplier)
        {
            return this.game.Random.Next(0, (int) (chance*multiplier)) <= 5;
        }

        private int CalculateChance(int minSinceLastMagic, int goldCoins, int blueCoins, int maxEnergy = 0)
        {
            var chance = 1000;

            // The more energy you have the lower is your chance
            chance += maxEnergy;

            // If there are at least 15 gold or blue coins chance gets much lower
            if (goldCoins >= 15 || blueCoins >= 15)
            {
                chance += goldCoins*5;
                chance += blueCoins*5;
            }

            // Time since last magic also has impact on chance
            if (minSinceLastMagic < 1000)
            {
                chance += (1000 - minSinceLastMagic)*2;
            }
            if (minSinceLastMagic < 500)
            {
                chance += (500 - minSinceLastMagic)*3;
            }

            return chance;
        }

        private TimeSpan CalculateDifference(Player player)
        {
            if (player.CoinTimer < DateTime.UtcNow)
            {
                player.CoinTimer = DateTime.UtcNow;
            }

            player.CoinTimer += TimeSpan.FromSeconds(10);

            var difference = player.CoinTimer - DateTime.UtcNow;
            if (difference.TotalMinutes < 1)
            {
                player.CoinTimer += difference;
            }
            else
            {
                player.CoinTimer -= TimeSpan.FromSeconds(difference.TotalSeconds/20);
            }

            return difference;
        }
    }
}