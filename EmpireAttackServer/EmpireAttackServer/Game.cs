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
        private List<Faction> factions;
        private Dictionary<Faction, Point> Capitals;
        private Dictionary<Faction, int> Population;

        #endregion Public Fields

        #region Public Constructors

        public Game()
        {
            Capitals = new Dictionary<Faction, Point>();
            Population = new Dictionary<Faction, int>();
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
        public void Initialize(int mapLoadType, List<Faction> factions)
        {
            //Save copy of available factions
            this.factions = factions;

            foreach(Faction f in factions)
            {
                Population.Add(f, 1);
            }
            //Load Map
            if (mapLoadType == 1)
            {
                map = new MapPNGImport(AppDomain.CurrentDomain.BaseDirectory + @"/Map.png");
            }
            //Set Capitals
            SetCapitals(factions.Count);
        }

        /// <summary>
        /// Called every Gameupdate
        /// </summary>
        public void Update()
        {
            foreach(Faction f in factions)
            {
                AddFactionPopulation(f, 1);
            }
        }

        public void LateUpdate()
        {
            //Update Map population
            map.UpdateMapPopulation();
            //TODO: Send update information
        }

        public bool OccupyFromDelta(Faction faction, int X, int Y, int pop)
        {
            if(pop > Population[faction])
            {
                return false;
            }
            int prevPopulation = map.tileMap[X][Y].Population;
            if(map.CanOccupyTile(faction, pop, X, Y))
            {
                map.OccupyTile(faction, pop, X, Y);
                SubstractPopulation(faction, pop);
                return true;
            }
            return false;
        }

        public bool PopulationFromDelta(Faction faction, int X, int Y, int pop)
        {
            if (pop > Population[faction])
            {
                return false;
            }
            int prevPopulation = map.tileMap[X][Y].Population;
            if (map.tileMap[X][Y].Faction.Equals(faction))
            {
                map.AddPopulation(X, Y, pop);
                SubstractPopulation(faction, pop);
                return true;
            }
            else if(map.IsNeighbor(faction, X, Y))
            {
                map.AddPopulation(X, Y, -pop);
                SubstractPopulation(faction, pop);
                return true;
            }
            return false;
        }

        public int GetPopulationOnTile(int x, int y)
        {
            return map.tileMap[x][y].Population;
        }

        public Faction GetFactionOnTile(int x, int y)
        {
            return map.tileMap[x][y].Faction;
        }

        #endregion Public Methods

        #region Private Methods

        private void SetCapitals(int numberOfFactions)
        {
            int[] capitalCoords = map.GetCapitals();
            map.RemoveCapitals();
            Random random = new Random();
            List<int> usedIndex = new List<int>();
            for(int i = 0; i < numberOfFactions; i++)
            {
                int index;
                do
                {
                    index = random.Next(0, (capitalCoords.Length / 2));
                    index = index * 2;
                } while (usedIndex.Contains(index));
                
                Capitals.Add(factions[i], new Point(capitalCoords[index], capitalCoords[index + 1]));
                map.SetCapitalAtPosition(capitalCoords[index], capitalCoords[index + 1], factions[i]);
            }
        }

        private void AddFactionPopulation(Faction faction, int amount)
        {
            Population[faction] += amount;
            UpdatePopulationEventArgs args = new UpdatePopulationEventArgs();
            args.faction = faction;
            args.amount = Population[faction];
            OnUpdatePopulation(args);
        }

        private void SubstractPopulation(Faction faction, int amount)
        {
            Population[faction] -= amount;
            UpdatePopulationEventArgs args = new UpdatePopulationEventArgs();
            args.faction = faction;
            args.amount = Population[faction];
            OnUpdatePopulation(args);
        }

        private void ZeroPopulation(Faction faction)
        {
            Population[faction] = 0;
            UpdatePopulationEventArgs args = new UpdatePopulationEventArgs();
            args.faction = faction;
            args.amount = Population[faction];
            OnUpdatePopulation(args);
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

        public event EventHandler<UpdatePopulationEventArgs> UpdatePopulation;

        public class UpdatePopulationEventArgs : EventArgs
        {
            #region Public Properties

            public Faction faction;
            public int amount;

            #endregion Public Properties
        }

        protected virtual void OnUpdatePopulation(UpdatePopulationEventArgs e)
        {
            UpdatePopulation?.Invoke(this, e);
        }
    }
}