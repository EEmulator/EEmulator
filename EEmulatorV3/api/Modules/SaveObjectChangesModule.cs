using System;
using System.Collections.Generic;
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
                throw new NotImplementedException($"The module {nameof(SaveObjectChangesModule)} (/api/88) has not been implemented yet.");
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
