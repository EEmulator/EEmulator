using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.Crews
{
    public class CrewObject
    {
        protected readonly Crew Crew;
        protected readonly DatabaseObject DatabaseObject;

        protected CrewObject(Crew crew, DatabaseObject dbo)
        {
            this.Crew = crew;
            this.DatabaseObject = dbo;
        }
    }
}