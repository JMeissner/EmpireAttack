using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpireAttackServer.TileMap
{
    public interface IMap
    {
        bool occupyTile(int x, int y);

        void getPopulation(string faction);

        bool canOccupyTile(int x, int y);

        bool isConnected(int x, int y);

        void addPopulation(int x, int y, int amount);

        void updateMapPopulation();
    }
}
