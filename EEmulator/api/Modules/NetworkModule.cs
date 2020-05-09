using System.Collections.Generic;
using System.Linq;
using EEmulator.Messages;
using Nancy;

namespace EEmulator.api.Modules
{
    public class NetworkModule : NancyModule
    {
        public class GameInfo
        {
            public string GameId { get; set; }
            public List<RoomInfo> Rooms { get; set; }
        }

        public NetworkModule()
        {
            this.Get("/rooms", x => this.Response.AsJson(GameManager.Games.Select(game => new GameInfo() { GameId = game.GameId, Rooms = game.Rooms }).ToList()));
            this.Get("/status", x => this.Response.AsText("Online!"));
        }
    }
}
