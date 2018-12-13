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
    public enum GameMessageTypes
    {
        /// <summary>
        /// Updates the Tile at the specified position
        /// </summary>
        UpdateTileStatus,

        /// <summary>
        /// Login for players, so their names get matched with the server
        /// </summary>
        LogIn,

        /// <summary>
        /// Logout for players, so the server cuts their connection
        /// </summary>
        LogOut,

        /// <summary>
        /// Chatmessage between players or the server
        /// </summary>
        ChatMessage,

        /// <summary>
        /// One Time sync (Map, Population, Factions, etc)
        /// </summary>
        SyncAll,

        /// <summary>
        /// Sends the TileStatus of a Tile to ALL connected players
        /// </summary>
        SyncTileStatus,

        /// <summary>
        /// Sends the Population to a player
        /// </summary>
        SyncPopulation,

        /// <summary>
        /// Heartbeat message for afk player detection
        /// </summary>
        Heartbeat,

        /// <summary>
        /// Commands for admins etc
        /// </summary>
        ServerCommand

        //TODO: Might need more Networkmessages
    }
}