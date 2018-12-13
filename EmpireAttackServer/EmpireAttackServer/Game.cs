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

        #endregion Public Fields

        #region Public Constructors

        public Game(int maxPlayers)
        {
            this.MaxPlayers = maxPlayers;
        }

        #endregion Public Constructors

        #region Public Methods

        public void Initialize()
        {
        }

        public void Update()
        {
        }

        #endregion Public Methods
    }
}