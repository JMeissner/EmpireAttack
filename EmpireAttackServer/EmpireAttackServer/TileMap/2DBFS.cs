using EmpireAttackServer.Players;
using EmpireAttackServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpireAttackServer.TileMap
{
    class _2DBFS
    {
        #region Private Fields

        private int maxX;
        private int maxY;
        private Queue<Point> queue;

        private Tile[][] workingCopyMap;
        private Faction workingFaction;

        #endregion Private Fields

        #region Public Constructors

        public _2DBFS()
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public void BFS(Tile[][] map, int x, int y, int limit)
        {
            workingFaction = map[x][y].Faction;
            maxX = map.Length;
            maxY = map[0].Length;
            //Reset Map
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[0].Length; j++)
                {
                    if (map[i][j].Faction == workingFaction)
                    {
                        map[i][j].IsConnected = false;
                        map[i][j].IsVisited = false;
                    }
                }
            }

            //Init BFS
            workingCopyMap = map;
            queue = new Queue<Point>();
            map[x][y].IsConnected = true;
            map[x][y].IsVisited = true;
            GetNeighbors(x, y, map);
            int counter = 0;
            //BFS Loop
            //No Counter due to large map sizes
            while (queue.Any() /*&& counter < limit*/)
            {
                Point p = queue.Dequeue();
                map[p.x][p.y].IsConnected = true;
                map[p.x][p.y].IsVisited = true;
                workingCopyMap = map;
                GetNeighbors(p.x, p.y, map);
                counter++;
            }
            //CleanUp
            workingCopyMap = null;
            workingFaction = Faction.NONE;
            maxX = 0;
            maxY = 0;
            queue = null;
        }

        #endregion Public Methods

        #region Private Methods

        private void GetNeighbors(int x, int y, Tile[][] map)
        {
            //Up
            if (x + 1 < maxX)
            {
                if (map[x + 1][y].Faction.Equals(workingFaction) && !map[x + 1][y].IsVisited && !queue.Contains(new Point(x + 1, y)))
                {
                    queue.Enqueue(new Point(x + 1, y));
                }
            }
            //Down
            if (x - 1 >= 0)
            {
                if (map[x - 1][y].Faction.Equals(workingFaction) && !map[x - 1][y].IsVisited && !queue.Contains(new Point(x - 1, y)))
                {
                    queue.Enqueue(new Point(x - 1, y));
                }
            }
            //Right
            if (y + 1 < maxY)
            {
                if (map[x][y + 1].Faction.Equals(workingFaction) && !map[x][y + 1].IsVisited && !queue.Contains(new Point(x, y + 1)))
                {
                    queue.Enqueue(new Point(x, y + 1));
                }
            }
            //Left
            if (y - 1 >= 0)
            {
                if (map[x][y - 1].Faction.Equals(workingFaction) && !map[x][y - 1].IsVisited && !queue.Contains(new Point(x, y - 1)))
                {
                    queue.Enqueue(new Point(x, y - 1));
                }
            }
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