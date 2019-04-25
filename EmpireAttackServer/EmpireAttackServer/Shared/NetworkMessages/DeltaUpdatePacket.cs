using LiteNetLib;
using LiteNetLib.Utils;

namespace EmpireAttackServer.Shared.NetworkMessages
{
    class DeltaUpdatePacket : IPacket
    {
        public Faction faction;
        public int FieldX;
        public int FieldY;
        public int Population;
        public PacketTypes MessageType { get { return PacketTypes.DELTAUPDATE; } private set { } }

        public DeltaUpdatePacket(Faction faction, int X, int Y, int population)
        {
            this.faction = faction;
            this.FieldX = X;
            this.FieldY = Y;
            this.Population = population;
        }

        public DeltaUpdatePacket(NetPacketReader im)
        {
            this.Decode(im);
        }

        public void Decode(NetPacketReader im)
        {
            faction = (Faction)im.GetByte();
            FieldX = im.GetInt();
            FieldY = im.GetInt();
            Population = im.GetInt();
        }

        public void Encode(NetDataWriter om)
        {
            om.Put((byte)PacketTypes.DELTAUPDATE);
            om.Put((byte)faction);
            om.Put(FieldX);
            om.Put(FieldY);
            om.Put(Population);
        }
    }
}
