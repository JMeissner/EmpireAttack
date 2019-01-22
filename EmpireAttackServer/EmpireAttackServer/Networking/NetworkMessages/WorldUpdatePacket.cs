using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmpireAttackServer.Players;
using Lidgren.Network;

namespace EmpireAttackServer.Networking.NetworkMessages
{
    class WorldUpdatePacket : IPacket
    {
        #region Public Fields

        public int Tilecount;
        public Tile[][] tiles;

        #endregion Public Fields

        #region Public Constructors

        public WorldUpdatePacket(Tile[][] map)
        {
            //How many Tiles we got
            Tilecount = map.Length * map.Length;
            tiles = map;
        }

        public WorldUpdatePacket(NetIncomingMessage im)
        {
            this.Decode(im);
        }

        #endregion Public Constructors

        #region Public Properties

        public PacketTypes MessageType { get { return PacketTypes.WORLDUPDATE; } private set { } }

        #endregion Public Properties

        #region Public Methods

        public void Decode(NetIncomingMessage im)
        {
            //Read Tilecount
            Tilecount = im.ReadInt32();
            //Create Array
            tiles = new Tile[(int)Math.Sqrt(Tilecount)][];
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = new Tile[(int)Math.Sqrt(Tilecount)];
            }
            //Decode Tiles and put them into Array
            for (int i = 0; i < tiles.Length; i++)
            {
                for (int j = 0; j < tiles[0].Length; j++)
                {
                    Faction f = (Faction)im.ReadByte();
                    int p = im.ReadInt32();
                    TileType t = (TileType)im.ReadByte();
                    Tile tempTile = new Tile(f, p, t);
                    tiles[i][j] = tempTile;
                }
            }
        }

        public void Encode(NetOutgoingMessage om)
        {
            om.Write((byte)PacketTypes.WORLDUPDATE);
            om.Write(Tilecount);
            for (int i = 0; i < tiles.Length; i++)
            {
                for (int j = 0; j < tiles[0].Length; j++)
                {
                    om.Write((Byte)tiles[i][j].Faction);
                    om.WriteVariableInt32(tiles[i][j].Population);
                    om.Write((Byte)tiles[i][j].Type);
                }
            }
        }

        #endregion Public Methods
    }
}