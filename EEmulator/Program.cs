using System;
using System.Diagnostics;
using System.Threading;
using EEmulator.Messages;
using Nancy;
using Nancy.Hosting.Self;

namespace EEmulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"
                     _____ _____                _       _             
   Version 0.2.0    |  ___|  ___|              | |     | |     ""thanks atilla""
     written by     | |__ | |__ _ __ ___  _   _| | __ _| |_ ___  _ __ 
    miou & jesse    |  __||  __| '_ ` _ \| | | | |/ _` | __/ _ \| '__|
====================| |___| |__| | | | | | |_| | | (_| | || (_) | |============
                    \____/\____/_| |_| |_|\__,_|_|\__,_|\__\___/|_|" + "\n\n");

            if (Debugger.IsAttached)
            {
                Console.WriteLine("WARNING: Debug Mode is enabled.");

                args = new string[] { "EverybodyEdits", "v188", "localhost:8184", "PW01" };
            }

            if (args.Length < 3)
            {
                Console.WriteLine("ERROR: You must specify a game, a version, an endpoint, and a world ID (in that order)");
                return;
            }

            var game = args[0];
            var version = args[1];
            var use_endpoint = args[2];
            var force_world_id = args[3];

            GameManager.ForceWorldId = force_world_id != "null" ? force_world_id : null;
            GameManager.GameServerEndPoint = new ServerEndpoint() { Address = use_endpoint.Split(':')[0], Port = int.Parse(use_endpoint.Split(':')[1]) };

            GameManager.WebAPI = new NancyHost(new Uri("http://localhost:80/"), new DefaultNancyBootstrapper(), new HostConfiguration
            {
                RewriteLocalhost = true,
                UrlReservations = new UrlReservations { CreateAutomatically = true },
                AllowChunkedEncoding = false
            });

            GameManager.WebAPI.Start();
            GameManager.PatchDevelopmentServer();

            if (game == "EverybodyEdits")
            {
                switch (version)
                {
                    case "v0500":
                        GameManager.Run(new EverybodyEdits(EverybodyEditsVersion.v0500));
                        break;

                    case "v0700":
                        GameManager.Run(new EverybodyEdits(EverybodyEditsVersion.v0700));
                        break;

                    case "v0800":
                        GameManager.Run(new EverybodyEdits(EverybodyEditsVersion.v0800));
                        break;

                    case "v89":
                        GameManager.Run(new EverybodyEdits(EverybodyEditsVersion.v89));
                        break;

                    case "v188":
                        GameManager.Run(new EverybodyEdits(EverybodyEditsVersion.v188));
                        break;

                    default:
                        Console.WriteLine("ERROR: A preset for the specified game and version could not be found.");
                        break;
                }
            }

            Thread.Sleep(-1);
        }
    }
}
