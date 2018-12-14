using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpireAttackServer.TileMap
{
    public class MapBase : IMap
    {
        public Tile[][] tileMap;

        public MapBase()
        {

        }

        public void AddPopulation(int x, int y, int amount)
        {
            throw new NotImplementedException();
        }

        public bool CanOccupyTile(int x, int y)
        {
            throw new NotImplementedException();
        }

        public void GetPopulation(string faction)
        {
            throw new NotImplementedException();
        }

        public bool IsConnected(int x, int y)
        {
            bool isconnected = false;
            if (tileMap[x+1][y].Faction.Equals(tileMap[x][y].Faction)) { isconnected = true; }
            if (tileMap[x-1][y].Faction.Equals(tileMap[x][y].Faction)) { isconnected = true; }
            if (tileMap[x][y+1].Faction.Equals(tileMap[x][y].Faction)) { isconnected = true; }
            if (tileMap[x][y-1].Faction.Equals(tileMap[x][y].Faction)) { isconnected = true; }
            return isconnected;
        }

        public bool OccupyTile(int x, int y)
        {
            throw new NotImplementedException();
        }

        public void UpdateMapPopulation()
        {
            for(int i = 0; i < tileMap.Length; i++)
            {
                for(int j = 0; j < tileMap[0].Length; j++)
                {
                    if(!tileMap[i][j].Faction.Equals("None"))
                    {
                        if (IsConnected(i, j))
                        {
                            tileMap[i][j].Population += 1;
                        }
                        else
                        {
                            tileMap[i][j].Population -= 1;
                        }
                    }
                }
            }
        }
    }
}
