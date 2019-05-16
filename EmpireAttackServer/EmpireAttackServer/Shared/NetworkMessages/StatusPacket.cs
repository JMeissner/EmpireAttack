using System;
using LiteNetLib;
using LiteNetLib.Utils;

namespace EmpireAttackServer.Shared.NetworkMessages
{
    class StatusPacket : IPacket
    {
        public bool IsRequest;
        public StatusMsg Status;

        public PacketTypes MessageType { get { return PacketTypes.STATUS; } private set { } }

        public void Decode(NetPacketReader im)
        {
            throw new NotImplementedException();
        }

        public void Encode(NetDataWriter om)
        {
            throw new NotImplementedException();
        }
    }

    public enum StatusMsg
    {
        INIT,
        FACTIONLOAD,
        FACTIONSELECT
    }
}
