using System;

namespace EEmulatorV3
{
    public class EverybodyEdits : IGame
    {
        public EverybodyEditsVersion Version { get; }

        public GameAssembly GameAssembly =>
            this.Version == EverybodyEditsVersion.v0500 ? new GameAssembly("FlixelWalker.dll", "FlixelWalker.pdb")
            : throw new NotImplementedException($"The version of game specified '{ this.Version }' does not have a game assembly associated with it.");

        public string GameId =>
            this.Version == EverybodyEditsVersion.v0500 ? "everybody-edits-v5"
            : throw new NotImplementedException($"The version of game specified '{ this.Version }' does not have a game id associated with it.");

        public EverybodyEdits(EverybodyEditsVersion version)
        {
            this.Version = version;
        }
    }
}
