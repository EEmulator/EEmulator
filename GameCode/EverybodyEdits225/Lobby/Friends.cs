using System.Collections.Generic;
using System.Linq;
using EverybodyEdits.Common;
using EverybodyEdits.Game;
using EverybodyEdits.Lobby.Invitations;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Lobby
{
    internal class Friends
    {
        private const string FriendsTable = "Friends";

        private const int MaxFriends = 70;
        private const int MaxFriendsBeta = 130;

        private readonly Client client;
        private readonly string connectUserId;
        private readonly string name;

        public Friends(Client client, string connectUserId, string name)
        {
            this.client = client;
            this.connectUserId = connectUserId;
            this.name = name;
        }

        public bool PlayerIsGoldMember { private get; set; }
        public bool PlayerIsBetaMember { private get; set; }

        private int MaxFriendsAllowed
        {
            get
            {
                return this.PlayerIsBetaMember || this.PlayerIsGoldMember
                    ? MaxFriendsBeta
                    : MaxFriends;
            }
        }

        private void GetPendingInvitationsCount(Callback<int> callback)
        {
            InvitationHelper.GetInvitationsFrom(this.client.BigDB, InvitationType.Friend,
                this.name, invitations =>
                    callback(invitations.Count(invite =>
                        invite.Status == InvitationStatus.Pending)));
        }

        private void GetFriendsCount(Callback<int> callback)
        {
            this.GetFriendKeys(this.connectUserId, friends =>
                callback(friends.Count));
        }

        private void CreateInvitation(string recipientName,
            Callback successCallback, Callback<InvitationError> errorCallback)
        {
            if (this.name == recipientName)
            {
                errorCallback(InvitationError.SendingToSelf);
                return;
            }

            CommonPlayer.GetId(this.client.BigDB, recipientName, recipientId =>
            {
                if (recipientId == null)
                {
                    errorCallback(InvitationError.PlayerNotFound);
                    return;
                }

                this.GetFriendKeys(this.connectUserId, friends =>
                {
                    if (friends.Contains(recipientId))
                    {
                        errorCallback(InvitationError.AlreadyAdded);
                        return;
                    }

                    InvitationHelper.GetInvitation(this.client.BigDB, InvitationType.Friend,
                        this.name, recipientName, oldInvitation =>
                        {
                            if (oldInvitation.Exists)
                            {
                                errorCallback(InvitationError.AlreadySent);
                                return;
                            }

                            this.GetPendingInvitationsCount(pendingInvitationsCount =>
                            {
                                if (friends.Count + pendingInvitationsCount >= this.MaxFriendsAllowed)
                                {
                                    errorCallback(InvitationError.LimitReached);
                                    return;
                                }

                                InvitationHelper.GetInvitation(this.client.BigDB, InvitationType.Friend,
                                    recipientName, this.name, invitationToMe =>
                                    {
                                        if (invitationToMe.Exists && invitationToMe.Status == InvitationStatus.Pending)
                                        {
                                            invitationToMe.Status = InvitationStatus.Accepted;
                                            this.AddOrRemoveFriend(recipientId, true,
                                                () => invitationToMe.Save(successCallback));
                                            return;
                                        }

                                        InvitationBlocking.IsUserBlocked(this.client.BigDB, recipientId, this.name,
                                            blocked =>
                                            {
                                                if (blocked)
                                                {
                                                    errorCallback(InvitationError.Blocked);
                                                    return;
                                                }

                                                var newInvitation = new DatabaseObject();
                                                newInvitation.Set("Sender", this.name);
                                                newInvitation.Set("Recipient", recipientName);
                                                newInvitation.Set("Status", (int) InvitationStatus.Pending);

                                                InvitationHelper.CreateInvitation(this.client.BigDB,
                                                    InvitationType.Friend,
                                                    this.name, recipientName,
                                                    invitation => successCallback());
                                            });
                                    });
                            });
                        });
                });
            });
        }

        private void AnswerInvitation(string senderName, bool accept, Callback<string> successCallback,
            Callback<InvitationError> errorCallback)
        {
            InvitationHelper.GetInvitation(this.client.BigDB, InvitationType.Friend,
                senderName, this.name, invitation =>
                {
                    if (!invitation.Exists)
                    {
                        errorCallback(InvitationError.InvitationNotFound);
                        return;
                    }

                    CommonPlayer.GetId(this.client.BigDB, senderName, senderId =>
                    {
                        if (senderId == null)
                        {
                            InvitationHelper.DeleteInvitation(this.client.BigDB, InvitationType.Friend, senderName,
                                this.name, result =>
                                    errorCallback(InvitationError.PlayerNotFound));
                            return;
                        }

                        this.GetFriendsCount(numFriends =>
                        {
                            if (accept)
                            {
                                if (numFriends >= this.MaxFriendsAllowed)
                                {
                                    errorCallback(InvitationError.LimitReached);
                                    return;
                                }

                                invitation.Status = InvitationStatus.Accepted;
                                this.AddOrRemoveFriend(senderId, true);
                            }
                            else
                            {
                                invitation.Status = InvitationStatus.Rejected;
                            }

                            invitation.Save(() => successCallback(senderId));
                        });
                    });
                });
        }

        private void AddOrRemoveFriend(string friendId, bool add, Callback callback = null)
        {
            this.GetFriends(this.connectUserId, myFriends =>
            {
                myFriends.Set(friendId, add);
                myFriends.Save(delegate
                {
                    this.GetFriends(friendId, theirFriends =>
                    {
                        theirFriends.Set(this.connectUserId, add);
                        theirFriends.Save(() =>
                        {
                            if (callback != null)
                            {
                                callback.Invoke();
                            }
                        });
                    });
                });
            });
        }

        private void GetFriends(string userId, Callback<DatabaseObject> callback)
        {
            this.client.BigDB.LoadOrCreate(FriendsTable, userId, callback);
        }

        public void GetFriendKeys(string userId, Callback<List<string>> callback)
        {
            this.GetFriends(userId, friends =>
            {
                var keys = friends.Properties.Where(friends.GetBool).ToList();

                callback(keys);
            });
        }

        public void ProcessMessages(QueueItem item, LobbyPlayer player)
        {
            switch (item.Method)
            {
                case "getFriends":
                {
                    this.GetFriendKeys(player.ConnectUserId, keys =>
                    {
                        if (keys.Count <= 0)
                        {
                            player.Send(item.Method);
                            return;
                        }

                        OnlineStatus.GetOnlineStatus(this.client, keys.ToArray(), status =>
                        {
                            var rtn = Message.Create(item.Method);
                            foreach (var stat in status.Where(stat => stat != null))
                            {
                                stat.ToMessage(rtn);
                            }
                            player.Send(rtn);
                        });
                    });
                    break;
                }

                case "getPending":
                {
                    InvitationHelper.GetInvitationsFrom(this.client.BigDB, InvitationType.Friend,
                        this.name, invites =>
                        {
                            var rtn = Message.Create(item.Method);
                            foreach (var invite in invites)
                            {
                                rtn.Add(invite.Recipient);
                                rtn.Add((int) invite.Status);
                            }
                            player.Send(rtn);
                        });
                    break;
                }

                case "getInvitesToMe":
                {
                    InvitationHelper.GetInvitationsTo(this.client.BigDB, InvitationType.Friend,
                        this.name, invites =>
                        {
                            var rtn = Message.Create(item.Method);
                            foreach (var invite in invites.Where(it => it.Status == InvitationStatus.Pending))
                            {
                                rtn.Add(invite.Sender);
                            }
                            player.Send(rtn);
                        });
                    break;
                }

                case "getBlockedUsers":
                {
                    InvitationBlocking.GetBlockedUsers(this.client.BigDB, this.connectUserId, blockedUsers =>
                    {
                        var rtn = Message.Create(item.Method);
                        foreach (var blockedUser in blockedUsers)
                        {
                            rtn.Add(blockedUser);
                        }
                        player.Send(rtn);
                    });
                    break;
                }

                case "createInvite":
                {
                    if (!player.HasFriendFeatures)
                    {
                        player.Send(item.Method, false);
                        return;
                    }

                    var friendName = item.Message.GetString(0).ToLower();

                    this.CreateInvitation(friendName,
                        () => player.Send(item.Method, true),
                        error =>
                        {
                            switch (error)
                            {
                                case InvitationError.PlayerNotFound:
                                {
                                    player.Send(item.Method, false,
                                        "Unknown user. Please check your spelling.");
                                    break;
                                }
                                case InvitationError.AlreadyAdded:
                                {
                                    player.Send(item.Method, false,
                                        "This user is already on your friendslist!");
                                    break;
                                }
                                case InvitationError.AlreadySent:
                                {
                                    player.Send(item.Method, false,
                                        "You already have a pending invitation for this user.");
                                    break;
                                }
                                case InvitationError.LimitReached:
                                {
                                    player.Send(item.Method, false,
                                        "You cannot have more than " +
                                        this.MaxFriendsAllowed +
                                        " friends and invites.");
                                    break;
                                }
                                case InvitationError.Blocked:
                                {
                                    player.Send(item.Method, false,
                                        "This user is blocking friend requests.");
                                    break;
                                }
                                case InvitationError.SendingToSelf:
                                {
                                    player.Send(item.Method, false,
                                        "You cannot add yourself.");
                                    break;
                                }
                            }
                        });
                    break;
                }

                case "answerInvite":
                {
                    if (!player.HasFriendFeatures)
                    {
                        player.Send(item.Method, false);
                        return;
                    }

                    var invitedBy = item.Message.GetString(0).ToLower();
                    var accept = item.Message.GetBoolean(1);

                    this.AnswerInvitation(invitedBy, accept,
                        senderId =>
                        {
                            OnlineStatus.GetOnlineStatus(this.client, senderId, onlineStatus =>
                            {
                                var rtn = Message.Create(item.Method, true);
                                player.Send(onlineStatus.ToMessage(rtn));
                            });
                        },
                        error =>
                        {
                            switch (error)
                            {
                                case InvitationError.PlayerNotFound:
                                {
                                    player.Send(item.Method, false,
                                        "Sorry, the sender of this friend request does not exist.");
                                    break;
                                }
                                case InvitationError.InvitationNotFound:
                                {
                                    player.Send(item.Method, false,
                                        "Sorry, the friend request does not exist anymore.");
                                    break;
                                }
                                case InvitationError.LimitReached:
                                {
                                    player.Send(item.Method, false,
                                        "You cannot have more than " +
                                        this.MaxFriendsAllowed +
                                        " friends.");
                                    break;
                                }
                            }
                        });
                    break;
                }

                case "deleteInvite":
                {
                    var recipientName = item.Message.GetString(0).ToLower();

                    InvitationHelper.DeleteInvitation(this.client.BigDB, InvitationType.Friend,
                        this.name, recipientName, success =>
                        {
                            if (!success)
                            {
                                this.client.ErrorLog.WriteError(
                                    "Error deleting invitation from " + player.Name + " to " +
                                    recipientName, "Invite not found",
                                    "Error deleting pending invitation", null);
                            }
                            player.Send(item.Method, success);
                        });
                    break;
                }

                case "blockUserInvites":
                {
                    var invitedByName = item.Message.GetString(0).ToLower();
                    var shouldBlock = item.Message.GetBoolean(1);

                    InvitationBlocking.BlockUser(this.client.BigDB, this.connectUserId, invitedByName, shouldBlock,
                        () =>
                        {
                            if (shouldBlock)
                            {
                                InvitationHelper.GetInvitation(this.client.BigDB, InvitationType.Friend, invitedByName,
                                    this.name, invitation =>
                                    {
                                        if (invitation.Exists)
                                        {
                                            invitation.Status = InvitationStatus.Rejected;
                                            invitation.Save(() => player.Send(item.Method, true));
                                        }
                                        else
                                        {
                                            player.Send(item.Method, true);
                                        }
                                    });
                            }
                            else
                            {
                                player.Send(item.Method, true);
                            }
                        });
                    break;
                }

                case "getBlockStatus":
                {
                    InvitationBlocking.IsBlockingAllUsers(this.client.BigDB, this.connectUserId,
                        isBlocking => { player.Send(item.Method, isBlocking); });
                    break;
                }

                case "blockAllInvites":
                {
                    var shouldBlock = item.Message.GetBoolean(0);

                    InvitationBlocking.BlockAllFriends(this.client.BigDB, this.connectUserId, shouldBlock);
                    player.Send(item.Method, shouldBlock);
                    break;
                }

                case "deleteFriend":
                {
                    CommonPlayer.GetId(this.client.BigDB, item.Message.GetString(0).ToLower(), friendId =>
                    {
                        this.AddOrRemoveFriend(friendId, false,
                            delegate { player.Send(item.Method, true); });
                    });
                    break;
                }

                case "GetOnlineStatus":
                {
                    var id = item.Message.Count > 0 ? item.Message.GetString(0) : player.ConnectUserId;
                    OnlineStatus.GetOnlineStatus(this.client, id,
                        onlineStatus => player.Send(onlineStatus.ToMessage(item.Method)));
                    break;
                }
            }
        }
    }
}