using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpireAttackServer.TileMap
{
    class _2DDFS
    {
        #region Private Fields

        private Point destination;
        private List<Point> neighbors;
        private Tile[][] tileMap;
        private bool[][] visitedTiles;

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

        #endregion Private Fields

        #region Public Constructors

        public _2DDFS(Tile[][] map)
        {
            //Copy Tilemap
            tileMap = map;
            //Created new visitedArray
            visitedTiles = new bool[tileMap.Length][];
            for (int i = 0; i < tileMap.Length; i++)
            {
                visitedTiles[i] = new bool[tileMap[0].Length];
            }
            for (int i = 0; i < tileMap.Length; i++)
            {
                for (int j = 0; j < tileMap[0].Length; j++)
                {
                    visitedTiles[i][j] = false;
                }
            }
        }

        #endregion Public Constructors

        #region Public Methods

        public bool isConnected(int Sx, int Sy, int Dx, int Dy)
        {
            destination = new Point(Dx, Dy);
            return false;
        }

        #endregion Public Methods

        #region Private Methods

        private bool DFS(Point point)
        {
            if (point.x == destination.x && point.y == destination.y)
            {
            }

            return false;
        }

        private List<Point> getNeighbors(Point p)
        {
            //TODO: Check Bounds!!!
            List<Point> np = new List<Point>();
            int x = p.x;
            int y = p.y;
            if (tileMap[x + 1][y].Faction.Equals(tileMap[x][y].Faction)) { np.Add(new Point(x + 1, y)); }
            if (tileMap[x - 1][y].Faction.Equals(tileMap[x][y].Faction)) { np.Add(new Point(x - 1, y)); }
            if (tileMap[x][y + 1].Faction.Equals(tileMap[x][y].Faction)) { np.Add(new Point(x, y + 1)); }
            if (tileMap[x][y - 1].Faction.Equals(tileMap[x][y].Faction)) { np.Add(new Point(x, y - 1)); }

            return np;
        }

        #endregion Private Methods
    }
}