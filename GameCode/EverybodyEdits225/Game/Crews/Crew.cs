using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using EverybodyEdits.Game.Mail;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.Crews
{
    public class Crew
    {
        private readonly Client client;
        public DatabaseObject DatabaseObject;

        public Crew(Client client)
        {
            this.client = client;
        }

        public string Id
        {
            get { return this.DatabaseObject != null ? this.DatabaseObject.Key : ""; }
        }

        public string Name
        {
            get { return this.DatabaseObject != null ? this.DatabaseObject.GetString("Name", "") : ""; }
        }

        public string Creator
        {
            get { return this.DatabaseObject != null ? this.DatabaseObject.GetString("Creator", "") : ""; }
            set
            {
                if (this.DatabaseObject != null)
                {
                    this.DatabaseObject.Set("Creator", value);
                }
            }
        }

        public uint Subscribers
        {
            get { return this.DatabaseObject != null ? this.DatabaseObject.GetUInt("Subscribers", 0) : 0; }
            set
            {
                if (this.DatabaseObject != null)
                {
                    this.DatabaseObject.Set("Subscribers", value);
                }
            }
        }

        public string LogoWorld
        {
            get { return this.DatabaseObject != null ? this.DatabaseObject.GetString("LogoWorld", "") : ""; }
            set
            {
                if (this.DatabaseObject != null)
                {
                    this.DatabaseObject.Set("LogoWorld", value);
                }
            }
        }

        public uint TextColor
        {
            private get
            {
                return this.DatabaseObject != null ? this.DatabaseObject.GetUInt("TextColor", 0xFFFFFF) : 0xFFFFFF;
            }
            set
            {
                if (this.DatabaseObject != null)
                {
                    this.DatabaseObject.Set("TextColor", value);
                }
            }
        }

        public uint PrimaryColor
        {
            private get
            {
                return this.DatabaseObject != null ? this.DatabaseObject.GetUInt("PrimaryColor", 0x333333) : 0x333333;
            }
            set
            {
                if (this.DatabaseObject != null)
                {
                    this.DatabaseObject.Set("PrimaryColor", value);
                }
            }
        }

        public uint SecondaryColor
        {
            private get
            {
                return this.DatabaseObject != null ? this.DatabaseObject.GetUInt("SecondaryColor", 0x111111) : 0x111111;
            }
            set
            {
                if (this.DatabaseObject != null)
                {
                    this.DatabaseObject.Set("SecondaryColor", value);
                }
            }
        }

        public int ProfileDividers
        {
            get { return this.DatabaseObject != null ? this.DatabaseObject.GetInt("ProfileDividers", 0) : 0; }
            set
            {
                if (this.DatabaseObject != null)
                {
                    this.DatabaseObject.Set("ProfileDividers", value);
                }
            }
        }

        public string Faceplate
        {
            private get { return this.DatabaseObject != null ? this.DatabaseObject.GetString("Faceplate", "") : ""; }
            set
            {
                if (this.DatabaseObject != null)
                {
                    this.DatabaseObject.Set("Faceplate", value);
                }
            }
        }

        public int FaceplateColor
        {
            private get { return this.DatabaseObject != null ? this.DatabaseObject.GetInt("FaceplateColor", 0) : 0; }
            set
            {
                if (this.DatabaseObject != null)
                {
                    this.DatabaseObject.Set("FaceplateColor", value);
                }
            }
        }

        public DatabaseObject Faceplates
        {
            get { return this.DatabaseObject != null ? this.DatabaseObject.GetObject("Faceplates") : null; }
        }

        public List<CrewRank> Ranks
        {
            get
            {
                var ranks = new List<CrewRank>();
                if (this.DatabaseObject != null)
                {
                    var ranksArray = this.DatabaseObject.GetArray("Ranks");
                    for (var i = 0; i < ranksArray.Count; i++)
                    {
                        ranks.Add(new CrewRank(i, this, ranksArray.GetObject(i)));
                    }
                }
                return ranks;
            }
        }

        public bool isContest {
            get { return this.DatabaseObject != null ? this.DatabaseObject.GetBool("Contest",false) : false; }
        }

        public void Load(string crew, Callback callback = null)
        {
            if (crew == "")
            {
                if (callback != null)
                {
                    callback.Invoke();
                }
                return;
            }

            this.client.BigDB.Load("Crews", crew, dbo =>
            {
                this.DatabaseObject = dbo;
                if (callback != null)
                {
                    callback.Invoke();
                }
            });
        }

        public void SendGetMessage(BasePlayer player)
        {
            var rtn = Message.Create("getCrew");

            var exists = this.DatabaseObject != null;

            //if there is an error this should be true
            rtn.Add(!exists);

            if (!exists)
            {
                player.Send(rtn);
                return;
            }

            rtn.Add(this.Id);
            rtn.Add(this.Name);
            rtn.Add(this.Subscribers);
            rtn.Add(this.LogoWorld);

            var rankId = this.GetRankId(player);
            rtn.Add(rankId);

            if (rankId >= 0)
            {
                rtn.Add(this.Unlocked("Descriptions"));
                rtn.Add(this.Unlocked("ColorPick"));
            }

            rtn.Add(this.TextColor);
            rtn.Add(this.PrimaryColor);
            rtn.Add(this.SecondaryColor);
            rtn.Add(this.Faceplate);
            rtn.Add(this.FaceplateColor);

            var faceplatesObj = this.Faceplates;
            if (faceplatesObj != null)
            {
                var faceplates =
                    (from faceplate in faceplatesObj where (bool) faceplate.Value select faceplate.Key).ToArray();
                rtn.Add(faceplates.Length);
                foreach (var faceplate in faceplates)
                {
                    rtn.Add(faceplate);
                }
            }
            else
            {
                rtn.Add(0);
            }

            var ranks = this.Ranks;
            rtn.Add(ranks.Count);
            foreach (var rank in ranks)
            {
                rtn.Add(rank.Name);
                rtn.Add(rank.PowersString);
            }

            this.GetWorlds(worlds =>
            {
                rtn.Add(worlds.Count);
                foreach (var world in worlds)
                {
                    rtn.Add(world);
                }

                this.GetMembers(members =>
                {
                    foreach (var member in members)
                    {
                        rtn.Add(member.Name);
                        rtn.Add(member.About);
                        rtn.Add(member.RankId);
                        rtn.Add(member.Smiley);
                        rtn.Add(member.SmileyGoldBorder);
                    }

                    player.Send(rtn);
                });
            });
        }

        public void Save(Callback callback = null)
        {
            if (this.DatabaseObject != null)
            {
                this.DatabaseObject.Save(callback);
            }
        }

        public void Disband()
        {
            this.client.BigDB.DeleteKeys("Crews", this.Id);
            this.DatabaseObject = null;
        }

        public bool Unlocked(string id)
        {
            if (this.DatabaseObject == null)
            {
                return false;
            }
            var unlocks = this.DatabaseObject.GetObject("Unlocks");
            return unlocks != null && unlocks.GetBool(id, false);
        }

        public void SetUnlocked(string id, bool unlocked = true)
        {
            if (this.DatabaseObject == null)
            {
                return;
            }

            var unlocks = this.DatabaseObject.GetObject("Unlocks");
            if (unlocks == null)
            {
                unlocks = new DatabaseObject();
                this.DatabaseObject.Set("Unlocks", unlocks);
            }
            unlocks.Set(id, unlocked);
        }

        public bool IsMember(BasePlayer player)
        {
            if (this.DatabaseObject != null)
            {
                var members = this.DatabaseObject.GetObject("Members");
                return members != null && members.Contains(player.ConnectUserId);
            }
            return false;
        }

        public bool HasPower(BasePlayer player, CrewPower power)
        {
            if (this.DatabaseObject == null || !this.IsMember(player))
            {
                return false;
            }
            if (player.ConnectUserId == this.Creator)
            {
                return true;
            }

            var rank = this.GetRank(player);
            return rank != null && rank.Powers.Contains(power);
        }

        public void GetMembers(Callback<List<CrewMember>> callback)
        {
            var members = new List<CrewMember>();
            if (this.DatabaseObject == null)
            {
                callback(members);
                return;
            }

            var membersObject = this.DatabaseObject.GetObject("Members");

            this.client.BigDB.LoadKeys("PlayerObjects", membersObject.Properties.ToArray(), playerObjects =>
            {
                members.AddRange(from player in playerObjects
                    where player != null
                    select
                        new CrewMember(player.Key, player.GetString("name", ""), player.GetInt("smiley", 0),
                            player.GetBool("smileyGoldBorder", false), this, membersObject.GetObject(player.Key)));

                callback(members);
            });
        }

        public void AddMember(BasePlayer player)
        {
            var newMember = new DatabaseObject();
            newMember.Set("Rank", 1);

            this.DatabaseObject.GetObject("Members").Set(player.ConnectUserId, newMember);

            this.client.BigDB.LoadOrCreate("CrewMembership", player.ConnectUserId, membership =>
            {
                membership.Set(this.Id, this.Name);
                membership.Save();
            });
        }

        public void SwapMembers(string userId1, string userId2, Callback callback = null)
        {
            var membersObject = this.DatabaseObject.GetObject("Members");

            DatabaseObject obj1 = membersObject.GetObject(userId1);
            DatabaseObject obj2 = membersObject.GetObject(userId2);
            membersObject.Remove(userId1).Remove(userId2);
            if (obj2 != null) membersObject.Set(userId1, obj2);
            if (obj1 != null) membersObject.Set(userId2, obj1);

            if (this.Creator == userId1)
            {
                this.Creator = userId2;
            }
            else if (this.Creator == userId2)
            {
                this.Creator = userId1;
            }

            this.Save(callback);
        }

        private void GetWorlds(Callback<List<string>> callback)
        {
            this.client.BigDB.LoadRange("Worlds", "ByCrew", new object[] {this.Id}, null, null, 1000, worlds =>
            {
                callback(worlds.Where(world => world != null && !world.GetBool("IsCrewLogo", false))
                    .Select(world => world.Key).ToList());
            });
        }

        public int GetRankId(BasePlayer player)
        {
            if (this.DatabaseObject == null || !this.IsMember(player))
            {
                return -1;
            }
            return this.DatabaseObject.GetObject("Members").GetObject(player.ConnectUserId).GetInt("Rank", -1);
        }

        public CrewRank GetRank(BasePlayer player)
        {
            return this.GetRank(this.GetRankId(player));
        }

        public CrewRank GetRank(int id)
        {
            var ranks = this.Ranks;
            if (ranks.Count < id || id < 0)
            {
                return null;
            }

            return ranks[id];
        }

        public void AddRank()
        {
            this.DatabaseObject.GetArray("Ranks")
                .Add(new DatabaseObject()
                    .Set("Name", "New Rank")
                    .Set("Powers", "0"));
        }

        private List<CrewPower> GetPowersForRank(int rank)
        {
            var powers = new List<CrewPower>();

            if (this.DatabaseObject != null && rank >= 0)
            {
                var ranks = this.Ranks;
                if (ranks.Count > rank)
                {
                    powers = ranks[rank].Powers;
                }
            }

            return powers;
        }

        public void PublishReleaseNotification(string roomId)
        {
            NotificationHelper.PublishNotification(this.client.BigDB,
                new EENotification
                {
                    Title = this.Name,
                    Body = "Hey! We just released our new level, click the link below to check it out!",
                    Channel = "crew" + this.Id,
                    PublishDate = DateTime.UtcNow,
                    ImageUrl = this.LogoWorld,
                    RoomId = roomId
                }, null);
        }

        public void PublishNotification(string body, Callback<DatabaseObject> callback = null)
        {
            NotificationHelper.PublishNotification(this.client.BigDB,
                new EENotification
                {
                    Title = this.Name,
                    Body = body,
                    Channel = "crew" + this.Id,
                    PublishDate = DateTime.UtcNow,
                    ImageUrl = this.LogoWorld
                }, callback);
        }

        public void GetMemberByName(string name, Callback<CrewMember> callback)
        {
            this.GetMembers(members => callback(members.FirstOrDefault(it => it.Name == name)));
        }

        public List<CrewPower> GetPowersForPlayer(BasePlayer player)
        {
            var powers = new List<CrewPower>();

            if (this.DatabaseObject == null || !this.IsMember(player) || this.Id.StartsWith("_"))
            {
                return powers;
            }

            var rank = this.GetRankId(player);
            return this.GetPowersForRank(rank);
        }
    }
}