using EmpireAttackServer.Shared;

namespace EmpireAttackServer.TileMap
{
    public interface IMap
    {
        #region Public Methods

        void AddPopulation(int x, int y, int amount);

        bool CanOccupyTile(Faction faction, int attackingForce, int x, int y);

        void GetPopulation(Faction faction);

        bool OccupyTile(Faction faction, int attackingForce, int x, int y);

        void UpdateMapPopulation();

        #endregion Public Methods
    }
}