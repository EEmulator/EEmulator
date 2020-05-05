using System.Collections.Generic;
using ProtoBuf;

namespace EEmulatorV3.Messages
{
    [ProtoContract]
    public class PayVaultBuyItemInfo
    {
        [ProtoMember(1)]
        public string ItemKey { get; set; }

        [ProtoMember(2)]
        public List<ObjectProperty> Payload { get; set; }
    }
}