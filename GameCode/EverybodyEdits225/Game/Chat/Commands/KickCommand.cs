using System.Linq;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class KickCommand : ChatCommand
    {
        public KickCommand(EverybodyEdits game) : base(game, CommandAccess.CrewMember, "kick")
        {
            this.LimitArguments(1, "You must specify a player to kick.");
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            var reason = commandInput.Length > 2 ? string.Join(" ", commandInput.Skip(2)) : "";
            var wereKicked = false;
            var kickTime = 5;

            this.Game.FilteredPlayers.Where(p => p.Name.ToLower() == commandInput[1].ToLower()).ToList().ForEach(target =>
            {
                if (this.Game.BaseWorld.IsPartOfCrew && this.Game.Crew.IsMember(target) &&
                    !player.Owner && player.Name != target.Name)
                {
                    this.SendSystemMessage(player, "You can not kick another crew member.");
                    return;
                }
                else
                {
                    if (target.IsModerator && !player.IsAdmin && !player.IsModerator ||
                        target.IsAdmin && !player.IsAdmin)
                    {
                        var role = target.IsAdmin ? "an Administrator" : "a Moderator";
                        this.SendSystemMessage(player, "You can not kick {0}.", role);
                        return;
                    }
                    else
                    {
                        if (!wereKicked)
                        {
                            if (player.Name == target.Name)
                            {
                                this.BroadcastSystemMessage("{0} is no more{1}", player.Name,
                                    reason.Trim() == "" ? "" : " - " + reason);

                                kickTime = 0;
                            }
                            else
                            {
                                this.BroadcastSystemMessage("{0} kicked {1}{2}", player.Name, target.Name,
                                    reason == "" ? "" : " - " + reason);
                            }
                        }

                        this.Game.KickUser(target, kickTime);

                        target.SendMessage("info", "You were kicked",
                            "You were kicked by " + player.Name.ToUpper() + ".\n" +
                            (reason != "" ? "Reason: " + reason : ""));

                        target.HasBeenKicked = true;
                        target.Disconnect();
                    }
                }

                wereKicked = true;
            });

            if (!wereKicked)
            {
                this.SendSystemMessage(player, "Unknown user {0}", commandInput[1]);
            }
        }
    }
}