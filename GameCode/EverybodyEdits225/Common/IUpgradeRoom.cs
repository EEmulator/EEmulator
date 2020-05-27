using PlayerIO.GameLibrary;

namespace EverybodyEdits.Common
{
    public interface IUpgradeRoom<TPlayer> where TPlayer : BasePlayer, new()
    {
        UpgradeChecker<TPlayer> UpgradeChecker { get; }
    }
}