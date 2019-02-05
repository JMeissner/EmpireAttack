using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmpireAttackServer.Shared;
using Lidgren.Network;

namespace EmpireAttackServer.Players
{
    class PlayerManager
    {
        #region Private Fields

        private List<Player> players;

        #endregion Private Fields

        #region Public Constructors

        public PlayerManager()
        {
            players = new List<Player>();
        }

        #endregion Public Constructors

        #region Public Methods

        public bool AddPlayer(string name, Faction faction, NetConnection IP)
        {
            Player newp = new Player(name, IP, faction);
            if (!PlayerExists(IP))
            {
                players.Add(newp);
            }
            return false;
        }

        public Player GetPlayer(NetConnection IP)
        {
            foreach (Player p in players)
            {
                if (p.IP.Equals(IP))
                {
                    return p;
                }
            }
            return null;
        }

        public bool PlayerExists(NetConnection IP)
        {
            foreach (Player p in players)
            {
                if (p.IP.Equals(IP))
                {
                    return true;
                }
            }
            return false;
        }

        public bool RemovePlayer(NetConnection IP)
        {
            foreach (Player p in players)
            {
                if (p.IP.Equals(IP))
                {
                    players.Remove(p);
                    return true;
                }
            }
            return false;
        }

        #endregion Public Methods
    }
}