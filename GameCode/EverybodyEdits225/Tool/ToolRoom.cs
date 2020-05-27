using PlayerIO.GameLibrary;

namespace EverybodyEdits.Tool
{
    [RoomType("ToolRoom")]
    public class ToolRoom : Game<ToolPlayer>
    {
        // This method is called when an instance of your the game is created
        public override void GameStarted()
        {
            this.PreloadPlayerObjects = true;
        }

        public override bool AllowUserJoin(ToolPlayer player)
        {
            return player.PlayerObject.GetBool("isAdministrator", false) ||
                   player.PlayerObject.GetBool("isModerator", false);
        }

        public override void GotMessage(ToolPlayer player, Message m)
        {
            switch (m.Type)
            {
                case "latest":
                {
                    this.PlayerIO.BigDB.LoadRange("AbuseReports", "Date", null, null, null, 100,
                        latest =>
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

                case "getPlayerBans":
                {
                    var key = m.GetString(0);
                    PlayerIO.BigDB.Load("TempBans", key, ban =>
                    {
                        if (ban != null)
                        {
                            var rtnMessage = Message.Create("getPlayerBans");
                            rtnMessage.Add(ban.Key);
                            rtnMessage.Add(ban.GetString("Name"));
                            AddUserBanToMessage(ban, rtnMessage);
                            player.Send(rtnMessage);
                        }
                        else
                        {
                            player.Send(Message.Create("getSinglePlayerBanError", "No user '" + key + "'"));
                        }
                    }, value => { });
                    break;
                }

                case "setAbuseReportActionUndertaken":
                {
                    var key = m.GetString(0);

                    this.PlayerIO.BigDB.Load("AbuseReports", key, delegate(DatabaseObject report)
                    {
                        var action = m.GetString(1);
                        report.Set("ActionUndertaken", action);
                        report.Save(() => player.Send("setAbuseReportActionUndertaken", true),
                            error => player.Send("setAbuseReportActionUndertaken", false, error.Message));
                    }, error => player.Send("setAbuseReportActionUndertakenError", false, error.Message));
                    break;
                }

                case "setAbuseReportStatus":
                {
                    var key = m.GetString(0);

                    this.PlayerIO.BigDB.Load("AbuseReports", key, delegate(DatabaseObject report)
                    {
                        var status = m.GetString(1);
                        report.Set("State", status);
                        report.Save(() => player.Send("setAbuseReportStatus", true),
                            error => player.Send("setAbuseReportStatus", false, error.Message));
                    }, error => player.Send("setAbuseReportStatusError", false, error.Message));
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
            msg.Add(report.GetString("ActionUndertaken", "No action undertaken yet."));
            msg.Add(string.Join("|", report.GetArray("ChatLog")));
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
    }
}