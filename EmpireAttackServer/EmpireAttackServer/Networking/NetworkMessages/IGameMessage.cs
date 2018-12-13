using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace EmpireAttackServer.Networking.NetworkMessages
{
    public interface IGameMessage
    {
        #region Public Properties

        /// <summary>
        /// Gets MessageType.
        /// </summary>
        GameMessageTypes MessageType { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// The decode.
        /// </summary>
        /// <param name="im">
        /// The im.
        /// </param>
        void Decode(NetIncomingMessage im);

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="om">
        /// The om.
        /// </param>
        void Encode(NetOutgoingMessage om);

        #endregion Public Methods
    }
}