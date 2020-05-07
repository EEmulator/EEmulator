using System;
using System.Threading;
using Nancy;
using Nancy.Hosting.Self;

namespace EEmulator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
                return;

            var game = args[0];
            var version = args[1];

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
                }
            }

            Thread.Sleep(-1);
        }
    }
}
