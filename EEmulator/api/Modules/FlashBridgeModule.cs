using System;
using System.IO;
using Nancy;

namespace EEmulator.api.Modules
{
    // TODO: update module to handle games outside of Everybody Edits
    public class FlashBridgeModule : NancyModule
    {
        public FlashBridgeModule()
        {
            this.Get("/flashbridge/1/{version}", x =>
            {
                if (Enum.TryParse<EverybodyEditsVersion>(((string)x.version).Split(' ')[0], out var gameVersion))
                {
                    switch (gameVersion)
                    {
                        case EverybodyEditsVersion.v0500:
                            return this.Response.FromStream(new MemoryStream(File.ReadAllBytes(@"bridge\1.swf")), "text/html; charset=utf-8");

                        case EverybodyEditsVersion.v89:
                            return this.Response.FromStream(new MemoryStream(File.ReadAllBytes(@"bridge\v89.swf")), "text/html; charset=utf-8");

                        default: throw new NotImplementedException("The game version requested does not have a corresponding flash bridge version.");
                    }
                }

                return this.Response.FromStream(new MemoryStream(File.ReadAllBytes(@"bridge\1.swf")), "text/html; charset=utf-8");
            });

            this.Get("/flashbridge/1", x =>
            {
                return this.Response.FromStream(new MemoryStream(File.ReadAllBytes(@"bridge\1.swf")), "text/html; charset=utf-8");
            });

            this.Get("crossdomain.xml", x => this.Response.FromStream(new MemoryStream(File.ReadAllBytes(@"bridge\crossdomain.xml")), "application/xml"));
            this.Get("/clients/{version}", x => this.Response.FromStream(new MemoryStream(File.ReadAllBytes(Path.Combine(@"games", "EverybodyEdits", "clients", ((string)x.version).Split(' ')[0], "EverybodyEdits.swf"))), "application/xml"));
        }
    }
}
