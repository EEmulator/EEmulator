namespace EEmulator
{
    public interface IGame
    {
        string GameId { get; }

        GameAssembly GameAssembly { get; }
        BigDB BigDB { get; }

        void Run();
    }
}
