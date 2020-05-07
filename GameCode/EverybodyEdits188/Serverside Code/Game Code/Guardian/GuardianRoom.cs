using PlayerIO.GameLibrary;

namespace EverybodyEdits.Guardian
{
    public class GuardianPlayer : BasePlayer
    {
    }

    [RoomType("GuardianRoom")]
    public class GuardianRoom : Game<GuardianPlayer>
    {
        // This method is called when an instance of your the game is created
        public override void GameStarted()
        {
            this.PreloadPlayerObjects = true;
        }

        public override bool AllowUserJoin(GuardianPlayer player)
        {
            /*if(player.ConnectUserId.IndexOf("armor") == 0) { 
				
				player.Send("auth", Create(player.ConnectUserId, secret));

			}else player.Disconnect();*/
            if (player.PlayerObject.GetBool("isGuardian", false))
            {
                return true;
            }
            return false;
        }

        public override void GotMessage(GuardianPlayer player, Message m)
        {
            switch (m.Type)
            {
                case "latest":
                {
                    this.PlayerIO.BigDB.LoadRange("AbuseReports", "Date", null, null, null, 100,
                        delegate (DatabaseObject[] latest)
                        {
                            var rtnMessage = Message.Create("latest");
                            foreach (var report in latest)
                            {
                                this.AddReportToMessage(report, rtnMessage);
                            }
                            player.Send(rtnMessage);
                        });

                    break;
                }

                /*

            case "getPlayerBans":
                {


                    string key = m.GetString(0);
                    PlayerIO.BigDB.Load("GuardianBans", key, delegate(DatabaseObject ban)
                    {
                        if (ban != null)
                        {
                            Message rtnMessage = Message.Create("getPlayerBans");
                            rtnMessage.Add(ban.Key);
                            rtnMessage.Add(ban.GetString("Name"));
                            AddUserBanToMessage(ban, rtnMessage);
                            player.Send(rtnMessage);
                        }
                        else
                        {
                            player.Send(Message.Create("getSinglePlayerBanError","No user '" + key + "'"));
                        }
                    },
                    delegate(PlayerIOError error)
                    {

                    });



                    break;
                }


            case "getWorldBans":
                {


                    string key = m.GetString(0);
                    PlayerIO.BigDB.Load("WorldBans", key, delegate(DatabaseObject ban)
                    {
                        if (ban != null)
                        {
                            Message rtnMessage = Message.Create("getWorldBans");
                            rtnMessage.Add(ban.Key);
                            rtnMessage.Add(ban.GetString("Name"));
                            AddUserBanToMessage(ban, rtnMessage);
                            player.Send(rtnMessage);
                        }
                        else
                        {
                            player.Send(Message.Create("getWorldBansError", "No world with id  '" + key + "'"));
                        }
                    },
                    delegate(PlayerIOError error)
                    {
                        player.Send(Message.Create("getWorldBansError", "No world with id '" + key + "'"));
                    });

                    break;
                }

            case "setWorldBan":
                {
                    string key = m.GetString(0);
                    string ticketid = m.GetString(0);
                    bool status = m.GetBoolean(2);

                    PlayerIO.BigDB.Load("WorldBans", key, delegate(DatabaseObject banObj)
                    {
                        if (banObj != null)
                        {
                            DatabaseArray bans = banObj.GetArray("Bans");

                            foreach (DatabaseObject ban in bans)
                            {
                                if (ban.GetString("Id") == ticketid)
                                {
                                    ban.Set("Active", status);
                                    ban.Save(delegate()
                                    {
                                        Message rtnMessage = Message.Create("setWorldBan");
                                        rtnMessage.Add(ban.Key);
                                        rtnMessage.Add(ban.GetString("Name"));
                                        AddUserBanToMessage(ban, rtnMessage);
                                        player.Send(rtnMessage);
                                    });
                                    return;
                                }
                            }
                            player.Send(Message.Create("setWorldBanError", "No ban with id '" + ticketid + "'"));
                        }
                        else
                        {
                            player.Send(Message.Create("setWorldBanError", "No world '" + key + "'"));
                        }
                    },
                    delegate(PlayerIOError error)
                    {

                    });

                    break;
                }

             */
                case "setAbuseReportStatus":
                {
                    var key = m.GetString(0);

                    this.PlayerIO.BigDB.Load("AbuseReports", key, delegate (DatabaseObject report)
                    {
                        var status = m.GetString(1);
                        report.Set("State", status);
                        report.Save(delegate { player.Send("setAbuseReportStatus", true); },
                            delegate (PlayerIOError error) { player.Send("setAbuseReportStatus", false, error.Message); });
                    }, delegate (PlayerIOError error) { player.Send("setAbuseReportStatusError", false, error.Message); });
                    break;
                }
            }
        }

        private void AddReportToMessage(DatabaseObject report, Message msg)
        {
            msg.Add(report.Key);
            msg.Add(report.GetString("State"));
            msg.Add(report.GetDateTime("Date").ToString());
            msg.Add(report.GetString("WorldId"));
            msg.Add(report.GetString("WorldName"));
            msg.Add(report.GetString("ReportedByUsername"));
            msg.Add(report.GetString("ReportedUsername"));
            msg.Add(report.GetString("Reason"));

            var chatlog = report.GetArray("ChatLog");
            var chatlogstring = "";
            foreach (var item in chatlog)
            {
                chatlogstring += item + "|";
            }

            msg.Add(chatlogstring);
        }

        private void AddUserBanToMessage(DatabaseObject banObj, Message msg)
        {
            var bans = banObj.GetArray("Bans");
            foreach (DatabaseObject ban in bans)
            {
                msg.Add(ban.GetString("Id"));
                msg.Add(ban.GetDateTime("Date").ToString());
                msg.Add(ban.GetDateTime("Expires").ToString());
                msg.Add(ban.GetString("BannedBy"));
                msg.Add(ban.GetString("Reason"));
            }
        }

        private void AddWorldBanToMessage(DatabaseObject banObj, Message msg)
        {
            var bans = banObj.GetArray("Bans");
            foreach (DatabaseObject ban in bans)
            {
                msg.Add(ban.GetString("Id"));
                msg.Add(ban.GetDateTime("Date").ToString());
                msg.Add(ban.GetString("BannedBy"));
                msg.Add(ban.GetString("Reason"));
                msg.Add(ban.GetBool("Active"));
            }
        }
    }
}