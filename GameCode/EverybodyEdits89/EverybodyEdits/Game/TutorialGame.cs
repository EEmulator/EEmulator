using System;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game
{
    [RoomType("Tutorial89")]
    public class TutorialGame : EverybodyEdits
    {
        public override void GameStarted()
        {
            this.isTutorialRoom = true;
            base.GameStarted();
        }

        protected override void loadWorld(string roomid)
        {
            base.loadWorld("tutorialWorld");
        }

        protected override bool canEdit(Player player, Message m)
        {
            var layerNum = m.GetInt(0U);
            var cx = (uint)m.GetInt(1U);
            var cy = (uint)m.GetInt(2U);
            Console.WriteLine(cx + " - " + cy);
            return cx >= 221U && cx <= 301U && cy >= 5U && cy <= 33U;
        }
    }
}
