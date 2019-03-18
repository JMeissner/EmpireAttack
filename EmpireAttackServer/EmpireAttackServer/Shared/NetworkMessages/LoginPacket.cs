using LiteNetLib;
using LiteNetLib.Utils;

namespace EmpireAttackServer.Shared
{
    class LoginPacket : IPacket
    {
        #region Public Properties

        public byte LoginMsgType;
        public byte Faction;
        public string PlayerName;
        public Faction[] availableFactions;
        public PacketTypes MessageType { get { return PacketTypes.LOGIN; } private set { } }

        #endregion Public Properties

        #region Public Constructors

        public LoginPacket(NetPacketReader im)
        {
            this.Decode(im);
        }

        public LoginPacket(string pName, Faction Faction, LoginMsg loginMsg)
        {
            this.LoginMsgType = (byte)loginMsg;
            this.PlayerName = pName;
            this.Faction = (byte)Faction;
        }

        public LoginPacket(string pName, Faction[] factions)
        {
            this.LoginMsgType = (byte)LoginMsg.FACTIONLOAD;
            this.availableFactions = factions;
            this.PlayerName = pName;
        }

        #endregion Public Constructors

        #region Public Methods

        public void Decode(NetPacketReader im)
        {
            this.LoginMsgType = im.GetByte();
            this.PlayerName = im.GetString();
            switch((LoginMsg)LoginMsgType)
            {
                case LoginMsg.INIT:
                    break;
                case LoginMsg.FACTIONLOAD:
                    int f_lenght = im.GetInt();
                    availableFactions = new Faction[f_lenght];
                    for(int i = 0; i < availableFactions.Length; i++)
                    {
                        availableFactions[i] = (Faction)im.GetByte();
                    }
                    break;
                case LoginMsg.FACTIONSELECT:
                    this.Faction = im.GetByte();
                    break;
            }
        }

        public void Encode(NetDataWriter om)
        {
            om.Put((byte)PacketTypes.LOGIN);
            om.Put(LoginMsgType);
            om.Put(PlayerName);
            switch((LoginMsg)LoginMsgType)
            {
                case LoginMsg.INIT:
                    break;
                case LoginMsg.FACTIONLOAD:
                    om.Put(availableFactions.Length);
                    foreach(Faction f in availableFactions)
                    {
                        om.Put((byte)f);
                    }
                    break;
                case LoginMsg.FACTIONSELECT:
                    om.Put(Faction);
                    break;
            }
        }

        #endregion Public Methods

        public enum LoginMsg
        {
            INIT,
            FACTIONLOAD,
            FACTIONSELECT
        }
    }
}