using System;
using LiteNetLib;
using LiteNetLib.Utils;

namespace EmpireAttackServer.Shared
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

        public WorldUpdatePacket(NetPacketReader im)
        {
            this.Decode(im);
        }

        #endregion Public Constructors

        #region Public Properties

        public PacketTypes MessageType { get { return PacketTypes.WORLDUPDATE; } private set { } }

        #endregion Public Properties

        #region Public Methods

        public void Decode(NetPacketReader im)
        {
            //Read Tilecount
            Tilecount = im.GetInt();
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
                    Faction f = (Faction)im.GetByte();
                    int p = im.GetInt();
                    TileType t = (TileType)im.GetByte();
                    Tile tempTile = new Tile(f, p, t);
                    tiles[i][j] = tempTile;
                }
            }
        }

        public void Encode(NetDataWriter om)
        {
            om.Put((byte)PacketTypes.WORLDUPDATE);
            om.Put(Tilecount);
            for (int i = 0; i < tiles.Length; i++)
            {
                for (int j = 0; j < tiles[0].Length; j++)
                {
                    om.Put((Byte)tiles[i][j].Faction);
                    om.Put(tiles[i][j].Population);
                    om.Put((Byte)tiles[i][j].Type);
                }
            }
        }

        #endregion Public Methods
    }
}