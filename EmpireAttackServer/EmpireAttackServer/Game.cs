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
        private Root root;
        private Dictionary<Faction, Point> Capitals;
        private List<Faction> eliminatedFactions;
        private List<Faction> factions;
        private Dictionary<Faction, int> Population;

        #endregion Public Fields

        #region Public Constructors

        public Game(Root root)
        {
            this.root = root;
            Capitals = new Dictionary<Faction, Point>();
            Population = new Dictionary<Faction, int>();
            eliminatedFactions = new List<Faction>();
        }

        #endregion Public Constructors

        #region Public Methods

        public Faction GetFactionOnTile(int x, int y)
        {
            return map.tileMap[x][y].Faction;
        }

        public int GetPopulationOnTile(int x, int y)
        {
            return map.tileMap[x][y].Population;
        }

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
        public void Initialize(int mapLoadType, int NumberOfFactions)
        {
            //Faction Initialization
            factions = new List<Faction>();
            for (int i = 0; i < NumberOfFactions; i++)
            {
                int f = 0;
                do
                {
                    Random r = new Random();
                    f = r.Next(1, Enum.GetNames(typeof(Faction)).Length);
                } while (factions.Contains((Faction)f));
                factions.Add((Faction)f);
            }

            foreach (Faction f in factions)
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
        /// Updates the map and stacks population on connected tiles
        /// </summary>
        public void LateUpdate()
        {
            //Update Map population
            foreach (Faction f in Capitals.Keys)
            {
                int updatedTiles = map.UpdateMapPopulation(Capitals[f].x, Capitals[f].y);
                Console.WriteLine("GAME>>UPDATED {0} TILES FROM: {1}.", updatedTiles, f.ToString());
            }
            OnReSync(new EventArgs());
        }

        /// <summary>
        /// Try to occupy a field with a given population
        /// </summary>
        /// <param name="faction">attacking faction</param>
        /// <param name="X">Tilemap X</param>
        /// <param name="Y">Tilemap Y</param>
        /// <param name="pop">Attacking population</param>
        /// <returns>true if successful, false otherwise</returns>
        public bool OccupyFromDelta(Faction faction, int X, int Y, int pop)
        {
            if (pop > Population[faction])
            {
                return false;
            }
            int prevPopulation = map.tileMap[X][Y].Population;
            if (map.CanOccupyTile(faction, pop, X, Y))
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
            else if (map.IsNeighbor(faction, X, Y))
            {
                map.AddPopulation(X, Y, -pop);
                SubstractPopulation(faction, pop);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Called every Gameupdate
        /// </summary>
        public void Update()
        {
            foreach (Faction f in factions)
            {
                AddFactionPopulation(f, 1);
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void AddFactionPopulation(Faction faction, int amount)
        {
            Population[faction] += amount;
            UpdatePopulationEventArgs args = new UpdatePopulationEventArgs();
            args.faction = faction;
            args.amount = Population[faction];
            OnUpdatePopulation(args);
        }

        private void SetCapitals(int numberOfFactions)
        {
            int[] capitalCoords = map.GetCapitals();
            map.RemoveCapitals();
            Random random = new Random();
            List<int> usedIndex = new List<int>();
            for (int i = 0; i < numberOfFactions; i++)
            {
                int index;
                do
                {
                    index = random.Next(0, (capitalCoords.Length / 2));
                    index = (index * 2) % capitalCoords.Length;
                } while (usedIndex.Contains(index));

                Capitals.Add(factions[i], new Point(capitalCoords[index], capitalCoords[index + 1]));
                map.SetCapitalAtPosition(capitalCoords[index], capitalCoords[index + 1], factions[i]);
            }
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

        private void OvertakeEnemyCapital(Faction attacker, Faction defender)
        {
            eliminatedFactions.Add(defender);
            //TODO: Callback to ServerMain
            //TODO: Map -> Overtake Tiles
            //TODO: Sync
        }

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

        #region Public Events

        public event EventHandler<EventArgs> ReSync;

        public event EventHandler<UpdatePopulationEventArgs> UpdatePopulation;

        #endregion Public Events

        #region Protected Methods

        protected virtual void OnReSync(EventArgs e)
        {
            ReSync?.Invoke(this, e);
        }

        protected virtual void OnUpdatePopulation(UpdatePopulationEventArgs e)
        {
            UpdatePopulation?.Invoke(this, e);
        }

        #endregion Protected Methods

        #region Public Classes

        public class UpdatePopulationEventArgs : EventArgs
        {
            #region Public Properties

            public int amount;
            public Faction faction;

            #endregion Public Properties
        }

        #endregion Public Classes
    }
}