using System.Diagnostics;

namespace EverybodyEdits.Game.CountWorld
{
    [DebuggerDisplay("Id = {Id}")]
    public struct ForegroundBlock
    {
        private readonly object args;
        private readonly uint type;
        private readonly BlockArgsType argsType;

        public ForegroundBlock(uint type)
        {
            this.args = null;
            this.argsType = BlockArgsType.None;
            this.type = type;
        }

        public ForegroundBlock(uint type, uint args)
        {
            this.args = args;
            this.argsType = BlockArgsType.Number;
            this.type = type;
        }

        public ForegroundBlock(uint type, string args)
        {
            this.args = args;
            this.argsType = BlockArgsType.String;
            this.type = type;
        }

        public ForegroundBlock(uint type, uint portalId, uint portalTarget, uint portalRotation)
        {
            this.args = new PortalArgs(portalId, portalTarget, portalRotation);
            this.argsType = BlockArgsType.Portal;
            this.type = type;
        }

        public ForegroundBlock(uint type, string text, string color, uint wrapLength)
        {
            this.args = new ColorTextArgs(text, color, wrapLength);
            this.argsType = BlockArgsType.ColorText;
            this.type = type;
            
        }

        public ForegroundBlock(uint type, string text, uint signType)
        {
            this.args = new SignArgs(text, signType);
            this.argsType = BlockArgsType.Sign;
            this.type = type;
        }

        public uint Type {
            get { return this.type; }
        }

        public uint SignType {
            get { return ((SignArgs)this.args).SignType; }
        }

        public BlockArgsType ArgsType {
            get { return this.argsType; }
        }

        public string Text {
            get {
                if (this.ArgsType == BlockArgsType.ColorText) {
                    return ((ColorTextArgs)this.args).Text;
                }
                else if (this.ArgsType == BlockArgsType.Sign) {
                    return ((SignArgs)this.args).Text;
                }
                return (string)this.args;
            }
        }

        public string Color {
            get { return ((ColorTextArgs)this.args).Color; }
        }
        
        public uint WrapLength {
            get { return ((ColorTextArgs)this.args).WrapLength; }
        }

        public uint PortalId {
            get { return this.GetPortalArgs().PortalId; }
        }

        public uint PortalTarget {
            get { return this.GetPortalArgs().PortalTarget; }
        }

        public uint PortalRotation {
            get { return this.GetPortalArgs().PortalRotation; }
        }

        public uint Number {
            get {
                if (this.type >= 1101 && this.type <= 1105) return 1u;
                if (this.args is uint) {
                    return (uint)this.args;
                }
                return 0;
            }
        }

        private PortalArgs GetPortalArgs()
        {
            return (PortalArgs)this.args;
        }

        private class PortalArgs
        {
            public PortalArgs(uint portalId, uint portalTarget, uint portalRotation)
            {
                this.PortalId = portalId;
                this.PortalTarget = portalTarget;
                this.PortalRotation = portalRotation;
            }

            public uint PortalId { get; private set; }
            public uint PortalTarget { get; private set; }
            public uint PortalRotation { get; private set; }
        }

        private class ColorTextArgs
        {
            public ColorTextArgs(string text, string color, uint wrapLength)
            {
                this.Text = text;
                this.Color = color;
                this.WrapLength = wrapLength;
            }

            public string Text { get; private set; }
            public string Color { get; private set; }
            public uint WrapLength { get; private set; }
        }

        private class SignArgs
        {
            public SignArgs(string text, uint signType)
            {
                this.Text = text;
                this.SignType = signType;
            }

            public string Text { get; private set; }
            public uint SignType { get; private set; }
        }

        public object[] GetArgs()
        {
            switch (this.ArgsType) {
                case BlockArgsType.None:
                    return new object[0];
                case BlockArgsType.Portal:
                    return new object[] { this.PortalRotation, this.PortalId, this.PortalTarget };
                case BlockArgsType.ColorText:
                    return new object[] { this.Text, this.Color, this.Number };
                case BlockArgsType.Sign:
                    return new object[] { this.Text, this.SignType };
                default:
                    return new[] { this.args };
            }
        }
    }
}