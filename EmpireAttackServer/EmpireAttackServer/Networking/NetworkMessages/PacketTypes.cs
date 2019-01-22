using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpireAttackServer.Networking.NetworkMessages
{
    /// <summary>
    /// The game message types.
    /// </summary>
    public enum PacketTypes
    {
        LOGIN,

        WORLDUPDATE,

        TEST
    }
}