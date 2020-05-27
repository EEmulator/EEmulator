using EverybodyEdits.Game.CountWorld;
using System.Collections.Generic;

namespace EverybodyEdits.Game
{
    public class DBBrick
    {
        public readonly uint Goal;

        public readonly uint Id;
        public readonly uint Rotation;
        public readonly uint Target;
        public readonly string TargetWorld;

        public readonly string Text;
        public readonly string TextColor;
        public readonly uint SignType;
        public readonly uint Type;

        public readonly List<uint> Xs = new List<uint>();
        public readonly List<uint> Ys = new List<uint>();
        public int Layer;

        public DBBrick(ForegroundBlock b)
        {
            this.Type = b.Type;

            switch (this.Type)
            {
                case (uint)ItemTypes.CoinDoor:
                case (uint)ItemTypes.CoinGate:
                case (uint)ItemTypes.BlueCoinDoor:
                case (uint)ItemTypes.BlueCoinGate:
                case (uint)ItemTypes.DeathDoor:
                case (uint)ItemTypes.DeathGate:
                case (uint)ItemTypes.SwitchPurple:
                case (uint)ItemTypes.DoorPurple:
                case (uint)ItemTypes.GatePurple:
                case (uint)ItemTypes.EffectTeam:
                case (uint)ItemTypes.TeamDoor:
                case (uint)ItemTypes.TeamGate:
                case (uint)ItemTypes.EffectCurse:
                case (uint)ItemTypes.EffectFly:
                case (uint)ItemTypes.EffectJump:
                case (uint)ItemTypes.EffectProtection:
                case (uint)ItemTypes.EffectRun:
                case (uint)ItemTypes.EffectZombie:
                case (uint)ItemTypes.EffectLowGravity:
                case (uint)ItemTypes.EffectMultijump:
                case (uint)ItemTypes.SwitchOrange:
                case (uint)ItemTypes.DoorOrange:
                case (uint)ItemTypes.GateOrange:
                    this.Goal = b.Number;
                    break;

                case (uint)ItemTypes.Portal:
                case (uint)ItemTypes.PortalInvisible:
                    this.Id = b.PortalId;
                    this.Target = b.PortalTarget;
                    this.Rotation = b.PortalRotation;
                    break;

                case (uint)ItemTypes.WorldPortal:
                    this.TargetWorld = b.Text;
                    break;

                case (uint)ItemTypes.Label:
                    this.Text = b.Text;
                    this.TextColor = b.Color;
                    this.Goal = b.WrapLength;
                    break;

                case (uint)ItemTypes.Piano:
                case (uint)ItemTypes.Drums:
                case (uint)ItemTypes.Guitar:
                    this.Id = b.Number;
                    break;

                case (uint)ItemTypes.Spike:
                case (uint)ItemTypes.GlowyLineBlueSlope:
                case (uint)ItemTypes.GlowyLineBlueStraight:
                case (uint)ItemTypes.GlowyLineGreenSlope:
                case (uint)ItemTypes.GlowyLineGreenStraight:
                case (uint)ItemTypes.GlowyLineYellowSlope:
                case (uint)ItemTypes.GlowyLineYellowStraight:
                case (uint)ItemTypes.GlowyLineRedSlope:
                case (uint)ItemTypes.GlowyLineRedStraight:
                case (uint)ItemTypes.OnewayCyan:
                case (uint)ItemTypes.OnewayOrange:
                case (uint)ItemTypes.OnewayYellow:
                case (uint)ItemTypes.OnewayPink:
                case (uint)ItemTypes.OnewayGray:
                case (uint)ItemTypes.OnewayBlue:
                case (uint)ItemTypes.OnewayRed:
                case (uint)ItemTypes.OnewayGreen:
                case (uint)ItemTypes.OnewayBlack:
                case (uint)ItemTypes.OnewayWhite:
                case (uint)ItemTypes.MedievalAxe:
                case (uint)ItemTypes.MedievalBanner:
                case (uint)ItemTypes.MedievalCoatOfArms:
                case (uint)ItemTypes.MedievalShield:
                case (uint)ItemTypes.MedievalSword:
                case (uint)ItemTypes.MedievalTimber:
                case (uint)ItemTypes.ToothBig:
                case (uint)ItemTypes.ToothSmall:
                case (uint)ItemTypes.ToothTriple:
                case (uint)ItemTypes.DojoLightLeft:
                case (uint)ItemTypes.DojoLightRight:
                case (uint)ItemTypes.DojoDarkLeft:
                case (uint)ItemTypes.DojoDarkRight:
                case (uint)ItemTypes.DomesticLightBulb:
                case (uint)ItemTypes.DomesticTap:
                case (uint)ItemTypes.DomesticPainting:
                case (uint)ItemTypes.DomesticVase:
                case (uint)ItemTypes.DomesticTv:
                case (uint)ItemTypes.DomesticWindow:
                case (uint)ItemTypes.HalfBlockDomesticBrown:
                case (uint)ItemTypes.HalfBlockDomesticWhite:
                case (uint)ItemTypes.HalfBlockDomesticYellow:
                case (uint)ItemTypes.Halloween2015WindowRect:
                case (uint)ItemTypes.Halloween2015WindowCircle:
                case (uint)ItemTypes.Halloween2015Lamp:
                case (uint)ItemTypes.NewYear2015Balloon:
                case (uint)ItemTypes.NewYear2015Streamer:
                case (uint)ItemTypes.FairytaleFlowers:
                case (uint)ItemTypes.HalfBlockFairytaleOrange:
                case (uint)ItemTypes.HalfBlockFairytaleGreen:
                case (uint)ItemTypes.HalfBlockFairytaleBlue:
                case (uint)ItemTypes.HalfBlockFairytalePink:
                case (uint)ItemTypes.HalfBlockWhite:
                case (uint)ItemTypes.HalfBlockGray:
                case (uint)ItemTypes.HalfBlockBlack:
                case (uint)ItemTypes.HalfBlockRed:
                case (uint)ItemTypes.HalfBlockOrange:
                case (uint)ItemTypes.HalfBlockYellow:
                case (uint)ItemTypes.HalfBlockGreen:
                case (uint)ItemTypes.HalfBlockCyan:
                case (uint)ItemTypes.HalfBlockBlue:
                case (uint)ItemTypes.HalfBlockPurple:
                case (uint)ItemTypes.SpringDaisy:
                case (uint)ItemTypes.SpringTulip:
                case (uint)ItemTypes.SpringDaffodil:
                case (uint)ItemTypes.SummerIceCream:
                case (uint)ItemTypes.SummerFlag:
                case (uint)ItemTypes.SummerAwning:
                case (uint)ItemTypes.CaveCrystal:
                case (uint)ItemTypes.RestaurantBowl:
                case (uint)ItemTypes.RestaurantCup:
                case (uint)ItemTypes.RestaurantPlate:
                case (uint)ItemTypes.Halloween2016Eyes:
                case (uint)ItemTypes.Halloween2016Rotatable:
                case (uint)ItemTypes.Halloween2016Pumpkin:
                case (uint)ItemTypes.HalfBlockChristmas2016PresentRed:
                case (uint)ItemTypes.HalfBlockChristmas2016PresentGreen:
                case (uint)ItemTypes.HalfBlockChristmas2016PresentWhite:
                case (uint)ItemTypes.HalfBlockChristmas2016PresentBlue:
                case (uint)ItemTypes.HalfBlockChristmas2016PresentYellow:
                case (uint)ItemTypes.Christmas2016LightsDown:
                case (uint)ItemTypes.Christmas2016LightsUp:
                case (uint)ItemTypes.EffectGravity:
                case (uint)ItemTypes.IndustrialPipeThick:
                case (uint)ItemTypes.IndustrialPipeThin:
                case (uint)ItemTypes.IndustrialTable:
                case (uint)ItemTypes.DomesticPipeStraight:
                case (uint)ItemTypes.DomesticPipeT:
                case (uint)ItemTypes.DomesticFrameBorder:
                    this.Rotation = b.Number;
                    break;

                case (uint)ItemTypes.TextSign:
                    this.Text = b.Text;
                    this.SignType = b.SignType;
                    break;
            }
        }

        public DBBrick(BackgroundBlock b)
        {
            this.Type = b.Type;
        }

        public bool EqualForegroundBrickValues(ForegroundBlock b)
        {
            if (b.Type != this.Type)
            {
                return false;
            }

            switch (b.Type)
            {
                case (uint)ItemTypes.CoinDoor:
                case (uint)ItemTypes.CoinGate:
                case (uint)ItemTypes.BlueCoinDoor:
                case (uint)ItemTypes.BlueCoinGate:
                case (uint)ItemTypes.DeathDoor:
                case (uint)ItemTypes.DeathGate:
                case (uint)ItemTypes.SwitchPurple:
                case (uint)ItemTypes.DoorPurple:
                case (uint)ItemTypes.GatePurple:
                case (uint)ItemTypes.EffectTeam:
                case (uint)ItemTypes.TeamDoor:
                case (uint)ItemTypes.TeamGate:
                case (uint)ItemTypes.EffectCurse:
                case (uint)ItemTypes.EffectZombie:
                case (uint)ItemTypes.EffectFly:
                case (uint)ItemTypes.EffectJump:
                case (uint)ItemTypes.EffectProtection:
                case (uint)ItemTypes.EffectRun:
                case (uint)ItemTypes.EffectLowGravity:
                case (uint)ItemTypes.EffectMultijump:
                case (uint)ItemTypes.SwitchOrange:
                case (uint)ItemTypes.DoorOrange:
                case (uint)ItemTypes.GateOrange:
                    return this.Goal == b.Number;

                case (uint)ItemTypes.EffectGravity:
                    return this.Rotation == b.Number;

                case (uint)ItemTypes.Portal:
                case (uint)ItemTypes.PortalInvisible:
                    return this.Id == b.PortalId && this.Rotation == b.PortalRotation && this.Target == b.PortalTarget;

                case (uint)ItemTypes.WorldPortal:
                    return this.TargetWorld.Equals(b.Text);

                case (uint)ItemTypes.Spike:
                case (uint)ItemTypes.GlowyLineBlueSlope:
                case (uint)ItemTypes.GlowyLineBlueStraight:
                case (uint)ItemTypes.GlowyLineGreenSlope:
                case (uint)ItemTypes.GlowyLineGreenStraight:
                case (uint)ItemTypes.GlowyLineYellowSlope:
                case (uint)ItemTypes.GlowyLineYellowStraight:
                case (uint)ItemTypes.GlowyLineRedSlope:
                case (uint)ItemTypes.GlowyLineRedStraight:
                case (uint)ItemTypes.OnewayCyan:
                case (uint)ItemTypes.OnewayOrange:
                case (uint)ItemTypes.OnewayYellow:
                case (uint)ItemTypes.OnewayPink:
                case (uint)ItemTypes.OnewayGray:
                case (uint)ItemTypes.OnewayBlue:
                case (uint)ItemTypes.OnewayRed:
                case (uint)ItemTypes.OnewayGreen:
                case (uint)ItemTypes.OnewayBlack:
                case (uint)ItemTypes.OnewayWhite:
                case (uint)ItemTypes.MedievalAxe:
                case (uint)ItemTypes.MedievalBanner:
                case (uint)ItemTypes.MedievalCoatOfArms:
                case (uint)ItemTypes.MedievalShield:
                case (uint)ItemTypes.MedievalSword:
                case (uint)ItemTypes.MedievalTimber:
                case (uint)ItemTypes.ToothBig:
                case (uint)ItemTypes.ToothSmall:
                case (uint)ItemTypes.ToothTriple:
                case (uint)ItemTypes.DojoLightLeft:
                case (uint)ItemTypes.DojoLightRight:
                case (uint)ItemTypes.DojoDarkLeft:
                case (uint)ItemTypes.DojoDarkRight:
                case (uint)ItemTypes.DomesticLightBulb:
                case (uint)ItemTypes.DomesticTap:
                case (uint)ItemTypes.DomesticPainting:
                case (uint)ItemTypes.DomesticVase:
                case (uint)ItemTypes.DomesticTv:
                case (uint)ItemTypes.DomesticWindow:
                case (uint)ItemTypes.HalfBlockDomesticBrown:
                case (uint)ItemTypes.HalfBlockDomesticWhite:
                case (uint)ItemTypes.HalfBlockDomesticYellow:
                case (uint)ItemTypes.Halloween2015WindowRect:
                case (uint)ItemTypes.Halloween2015WindowCircle:
                case (uint)ItemTypes.Halloween2015Lamp:
                case (uint)ItemTypes.NewYear2015Balloon:
                case (uint)ItemTypes.NewYear2015Streamer:
                case (uint)ItemTypes.FairytaleFlowers:
                case (uint)ItemTypes.HalfBlockFairytaleOrange:
                case (uint)ItemTypes.HalfBlockFairytaleGreen:
                case (uint)ItemTypes.HalfBlockFairytaleBlue:
                case (uint)ItemTypes.HalfBlockFairytalePink:
                case (uint)ItemTypes.SpringDaisy:
                case (uint)ItemTypes.SpringTulip:
                case (uint)ItemTypes.SpringDaffodil:
                case (uint)ItemTypes.SummerIceCream:
                case (uint)ItemTypes.SummerFlag:
                case (uint)ItemTypes.SummerAwning:
                case (uint)ItemTypes.CaveCrystal:
                case (uint)ItemTypes.RestaurantBowl:
                case (uint)ItemTypes.RestaurantCup:
                case (uint)ItemTypes.RestaurantPlate:
                case (uint)ItemTypes.Halloween2016Eyes:
                case (uint)ItemTypes.Halloween2016Rotatable:
                case (uint)ItemTypes.Halloween2016Pumpkin:
                case (uint)ItemTypes.HalfBlockChristmas2016PresentRed:
                case (uint)ItemTypes.HalfBlockChristmas2016PresentGreen:
                case (uint)ItemTypes.HalfBlockChristmas2016PresentWhite:
                case (uint)ItemTypes.HalfBlockChristmas2016PresentBlue:
                case (uint)ItemTypes.HalfBlockChristmas2016PresentYellow:
                case (uint)ItemTypes.Christmas2016LightsDown:
                case (uint)ItemTypes.Christmas2016LightsUp:
                case (uint)ItemTypes.HalfBlockWhite:
                case (uint)ItemTypes.HalfBlockGray:
                case (uint)ItemTypes.HalfBlockBlack:
                case (uint)ItemTypes.HalfBlockRed:
                case (uint)ItemTypes.HalfBlockOrange:
                case (uint)ItemTypes.HalfBlockYellow:
                case (uint)ItemTypes.HalfBlockGreen:
                case (uint)ItemTypes.HalfBlockCyan:
                case (uint)ItemTypes.HalfBlockBlue:
                case (uint)ItemTypes.HalfBlockPurple:
                case (uint)ItemTypes.IndustrialPipeThick:
                case (uint)ItemTypes.IndustrialPipeThin:
                case (uint)ItemTypes.IndustrialTable:
                case (uint)ItemTypes.DomesticPipeStraight:
                case (uint)ItemTypes.DomesticPipeT:
                case (uint)ItemTypes.DomesticFrameBorder:
                    return this.Rotation == b.Number;

                case (uint)ItemTypes.Label:
                    return this.Text.Equals(b.Text) && this.TextColor.Equals(b.Color) && this.Goal == b.WrapLength;

                case (uint)ItemTypes.Piano:
                case (uint)ItemTypes.Drums:
                case (uint)ItemTypes.Guitar:
                    return this.Id.Equals(b.Number);

                case (uint)ItemTypes.TextSign:
                    return this.Text.Equals(b.Text) && this.SignType.Equals(b.SignType);
            }

            return true;
        }

        public bool EqualBackgroundBrickValues(BackgroundBlock b)
        {
            return b.Type == this.Type;
        }
    }
}
