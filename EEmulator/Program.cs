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
            GameManager.WebAPI = new NancyHost(new Uri("http://localhost:80/"), new DefaultNancyBootstrapper(), new HostConfiguration
            {
                RewriteLocalhost = true,
                UrlReservations = new UrlReservations { CreateAutomatically = true },
                AllowChunkedEncoding = false
            });

            GameManager.WebAPI.Start();
            GameManager.PatchDevelopmentServer();

            //GameManager.Run(new EverybodyEdits(EverybodyEditsVersion.v0500));
            //GameManager.Run(new EverybodyEdits(EverybodyEditsVersion.v0700));
            //GameManager.Run(new EverybodyEdits(EverybodyEditsVersion.v0800));
            GameManager.Run(new EverybodyEdits(EverybodyEditsVersion.v89));

            Thread.Sleep(-1);
        }
    }
}
