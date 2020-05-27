using System;
using System.Linq;
using EverybodyEdits.Common;
using EverybodyEdits.Lobby;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.Campaigns
{
    public class Campaign
    {
        private readonly Client client;

        public Campaign(Client client)
        {
            this.client = client;
        }

        public bool Visible
        {
            get { return this.Data != null && this.Data.Visible; }
        }

        public CampaignData Data { get; private set; }

        public void Load(string campaignId, Callback callback = null)
        {
            if (campaignId == "")
            {
                if (callback != null)
                {
                    callback.Invoke();
                }
                return;
            }

            this.client.BigDB.Load("Campaigns", campaignId, dbo =>
            {
                this.Data = new CampaignData(dbo);
                if (callback != null)
                {
                    callback.Invoke();
                }
            });
        }

        public CampaignWorld GetWorld(string worldId)
        {
            return this.Data.Levels.FirstOrDefault(it => it.Id == worldId);
        }

        public void SendJoinMessage(Player player, string worldId, Action<Message> broadcast, Callback callback)
        {
            var rtn = Message.Create("joinCampaign");
            rtn.Add(this.Data.Title);

            var campaignWorld = this.GetWorld(worldId);
            if (player.IsGuest || campaignWorld == null)
            {
                callback();
                rtn.Add(-1);
                player.SendMessage(rtn);
                return;
            }

            this.client.BigDB.Load("Config", "campaigns", campaignSettings =>
            {
                CampaignPlayer.Load(this.client, player.ConnectUserId, this.Data.Id,
                    delegate (CampaignPlayer campaignPlayer)
                    {
                        // First goes "init" message
                        callback();

                        player.InCampaign = true;
                        var status = campaignPlayer.GetStatus(campaignWorld.Tier);
                        if (status == CampaignStatus.Locked || !campaignSettings.GetBool("enabled", true))
                        {
                            rtn.Add(-1);
                            player.SendMessage(rtn);
                        }
                        else if (this.Data.BetaOnly && !player.HasBeta)
                        {
                            rtn.Add(2);
                            player.SendMessage(rtn);
                        }
                        else
                        {
                            if (status == CampaignStatus.Unlocked)
                            {
                                player.IsInCampaignMode = true;

                                var backup = new CampaignProgressBackup(campaignPlayer.DatabaseObject, player);
                                backup.Restore(worldId, broadcast);
                            }
                            else if (status == CampaignStatus.Completed && campaignPlayer.CurrentTier == this.Data.Tiers)
                            {
                                // Award reward in case of earlier error
                                this.AwardBadge(player, badge => { });
                            }

                            rtn.Add((int) status,
                                campaignWorld.Difficulty,
                                campaignWorld.Tier,
                                this.Data.Tiers);

                            player.SendMessage(rtn);
                        }
                    });
            });
        }

        public void CompleteWorld(Player player, string worldId, Shop shop)
        {
            var campWorld = this.GetWorld(worldId);
            if (campWorld != null &&
                player.IsInCampaignMode &&
                !(player.IsBot ?? true))
            {
                CampaignPlayer.Load(this.client, player.ConnectUserId, this.Data.Id, delegate(CampaignPlayer campPlayer)
                {
                    if (campPlayer.GetStatus(campWorld.Tier) == CampaignStatus.Unlocked)
                    {
                        player.RefreshPlayerObject(() =>
                        {
                            campPlayer.CurrentTier += 1;
                            campPlayer.LastUpdated = DateTime.UtcNow;
                            campPlayer.Save(() =>
                            {
                                this.AwardWorldRewards(player, campWorld, shop);
                                player.PlayerObject.Save();

                                var rewardsMessage = Message.Create("campaignRewards");

                                if (campPlayer.CurrentTier == this.Data.Tiers)
                                {
                                    this.AwardBadge(player, achievment =>
                                    {
                                        rewardsMessage.Add(true);
                                        rewardsMessage.Add(achievment.Title);
                                        rewardsMessage.Add(achievment.Description);
                                        rewardsMessage.Add(achievment.ImageUrl);
                                        this.AddRewardsToMessage(rewardsMessage, campWorld);
                                        player.SendMessage(rewardsMessage);
                                    });
                                }
                                else
                                {
                                    rewardsMessage.Add(false);
                                    var nextWorld = this.Data.Levels.First(world => world.Tier == campPlayer.CurrentTier);
                                    rewardsMessage.Add(nextWorld.Image);
                                    this.AddRewardsToMessage(rewardsMessage, campWorld);
                                    player.SendMessage(rewardsMessage);
                                }
                            });
                        });
                    }
                    else
                    {
                        player.SendMessage("completedLevel");
                    }
                });
            }
            else
            {
                player.SendMessage("completedLevel");
            }
        }

        private void AddRewardsToMessage(Message message, CampaignWorld world)
        {
            foreach (var reward in world.Rewards)
            {
                message.Add(reward.Key);
                message.Add(reward.Value);
            }
        }

        private void AwardWorldRewards(CommonPlayer player, CampaignWorld world, Shop shop)
        {
            foreach (var reward in world.Rewards)
            {
                switch (reward.Key)
                {
                    case "maxEnergy":
                        player.MaxEnergy += (int) reward.Value;
                        Console.WriteLine("Rewarded {0} with +{1} maximum energy.", player.Name, reward.Value);
                        break;
                    case "energy":
                        player.AddEnergy((int) reward.Value);
                        Console.WriteLine("Rewarded {0} with +{1} energy.", player.Name, reward.Value);
                        break;
                    case "energyRefill":
                        player.RefillEnergy();
                        Console.WriteLine("Rewarded {0} with energy refill.", player.Name);
                        break;

                    case "gems":
                        var amount = reward.Value;
                        player.PayVault.Credit(amount, "Completing " + this.Data.Title + " campaign",
                            delegate { Console.WriteLine("Rewarded {0} with {1} gems.", player.Name, amount); });
                        break;

                    default:
                        var item = shop.GetShopItem(reward.Key);

                        if (item != null && item.Key == reward.Key && (!player.PayVault.Has(item.Key) || item.Reusable))
                        {
                            var count = item.Reusable ? reward.Value : 1;
                            var items = new BuyItemInfo[count];
                            for (var i = 0; i < items.Length; i++)
                            {
                                items[i] = new BuyItemInfo(item.Key);
                            }

                            player.PayVault.Give(items,
                                delegate
                                {
                                    Console.WriteLine("Rewarded {0} with {1} x{2}.", player.Name, item.Key, count);
                                });
                        }
                        break;
                }
            }
        }

        private void AwardBadge(CommonPlayer player, Callback<Achievement> callback)
        {
            player.Achievements.Refresh(() =>
            {
                // Don't award if it's already completed
                var a = player.Achievements.Get(this.Data.Badge);
                if (a.Completed)
                {
                    callback(a);
                    return;
                }

                player.Achievements.ProgressComplete(this.Data.Badge, delegate(Achievement a2)
                {
                    callback(a2);
                    Console.WriteLine("Rewarded {0} with {1} badge.", player.Name, this.Data.Badge);
                }, error =>
                {
                    this.client.ErrorLog.WriteError("Error giving " + this.Data.Badge + " badge to " + player.Name,
                        error);
                    callback(a);
                });
            });
        }
    }
}