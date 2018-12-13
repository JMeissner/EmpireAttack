using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpireAttackServer
{
    public enum TileType
    {
        Normal, Forest, Hills, Urban, Water
    };

    public class Tile
    {
        #region Public Properties

        public int Faction { get; set; }
        public int Population { get; set; }
        public TileType Type { get; set; }

        #endregion Public Properties
    }
}