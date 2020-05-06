using System;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game
{
    public class Player : BasePlayer
    {
        public bool isguest
        {
            get
            {
                return base.ConnectUserId == "simpleguest";
            }
        }

        public int Timestamp = 0;

        public int face = 0;

        public double x = 16.0;

        public double y = 16.0;

        public bool canEdit = false;

        public int cheat = 0;

        public bool haveSmileyPackage;

        public bool isgod = false;

        public bool ismod = false;

        public bool canbemod = false;

        public DateTime lastEdit = DateTime.Now;

        public int threshold = 150;

        public bool ready = false;

        public bool owner = false;

        public bool canchat = false;

        public string room0 = "";

        public string name = "";

        public string betaonlyroom = "";

        public bool canWinEnergy = true;

        public DateTime lastCoin = DateTime.Now;

        public DateTime lastmove = DateTime.Now;

        public DateTime lastChat = DateTime.Now;

        public int horizontal = 0;

        public int vertical = 0;

        public int coins = 0;

        public int bcoins = 0;

        public DateTime coinTimer = DateTime.Now.AddMinutes(1.0);
    }
}
