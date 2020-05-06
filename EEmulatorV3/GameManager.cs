using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Nancy;
using Nancy.Bootstrapper;
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
        public static byte[] EncryptionKey = new byte[] { 0xC0, 0xFF, 0xEE, 0x80, 0x08, 0x55, 0x10, 0xA1, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7, 0xA8, 0xA9 };

        public static void Run(IGame game)
        {
            Games.Add(game);
            game.Run();
        }

        public static void PatchDevelopmentServer()
        {
            var channel = new PlayerIOChannel.MultiplayerChannelHost("");
            var field = channel.GetType().GetField("apiRegex", BindingFlags.Static | BindingFlags.NonPublic);
            field.SetValue(channel, new Regex("", RegexOptions.Compiled));
            Micrologger.Output.AddFixedTarget(new ConsoleTarget(MicrologLevel.Trace, new MicrologLayout(null)), false);
        }

        public static IGame GetGameFromToken(string token)
        {
            var gameConnectId = token.Split(':')[0];
            var game = Games.Find(x => x.GameId == gameConnectId);
            return game;
        }
    }

    public class BeforeAllRequests : IApplicationStartup
    {
        public void Initialize(IPipelines pipelines)
        {
            pipelines.BeforeRequest.AddItemToStartOfPipeline(ctx =>
            {
                if (ctx != null)
                {
                    Console.WriteLine("Request: " + ctx.Request.Session + " >> " + ctx.Request.Url);
                }
                return null;
            });
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
