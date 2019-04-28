using System;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace EmpireAttackServer.Shared.NetworkMessages
{
    class GameEndedPacket : IPacket
    {
        #region Public Fields

        public Dictionary<Faction, int> NoOfTiles;
        public Faction Winner;

        #endregion Public Fields

        #region Public Properties

        public PacketTypes MessageType { get { return PacketTypes.GAMEENDED; } private set { } }

        #endregion Public Properties

        #region Public Constructors

        public GameEndedPacket(Faction winner, Dictionary<Faction, int> NoOfTiles)
        {
            Winner = winner;
            this.NoOfTiles = NoOfTiles;
        }

        public GameEndedPacket(NetPacketReader im)
        {
            Decode(im);
        }

        #endregion Public Constructors

        #region Public Methods

        public void Decode(NetPacketReader im)
        {
            NoOfTiles = new Dictionary<Faction, int>();
            Winner = (Faction)im.GetByte();
            int noFactions = im.GetInt();
            for (int i = 0; i < noFactions; i++)
            {
                NoOfTiles.Add((Faction)im.GetByte(), im.GetInt());
            }
        }

        public void Encode(NetDataWriter om)
        {
            om.Put((Byte)PacketTypes.GAMEENDED);
            om.Put((Byte)Winner);
            om.Put(NoOfTiles.Keys.Count);
            foreach (Faction f in NoOfTiles.Keys)
            {
                om.Put((Byte)f);
                om.Put(NoOfTiles[f]);
            }
        }

        #endregion Public Methods
    }
}