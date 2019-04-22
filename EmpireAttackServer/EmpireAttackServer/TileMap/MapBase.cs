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
            tileMap[x][y].Population += amount;
        }

        public bool CanOccupyTile(Faction faction, int attackingForce, int x, int y)
        {
            if(tileMap[x][y].Faction.Equals(faction))
            {
                return false;
            }
            bool canOccupy = false;
            if(x+1 < tileMap.Length)
            {
                if (tileMap[x + 1][y].Faction.Equals(faction) && tileMap[x][y].Population < attackingForce) { canOccupy = true; }
            }
            if(x-1 >= 0)
            {
                if (tileMap[x - 1][y].Faction.Equals(faction) && tileMap[x][y].Population < attackingForce) { canOccupy = true; }
            }
            if(y+1 < tileMap[0].Length)
            {
                if (tileMap[x][y + 1].Faction.Equals(faction) && tileMap[x][y].Population < attackingForce) { canOccupy = true; }
            }
            if(y-1 >= 0)
            {
                if (tileMap[x][y - 1].Faction.Equals(faction) && tileMap[x][y].Population < attackingForce) { canOccupy = true; }
            }
            return canOccupy;
        }

        public void GetPopulation(Faction faction)
        {
            throw new NotImplementedException();
        }

        public TileType GetTileType(int x, int y)
        {
            return tileMap[x][y].Type;
        }

        public bool IsNeighbor(int x, int y)
        {
            bool isconnected = false;
            if (tileMap[x + 1][y].Faction.Equals(tileMap[x][y].Faction)) { isconnected = true; }
            if (tileMap[x - 1][y].Faction.Equals(tileMap[x][y].Faction)) { isconnected = true; }
            if (tileMap[x][y + 1].Faction.Equals(tileMap[x][y].Faction)) { isconnected = true; }
            if (tileMap[x][y - 1].Faction.Equals(tileMap[x][y].Faction)) { isconnected = true; }
            return isconnected;
        }

        public bool IsNeighbor(Faction faction, int x, int y)
        {
            bool isconnected = false;
            if (tileMap[x + 1][y].Faction.Equals(faction)) { isconnected = true; }
            if (tileMap[x - 1][y].Faction.Equals(faction)) { isconnected = true; }
            if (tileMap[x][y + 1].Faction.Equals(faction)) { isconnected = true; }
            if (tileMap[x][y - 1].Faction.Equals(faction)) { isconnected = true; }
            return isconnected;
        }

        public bool OccupyTile(Faction faction, int attackingForce, int x, int y)
        {
            int previousPop = tileMap[x][y].Population;
            tileMap[x][y].Faction = faction;
            tileMap[x][y].Population = attackingForce - previousPop;
            return true;
        }

        public void UpdateMapPopulation()
        {
            //BFS
            //Connected + 1
            //Unconnected - 1
        }

        public int[] GetCapitals()
        {
            List<int> rList = new List<int>();
            for(int i = 0; i < tileMap.Length; i++)
            {
                for(int j = 0; j < tileMap[0].Length; j++)
                {
                    if(tileMap[i][j].Type.Equals(TileType.Capital))
                    {
                        rList.Add(i);
                        rList.Add(j);
                    }
                }
            }
            return rList.ToArray();
        }

        public void RemoveCapitals()
        {
            for (int i = 0; i < tileMap.Length; i++)
            {
                for (int j = 0; j < tileMap[0].Length; j++)
                {
                    if (tileMap[i][j].Type.Equals(TileType.Capital))
                    {
                        tileMap[i][j].Type = TileType.Normal;
                    }
                }
            }
        }

        public void SetCapitalAtPosition(int i, int j, Faction faction)
        {
            tileMap[i][j].Type = TileType.Capital;
            tileMap[i][j].Faction = faction;
            tileMap[i][j].Population = 1;
        }

        #endregion Public Methods
    }
}