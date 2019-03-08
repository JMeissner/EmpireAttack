using EmpireAttackServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpireAttackServer.TileMap
{
    public class MapBase : IMap
    {
        #region Public Fields

        public Tile[][] tileMap;

        #endregion Public Fields

        #region Public Constructors

        public MapBase()
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public void AddPopulation(int x, int y, int amount)
        {
            throw new NotImplementedException();
        }

        public bool CanOccupyTile(Faction faction, int attackingForce, int x, int y)
        {
            throw new NotImplementedException();
        }

        public void GetPopulation(Faction faction)
        {
            throw new NotImplementedException();
        }

        public TileType GetTileType(int x, int y)
        {
            return tileMap[x][y].Type;
        }

        public bool IsConnected(int x, int y)
        {
            bool isconnected = false;
            if (tileMap[x + 1][y].Faction.Equals(tileMap[x][y].Faction)) { isconnected = true; }
            if (tileMap[x - 1][y].Faction.Equals(tileMap[x][y].Faction)) { isconnected = true; }
            if (tileMap[x][y + 1].Faction.Equals(tileMap[x][y].Faction)) { isconnected = true; }
            if (tileMap[x][y - 1].Faction.Equals(tileMap[x][y].Faction)) { isconnected = true; }
            return isconnected;
        }

        public bool OccupyTile(Faction faction, int attackingForce, int x, int y)
        {
            throw new NotImplementedException();
        }

        public void UpdateMapPopulation()
        {
            //BFS
            //Connected + 1
            //Unconnected - 1
        }

        #endregion Public Methods
    }
}