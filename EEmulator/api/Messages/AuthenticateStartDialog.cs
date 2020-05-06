using System.Collections.Generic;
using ProtoBuf;

namespace EEmulator.Messages
{
    [ProtoContract]
    public class AuthenticateStartDialog
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public List<KeyValuePair> Arguments { get; set; }
    }
}