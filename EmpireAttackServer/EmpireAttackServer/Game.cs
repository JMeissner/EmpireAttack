using EmpireAttackServer.Players;
using EmpireAttackServer.TileMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpireAttackServer
{
    class Game
    {
        #region Public Fields

        public MapBase map;
        private Dictionary<Faction, Point> Capitals;

        #endregion Public Fields

        #region Public Constructors

        public Game()
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public void Initialize(int mapLoadType)
        {
            //Load Map
            if (mapLoadType == 1)
            {
                map = new MapPNGImport(AppDomain.CurrentDomain.BaseDirectory + @"/Map.png");
            }
            //Set Capitals
        }

        public void Update()
        {
            //Update Map population
            map.UpdateMapPopulation();
            //TODO: Send update information
        }

        #endregion Public Methods

        #region Private Structs

        private struct Point
        {
            #region Private Fields

            public int x;
            public int y;

            #endregion Private Fields

            #region Public Constructors

            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            #endregion Public Constructors
        }

        #endregion Private Structs
    }
}