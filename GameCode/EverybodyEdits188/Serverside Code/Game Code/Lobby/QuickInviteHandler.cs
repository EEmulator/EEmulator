using System;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.QuickInviteHandler
{
    public class BlockPlayer : BasePlayer
    {
    }

    [RoomType("QuickInviteHandler188")]
    public class QuickInviteHandler : Game<BlockPlayer>
    {
        public override void GameStarted()
        {
        }

        public override void GameClosed()
        {
        }

        public override bool AllowUserJoin(BlockPlayer player)
        {
            return true;
        }

        public override void UserJoined(BlockPlayer player)
        {
            this.PlayerIO.BigDB.Load("Invitations", player.JoinData["inviteid"], delegate(DatabaseObject invitation)
            {
                Console.WriteLine("UserJoined. Loaded invitation: " + invitation);
                if (invitation != null)
                {
                    Console.WriteLine("Blocking? " + (player.JoinData["block"] == "true"));
                    if (player.JoinData["block"] == "true")
                    {
                        var email = invitation.GetString("recipientEmail");
                        this.PlayerIO.BigDB.LoadSingle("Ignores", "ignore", new[] {email},
                            delegate(DatabaseObject ignore)
                            {
                                var create = ignore == null;
                                if (create)
                                {
                                    ignore = new DatabaseObject();
                                }

                                ignore.Set("ignoreEmail", email);
                                ignore.Set("ignoreAll", true);

                                if (create)
                                {
                                    this.PlayerIO.BigDB.CreateObject("Ignores", null, ignore,
                                        delegate { player.Disconnect(); });
                                }
                                else
                                {
                                    ignore.Save(delegate { player.Disconnect(); });
                                }
                            });
                    }
                }
            });
        }

        public override void GotMessage(BlockPlayer player, Message message)
        {
        }
    }
}