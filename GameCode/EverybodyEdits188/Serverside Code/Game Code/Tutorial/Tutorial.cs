using System;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game
{
    // *******************************************************************************************
    // Game
    // *******************************************************************************************

    [RoomType("Tutorial188")]
    public class TutorialGame : EverybodyEdits
    {
        public override void GameStarted()
        {
            this.isTutorialRoom = true;
            base.GameStarted();
        }
    }

    [RoomType("Tutorial188_world_1")]
    public class TutorialGameWorld1 : TutorialGame
    {
        protected override void loadWorld(string roomid)
        {
            base.loadWorld(TutorialIds.GetTutorialId(1));
        }
    }

    [RoomType("Tutorial188_world_2")]
    public class TutorialGameWorld2 : TutorialGame
    {
        protected override void loadWorld(string roomid)
        {
            base.loadWorld(TutorialIds.GetTutorialId(2));
        }
    }

    [RoomType("Tutorial188_world_3")]
    public class TutorialGameWorld3 : TutorialGame
    {
        protected override void loadWorld(string roomid)
        {
            base.loadWorld(TutorialIds.GetTutorialId(3));
        }

        protected override bool canEdit(Player player, Message m)
        {
            var layerNum = m.GetInt(0);
            var cx = (uint)m.GetInt(1);
            var cy = (uint)m.GetInt(2);

            if (cx > 42 && cx < 119 && cy < 23) return true;

            return false;
        }
    }

    public class TutorialIds
    {
        public static string GetTutorialId(int num)
        {
            String[] tutorial_ids = {"PWAIjKWOiLbEI", "PWHEwiIsmRbEI", "PWk0f5X-yVbEI"};
            return tutorial_ids[num - 1];
        }
    }
}