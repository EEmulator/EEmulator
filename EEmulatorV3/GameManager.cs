using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Nancy;
using Nancy.ErrorHandling;
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
            game.Run();
            Games.Add(game);
        }

        public static void PatchDevelopmentServer()
        {
            var channel = new PlayerIOChannel.MultiplayerChannelHost("");
            var field = channel.GetType().GetField("apiRegex", BindingFlags.Static | BindingFlags.NonPublic);
            field.SetValue(channel, new Regex("", RegexOptions.Compiled));
            Micrologger.Output.AddFixedTarget(new ConsoleTarget(MicrologLevel.Trace, new MicrologLayout(null)), false);
        }
    }

    public class ErrorStatusCodeHandler : IStatusCodeHandler
    {
        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return statusCode == HttpStatusCode.InternalServerError;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            if (context.Items.TryGetValue(NancyEngine.ERROR_EXCEPTION, out var errorObject))
            {
                if (errorObject is Exception exception)
                {
                    throw exception;
                }
            }
        }
    }

    public class StatusCodeHandler : IStatusCodeHandler
    {
        private readonly IRootPathProvider _rootPathProvider;

        public StatusCodeHandler(IRootPathProvider rootPathProvider)
        {
            _rootPathProvider = rootPathProvider;
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return statusCode == HttpStatusCode.NotFound;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("(Nancy) 404 -> " + context.Request.Path);
            Console.ResetColor();
        }
    }
}
