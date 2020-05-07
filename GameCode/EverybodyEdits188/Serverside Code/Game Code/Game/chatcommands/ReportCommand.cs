using System;
using System.Text;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.ChatCommands
{
    internal class ReportCommand : ChatCommand
    {
        public ReportCommand(EverybodyEdits game)
            : base(game)
        {
        }

        public override void resolve(Player player, string[] commandInput)
        {
            var lastreport = player.lastreport;
            if (lastreport.AddSeconds(90) > DateTime.Now)
            {
                Console.WriteLine("Spamming. Not saved. " + lastreport + " vs " + DateTime.Now);
                player.Send("info", "Report submitted", "Thank you for your report, we'll look into it shortly.");
                return;
            }


            if (commandInput.Length < 3)
            {
                player.Send("info", "Report", "Please specify a playername and a reason for this report");
                return;
            }


            var reportedUserName = commandInput[1].Trim().ToLower();

            var existingReport = this._game.GetLocalCopyOfReport(reportedUserName);
            if (existingReport != null)
            {
                if (existingReport.GetDateTime("Date").AddSeconds(90) > DateTime.Now)
                {
                    Console.WriteLine("Existing report found. Not saved");
                    //if (existingReport.GetString("ReportedByUsername", "") == player.name)
                    //{
                    player.Send("info", "Report submitted", "Thank you for your report, we'll look into it shortly.");
                    return;
                    //}
                }
            }

            this._game.PlayerIO.BigDB.Load("usernames", reportedUserName, delegate(DatabaseObject o)
            {
                if (o == null || o.GetString("owner", null) == null)
                {
                    player.Send("write", this._game.SystemName, "User " + reportedUserName.ToUpper() + " not found");
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
                var chatMessages = this._game.GetLastChatMessages();
                var messages = new StringBuilder();
                foreach (var message in chatMessages)
                {
                    if (message != null)
                    {
                        messages.Append("name: " + message.name + ": " + message.text + " | ");
                    }
                }

                var signsList = this._game.BaseWorld.GetBrickTextSignList();
                var signtextArray = new DatabaseArray();
                foreach (var sign in signsList)
                {
                    signtextArray.Add(sign.text);
                }
                var chattextArray = new DatabaseArray();
                foreach (var message in chatMessages)
                {
                    chattextArray.Add(message.name + ": " + message.text);
                }

                var report = new DatabaseObject();
                report.Set("ReportedByUsername", player.name);
                report.Set("ReportedByConnectId", player.ConnectUserId);
                report.Set("State", "Open");
                report.Set("Date", DateTime.Now);
                report.Set("WorldId", this._game.BaseWorld.key);
                report.Set("WorldName", this._game.BaseWorld.name);
                report.Set("Reason", reason);
                report.Set("Signs", signtextArray);
                report.Set("ChatLog", chattextArray);
                report.Set("ReportedUsername", reportedUserName);
                report.Set("ReportedUserConnectId", reportedUserConnectId);

                this._game.SaveLocalCopyOfReport(report);

                player.lastreport = DateTime.Now;
                this._game.PlayerIO.BigDB.CreateObject("AbuseReports", null, report,
                    delegate
                    {
                        player.Send("info", "Report submitted", "Thank you for your report, we'll look into it shortly.");
                    }, delegate
                    {
                        player.lastreport = DateTime.Now.AddHours(-1);
                        player.Send("info", "Error occured", "An error occured in while submitting. Please try again.");
                    });
            });
        }

        // From: http://www.dotnetperls.com/remove-html-tags
        // Note this will remove ANYTHING within a opening and closing tag
        private string StripTagsCharArray(string source)
        {
            var array = new char[source.Length];
            var arrayIndex = 0;
            var inside = false;

            for (var i = 0; i < source.Length; i++)
            {
                var let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }
    }
}