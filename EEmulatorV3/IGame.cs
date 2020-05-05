namespace EEmulatorV3
{
    public interface IGame
    {
        string GameId { get; }
        GameAssembly GameAssembly { get; }
    }
}
