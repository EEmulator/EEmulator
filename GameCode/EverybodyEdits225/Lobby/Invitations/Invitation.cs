using PlayerIO.GameLibrary;

namespace EverybodyEdits.Lobby.Invitations
{
    internal class Invitation
    {
        private readonly DatabaseObject dbo;

        public Invitation(DatabaseObject dbo)
        {
            this.dbo = dbo;
        }

        public string Key
        {
            get { return this.dbo.Key; }
        }

        public bool Exists
        {
            get { return this.dbo != null; }
        }

        public string Sender
        {
            get { return this.dbo.GetString("Sender"); }
        }

        public string Recipient
        {
            get { return this.dbo.GetString("Recipient"); }
        }

        public InvitationStatus Status
        {
            get { return (InvitationStatus) this.dbo.GetInt("Status", (int) InvitationStatus.Unknown); }
            set { this.dbo.Set("Status", (int) value); }
        }

        public void Save(Callback callback = null)
        {
            this.dbo.Save(callback);
        }
    }
}