using LiteNetLib;
using LiteNetLib.Utils;

namespace EmpireAttackServer.Shared
{
    class LoginPacket : IPacket
    {
        #region Public Properties

        public byte Faction;
        public string PlayerName;
        public PacketTypes MessageType { get { return PacketTypes.LOGIN; } private set { } }

        #endregion Public Properties

        #region Public Constructors

        public LoginPacket(NetPacketReader im)
        {
            this.Decode(im);
        }

        public LoginPacket(string pName, Faction Faction)
        {
            this.PlayerName = pName;
            this.Faction = (byte)Faction;
        }

        #endregion Public Constructors

        #region Public Methods

        public void Decode(NetPacketReader im)
        {
            this.PlayerName = im.GetString();
            this.Faction = im.GetByte();
        }

        public void Encode(NetDataWriter om)
        {
            om.Put((byte)PacketTypes.LOGIN);
            om.Put(PlayerName);
            om.Put(Faction);
        }

        #endregion Public Methods
    }
}