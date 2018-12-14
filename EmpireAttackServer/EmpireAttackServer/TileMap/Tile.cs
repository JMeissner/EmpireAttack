﻿using System;
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

        public string Faction { get; set; }
        public int Population { get; set; }
        public TileType Type { get; set; }

        #endregion Public Properties

        #region Public Constructors

        public Tile(TileType type)
        {
            this.Faction = "None";
            this.Population = 0;
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