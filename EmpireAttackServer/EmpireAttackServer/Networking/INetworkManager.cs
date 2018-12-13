using System;
using Lidgren.Network;
using EmpireAttackServer.Networking.NetworkMessages;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmpireAttackServer.Networking
{
    public interface INetworkManager : IDisposable
    {
        #region Public Methods

        void Connect();

        NetOutgoingMessage CreateMessage();

        void Disconnect();

        NetIncomingMessage ReadMessage();

        void Recycle(NetIncomingMessage im);

        void SendMessage(IGameMessage gameMessage);

        #endregion Public Methods
    }
}