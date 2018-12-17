using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmpireAttackServer.Networking.NetworkMessages;
using Lidgren.Network;

namespace EmpireAttackServer.Networking
{
    public class ServerNetworkManager : INetworkManager
    {
        #region Private Fields

        private bool isDisposed;

        private NetServer netServer;

        #endregion Private Fields

        #region Public Methods

        public void Connect()
        {
            var config = new NetPeerConfiguration("Empire_0.1A")
            {
                Port = Convert.ToInt32("14242"),
                // SimulatedMinimumLatency = 0.2f,
                // SimulatedLoss = 0.1f
            };
            config.EnableMessageType(NetIncomingMessageType.WarningMessage);
            config.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
            config.EnableMessageType(NetIncomingMessageType.ErrorMessage);
            config.EnableMessageType(NetIncomingMessageType.Error);
            config.EnableMessageType(NetIncomingMessageType.DebugMessage);
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);

            this.netServer = new NetServer(config);
            this.netServer.Start();
        }

        public NetOutgoingMessage CreateMessage()
        {
            return this.netServer.CreateMessage();
        }

        public void Disconnect()
        {
            this.netServer.Shutdown("Exit 0");
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        public NetIncomingMessage ReadMessage()
        {
            return this.netServer.ReadMessage();
        }

        public void Recycle(NetIncomingMessage im)
        {
            this.netServer.Recycle(im);
        }

        public void SendToAll(IGameMessage gameMessage)
        {
            NetOutgoingMessage om = this.netServer.CreateMessage();
            om.Write((byte)gameMessage.MessageType);
            gameMessage.Encode(om);

            this.netServer.SendToAll(om, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendToClient(IGameMessage gameMessage, NetConnection clientConnection)
        {
            NetOutgoingMessage om = this.netServer.CreateMessage();
            om.Write((byte)gameMessage.MessageType);
            gameMessage.Encode(om);

            this.netServer.SendMessage(om, clientConnection, NetDeliveryMethod.ReliableOrdered);
        }

        #endregion Public Methods

        #region Private Methods

        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.Disconnect();
                }

                this.isDisposed = true;
            }
        }

        #endregion Private Methods
    }
}