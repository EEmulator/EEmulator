namespace EverybodyEdits.Game.Chat.Commands
{
    internal enum CommandAccess
    {
        Public,
        PlayerWithEdit,
        CrewMember,
        Owner,
        Moderator,
        Admin
    }
}