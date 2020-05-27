using System;
using System.Linq;
using System.Text;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class ReportCommand : ChatCommand
    {
        public ReportCommand(EverybodyEdits game)
            : base(game, CommandAccess.Public, "report", "reportabuse", "rep")
        {
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            if(player.IsGuest)
            {
                player.SendMessage("info", "Sorry", "Please register or sign in to report."); // someone make this better
                return;
            }
            var lastreport = player.LastReport;
            if (lastreport.AddSeconds(90) > DateTime.Now)
            {
                Console.WriteLine("Spamming. Not saved. " + lastreport + " vs " + DateTime.Now);
                player.SendMessage("info", "Report submitted", "Thank you for your report, we'll look into it shortly.");
                return;
            }


            if (commandInput.Length < 3)
            {
                player.SendMessage("info", "Report", "Please specify a playername and a reason for this report");
                return;
            }


            var reportedUserName = commandInput[1].Trim().ToLower();

            var existingReport = this.Game.GetLocalCopyOfReport(reportedUserName);
            if (existingReport != null)
            {
                if (existingReport.GetDateTime("Date").AddSeconds(90) > DateTime.Now)
                {
                    Console.WriteLine("Existing report found. Not saved");
                    player.SendMessage("info", "Report submitted",
                        "Thank you for your report, we'll look into it shortly.");
                    return;
                }
            }

            this.Game.PlayerIO.BigDB.Load("usernames", reportedUserName, delegate(DatabaseObject o)
            {
                if (o == null || o.GetString("owner", null) == null)
                {
                    this.SendSystemMessage(player, "User " + reportedUserName.ToUpper() + " not found");
                    return;
                }

                var reportedUserConnectId = o.GetString("owner");

                // if theres a reason
                var reason = "";
                for (var a = 2; a < commandInput.Length; a++)
                {
                    reason += commandInput[a] + " ";
                }

                // get current chat texts
                var chatMessages = this.Game.Chat.LastChatMessages;
                var messages = new StringBuilder();
                foreach (var message in chatMessages.Where(message => message != null))
                {
                    messages.Append("name: " + message.SenderName + ": " + message.Text + " | ");
                }

                var signsList = this.Game.BaseWorld.GetBrickTextSignList();
                var signtextArray = new DatabaseArray();
                foreach (var sign in signsList)
                {
                    signtextArray.Add(sign.Text);
                }
                var chattextArray = new DatabaseArray();
                foreach (var message in chatMessages)
                {
                    chattextArray.Add(message.SenderName + ": " + message.Text);
                }

                var report = new DatabaseObject();
                report.Set("ReportedByUsername", player.Name);
                report.Set("ReportedByConnectId", player.ConnectUserId);
                report.Set("State", "Open");
                report.Set("Date", DateTime.Now);
                report.Set("WorldId", this.Game.BaseWorld.Key);
                report.Set("WorldName", this.Game.BaseWorld.Name);
                report.Set("Reason", reason);
                report.Set("Signs", signtextArray);
                report.Set("ChatLog", chattextArray);
                report.Set("ReportedUsername", reportedUserName);
                report.Set("ReportedUserConnectId", reportedUserConnectId);

                this.Game.SaveLocalCopyOfReport(report);

                player.LastReport = DateTime.Now;
                this.Game.PlayerIO.BigDB.CreateObject("AbuseReports", null, report,
                    delegate
                    {
                        player.SendMessage("info", "Report submitted",
                            "Thank you for your report, we'll look into it shortly.");
                    }, delegate
                    {
                        player.LastReport = DateTime.Now.AddHours(-1);
                        player.SendMessage("info", "Error occurred",
                            "An error occurred in while submitting. Please try again.");
                    });
            });
        }
    }
}