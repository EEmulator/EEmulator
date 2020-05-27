using System;
using System.Collections.Generic;
using System.Linq;
using EverybodyEdits.Common;
using EverybodyEdits.Game.AntiCheat;
using EverybodyEdits.Game.CountWorld;
using EverybodyEdits.Game.Crews;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game
{
    public class World
    {
        private readonly Client client;
        private CountWorld.CountWorld data;
        private DatabaseObject dbo;
        private string forcedKey;
        private int spawnOffset;

        public World(Client c)
        {
            this.client = c;
            this.data = new CountWorld.CountWorld(this.Width, this.Height);
            this.Reset();
        }

        public AntiCheatData AntiCheatData { get; private set; }

        public string Key {
            get {
                if (this.dbo != null) {
                    return this.dbo.Key;
                }
                return this.forcedKey ?? "";
            }
            set { this.forcedKey = value; }
        }

        public int Type {
            get { return this.dbo != null ? this.dbo.GetInt("type", 0) : 0; }
            set {
                if (this.dbo != null) {
                    this.dbo.Set("type", value);
                }
            }
        }

        public int Width {
            get {
                // Default width is also the width of an open world
                return this.dbo != null ? this.dbo.GetInt("width", 200) : 200;
            }
        }

        public int Height {
            get {
                // Default height is also the height of an open world
                return this.dbo != null ? this.dbo.GetInt("height", 200) : 200;
            }
        }

        public int Plays {
            get { return this.dbo != null ? this.dbo.GetInt("plays", 0) : 0; }
            set {
                if (this.dbo != null) {
                    this.dbo.Set("plays", value);
                }
            }
        }

        public string OwnerId {
            get { return this.dbo != null ? this.dbo.GetString("owner", "") : ""; }
        }

        public string Name {
            get { return this.dbo != null ? this.dbo.GetString("name", "Untitled World") : "Untitled World"; }
            set {
                if (this.dbo != null) {
                    this.dbo.Set("name", value);
                }
            }
        }

        public bool Visible {
            get { return this.dbo == null || this.dbo.GetBool("visible", true); }
            set {
                if (this.dbo != null) {
                    this.dbo.Set("visible", value);
                }
            }
        }

        public bool FriendsOnly {
            get { return this.dbo != null && this.dbo.GetBool("friendsOnly", false); }
            set {
                if (this.dbo != null) {
                    this.dbo.Set("friendsOnly", value);
                }
            }
        }

        public bool HideLobby {
            get { return this.dbo != null && this.dbo.GetBool("HideLobby", false); }
            set {
                if (this.dbo != null) {
                    this.dbo.Set("HideLobby", value);
                }
            }
        }

        public bool AllowSpectating {
            get { return this.dbo == null || this.dbo.GetBool("allowSpectating", true); }
            set {
                if (this.dbo != null) {
                    this.dbo.Set("allowSpectating", value);
                }
            }
        }

        public int CurseLimit {
            get { return this.dbo != null ? this.dbo.GetInt("curseLimit", 0) : 0; }
            set {
                if (this.dbo != null) {
                    this.dbo.Set("curseLimit", value);
                }
            }
        }

        public int ZombieLimit {
            get { return this.dbo != null ? this.dbo.GetInt("zombieLimit", 0) : 0; }
            set {
                if (this.dbo != null) {
                    this.dbo.Set("zombieLimit", value);
                }
            }
        }

        public string Campaign {
            get { return this.dbo != null ? this.dbo.GetString("Campaign", "") : ""; }
        }

        public bool IsPartOfCampaign {
            get { return this.Campaign != ""; }
        }

        public string Crew {
            get { return this.dbo != null ? this.dbo.GetString("Crew", "") : ""; }
            set {
                if (this.dbo != null) {
                    this.dbo.Set("Crew", value);
                }
            }
        }

        public bool IsPartOfCrew {
            get { return this.Crew != ""; }
        }

        public bool IsCrewLogo {
            get { return this.dbo != null && this.dbo.GetBool("IsCrewLogo", false); }
        }

        public WorldStatus Status {
            get {
                if (this.dbo != null) {
                    return (WorldStatus)this.dbo.GetInt("Status", 1);
                }
                return WorldStatus.Open;
            }
            set {
                if (this.dbo != null) {
                    if (value == WorldStatus.NonCrew) {
                        this.dbo.Remove("Status");
                    }
                    else {
                        this.dbo.Set("Status", (int)value);
                    }
                }
            }
        }

        public bool CrewVisibleInLobby {
            get { return !IsCrewLogo && Status != WorldStatus.Wip; }
        }

        public string WorldDescription {
            get { return this.dbo != null ? this.dbo.GetString("worldDescription", "") : ""; }
            set {
                if (this.dbo != null) {
                    this.dbo.Set("worldDescription", value);
                }
            }
        }

        public bool IsArtContest
        {
            get { return this.dbo != null && this.dbo.GetBool("IsArtContest", false); }
            set {
                if (this.dbo != null) {
                    this.dbo.Set("IsArtContest", value);
                }
            }
        }

        public bool IsFeatured {
            get { return this.dbo != null && this.dbo.GetBool("IsFeatured", false); }
        }

        public double GravityMultiplier {
            get {
                if (this.dbo != null && this.dbo.Contains("Gravity")) {
                    return this.dbo.GetDouble("Gravity", 1);
                }
                return this.Type == (int)WorldTypes.MoonLarge ? 0.16 : 1;
            }
        }

        public uint BackgroundColor {
            get { return this.dbo != null ? this.dbo.GetUInt("backgroundColor", 0x00000000) : 0x00000000; }
            set {
                if (this.dbo != null) {
                    this.dbo.Set("backgroundColor", value);
                }
            }
        }

        public uint BorderType {
            get {
                if (this.dbo != null && this.dbo.Contains("BorderType")) {
                    return (uint)this.dbo.GetInt("BorderType", 9);
                }
                switch (this.Type) {
                    case (int)WorldTypes.MoonLarge: {
                            return 182;
                        }
                    default: {
                            return 9;
                        }
                }
            }
        }

        public uint FillType {
            get {
                switch (this.Type) {
                    default: {
                            return 0;
                        }
                }
            }
        }

        public int Favorites {
            get { return this.dbo != null ? this.dbo.GetInt("Favorites", 0) : 0; }
            set {
                if (this.dbo != null) {
                    this.dbo.Set("Favorites", value);
                }
            }
        }

        public int Likes {
            get { return this.dbo != null ? this.dbo.GetInt("Likes", 0) : 0; }
            set {
                if (this.dbo != null) {
                    this.dbo.Set("Likes", value);
                }
            }
        }

        public bool MinimapEnabled {
            get { return this.dbo == null || this.dbo.GetBool("MinimapEnabled", true); }
            set { if (this.dbo != null) this.dbo.Set("MinimapEnabled", value); }
        }

        public bool LobbyPreviewEnabled {
            get { return this.dbo == null || this.dbo.GetBool("LobbyPreviewEnabled", true); }
            set { if (this.dbo != null) this.dbo.Set("LobbyPreviewEnabled", value); }
        }

        public DatabaseObject GetDatabaseObject()
        {
            return this.dbo;
        }

        public void FromDatabaseObject(DatabaseObject db)
        {
            this.dbo = db;
            AntiCheatData.GetAntiCheatData(this.client, this.dbo.Key, value => { this.AntiCheatData = value; });

            if (this.dbo.Contains("hidelobby")) {
                if (!this.dbo.Contains("HideLobby"))
                    this.HideLobby = this.dbo.GetBool("hidelobby");
                this.dbo.Remove("hidelobby");
            }

            // If world is created before 2012-08-30, it does not contain type. 
            if (!dbo.Contains("type")) {
                if (this.Width == 25 && this.Height == 25) {
                    this.Type = (int)WorldTypes.Small;
                }
                else if (this.Width == 50 && this.Height == 50) {
                    this.Type = (int)WorldTypes.Medium;
                }
                else if (this.Width == 100 && this.Height == 100) {
                    this.Type = (int)WorldTypes.Large;
                }
                else if (this.Width == 200 && this.Height == 200) {
                    this.Type = (int)WorldTypes.Massive;
                }
                else if (this.Width == 400 && this.Height == 50) {
                    this.Type = (int)WorldTypes.Wide;
                }
                else if (this.Width == 400 && this.Height == 200) {
                    this.Type = (int)WorldTypes.Great;
                }
                else if (this.Width == 100 && this.Height == 400) {
                    this.Type = (int)WorldTypes.Tall;
                }
                else if (this.Width == 636 && this.Height == 50) {
                    this.Type = (int)WorldTypes.UltraWide;
                }
                else if (this.Width == 150 && this.Height == 25) {
                    this.Type = (int)WorldTypes.Tutorial;
                }
                else if (this.Width == 110 && this.Height == 110) {
                    this.Type = (int)WorldTypes.MoonLarge;
                }
                else if (this.Width == 300 && this.Height == 300) {
                    this.Type = (int)WorldTypes.Huge;
                }
                else if (this.Width == 200 && this.Height == 400) {
                    this.Type = (int)WorldTypes.VerticalGreat;
                }
                else if (this.Width == 150 && this.Height == 150) {
                    this.Type = (int)WorldTypes.Big;
                }
            }

            //Create new uninitalized world object
            this.data = new CountWorld.CountWorld(this.Width, this.Height);

            //Load bytes from db. (legacy format, obsolete but maybe still used on some levels)
            var worlddata = dbo.GetArray("worlddata");

            if (worlddata != null) {
                this.UnserializeFromComplexObject(worlddata);
            }
            else {
                this.Reset();
                var worldbytes = dbo.GetBytes("world", null);
                if (worldbytes != null) {
                    this.UnserializeWorldFromBytes(worldbytes);
                }
            }
            /*
            loadWorldPresentation();
            */
        }

        private void UnserializeFromComplexObject(DatabaseArray worlddata)
        {
            for (var a = 0; a < worlddata.Count; a++) {
                if (!worlddata.Contains(a) || worlddata.GetObject(a).Count == 0) {
                    continue;
                }
                var ct = worlddata.GetObject(a);
                var type = (uint)ct.GetValue("type");
                var layerNum = ct.GetInt("layer", 0);
                var xs = ct.GetBytes("x", new byte[0]);
                var ys = ct.GetBytes("y", new byte[0]);
                var x1S = ct.GetBytes("x1", new byte[0]);
                var y1S = ct.GetBytes("y1", new byte[0]);

                for (var b = 0; b < x1S.Length; b++) {
                    var nx = x1S[b];
                    var ny = y1S[b];

                    this.SetBlock(ct, nx, ny, type, layerNum);
                }

                for (var b = 0; b < xs.Length; b += 2) {
                    var nx = (uint)((xs[b] << 8) + xs[b + 1]);
                    var ny = (uint)((ys[b] << 8) + ys[b + 1]);

                    this.SetBlock(ct, nx, ny, type, layerNum);
                }
            }
        }

        private void SetBlock(DatabaseObject ct, uint nx, uint ny, uint type, int layerNum)
        {
            switch (type) {
                case (uint)ItemTypes.CoinDoor: {
                        this.SetBrickCoindoor(
                            nx,
                            ny,
                            (uint)ct.GetValue("goal"));
                        break;
                    }

                case (uint)ItemTypes.CoinGate: {
                        this.SetBrickCoingate(
                            nx,
                            ny,
                            (uint)ct.GetValue("goal"));
                        break;
                    }

                case (uint)ItemTypes.BlueCoinDoor: {
                        this.SetBrickBlueCoindoor(
                            nx,
                            ny,
                            (uint)ct.GetValue("goal"));
                        break;
                    }

                case (uint)ItemTypes.BlueCoinGate: {
                        this.SetBrickBlueCoingate(
                            nx,
                            ny,
                            (uint)ct.GetValue("goal"));
                        break;
                    }

                case (uint)ItemTypes.DeathDoor: {
                        this.SetBrickDeathDoor(
                            nx,
                            ny,
                            (uint)ct.GetValue("goal"));
                        break;
                    }

                case (uint)ItemTypes.DeathGate: {
                        this.SetBrickDeathGate(
                            nx,
                            ny,
                            (uint)ct.GetValue("goal"));
                        break;
                    }

                case (uint)ItemTypes.DoorPurple: {
                        this.SetBrickDoorPurple(
                            nx,
                            ny,
                            ct.GetUInt("goal", 0));
                        break;
                    }
                case (uint)ItemTypes.GatePurple: {
                        this.SetBrickGatePurple(
                            nx,
                            ny,
                            ct.GetUInt("goal", 0));
                        break;
                    }
                case (uint)ItemTypes.SwitchPurple: {
                        this.SetBrickSwitchPurple(
                            nx,
                            ny,
                            ct.GetUInt("goal", 0));
                        break;
                    }

                case (uint)ItemTypes.DoorOrange: {
                        this.SetBrickDoorOrange(
                            nx,
                            ny,
                            ct.GetUInt("goal", 0));
                        break;
                    }
                case (uint)ItemTypes.GateOrange: {
                        this.SetBrickGateOrange(
                            nx,
                            ny,
                            ct.GetUInt("goal", 0));
                        break;
                    }
                case (uint)ItemTypes.SwitchOrange: {
                        this.SetBrickSwitchOrange(
                            nx,
                            ny,
                            ct.GetUInt("goal", 0));
                        break;
                    }

                case (uint)ItemTypes.EffectTeam: {
                        this.SetBrickTeamEffect(
                            nx,
                            ny,
                            ct.GetUInt("goal", 10));
                        break;
                    }
                case (uint)ItemTypes.TeamDoor: {
                        this.SetBrickTeamDoor(
                            nx,
                            ny,
                            ct.GetUInt("goal", 0));
                        break;
                    }
                case (uint)ItemTypes.TeamGate: {
                        this.SetBrickTeamGate(
                            nx,
                            ny,
                            ct.GetUInt("goal", 0));
                        break;
                    }

                case (uint)ItemTypes.EffectCurse:
                case (uint)ItemTypes.EffectZombie: {
                        this.SetBrickWithDuration(
                            type,
                            nx,
                            ny,
                            ct.GetUInt("goal", 0));
                        break;
                    }

                case (uint)ItemTypes.EffectFly:
                case (uint)ItemTypes.EffectJump:
                case (uint)ItemTypes.EffectRun:
                case (uint)ItemTypes.EffectProtection:
                case (uint)ItemTypes.EffectLowGravity: {
                        this.SetBrickWithOnStatus(
                            type,
                            nx,
                            ny,
                            ct.GetUInt("goal", 0));
                        break;
                    }

                case (uint)ItemTypes.EffectMultijump: {
                        this.SetBrickMultijump(type, nx, ny, ct.GetUInt("goal", 0));
                        break;
                    }

                case (uint)ItemTypes.PortalInvisible:
                case (uint)ItemTypes.Portal: {
                        this.SetBrickPortal(
                            type,
                            nx,
                            ny,
                            ct.GetUInt("rotation", 0),
                            ct.GetUInt("id", 0),
                            ct.GetUInt("target", 0));
                        break;
                    }

                case (uint)ItemTypes.WorldPortal: {
                        this.SetBrickWorldPortal(
                            nx,
                            ny,
                            ct.GetString("target", ""));
                        break;
                    }

                case (uint)ItemTypes.GlowyLineBlueStraight:
                case (uint)ItemTypes.GlowyLineBlueSlope:
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
                case (uint)ItemTypes.Spike:
                case (uint)ItemTypes.MedievalAxe:
                case (uint)ItemTypes.MedievalBanner:
                case (uint)ItemTypes.MedievalCoatOfArms:
                case (uint)ItemTypes.MedievalShield:
                case (uint)ItemTypes.MedievalSword:
                case (uint)ItemTypes.ToothBig:
                case (uint)ItemTypes.ToothSmall:
                case (uint)ItemTypes.ToothTriple:
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
                case (uint)ItemTypes.RestaurantBowl:
                case (uint)ItemTypes.RestaurantCup:
                case (uint)ItemTypes.RestaurantPlate:
                case (uint)ItemTypes.Halloween2016Eyes:
                case (uint)ItemTypes.Halloween2016Rotatable:
                case (uint)ItemTypes.Halloween2016Pumpkin:
                case (uint)ItemTypes.EffectGravity: {
                        this.SetBrickRotateable(
                            layerNum,
                            nx,
                            ny,
                            type,
                            ct.GetUInt("rotation", 0), 5);
                        break;
                    }

                case (uint)ItemTypes.HalfBlockChristmas2016PresentRed:
                case (uint)ItemTypes.HalfBlockChristmas2016PresentGreen:
                case (uint)ItemTypes.HalfBlockChristmas2016PresentWhite:
                case (uint)ItemTypes.HalfBlockChristmas2016PresentBlue:
                case (uint)ItemTypes.HalfBlockChristmas2016PresentYellow: {
                        this.SetBrickRotateable(
                            layerNum,
                            nx,
                            ny,
                            type,
                            ct.GetUInt("rotation", 1u));
                        break;
                    }

                case (uint)ItemTypes.NewYear2015Balloon:
                case (uint)ItemTypes.NewYear2015Streamer:
                case (uint)ItemTypes.Christmas2016LightsDown:
                case (uint)ItemTypes.Christmas2016LightsUp: {
                        this.SetBrickRotateable(layerNum, nx, ny, type, ct.GetUInt("rotation", 0), 5);
                        break;
                    }

                case (uint)ItemTypes.MedievalTimber:
                case (uint)ItemTypes.SummerFlag:
                case (uint)ItemTypes.SummerAwning:
                case (uint)ItemTypes.CaveCrystal: {
                        this.SetBrickRotateable(layerNum, nx, ny, type, ct.GetUInt("rotation", 0), 6);
                        break;
                    }

                case (uint)ItemTypes.DojoLightLeft:
                case (uint)ItemTypes.DojoLightRight:
                case (uint)ItemTypes.DojoDarkLeft:
                case (uint)ItemTypes.DojoDarkRight:
                case (uint)ItemTypes.IndustrialTable: {
                        this.SetBrickRotateable(layerNum, nx, ny, type, ct.GetUInt("rotation", 0), 3);
                        break;
                    }

                case (uint)ItemTypes.IndustrialPipeThick:
                case (uint)ItemTypes.IndustrialPipeThin:
                case (uint)ItemTypes.DomesticPipeStraight: {
                        this.SetBrickRotateable(layerNum, nx, ny, type, ct.GetUInt("rotation", 0), 2);
                        break;
                    }

                case (uint)ItemTypes.DomesticPipeT: {
                        this.SetBrickRotateable(layerNum, nx, ny, type, ct.GetUInt("rotation", 0), 4);
                        break;
                    }

                case (uint)ItemTypes.DomesticFrameBorder: {
                        this.SetBrickRotateable(layerNum, nx, ny, type, ct.GetUInt("rotation", 0), 11);
                        break;
                    }

                case 1000: {
                        this.SetBrickLabel(
                            nx,
                            ny,
                            ct.GetString("text", "no text found"),
                            ct.GetString("text_color", "#FFFFFF"),
                            ct.GetUInt("wrapLength", 200));
                        break;
                    }

                case (int)ItemTypes.Piano: {
                        this.SetBrickSound(
                            ItemTypes.Piano,
                            nx,
                            ny,
                            (int)ct.GetUInt("id", 0)
                            );
                        break;
                    }

                case (int)ItemTypes.Drums: {
                        this.SetBrickSound(
                            ItemTypes.Drums,
                            nx,
                            ny,
                            (int)ct.GetUInt("id", 0)
                            );
                        break;
                    }

                case (int)ItemTypes.Complete: {
                        this.SetBrickComplete(nx,
                            ny);
                        break;
                    }
                case (int)ItemTypes.TextSign: {
                        this.SetBrickTextSign(nx, ny, ct.GetString("text", "No text found."), ct.GetUInt("signtype", 0));
                        break;
                    }

                case (int)ItemTypes.Guitar: {
                        this.SetBrickSound(
                            ItemTypes.Guitar,
                            nx,
                            ny,
                            (int)ct.GetUInt("id", 0)
                            );
                        break;
                    }

                default: {
                        this.SetNormal(layerNum, nx, ny, type);
                        break;
                    }
            }
        }

        public void Save(bool saveworlddata, Callback callback = null)
        {
            if (saveworlddata) {
                this.SaveWorldData();
            }

            if (this.dbo == null) {
                return;
            }

            this.dbo.Save(callback);

            if (IsPartOfCampaign && this.AntiCheatData != null) {
                this.AntiCheatData.Save();
            }
        }

        private void SaveWorldData()
        {
            if (this.dbo == null) {
                return;
            }

            var bricks = this.GetForegroundBrickList(); // Get a list og foreground bricks...
            bricks.AddRange(this.GetBackgroundBrickList()); // ...add the list of background bricks

            var worlddata = new DatabaseArray();

            foreach (var b in bricks) {
                var cb = new DatabaseObject();

                var l = b.Xs.Count;

                var xs = new List<byte>(l);
                var ys = new List<byte>(l);
                var x1S = new List<byte>(l);
                var y1S = new List<byte>(l);

                for (var a = 0; a < l; a++) {
                    var x = b.Xs[a];
                    var y = b.Ys[a];
                    if (x < 256 && y < 256) {
                        x1S.Add((byte)x);
                        y1S.Add((byte)y);
                    }
                    else {
                        xs.Add((byte)((x & 0x0000ff00) >> 8));
                        xs.Add((byte)(x & 0x000000ff));
                        ys.Add((byte)((y & 0x0000ff00) >> 8));
                        ys.Add((byte)(y & 0x000000ff));
                    }
                }

                cb.Set("type", b.Type);
                cb.Set("layer", b.Layer);
                if (xs.Count > 0) {
                    cb.Set("x", xs.ToArray());
                }
                if (ys.Count > 0) {
                    cb.Set("y", ys.ToArray());
                }
                if (x1S.Count > 0) {
                    cb.Set("x1", x1S.ToArray());
                }
                if (y1S.Count > 0) {
                    cb.Set("y1", y1S.ToArray());
                }

                switch (b.Type) {
                    // Coin door
                    case (uint)ItemTypes.CoinDoor:
                    case (uint)ItemTypes.CoinGate:
                    case (uint)ItemTypes.BlueCoinDoor:
                    case (uint)ItemTypes.BlueCoinGate:
                    case (uint)ItemTypes.DeathDoor:
                    case (uint)ItemTypes.DeathGate:
                    case (uint)ItemTypes.SwitchPurple:
                    case (uint)ItemTypes.GatePurple:
                    case (uint)ItemTypes.DoorPurple:
                    case (uint)ItemTypes.EffectTeam:
                    case (uint)ItemTypes.TeamDoor:
                    case (uint)ItemTypes.TeamGate:
                    case (uint)ItemTypes.EffectCurse:
                    case (uint)ItemTypes.EffectFly:
                    case (uint)ItemTypes.EffectJump:
                    case (uint)ItemTypes.EffectRun:
                    case (uint)ItemTypes.EffectProtection:
                    case (uint)ItemTypes.EffectZombie:
                    case (uint)ItemTypes.EffectLowGravity:
                    case (uint)ItemTypes.EffectMultijump:
                    case (uint)ItemTypes.SwitchOrange:
                    case (uint)ItemTypes.GateOrange:
                    case (uint)ItemTypes.DoorOrange: {
                            cb.Set("goal", b.Goal);
                            break;
                        }

                    // Portals
                    case (uint)ItemTypes.PortalInvisible:
                    case (uint)ItemTypes.Portal: {
                            cb.Set("rotation", b.Rotation);
                            cb.Set("id", b.Id);
                            cb.Set("target", b.Target);
                            break;
                        }

                    // World Portals
                    case (uint)ItemTypes.WorldPortal: {
                            cb.Set("target", b.TargetWorld);
                            break;
                        }


                    // Spikes
                    case (uint)ItemTypes.Spike:
                    case (uint)ItemTypes.GlowyLineBlueStraight:
                    case (uint)ItemTypes.GlowyLineBlueSlope:
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
                    case (uint)ItemTypes.SummerFlag:
                    case (uint)ItemTypes.SummerAwning:
                    case (uint)ItemTypes.SummerIceCream:
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
                    case (uint)ItemTypes.DomesticFrameBorder: {
                            cb.Set("rotation", b.Rotation);
                            break;
                        }

                    case (uint)ItemTypes.Label: {
                            cb.Set("text", b.Text);
                            cb.Set("text_color", b.TextColor);
                            cb.Set("wrapLength", b.Goal);
                            break;
                        }

                    case (uint)ItemTypes.TextSign: {
                            cb.Set("text", b.Text);
                            cb.Set("signtype", b.SignType);
                            break;
                        }

                    case (uint)ItemTypes.Piano: {
                            cb.Set("id", b.Id);
                            break;
                        }

                    case (uint)ItemTypes.Drums: {
                            cb.Set("id", b.Id);
                            break;
                        }

                    case (uint)ItemTypes.Guitar: {
                            cb.Set("id", b.Id);
                            break;
                        }
                }
                worlddata.Add(cb);
            }

            this.dbo.Set("worlddata", worlddata);


            //Remove old database definition;
            if (this.dbo.Contains("world")) {
                this.dbo.Remove("world");
            }
        }


        private List<DBBrick> GetForegroundBrickList()
        {
            var dbbricks = new List<DBBrick>();
            var layerData = this.data.Foreground.Clone();

            for (uint y = 0; y < this.Height; y++) {
                for (uint x = 0; x < this.Width; x++) {
                    var cur = layerData[x, y];
                    // Set the presentation camera properties (using the spike tile as a camera for now)
                    /*
                    if (cur.type == (uint)ItemTypes.Spike)
                    {
                        worldPresentation.StartX = x;
                        worldPresentation.StartY = y;
                    }
                    */
                    if (cur.Type == 0) {
                        continue;
                    }

                    // Do we have an existing instance of this type (with exact same attributes)?
                    // (Attributes are only stored pr. object list in the DB, so treat i.e. coin doors with different goals as different types)
                    var dbb = dbbricks.FirstOrDefault(b => b.EqualForegroundBrickValues(cur));

                    if (dbb == null) {
                        dbb = new DBBrick(cur) { Layer = 0 };
                        // Layer
                        dbbricks.Add(dbb);
                    }

                    dbb.Xs.Add(x);
                    dbb.Ys.Add(y);
                }
            }

            return dbbricks;
        }

        private IEnumerable<DBBrick> GetBackgroundBrickList()
        {
            var dbbricks = new List<DBBrick>();
            var layerData = this.data.Background.Clone();

            for (uint y = 0; y < this.Height; y++) {
                for (uint x = 0; x < this.Width; x++) {
                    var cur = layerData[x, y];

                    // Set the presentation camera properties (using the spike tile as a camera for now)
                    /*
                    if (cur.type == (uint)ItemTypes.Spike)
                    {
                        worldPresentation.StartX = x;
                        worldPresentation.StartY = y;
                    }
                    */
                    if (cur.Type == 0) {
                        continue;
                    }

                    // Do we have an existing instance of this type (with exact same attributes)?
                    // (Attributes are only stored pr. object list in the DB, so treat i.e. coin doors with different goals as different types)
                    var dbb = dbbricks.FirstOrDefault(b => b.EqualBackgroundBrickValues(cur));

                    if (dbb == null) {
                        dbb = new DBBrick(cur) { Layer = 1 };
                        // Layer
                        dbbricks.Add(dbb);
                    }

                    dbb.Xs.Add(x);
                    dbb.Ys.Add(y);
                }
            }

            return dbbricks;
        }

        // gather all text signs and return them - this is used by the report command to document the current text of a sign
        // This is only invoked when the command is executed
        public List<DBBrick> GetBrickTextSignList()
        {
            var bricks = this.GetForegroundBrickList(); // signs are always in layer 0
            return bricks.Where(b => b.Type == (int)ItemTypes.TextSign).ToList();
        }

        public Item GetSpawn()
        {
            return this.SpawnCount > 0
                ? this.data.Foreground.Spawns[++this.spawnOffset % this.SpawnCount]
                : new Item(this.data.Foreground[1, 1], 1, 1);
        }

        public void Reset()
        {
            this.data = WorldUtils.GetClearedWorld(this.Width, this.Height, this.BorderType);
        }

        public void Reload()
        {
            this.FromDatabaseObject(this.dbo);
        }

        public uint GetBrickType(int layerNum, uint x, uint y)
        {
            try {
                return layerNum == 0 ? this.data.Foreground[x, y].Type : this.data.Background[x, y].Type;
            }
            catch (Exception e) {
                this.client.ErrorLog.WriteError("Unable to read value from world " + x + " " + y, e);
                return 0;
            }
        }

        public ForegroundBlock GetForeground(uint x, uint y)
        {
            return this.data.Foreground[x, y];
        }

        private void UnserializeWorldFromBytes(byte[] wd)
        {
            for (var y = 0U; y < this.Height; y++) {
                for (var x = 0U; x < this.Width; x++) {
                    this.data.Foreground[x, y] = new ForegroundBlock(wd[y * this.Width + x]);
                }
            }
        }

        // Serializa world data, and add it to a message. 
        // Used for sending the entire world to a new player when connecting
        public void AddToMessageAsComplexList(Message m)
        {
            m.Add("ws");

            var bricks = this.GetForegroundBrickList();
            var bricksBg = this.GetBackgroundBrickList();

            bricks.AddRange(bricksBg);

            foreach (var b in bricks) {
                m.Add(b.Type);
                m.Add(b.Layer);

                var l = b.Xs.Count;

                var xs = new byte[l * 2];
                var ys = new byte[l * 2];

                for (var a = 0; a < l; a++) {
                    xs[a * 2] = (byte)((b.Xs[a] & 0x0000ff00) >> 8);
                    xs[a * 2 + 1] = (byte)(b.Xs[a] & 0x000000ff);

                    ys[a * 2] = (byte)((b.Ys[a] & 0x0000ff00) >> 8);
                    ys[a * 2 + 1] = (byte)(b.Ys[a] & 0x000000ff);
                }

                m.Add(xs);
                m.Add(ys);

                switch (b.Type) {
                    //Coin doors and other stuffs
                    case (uint)ItemTypes.BlueCoinDoor:
                    case (uint)ItemTypes.BlueCoinGate:
                    case (uint)ItemTypes.CoinGate:
                    case (uint)ItemTypes.CoinDoor:
                    case (uint)ItemTypes.DeathDoor:
                    case (uint)ItemTypes.DeathGate:
                    case (uint)ItemTypes.DoorPurple:
                    case (uint)ItemTypes.GatePurple:
                    case (uint)ItemTypes.SwitchPurple:
                    case (uint)ItemTypes.EffectTeam:
                    case (uint)ItemTypes.TeamDoor:
                    case (uint)ItemTypes.TeamGate:
                    case (uint)ItemTypes.EffectCurse:
                    case (uint)ItemTypes.EffectFly:
                    case (uint)ItemTypes.EffectJump:
                    case (uint)ItemTypes.EffectRun:
                    case (uint)ItemTypes.EffectProtection:
                    case (uint)ItemTypes.EffectZombie:
                    case (uint)ItemTypes.EffectLowGravity:
                    case (uint)ItemTypes.EffectMultijump:
                    case (uint)ItemTypes.SwitchOrange:
                    case (uint)ItemTypes.GateOrange:
                    case (uint)ItemTypes.DoorOrange: {
                            m.Add(b.Goal);
                            break;
                        }

                    // Portals
                    case (int)ItemTypes.PortalInvisible:
                    case (uint)ItemTypes.Portal: {
                            m.Add(b.Rotation);
                            m.Add(b.Id);
                            m.Add(b.Target);
                            break;
                        }
                    // World Portals
                    case (uint)ItemTypes.WorldPortal: {
                            m.Add(b.TargetWorld);
                            break;
                        }

                    case (uint)ItemTypes.TextSign: {
                            m.Add(b.Text);
                            m.Add(b.SignType);
                            break;
                        }

                    case (uint)ItemTypes.GlowyLineBlueStraight:
                    case (uint)ItemTypes.GlowyLineBlueSlope:
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
                    case (uint)ItemTypes.Spike:
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
                    case (uint)ItemTypes.SummerFlag:
                    case (uint)ItemTypes.SummerAwning:
                    case (uint)ItemTypes.SummerIceCream:
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
                    case (uint)ItemTypes.DomesticFrameBorder: {
                            m.Add(b.Rotation);
                            break;
                        }
                    // Labels
                    case (uint)ItemTypes.Label: {
                            m.Add(b.Text);
                            m.Add(b.TextColor);
                            m.Add(b.Goal);
                            break;
                        }

                    //Piano
                    case (uint)ItemTypes.Piano: {
                            m.Add((int)b.Id);
                            break;
                        }

                    //Piano
                    case (uint)ItemTypes.Drums: {
                            m.Add((int)b.Id);
                            break;
                        }

                    case (uint)ItemTypes.Guitar: {
                            m.Add((int)b.Id);
                            break;
                        }
                }
            }
            m.Add("we");
        }

        public List<Item> GetPortals()
        {
            return this.data.Foreground.Portals;
        }

        public List<Item> GetInvisiblePortals()
        {
            return this.data.Foreground.InvisiblePortals;
        }

        #region SetBrick

        public bool SetBrickCoindoor(uint x, uint y, uint goal)
        {
            return goal <= 999 && this.SetNumber(0, x, y, (uint)ItemTypes.CoinDoor, goal);
        }

        public bool SetBrickBlueCoindoor(uint x, uint y, uint goal)
        {
            return goal <= 999 && this.SetNumber(0, x, y, (uint)ItemTypes.BlueCoinDoor, goal);
        }

        public bool SetBrickCoingate(uint x, uint y, uint goal)
        {
            return goal <= 999 && this.SetNumber(0, x, y, (uint)ItemTypes.CoinGate, goal);
        }

        public bool SetBrickBlueCoingate(uint x, uint y, uint goal)
        {
            return goal <= 999 && this.SetNumber(0, x, y, (uint)ItemTypes.BlueCoinGate, goal);
        }

        public bool SetBrickDeathDoor(uint x, uint y, uint goal)
        {
            if (goal < 1 || goal > 999) {
                return false;
            }
            return this.SetNumber(0, x, y, (uint)ItemTypes.DeathDoor, goal);
        }

        public bool SetBrickDeathGate(uint x, uint y, uint goal)
        {
            if (goal < 1 || goal > 999) {
                return false;
            }
            return this.SetNumber(0, x, y, (uint)ItemTypes.DeathGate, goal);
        }

        public bool SetBrickDoorPurple(uint x, uint y, uint goal)
        {
            return goal <= 999 && this.SetNumber(0, x, y, (uint)ItemTypes.DoorPurple, goal);
        }

        public bool SetBrickGatePurple(uint x, uint y, uint goal)
        {
            return goal <= 999 && this.SetNumber(0, x, y, (uint)ItemTypes.GatePurple, goal);
        }

        public bool SetBrickSwitchPurple(uint x, uint y, uint goal)
        {
            return goal <= 999 && this.SetNumber(0, x, y, (uint)ItemTypes.SwitchPurple, goal);
        }

        public bool SetBrickDoorOrange(uint x, uint y, uint goal)
        {
            return goal <= 999 && this.SetNumber(0, x, y, (uint)ItemTypes.DoorOrange, goal);
        }

        public bool SetBrickGateOrange(uint x, uint y, uint goal)
        {
            return goal <= 999 && this.SetNumber(0, x, y, (uint)ItemTypes.GateOrange, goal);
        }

        public bool SetBrickSwitchOrange(uint x, uint y, uint goal)
        {
            return goal <= 999 && this.SetNumber(0, x, y, (uint)ItemTypes.SwitchOrange, goal);
        }

        public bool SetBrickTeamEffect(uint x, uint y, uint goal)
        {
            return goal <= 6 && this.SetNumber(0, x, y, (uint)ItemTypes.EffectTeam, goal);
        }

        public bool SetBrickTeamDoor(uint x, uint y, uint goal)
        {
            return goal <= 6 && this.SetNumber(0, x, y, (uint)ItemTypes.TeamDoor, goal);
        }

        public bool SetBrickTeamGate(uint x, uint y, uint goal)
        {
            return goal <= 6 && this.SetNumber(0, x, y, (uint)ItemTypes.TeamGate, goal);
        }

        public bool SetBrickWithDuration(uint brickType, uint x, uint y, uint goal)
        {
            return goal <= 999 && this.SetNumber(0, x, y, brickType, goal);
        }

        public bool SetBrickWithOnStatus(uint brickType, uint x, uint y, uint goal)
        {
            if (goal != 0 && goal != 1) {
                return false;
            }

            return this.SetNumber(0, x, y, brickType, goal);
        }

        public bool SetBrickMultijump(uint brickType, uint x, uint y, uint goal)
        {
            if (goal > 1000)
                return false;

            return this.SetNumber(0, x, y, brickType, goal);
        }

        public bool SetBrickRotateable(int layerNum, uint x, uint y, uint type, uint rotation, uint rotations = 4)
        {
            return rotation < rotations && this.SetNumber(layerNum, x, y, type, rotation);
        }

        public bool SetBrickPortal(uint brickType, uint x, uint y, uint rotation, uint id, uint target)
        {
            return rotation < 4 && this.SetPortal(x, y, brickType, rotation, id, target);
        }

        public bool SetBrickWorldPortal(uint x, uint y, string target)
        {
            return target.Length <= 50 && this.SetString(x, y, (uint)ItemTypes.WorldPortal, target);
        }

        public bool SetBrickLabel(uint x, uint y, string text, string textColor, uint wrapLength)
        {
            if (text.Length < 1) {
                return false;
            }

            return this.SetColoredText(x, y, (uint)ItemTypes.Label, text, textColor, wrapLength);
        }

        public bool SetBrickTextSign(uint x, uint y, string text, uint signType)
        {
            if (text.Length > 500) {
                return false;
            }

            Console.WriteLine("New sign! Type " + signType);

            this.SetTextSign(x, y, (uint)ItemTypes.TextSign, text, signType);
            return true;
        }

        public bool SetBrickSound(ItemTypes type, uint x, uint y, int sound)
        {
            switch (type) {
                case ItemTypes.Piano:
                    return sound <= 60 && sound >= -27 && this.SetNumber(0, x, y, (uint)type, (uint)sound);

                case ItemTypes.Drums:
                    return sound <= 19 && sound >= 0 && this.SetNumber(0, x, y, (uint)type, (uint)sound);

                case ItemTypes.Guitar:
                    return sound <= 48 && sound >= 0 && this.SetNumber(0, x, y, (uint)type, (uint)sound);

                default:
                    return true;
            }
        }

        private void SetBrickComplete(uint x, uint y)
        {
            this.SetNormal(0, x, y, (uint)ItemTypes.Complete);
        }

        private bool SetPortal(uint x, uint y, uint id, uint portalRotation, uint portalId, uint portalTarget)
        {
            if (this.data.Foreground[x, y].Type == id &&
                this.data.Foreground[x, y].PortalId == portalId &&
                this.data.Foreground[x, y].PortalTarget == portalTarget &&
                this.data.Foreground[x, y].PortalRotation == portalRotation) {
                return false;
            }

            this.data.Foreground[x, y] = new ForegroundBlock(id, portalId, portalTarget, portalRotation);
            return true;
        }

        private bool SetColoredText(uint x, uint y, uint id, string text, string color, uint wrapLength)
        {
            if (this.data.Foreground[x, y].Type == id &&
                this.data.Foreground[x, y].Text == text &&
                this.data.Foreground[x, y].Color == color &&
                this.data.Foreground[x, y].Number == wrapLength) {
                return false;
            }

            this.data.Foreground[x, y] = new ForegroundBlock(id, text, color, wrapLength);
            return true;
        }

        private bool SetTextSign(uint x, uint y, uint id, string text, uint signType)
        {
            if (this.data.Foreground[x, y].Type == id &&
                this.data.Foreground[x, y].Text == text &&
                this.data.Foreground[x, y].SignType == signType) {
                return false;
            }

            this.data.Foreground[x, y] = new ForegroundBlock(id, text, signType);
            return true;
        }

        private bool SetString(uint x, uint y, uint id, string text)
        {
            if (this.data.Foreground[x, y].Type == id && this.data.Foreground[x, y].Text == text) {
                return false;
            }

            this.data.Foreground[x, y] = new ForegroundBlock(id, text);
            return true;
        }

        private bool SetNumber(int layerNum, uint x, uint y, uint id, uint arg)
        {
            switch (layerNum) {
                case 0:
                    if (this.data.Foreground[x, y].Type == id && this.data.Foreground[x, y].Number == arg) {
                        return false;
                    }

                    this.data.Foreground[x, y] = new ForegroundBlock(id, arg);
                    return true;
                case 1:
                    if (this.data.Background[x, y].Type == id && this.data.Background[x, y].Number == arg) {
                        return false;
                    }

                    this.data.Background[x, y] = new BackgroundBlock(id, arg);
                    return true;
            }

            return true;
        }

        public bool SetNormal(int layerNum, uint x, uint y, uint type)
        {
            switch (layerNum) {
                case 0:
                    if (this.data.Foreground[x, y].Type == type) {
                        return false;
                    }

                    this.data.Foreground[x, y] = new ForegroundBlock(type);
                    return true;
                case 1:
                    if (this.data.Background[x, y].Type == type) {
                        return false;
                    }

                    this.data.Background[x, y] = new BackgroundBlock(type);
                    return true;
            }

            return true;
        }

        #endregion

        #region List count getters

        public int CoinCount {
            get { return this.data.Foreground.CoinCount; }
        }

        public int BlueCoinCount {
            get { return this.data.Foreground.BlueCoinCount; }
        }

        private int SpawnCount {
            get {
                lock (this.data.Foreground.Spawns) {
                    return this.data.Foreground.Spawns.Count;
                }
            }
        }

        public int WorldPortalCount {
            get { return this.data.Foreground.WorldPortalCounts; }
        }

        public int DiamondCount {
            get { return this.data.Foreground.DiamondCounts; }
        }

        public int CakesCount {
            get { return this.data.Foreground.CakeCounts; }
        }

        public int HologramsCount {
            get { return this.data.Foreground.HologramCounts; }
        }

        #endregion
    }
}
