using System.Collections.Generic;
using System.Linq;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class SetTeamCommand : ChatCommand
    {
        private readonly List<string> teamNames = new List<string>();

        public SetTeamCommand(EverybodyEdits game)
            : base(game, CommandAccess.CrewMember, "setteam")
        {
            this.LimitArguments(2, "Please specify a username and a team name: \"None\", \"Red\", \"Blue\", \"Green\", \"Cyan\", \"Magenta\", \"Yellow\" or identifier.");
            this.PopulateTeamNames();
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            var username = commandInput[1].ToLower();
            var team = commandInput[2].ToLower();

            if (!player.PayVault.Has("brickeffectteam"))
            {
                this.SendSystemMessage(player, "Oops! Looks like you do not own team blocks.");
                return;
            }

            int teamId;
            if (!int.TryParse(team, out teamId))
            {
                if (this.teamNames.Contains(team))
                {
                    teamId = this.teamNames.IndexOf(team);
                }
                else
                {
                    this.SendSystemMessage(player,
                        "Invalid team name or identifier. Available teams: \"None\", \"Red\", \"Blue\", \"Green\", \"Cyan\", \"Magenta\", \"Yellow\"");
                    return;
                }
            }
            else if (teamId < 0 || teamId > 6)
            {
                this.SendSystemMessage(player, "Specified team identifier is out of available range: 0-6");
                return;
            }

            foreach (var pl in this.Game.FilteredPlayers.Where(pl => pl.Name.ToLower() == username))
            {
                if (pl.Team == teamId)
                {
                    this.SendSystemMessage(player, pl.Name.ToUpper() + " already has the specified team.");
                    return;
                }

                pl.Team = teamId;
                this.Game.BroadcastMessage("team", pl.Id, teamId);

                this.SendSystemMessage(player, "Team of {0} changed to {1}.", pl.Name.ToUpper(), teamNames[teamId]);
                return;
            }

            this.SendSystemMessage(player, "Player not found.");
        }

        private void PopulateTeamNames()
        {
            // Must be in order of ID.
            this.teamNames.Add("none");
            this.teamNames.Add("red");
            this.teamNames.Add("blue");
            this.teamNames.Add("green");
            this.teamNames.Add("cyan");
            this.teamNames.Add("magenta");
            this.teamNames.Add("yellow");
        }
    }
}