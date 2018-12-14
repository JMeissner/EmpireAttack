using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpireAttackServer.TileMap
{
    public interface IMap
    {
        bool OccupyTile(int x, int y);

        void GetPopulation(string faction);

        bool CanOccupyTile(int x, int y);

        bool IsConnected(int x, int y);

        void AddPopulation(int x, int y, int amount);

        void UpdateMapPopulation();
    }
}
