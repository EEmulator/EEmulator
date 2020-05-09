using System.Collections.Generic;
using EEmulator.Messages;

namespace EEmulator
{
    public interface IGame
    {
        string GameId { get; }

        string Description { get; set; }

        List<RoomInfo> Rooms { get; set; }

        GameAssembly GameAssembly { get; }

        BigDB BigDB { get; }

        void Run();
    }
}
