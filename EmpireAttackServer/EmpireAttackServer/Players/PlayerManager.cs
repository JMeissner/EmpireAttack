using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmpireAttackServer.Shared;
using LiteNetLib;

namespace EmpireAttackServer.Players
{
    class PlayerManager
    {
        #region Private Fields

        private List<Player> players;
        private Dictionary<String, Faction> allPlayers;

        #endregion Private Fields

        #region Public Constructors

        public PlayerManager()
        {
            players = new List<Player>();
            allPlayers = new Dictionary<string, Faction>();
        }

        #endregion Public Constructors

        #region Public Methods

        public bool AddPlayer(string name, Faction faction, NetPeer IP)
        {
            Player newp = new Player(name, IP, faction);
            if (!PlayerExists(IP))
            {
                players.Add(newp);
                if(!allPlayers.ContainsKey(name))
                {
                    allPlayers.Add(newp.name, newp.faction);
                }
            }
            return false;
        }

        public Player GetPlayer(NetPeer IP)
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

        public bool PlayerExists(NetPeer IP)
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

        public bool RemovePlayer(NetPeer IP)
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

        public Faction GetFactionFromName(String name)
        {
            if (allPlayers.ContainsKey(name))
            {
                return allPlayers[name];
            }
            return Faction.NONE;
        }

        #endregion Public Methods


    }
}