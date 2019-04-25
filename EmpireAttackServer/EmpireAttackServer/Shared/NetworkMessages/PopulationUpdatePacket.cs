using System;
using LiteNetLib;
using LiteNetLib.Utils;

namespace EmpireAttackServer.Shared.NetworkMessages
{
    class PopulationUpdatePacket : IPacket
    {
        public byte faction;

        public int amount;

        public PacketTypes MessageType { get { return PacketTypes.POPULATIONUPDATE; } private set { } }

        public PopulationUpdatePacket(byte faction, int amount)
        {
            this.faction = faction;
            this.amount = amount;
        }

        public PopulationUpdatePacket(NetPacketReader im)
        {
            this.Decode(im);
        }

        public void Decode(NetPacketReader im)
        {
            faction = im.GetByte();
            amount = im.GetInt();
        }

        public void Encode(NetDataWriter om)
        {
            om.Put((byte)PacketTypes.POPULATIONUPDATE);
            om.Put(faction);
            om.Put(amount);
        }
    }
}
