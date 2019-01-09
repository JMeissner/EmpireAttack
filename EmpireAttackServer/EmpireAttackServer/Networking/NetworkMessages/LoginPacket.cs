using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using EmpireAttackServer.Players;

namespace EmpireAttackServer.Networking.NetworkMessages
{
    class LoginPacket : IPacket
    {
        #region Public Properties

        public byte Faction;
        public NetConnection IP;
        public string PlayerName;
        public PacketTypes MessageType { get { return PacketTypes.LOGIN; } private set { } }

        #endregion Public Properties

        #region Public Constructors

        public LoginPacket(NetIncomingMessage im)
        {
            this.Decode(im);
            this.IP = im.SenderConnection;
        }

        public LoginPacket(string pName, Faction Faction)
        {
            this.PlayerName = pName;
            this.Faction = (byte)Faction;
        }

        #endregion Public Constructors

        #region Public Methods

        public Player CreatePlayer()
        {
            Player player = new Player(PlayerName, IP, (Faction)Faction);
            return player;
        }

        public void Decode(NetIncomingMessage im)
        {
            this.PlayerName = im.ReadString();
            this.Faction = im.ReadByte();
        }

        public void Encode(NetOutgoingMessage om)
        {
            om.Write((byte)PacketTypes.LOGIN);
            om.Write(PlayerName);
            om.Write(Faction);
        }

        #endregion Public Methods
    }
}