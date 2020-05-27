using System;
using System.Linq;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.AntiCheat
{
    public class BasicAntiCheat
    {
        private readonly int[] climbableBlocks =
        {
            118, 120, 98, 99, 424, 459, 460, 1534, // climbable
            4, 414, // dots
            114, 115, 116, 117 // boosts
        };

        private readonly int[] fluids =
        {
            119, 416, 369
        };

        private readonly EverybodyEdits game;

        private readonly int[] sometimesPassableBlocks =
        {
            26, 27, 28, 1008, 1009, 1010, 156, 185, 1012, 1028, 201, 165, 214, 207, // gates
            23, 24, 25, 1005, 1006, 1007, 157, 184, 1011, 1027, 200, 43, 213, 206, // doors
            61, 62, 63, 64, 89, 90, 91, 96, 97, 122, 123, 124, 125, 126, 127, 146, 154, 158, 194, 211, 216, // one ways
            1001, 1002, 1003, 1004, 1050, 1051, 1052, 1053, 1054, 1055, 1056, 1087, 1092, // one ways
            1041, 1042, 1043, 1075, 1076, 1077, 1078, 1101, 1102, 1103, 1104, 1105, // half bricks
            1116, 1117, 1118, 1119, 1120, 1121, 1122, 1123, 1124, 1125
        };

        private readonly int[] spawns =
        {
            242, 381, 255, 360
        };

        public BasicAntiCheat(EverybodyEdits game)
        {
            this.game = game;
        }

        private bool IsPassable(int blockId)
        {
            if (blockId == 77 || blockId == 83) {
                return true;
            }
            if (sometimesPassableBlocks.Contains(blockId)) {
                return true;
            }

            return !(9 <= blockId && blockId <= 97 || 122 <= blockId && blockId <= 217 || blockId >= 1001 && blockId <= 1499 || blockId >= 2000);
        }

        private bool IsFluid(int blockId)
        {
            return fluids.Contains(blockId);
        }

        private bool IsSpawn(int blockId)
        {
            return spawns.Contains(blockId);
        }

        private bool IsSolid(uint bx, uint by)
        {
            var blockId = (int)game.BaseWorld.GetBrickType(0, bx, by);
            return !IsPassable(blockId) || sometimesPassableBlocks.Contains(blockId);
        }

        private bool IsClimbable(int id)
        {
            return climbableBlocks.Contains(id);
        }

        private bool TouchesFluid(double x, double y, int range)
        {
            return Touches(x, y, range, IsFluid);
        }

        private bool TouchesSpawn(double x, double y, int range)
        {
            return Touches(x, y, range, IsSpawn);
        }

        private bool TouchesClimbable(double x, double y, int range)
        {
            return Touches(x, y, range, IsClimbable);
        }

        private bool TouchesSolid(double x, double y, int range)
        {
            return Touches(x, y, range, id => !IsPassable(id));
        }

        private bool Touches(double x, double y, int range, Func<int, bool> selector)
        {
            var bX = Math.Round(x) / 16;
            var bY = Math.Round(y) / 16;
            var bXMin = Math.Max(0, Math.Floor(bX) - range);
            var bXMax = Math.Min(Math.Ceiling(bX) + range, this.game.BaseWorld.Width - 1);
            var bYMin = Math.Max(0, Math.Floor(bY) - range);
            var bYMax = Math.Min(Math.Ceiling(bY) + range, this.game.BaseWorld.Height - 1);
            for (var cX = bXMin; cX <= bXMax; cX++) {
                for (var cY = bYMin; cY <= bYMax; cY++) {
                    var b = game.BaseWorld.GetBrickType(0, (uint)cX, (uint)cY);
                    if (selector((int)b)) {
                        return true;
                    }
                }
            }
            return false;
        }

        public void OnFace(Player p)
        {
            if (p.IsBot == null) {
                p.IsBot = false;
            }
        }

        public bool OnTrophy(Player p)
        {
            if (p.IsBot ?? true) {
                // Bot!
                this.game.LogCheatingUser(p, "Bot");
                return false;
            }

            var antiCheatData = this.game.BaseWorld.AntiCheatData;
            if (p.TotalMovements < antiCheatData.MinMoves) {
                this.game.KickAndLogCheatingUser(p, "TooFewMovements");
                return true;
            }

            if (p.Coins < antiCheatData.MinCoins) {
                this.game.KickAndLogCheatingUser(p, "TooFewCoins");
                return true;
            }

            if (p.BlueCoins < antiCheatData.MinBlueCoins) {
                this.game.KickAndLogCheatingUser(p, "TooFewBlueCoins");
                return true;
            }
            if (p.IsCheater) {
                this.game.KickAndLogCheatingUser(p, "TrophyAfterCheating");
                return true;
            }

            return false;
        }

        public void OnMove(Player p)
        {
            if (p.IsInGodMode || p.IsInModeratorMode || p.IsInAdminMode) {
                // Flying players are exempt of anticheat
                return;
            }

            if (TouchesSolid(p.X, p.Y, 0)) {
                // Cheater | Dont log when players get stuck at 0,0
                if (p.X != 0 || p.Y != 0) {
                    this.LogMovementCheatingUser(p, "MovingInsideBlocks");
                }

                p.IsCheater = true;
                return;
            }
            if (p.FlipGravity == 0) { 
                //Who really cares about anticheat anymore lets only check these if gravity is downwards....
                var blockX = (uint)((int)p.X + 8) >> 4;
                var blockY = (uint)((int)p.Y + 8) >> 4;

                var range = Math.Max(p.SpeedX, p.SpeedY) > 0.5 || Math.Min(p.SpeedX, p.SpeedY) < 0.5 ? 1 : 0;
                if (!TouchesClimbable(p.X, p.Y, range) && !TouchesFluid(p.X, p.Y, range) && !TouchesSpawn(p.X, p.Y, range)) {
                    if (!(p.SpeedX > 1) && !(p.SpeedY > 1) && !(p.SpeedX < -1) && !(p.SpeedY < -1)) {
                        var gravMultiplayer = p.HasActiveEffect(EffectId.LowGravity)
                            ? .15
                            : game.BaseWorld.GravityMultiplier;

                        var modX = p.ModifierX / gravMultiplayer;
                        var modY = p.ModifierY / gravMultiplayer;

                        if (modX != -2 && modX != 2 && modY != -2 && modY != 2) {
                            p.AntiFlightHeat++;

                            if (p.AntiFlightHeat > 4) {
                                // Cheater
                                this.LogMovementCheatingUser(p, "Flight");

                                p.IsCheater = true;
                            }
                        }
                        else if (p.AntiFlightHeat > 0) {
                            p.AntiFlightHeat--;
                        }
                    }
                }

                if (p.SpaceDown) {
                    var jumpHeight = p.HasActiveEffect(EffectId.Jump) ? 67.7 : 52;
                    if (p.SpeedX < -jumpHeight && this.IsSolid(blockX + 1u, blockY) ||
                        p.SpeedX > jumpHeight && this.IsSolid(blockX - 1u, blockY) ||
                        p.SpeedY < -jumpHeight && this.IsSolid(blockX, blockY + 1) ||
                        p.SpeedY > jumpHeight && this.IsSolid(blockX, blockY - 1)) {
                        p.AntiJumpHeat++;

                        if (p.AntiJumpHeat > 4) {
                            // Cheater
                            this.LogMovementCheatingUser(p, "JumpingTooHigh");

                            p.IsCheater = true;
                        }
                    }
                    else if (p.AntiJumpHeat > 0) {
                        p.AntiJumpHeat--;
                    }
                }
            }
        }

        private void LogMovementCheatingUser(Player p, string cheatType)
        {
            var dbo = new DatabaseObject();
            dbo.Set("x", p.X);
            dbo.Set("y", p.Y);
            dbo.Set("speedX", p.SpeedX);
            dbo.Set("speedY", p.SpeedY);
            dbo.Set("modifierX", p.ModifierX);
            dbo.Set("modifierY", p.ModifierY);
            dbo.Set("hor", p.Horizontal);
            dbo.Set("ver", p.Vertical);
            dbo.Set("SpaceDown", p.SpaceDown);
            dbo.Set("SpaceJP", p.SpaceJustPressed);
            dbo.Set("tickID", p.TickId);
            game.LogCheatingUser(p, cheatType, dbo);
        }
    }
}
