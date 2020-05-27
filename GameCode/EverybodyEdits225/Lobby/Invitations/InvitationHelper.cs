using System.Linq;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Lobby.Invitations
{
    internal class InvitationHelper
    {
        private const string InvitationsTable = "Invitations";

        public static void GetInvitation(BigDB bigDb, InvitationType type, string sender, string recipient,
            Callback<Invitation> callback)
        {
            bigDb.LoadSingle(InvitationsTable, "BySenderAndRecipient",
                new object[] { (int)type, sender, recipient }, dbo => { callback(new Invitation(dbo)); });
        }

        public static void GetInvitationsFrom(BigDB bigDb, InvitationType type, string sender,
            Callback<Invitation[]> callback)
        {
            bigDb.LoadRange(InvitationsTable, "BySender",
                new object[] { (int)type, sender }, null, null, 100,
                invitations => { callback(invitations.Select(dbo => new Invitation(dbo)).ToArray()); });
        }

        public static void GetInvitationsTo(BigDB bigDb, InvitationType type, string recipient,
            Callback<Invitation[]> callback)
        {
            bigDb.LoadRange(InvitationsTable, "ByRecipient",
                new object[] { (int)type, recipient }, null, null, 100,
                invitations => { callback(invitations.Select(dbo => new Invitation(dbo)).ToArray()); });
        }

        public static void CreateInvitation(BigDB bigDb, InvitationType type, string sender, string recipient,
            Callback<DatabaseObject> callback)
        {
            var newInvite = new DatabaseObject()
                .Set("Type", (int)type)
                .Set("Sender", sender)
                .Set("Recipient", recipient)
                .Set("Status", (int)InvitationStatus.Pending);
            bigDb.CreateObject(InvitationsTable, null, newInvite, callback);
        }

        public static void DeleteInvitation(BigDB bigDb, InvitationType type, string sender, string recipient,
            Callback<bool> callback)
        {
            bigDb.DeleteRange(InvitationsTable, "BySenderAndRecipient",
                new object[] { (int)type, sender, recipient }, null, null,
                () => callback(true),
                error => callback(false));
        }

        public static void DeleteInvitations(BigDB bigDb, Invitation[] invites)
        {
            bigDb.DeleteKeys(InvitationsTable, invites.Select(it => it.Key).ToArray());
        }
    }
}