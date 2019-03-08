using EmpireAttackServer.Players;
using EmpireAttackServer.Shared;
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

        /// <summary>
        /// Returns the tilemap for serverupdates etc
        /// </summary>
        /// <returns>tilemap (2d Array of Tiles)</returns>
        public Tile[][] GetTileMap()
        {
            if (map != null)
            {
                return map.tileMap;
            }
            return null;
        }

        /// <summary>
        /// Initializes the game with a specified way to load or generate the map
        /// </summary>
        /// <param name="mapLoadType">1=PNGImport, 2=... </param>
        public void Initialize(int mapLoadType)
        {
            //Load Map
            if (mapLoadType == 1)
            {
                map = new MapPNGImport(AppDomain.CurrentDomain.BaseDirectory + @"/Map.png");
            }
            //Set Capitals
        }

        /// <summary>
        /// Called every Gameupdate
        /// </summary>
        public void Update()
        {
            //Update Map population
            map.UpdateMapPopulation();
            //TODO: Send update information
        }

        #endregion Public Methods

        #region Private Methods

        private void SetCapitals(int numberOfFactions)
        {
            Random random = new Random();
            //Select Column of x1 and x4
            int x14col = random.Next(0, 6);
            //Select Column of x2 and x3
            int x23col = random.Next(map.tileMap[0].Length - 6, map.tileMap[0].Length);
            //Select Column of x5 and x6
            int x56col = random.Next((map.tileMap[0].Length / 2) - 3, (map.tileMap[0].Length / 2) + 3);
            //Set Capitals regarding numberOfFactions
            int tempX = 3;
            while (map.GetTileType(tempX, x14col) != TileType.Normal)
            {
                tempX++;
            }
            map.tileMap[tempX][x14col].Type = TileType.Capital;
            //Add capital to dictionary
            //Capitals.Add(Faction.Baguette, new Point(tempX, x14col));
        }

        #endregion Private Methods

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