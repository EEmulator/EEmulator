using System;
using System.Text.RegularExpressions;
using EverybodyEdits.Common;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Lobby
{
    public class Registration
    {
        private readonly Client playerIo;

        public Registration(Client client)
        {
            this.playerIo = client;
        }

        public void HandleMessage(LobbyPlayer player, Message message)
        {
            switch (message.Type)
            {
                case "finishTutorial":
                {
                    player.PlayerObject.Set("tutorialVersion", Config.TutorialVersion);
                    player.PlayerObject.Save(() => player.Send("finishTutorial"));
                    return;
                }

                case "acceptTerms":
                {
                    player.PlayerObject.Set("termsVersion", Config.TermsVersion);
                    if (player.PlayerObject.Contains("acceptTerms"))
                    {
                        player.PlayerObject.Remove("acceptTerms");
                    }
                    player.PlayerObject.Save(() => { player.Send("acceptTerms"); });
                    return;
                }

                case "checkUsername":
                {
                    var name = message.GetString(0);

                    this.playerIo.BigDB.Load("Usernames", name.ToLower(),
                        result =>
                        {
                            player.Send("checkUsername", name,
                                result == null || (!result.Contains("owner") && !result.Contains("oldowner")));
                        },
                        error => { player.Send("checkUsername", name, false); });
                    return;
                }

                case "setUsername":
                {
                    var username = player.PlayerObject.GetString("name", null);
                    if (username == null && player.ConnectUserId != "simpleguest")
                    {
                        var newname = message.GetString(0).ToLower();
                        var test = new Regex("[^0-9a-z]");
                        if (test.IsMatch(newname))
                        {
                            player.Send("error",
                                "Your username contains invalid charaters. Valid charaters are 0-9 and A-Z");
                        }
                        else if (newname.Length > 20)
                        {
                            player.Send("error", "Your username cannot be more than 20 characters long.");
                        }
                        else if (newname.Length < 3)
                        {
                            player.Send("error", "Your username must be atleast 3 characters long.");
                        }

                        else if (BadWords.ContainsBadWord(newname))
                        {
                            player.Send("error", "Your username contains inappropriate words.");
                        }
                        else
                        {
                            var obj = new DatabaseObject();
                            obj.Set("owner", player.ConnectUserId);
                            this.playerIo.BigDB.CreateObject("Usernames", newname, obj,
                                delegate
                                {
                                    Console.WriteLine("Set Username " + newname + " --> " + player.Name);
                                    player.PlayerObject.Set("name", newname);
                                    player.PlayerObject.Save(delegate
                                    {
                                        Console.WriteLine("Username saved " + player.Name);
                                        player.Send("username", newname);
                                        player.Send("setUsername");
                                    });
                                },
                                delegate
                                {
                                    player.Send("error", "The username " + newname.ToUpper() + " is already taken!");
                                }
                                );
                        }
                    }
                    else
                    {
                        player.Send("username", username);
                    }
                    return;
                }

                case "changeUsername":
                {
                    if (player.PayVault.Has("changeusername") && player.PlayerObject.GetBool("changename", false))
                    {
                        var username = player.PlayerObject.GetString("name", null);
                        if (username != null && player.ConnectUserId != "simpleguest")
                        {
                            var newname = message.GetString(0).ToLower();
                            var test = new Regex("[^0-9a-z]");
                            if (test.IsMatch(newname))
                            {
                                player.Send("error",
                                    "Your new username contains invalid charaters. Valid charaters are 0-9 and A-Z");
                            }
                            else if (newname.Length > 20)
                            {
                                player.Send("error", "Your new username cannot be more than 20 characters long.");
                            }
                            else if (newname.Length < 3)
                            {
                                player.Send("error", "Your new username must be atleast 3 characters long.");
                            }
                            else if (BadWords.ContainsBadWord(newname))
                            {
                                player.Send("error", "Your username contains inappropriate words.");
                            }
                            else
                            {
                                var obj = new DatabaseObject();
                                obj.Set("owner", player.ConnectUserId);
                                this.playerIo.BigDB.CreateObject("Usernames", newname, obj,
                                    delegate
                                    {
                                        this.playerIo.BigDB.Load("Usernames", username,
                                            delegate (DatabaseObject o)
                                            {
                                                o.Set("oldowner", player.ConnectUserId);
                                                o.Set("owner", "none");
                                                o.Save();

                                                Console.WriteLine("Change Username " + username + " --> " + newname);
                                                player.PlayerObject.Set("name", newname);
                                                player.PlayerObject.Set("oldname", username);
                                                player.PlayerObject.Set("changename", false);
                                                player.PlayerObject.Save(delegate
                                                {
                                                    Console.WriteLine("Username saved " + player.Name);
                                                    player.Send("username", newname);
                                                    player.Send("changeUsername");
                                                });
                                            }
                                            );
                                    },
                                    delegate
                                    {
                                        player.Send("error", "The username " + newname.ToUpper() + " is already taken!");
                                    }
                                    );
                            }
                        }
                        else
                        {
                            player.Send("username", username);
                        }
                    }
                    else
                    {
                        player.Send("error",
                            "You didn't purchase your name change yet!");
                        return;
                    }

                    return;
                }


                default:
                    return;
            }
        }
    }
}