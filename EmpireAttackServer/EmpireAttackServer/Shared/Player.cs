using Lidgren.Network;

namespace EmpireAttackServer.Shared
{
    public class Player
    {
        #region Public Fields

        public Faction faction;
        public NetConnection IP;
        public string name;

        #endregion Public Fields

        #region Public Constructors

        public Player(string name, NetConnection EndPoint, Faction faction)
        {
            this.name = name;
            this.IP = EndPoint;
            this.faction = faction;
        }

        #endregion Public Constructors
    }
}