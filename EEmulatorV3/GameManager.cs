using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Nancy.Hosting.Self;
using PlayerIO.ServerCore.Microlog;
using PlayerIO.ServerCore.PlayerIOClient;

namespace EEmulatorV3
{
    public static class GameManager
    {
        public static NancyHost WebAPI { get; set; }
        public static List<IGame> Games { get; } = new List<IGame>();

        public static void Run(IGame game)
        {
        }

        public static void PatchDevelopmentServer()
        {
            var channel = new PlayerIOChannel.MultiplayerChannelHost("");
            var field = channel.GetType().GetField("apiRegex", BindingFlags.Static | BindingFlags.NonPublic);
            field.SetValue(channel, new Regex("", RegexOptions.Compiled));
            Micrologger.Output.AddFixedTarget(new ConsoleTarget(MicrologLevel.Trace, new MicrologLayout(null)), false);
        }
    }
}
