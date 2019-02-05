namespace EmpireAttackServer.Shared
{
    public enum TileType
    {
        Normal, Forest, Hills, Urban, Water
    };

    public class Tile
    {
        #region Public Properties

        public Faction Faction { get; set; }
        public bool IsConnected { get; set; }
        public bool IsVisited { get; set; }
        public int Population { get; set; }
        public TileType Type { get; set; }

        #endregion Public Properties

        #region Public Constructors

        public Tile(TileType type)
        {
            this.Faction = Faction.NONE;
            this.Population = 0;
            this.Type = type;
        }

        public Tile(Faction faction, int population, TileType type)
        {
            this.Faction = faction;
            this.Population = population;
            this.Type = type;
        }

        #endregion Public Constructors

        #region Public Methods

        public string GetShortType()
        {
            string stype = "";
            switch (Type)
            {
                case TileType.Normal: stype = "N"; break;
                case TileType.Water: stype = "W"; break;
                case TileType.Forest: stype = "F"; break;
                case TileType.Hills: stype = "H"; break;
                case TileType.Urban: stype = "U"; break;
                default: stype = "E"; break;
            }
            return stype;
        }

        #endregion Public Methods
    }
}