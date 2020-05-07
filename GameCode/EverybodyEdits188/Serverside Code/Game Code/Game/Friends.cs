using System;
using System.Collections;
using System.Collections.Generic;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game
{
    internal class Friends
    {
        public const String INVITATIONS_TABLE = "Invitations";
        public const String FRIENDS_TABLE = "Friends";
        public const String BLOCKING_TABLE = "Ignores";

        public const int ERROR_INVITE_EXISTS = 1;
        public const int ERROR_INVITE_DOES_NOT_EXISTS = 2;
        public const int ERROR_MAX_FRIENDS = 5;
        public const int ERROR_CREATING = 10;
        public const int ERROR_SAVING = 11;
        public const int ERROR_ACTIVATING_OWN = 15;

        public const int MAX_FRIENDS = 50;
        public const int MAX_FRIENDS_BETA = 70;

        private readonly Client client;

        public Friends(Client c)
        {
            this.client = c;
        }

        public Boolean playerIsClubMember { get; set; }

        public Boolean playerIsBeta { get; set; }


        /**
		 * Returns the number of friends the player has plus number of invites the player has send.
		 **/

        public void getNumPotentialFriends(string connectionUserId, Callback<int> callback)
        {
            var potentialfriends = 0;
            this.client.BigDB.LoadOrCreate(FRIENDS_TABLE, connectionUserId, delegate (DatabaseObject friendsobj)
            {
                foreach (var key in friendsobj.Properties)
                {
                    if (friendsobj.GetBool(key)) potentialfriends++;
                }
                this.client.BigDB.LoadRange(INVITATIONS_TABLE, "senderId", new object[] { connectionUserId }, null, null,
                    100, delegate (DatabaseObject[] result)
                    {
                        foreach (var invite in result)
                        {
                            if (!invite.Contains("deletedBySender")) potentialfriends++;
                        }
                        callback.Invoke(potentialfriends);
                    });
            });
        }

        public void CreateInvitation(string connectionUserId, string senderName, string toMail, Callback successCallback,
            Callback<int> errorCallback)
        {
            this.getNumPotentialFriends(connectionUserId, delegate (int numfriends)
            {
                /*
                Console.WriteLine("Friends.cs, CreateInvitation -> " + getMaxFriendsAllowed());
                */
                if (numfriends < /*MAX_FRIENDS*/ this.getMaxFriendsAllowed())
                {
                    // Checking if player already send an invitation to this mail
                    this.client.BigDB.LoadRange(INVITATIONS_TABLE, "senderId", new object[] { connectionUserId, toMail },
                        null, null, 1000, delegate (DatabaseObject[] result)
                        {
                            var create_ok = true;
                            for (var i = 0; i < result.Length; i++)
                            {
                                create_ok = create_ok && result[i].GetBool("deletedBySender");
                            }

                            if (create_ok)
                            {
                                var invitation = new DatabaseObject();
                                invitation.Set("senderId", connectionUserId);
                                invitation.Set("senderName", senderName);
                                invitation.Set("recipientEmail", toMail);
                                invitation.Set("recipientId", "");
                                invitation.Set("deletedBySender", false);
                                invitation.Set("creationDate", DateTime.Now);

                                this.client.BigDB.CreateObject(INVITATIONS_TABLE, null, invitation,
                                    delegate (DatabaseObject newinvitation)
                                    {
                                        this.IsBlocked(toMail, connectionUserId, delegate (bool blocked)
                                        {
                                            if (!blocked)
                                            {
                                                this.SendInvitationEmail(toMail, senderName, newinvitation.Key);
                                            }
                                            else
                                            {
                                                newinvitation.Set("acceptedByRecipient", false);
                                                newinvitation.Save();
                                            }
                                            successCallback.Invoke();
                                        });
                                    }, delegate { errorCallback.Invoke(ERROR_CREATING); });
                            }
                            else
                            {
                                errorCallback.Invoke(ERROR_INVITE_EXISTS);
                            }
                        });
                }
                else
                {
                    errorCallback.Invoke(ERROR_MAX_FRIENDS);
                }
            });
        }

        public void DeletePending(string connectionUserId, String recipientEmail, Callback<bool> successCallback)
        {
            this.client.BigDB.LoadRange(INVITATIONS_TABLE, "senderId", new object[] { connectionUserId, recipientEmail },
                null, null, 1000, delegate (DatabaseObject[] result)
                {
                    if (result != null && result.Length > 0)
                    {
                        for (var i = 0; i < result.Length; i++)
                        {
                            result[i].Set("deletedBySender", true);
                            result[i].Save(delegate { successCallback.Invoke(true); });
                        }
                    }
                    else
                    {
                        successCallback.Invoke(false);
                    }
                }, delegate { successCallback.Invoke(false); });
        }

        public void IsBlocked(String email, String userid, Callback<bool> successCallback)
        {
            this.client.BigDB.LoadSingle(BLOCKING_TABLE, "ignore", new[] { email }, delegate (DatabaseObject ignore)
              {
                  if (ignore != null)
                  {
                      if (ignore.Contains("ignoreAll") && ignore.GetBool("ignoreAll"))
                      {
                          successCallback.Invoke(true);
                          return;
                      }
                      if (ignore.Contains("ignore") && ignore.GetObject("ignore").Contains(userid))
                      {
                          successCallback.Invoke(true);
                          return;
                      }
                  }
                  successCallback.Invoke(false);
              });
        }

        public void SendInvitationEmail(String recipientemail, String sendername, String invitekey)
        {
            var args = new Dictionary<string, string>();
            args.Add("template", "friend_invitation.txt");
            args.Add("gameid", "everybody-edits-su9rn58o40itdbnw69plyw");

            args.Add("senderemail", "no-reply@everybodyedits.com");
            args.Add("sendername", "Everybody Edits");
            args.Add("recipientemail", recipientemail);
            args.Add("subject", "Friend invitation from " + sendername.ToUpper());

            // Template arguments (are not escaped)
            var replace = new Dictionary<string, string>();
            replace.Add("from", sendername.ToUpper());
            replace.Add("invitekey", invitekey);
            //replace.Add("blocklink", "http%3A//everybodyedits.com/testing_alpha_120423/index.html?quickinvitelink=" + invitekey);

            var queryString = "";
            foreach (var key in replace.Keys)
            {
                queryString += key + "=" + replace[key] + "&";
            }
            queryString = queryString.Remove(queryString.Length - 1);

            this.client.Web.Post("http://api.playerio.com/services/email/send?" + queryString, args,
                delegate (HttpResponse reponse) { Console.WriteLine("Mail sendt: " + reponse); },
                delegate (PlayerIOError error) { Console.WriteLine("Error: " + error.Message); });
        }

        public void ActivateMyInvitation(string connectionUserId, String invite_id, Callback successCallback,
            Callback<int> errorCallback)
        {
            this.client.BigDB.Load(INVITATIONS_TABLE, invite_id,
                delegate (DatabaseObject invitation)
                {
                    if (invitation != null)
                    {
                        if (invitation.GetString("senderId") == connectionUserId)
                        {
                            invitation.Set("deletedBySender", true);
                            invitation.Save();
                            errorCallback.Invoke(ERROR_ACTIVATING_OWN);
                        }
                        else
                        {
                            invitation.Set("activiationDate", DateTime.Now);
                            invitation.Set("recipientId", connectionUserId);
                            invitation.Save(delegate { successCallback.Invoke(); });
                        }
                    }
                    else
                    {
                        errorCallback.Invoke(ERROR_INVITE_DOES_NOT_EXISTS);
                    }
                }
                );
        }

        public void AnswerMyInvitation(string connectionUserId, String sender_name, bool accept,
            Callback successCallback, Callback<int> errorCallback)
        {
            /*
            Console.WriteLine("answerInvite from " + sender_name + ": " + accept);
            Console.WriteLine("Friends.cs, AnswerMyInvitation -> " + getMaxFriendsAllowed());
            */
            this.getNumPotentialFriends(connectionUserId, delegate (int num)
            {
                if (num < /*MAX_FRIENDS*/ this.getMaxFriendsAllowed() || !accept)
                {
                    this.client.BigDB.LoadRange(INVITATIONS_TABLE, "senderName",
                        new object[] { sender_name, connectionUserId }, null, null, 1000,
                        delegate (DatabaseObject[] invitations)
                        {
                            if (invitations != null && invitations.Length > 0)
                            {
                                var found = false;
                                for (var i = 0; i < invitations.Length; i++)
                                {
                                    if (!invitations[i].GetBool("deletedBySender", false) &&
                                        !invitations[i].Contains("acceptedByRecipient") || !accept)
                                    {
                                        found = true;
                                        invitations[i].Set("acceptedByRecipient", accept);
                                        invitations[i].Save();

                                        if (accept)
                                        {
                                            this.AddFriends(connectionUserId, invitations[i].GetString("senderId"),
                                                delegate { successCallback.Invoke(); });
                                        }
                                        else
                                        {
                                            successCallback.Invoke();
                                        }
                                    }
                                }
                                if (!found)
                                {
                                    errorCallback.Invoke(ERROR_INVITE_DOES_NOT_EXISTS);
                                }
                            }
                            else
                            {
                                errorCallback.Invoke(ERROR_INVITE_DOES_NOT_EXISTS);
                            }
                        });
                }
                else
                {
                    errorCallback.Invoke(ERROR_MAX_FRIENDS);
                }
            });
        }


        public void GetFriendKeys(String userid, Callback<string[]> callback)
        {
            this.client.BigDB.LoadOrCreate(FRIENDS_TABLE, userid, delegate (DatabaseObject friendsobj)
            {
                var keys = new ArrayList();
                foreach (var key in friendsobj.Properties)
                {
                    if (friendsobj.GetBool(key))
                    {
                        keys.Add(key);
                    }
                }
                var keylist = new string[keys.Count];
                keys.CopyTo(keylist);
                callback.Invoke(keylist);
            });
        }

        public void AddFriends(String userid, String friendid, Callback callback = null)
        {
            this.AddFriend(userid, friendid,
                delegate { this.AddFriend(friendid, userid, delegate { callback.Invoke(); }); });
        }

        public void AddFriend(String userid, String friendid, Callback callback = null)
        {
            this.client.BigDB.LoadOrCreate(FRIENDS_TABLE, userid, delegate (DatabaseObject friends)
            {
                friends.Set(friendid, true);
                friends.Save(delegate { if (callback != null) callback.Invoke(); });
            }
                );
        }

        public void RemoveFriends(String userid, String friendid, Callback<bool> callback = null)
        {
            this.RemoveFriend(userid, friendid,
                delegate (bool success1)
                {
                    this.RemoveFriend(friendid, userid,
                        delegate (bool success2) { callback.Invoke(success1 && success2); });
                });
        }

        public void RemoveFriend(String userid, String friendid, Callback<bool> callback = null)
        {
            this.client.BigDB.LoadOrCreate(FRIENDS_TABLE, userid, delegate (DatabaseObject friends)
            {
                if (friends != null)
                {
                    friends.Set(friendid, false);
                    friends.Save(delegate { if (callback != null) callback.Invoke(true); });
                }
            }
                );
        }

        public void BlockInvite(String userid, String invited_by, Callback successCallback,
            Callback<int> errorCallback = null)
        {
            this.client.BigDB.LoadSingle(INVITATIONS_TABLE, "senderName", new object[] { invited_by, userid },
                delegate (DatabaseObject invitation)
                {
                    if (invitation != null)
                    {
                        var email = invitation.GetString("recipientEmail");
                        this.client.BigDB.LoadSingle(BLOCKING_TABLE, "ignore", new[] { email },
                            delegate (DatabaseObject ignore)
                            {
                                var create = (ignore == null);
                                if (create)
                                {
                                    ignore = new DatabaseObject();
                                }

                                ignore.Set("ignoreEmail", email);
                                ignore.Set("owner", userid);
                                if (!ignore.Contains("ignore"))
                                {
                                    ignore.Set("ignore", new DatabaseObject());
                                }
                                ignore.GetObject("ignore")
                                    .Set(invitation.GetString("senderId"), invitation.GetString("senderName"));

                                if (create)
                                {
                                    this.client.BigDB.CreateObject(BLOCKING_TABLE, null, ignore, delegate { });
                                }
                                else
                                {
                                    ignore.Save(delegate { successCallback.Invoke(); },
                                        delegate { if (errorCallback != null) errorCallback.Invoke(ERROR_SAVING); });
                                }
                            });
                    }
                    else
                    {
                        if (errorCallback != null) errorCallback.Invoke(ERROR_INVITE_DOES_NOT_EXISTS);
                    }
                });
        }

        public void UnblockInvite(String id, String mail, String invited_by, Callback successCallback,
            Callback<int> errorCallback = null)
        {
            this.client.BigDB.LoadSingle(BLOCKING_TABLE, "ignore", new[] { mail }, delegate (DatabaseObject ignore)
              {
                  if (ignore != null)
                  {
                      if (invited_by == "all" && ignore.Contains("ignoreAll"))
                      {
                          ignore.Set("ignoreAll", false);
                          ignore.Save(delegate { successCallback.Invoke(); },
                              delegate { if (errorCallback != null) errorCallback.Invoke(ERROR_SAVING); });
                      }
                      else if (ignore.Contains("ignore"))
                      {
                          foreach (var key in ignore.GetObject("ignore").Properties)
                          {
                              if (ignore.GetObject("ignore").GetString(key) == invited_by)
                              {
                                  ignore.GetObject("ignore").Remove(key);
                                  ignore.Save(delegate { successCallback.Invoke(); },
                                      delegate { if (errorCallback != null) errorCallback.Invoke(ERROR_SAVING); });
                                  break;
                              }
                          }
                      }
                  }
                  else
                  {
                      if (errorCallback != null) errorCallback.Invoke(ERROR_SAVING);
                  }
              });
        }

        public void GetPending(String userid, Callback<ArrayList> callback)
        {
            this.client.BigDB.LoadRange(INVITATIONS_TABLE, "senderId", new object[] { userid }, null, null, 100,
                delegate (DatabaseObject[] invitelist)
                {
                    var rtn = new ArrayList();
                    Console.WriteLine("invitelist length: " + invitelist.Length);
                    for (var i = 0; i < invitelist.Length; i++)
                    {
                        Console.WriteLine("invite " + i + ": " + invitelist[i].Key + " - " +
                                          invitelist[i].GetBool("deletedBySender") + " / " +
                                          invitelist[i].Contains("acceptedByRecipient"));
                        if (!invitelist[i].GetBool("deletedBySender") && !invitelist[i].Contains("acceptedByRecipient"))
                        {
                            rtn.Add(invitelist[i].GetString("recipientEmail"));
                        }
                    }
                    callback.Invoke(rtn);
                });
        }

        public void GetInvitesToMe(String userid, Callback<ArrayList> callback)
        {
            this.client.BigDB.LoadRange(INVITATIONS_TABLE, "recipientId", new object[] { userid }, null, null, 100,
                delegate (DatabaseObject[] invitelist)
                {
                    var rtn = new ArrayList();
                    for (var i = 0; i < invitelist.Length; i++)
                    {
                        var invite = invitelist[i];
                        if (!invite.Contains("acceptedByRecipient"))
                        {
                            rtn.Add(invite.GetString("senderName"));
                            rtn.Add(invite.GetBool("deletedBySender"));
                            rtn.Add(invite.GetString("recipientEmail"));
                        }
                    }
                    callback.Invoke(rtn);
                });
        }

        public void GetBlockedUsers(String userid, Callback<ArrayList> callback)
        {
            this.client.BigDB.LoadRange(BLOCKING_TABLE, "byOwner", new object[] { userid }, null, null, 100,
                delegate (DatabaseObject[] ignorelist)
                {
                    var rtn = new ArrayList();
                    for (var i = 0; i < ignorelist.Length; i++)
                    {
                        var mail = ignorelist[i].GetString("ignoreEmail");
                        if (ignorelist[i].Contains("ignoreAll") && ignorelist[i].GetBool("ignoreAll"))
                        {
                            rtn.Add(mail);
                            rtn.Add("all");
                        }
                        else if (ignorelist[i].Contains("ignore"))
                        {
                            var ignoreobj = ignorelist[i].GetObject("ignore");
                            foreach (var key in ignoreobj.Properties)
                            {
                                rtn.Add(mail);
                                rtn.Add(ignoreobj.GetString(key));
                            }
                        }
                    }
                    callback.Invoke(rtn);
                });
        }

        public int getMaxFriendsAllowed()
        {
            if (this.playerIsClubMember || this.playerIsBeta)
            {
                return MAX_FRIENDS_BETA;
            }
            return MAX_FRIENDS;
        }
    }
}