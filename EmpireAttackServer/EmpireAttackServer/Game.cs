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

        public List<String> Factions;
        public int MaxPlayers;
        public Dictionary<UInt64, String> PlayerFaction;

        public MapBase map;

        #endregion Public Fields

        #region Public Constructors

        public Game(int maxPlayers)
        {
            this.MaxPlayers = maxPlayers;
        }

        #endregion Public Constructors

        #region Public Methods

        public void Initialize(int mapLoadType)
        {
            if(mapLoadType == 1)
            {
                map = new MapPNGImport(AppDomain.CurrentDomain.BaseDirectory + @"/Map.png");
            }
        }

        public void Update()
        {
            //Update Map population
            map.UpdateMapPopulation();
            //TODO: Send update information
        }

        #endregion Public Methods
    }
}