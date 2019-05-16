namespace EmpireAttackServer.Shared
{
    /// <summary>
    /// The game message types.
    /// </summary>
    public enum PacketTypes
    {
        LOGIN,

        WORLDUPDATE,

        DELTAUPDATE,

        POPULATIONUPDATE,

        GAMEENDED,

        STATUS,

        TEST
    }
}