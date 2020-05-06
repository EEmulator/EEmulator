using System.Collections.Generic;
using ProtoBuf;

namespace EEmulator.Messages
{
    [ProtoContract]
    public class PayVaultContents
    {
        [ProtoMember(1)]
        public string Version { get; set; }

        [ProtoMember(2)]
        public int Coins { get; set; }

        [ProtoMember(3)]
        public List<PayVaultItem> Items { get; set; }
    }
}