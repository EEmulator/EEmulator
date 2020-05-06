using System;
using PlayerIO.GameLibrary;

namespace MyGame
{
	public class Player : BasePlayer
	{
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

		public string room0 = "";
	}
}
