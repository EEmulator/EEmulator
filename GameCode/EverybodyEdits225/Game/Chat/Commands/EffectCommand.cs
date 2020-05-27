using System.Collections.Generic;
using PlayerIO.GameLibrary;
using System.Linq;
using System;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class EffectCommand : ChatCommand
    {
        private readonly List<EffectId> argumentEffects = new List<EffectId> { EffectId.Curse, EffectId.Zombie, EffectId.Fire, EffectId.Multijump, EffectId.Gravity };
        private readonly List<EffectId> blacklistEffects = new List<EffectId> { EffectId.Team };

        public EffectCommand(EverybodyEdits game) : base(game, CommandAccess.CrewMember, "removeeffect", "giveeffect", "reffect", "geffect")
        {
            this.LimitArguments(2, "You must specify a player and an effect");
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            var take = commandInput[0].StartsWith("r");
            var effectname = commandInput[2];
            var username = commandInput[1];

            EffectId effect;
            if (Enum.TryParse(effectname, true, out effect)) {
                if (!Enum.IsDefined(effect.GetType(), effect)) {
                    this.SendSystemMessage(player, "Unknown effect id.");
                    return;
                }

                if (this.blacklistEffects.Contains(effect)) {
                    this.SendSystemMessage(player, "Please use the command 'setteam' instead.");
                    return;
                }

                var target = this.Game.FilteredPlayers.FirstOrDefault(p => p.Name.ToLower() == username.ToLower());
                if (target == null) return;

                var effectId = Convert.ToInt32(effect);
                var textMessage = effect + " Effect was " + (take ? "taken from" : "given to") + " " + target.Name.ToUpper();

                if (take) {
                    target.RemoveEffect(effect);
                    this.Game.BroadcastMessage("effect", target.Id, effectId, false);
                }
                else {
                    if (!this.CanGiveEffect(player, effect)) {
                        this.SendSystemMessage(player, "You do not own this effect.");
                        return;
                    }

                    var message = Message.Create("effect", target.Id, effectId, true);

                    this.Game.CheckEffects();

                    var effectToGive = new Effect(effectId);
                    if (this.argumentEffects.Contains(effect)) {
                        int value;
                        if (commandInput.Length > 3) {
                            int.TryParse(commandInput[3], out value);
                        }
                        else {
                            this.SendSystemMessage(player, "You must give {0} for this effect!", effect == EffectId.Multijump ? "an amount" : effect == EffectId.Gravity ? "a direction" : "a duration");
                            return;
                        }

                        if (effect == EffectId.Multijump) {
                            if (value == 1) {
                                player.RemoveEffect(effect);
                                this.Game.BroadcastMessage("effect", target.Id, effectId, false);
                                this.SendSystemMessage(player, "{0} Effect was given to {1} with 1 jump", effect, username.ToUpper());
                                return;
                            }
                            var jumpsToGive = value < 0 || value > 999 ? (value == -1 || value == 1000) ? 1000 : 1 : value;
                            message.Add(jumpsToGive);
                            textMessage += " with " + (jumpsToGive == 1000 ? "inf" : jumpsToGive.ToString()) + " jumps";
                        }
                        else if (effect == EffectId.Gravity) {
                            GravityDirection direction;
                            if (Enum.TryParse(commandInput[3], true, out direction)) {
                                if (direction < GravityDirection.Down)
                                    direction = GravityDirection.Down;

                                if (direction > GravityDirection.None)
                                    direction = GravityDirection.None;

                                if (direction == GravityDirection.Down) {
                                    target.RemoveEffect(effect);
                                    this.Game.BroadcastMessage("effect", target.Id, effectId, false);
                                    this.SendSystemMessage(player, "Gravity Effect was taken from " + target.Name.ToUpper());
                                    return;
                                }

                                effectToGive = new Effect(effectId, (int)direction) {
                                    CanExpire = false
                                };

                                textMessage = direction + " Gravity Effect was given to " + target.Name.ToUpper();
                                message.Add((int)direction);
                            }
                            else {
                                this.SendSystemMessage(player, "Failed parsing Direction.");
                                return;
                            }
                        }
                        else effectToGive = new Effect(effectId, value <= 0 || value > 999 ? 10 : value);
                    }

                    if (effectToGive.CanExpire) {
                        if (target.HasActiveEffect(EffectId.Protection)) {
                            player.RemoveEffect(EffectId.Protection);
                            this.Game.BroadcastMessage("effect", target.Id, 3, false);
                        }

                        effectToGive.Activate();

                        message.Add(effectToGive.TimeLeft);
                        message.Add(effectToGive.Duration);

                        textMessage += " for " + effectToGive.Duration + " second" + (effectToGive.Duration == 1 ? "" : "s");
                    }

                    target.AddEffect(effectToGive);
                    this.Game.BroadcastMessage(message);
                }

                this.SendSystemMessage(player, textMessage);
            }
            else this.SendSystemMessage(player, "Unknown effect id.");
        }

        private bool CanGiveEffect(Player player, EffectId effect)
        {
            var brick = "brickeffect" + effect.ToString().ToLower();

            switch (effect) {
                case EffectId.Fire: brick = "bricklava"; break;
                case EffectId.Run: brick = "brickeffectspeed"; break;
            }

            return player.HasBrickPack(brick);
        }
    }

    enum GravityDirection
    {
        Down,
        Left,
        Up,
        Right,
        None
    }
}