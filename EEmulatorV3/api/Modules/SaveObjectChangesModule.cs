using System;
using System.Collections.Generic;
using System.Linq;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class SaveObjectChangesModule : NancyModule
    {
        public SaveObjectChangesModule()
        {
            this.Post("/api/88", ctx =>
            {
                var args = Serializer.Deserialize<SaveObjectChangesArgs>(this.Request.Body);
                var token = this.Request.Headers["playertoken"].FirstOrDefault();
                var game = GameManager.GetGameFromToken(token);

                var versions = new List<string>();

                foreach (var set in args.Changesets)
                {
                    var dbo = game.BigDB.Load(set.Table, set.Key);

                    if (set.FullOverwrite)
                    {
                    }

                    versions.Add("1");
                }

                return PlayerIO.CreateResponse(token, true, new SaveObjectChangesOutput() { Versions = versions });
            });
        }
    }

    [ProtoContract]
    public class SaveObjectChangesArgs
    {
        [ProtoMember(1)]
        public LockType LockType { get; set; }

        [ProtoMember(2)]
        public List<BigDBChangeset> Changesets { get; set; }

        [ProtoMember(3)]
        public bool CreateIfMissing { get; set; }
    }

    [ProtoContract]
    public class SaveObjectChangesOutput
    {
        [ProtoMember(1)]
        public List<string> Versions { get; set; }
    }
}
