using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteNetLib;
using EmpireAttackServer.Players;
using EmpireAttackServer.Shared;
using LiteNetLib.Utils;
using static EmpireAttackServer.Shared.LoginPacket;

namespace EmpireAttackServer.Networking
{
    class ServerManager
    {
        #region Private Fields

        //App ID to connect the right versions
        private static string APPID;

        //Max number of connections
        private static int MAXPLAYERS;

        //Port of the server
        private static int PORT;

        //LiteNet Objects
        private EventBasedNetListener listener;

        private NetManager server;

        #endregion Private Fields

        #region Public Constructors

        public ServerManager(string APPID, int PORT, int MAXPLAYERS)
        {
            ServerManager.APPID = APPID;
            ServerManager.PORT = PORT;
            ServerManager.MAXPLAYERS = MAXPLAYERS;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Start Server and write Config to Console
        /// </summary>
        public void Initialize()
        {
            //Start Server
            listener = new EventBasedNetListener();
            server = new NetManager(listener);
            server.DisconnectTimeout = 5000;
            server.Start(PORT);

            //Register Events
            listener.ConnectionRequestEvent += OnConnectionRequestEvent;
            listener.NetworkReceiveEvent += OnNetworkReceiveEvent;
            listener.PeerConnectedEvent += OnPeerConnectedEvent;
            listener.PeerDisconnectedEvent += OnPeerDisconnectedEvent;
            listener.NetworkErrorEvent += OnNetworkErrorEvent;

            // Console
            Console.WriteLine("Server Started...");
            Console.WriteLine("|---  CONFIGURATION  ---");
            Console.WriteLine("| > Type: {0}", APPID);
            Console.WriteLine("| > Port: {0}", PORT);
            Console.WriteLine("| > Max. Players: {0}", MAXPLAYERS);
            Console.WriteLine("|--- /CONFIGURATION  ---");
        }

        /// <summary>
        /// Called Each servertick (50ms). Used to do actions on the server
        /// </summary>
        public void OnUpdate()
        {
            server.PollEvents();
        }

        /// <summary>
        /// Sends the map to the specified Player
        /// </summary>
        /// <param name="netConnection">Player</param>
        /// <param name="tiles">map</param>
        public void SendMapToPlayer(NetPeer netConnection, Tile[][] tiles)
        {
            WorldUpdatePacket packet = new WorldUpdatePacket(tiles);
            NetDataWriter msg = new NetDataWriter();
            packet.Encode(msg);
            SendMessageToConnection(netConnection, msg);
        }

        public void SendLogInInfoToPlayer(NetPeer netConnection, string playerName, Faction[] factions)
        {
            LoginPacket packet = new LoginPacket(playerName, factions);
            NetDataWriter msg = new NetDataWriter();
            packet.Encode(msg);
            SendMessageToConnection(netConnection, msg);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Handles player logins and the according callbacks
        /// </summary>
        /// <param name="peer">connected peer</param>
        /// <param name="reader">Netpacket without 1st byte</param>
        private void HandlePlayerLogin(NetPeer peer, NetPacketReader reader)
        {
            LoginPacket loginPacket = new LoginPacket(reader);
            PlayerConnectedEventArgs args1 = new PlayerConnectedEventArgs();
            switch ((LoginMsg)loginPacket.LoginMsgType)
            {
                case LoginMsg.INIT:
                    args1.NetPeer = peer;
                    args1.hasSelected = false;
                    args1.PlayerFaction = (Faction)loginPacket.Faction;
                    args1.PlayerName = loginPacket.PlayerName;
                    break;
                case LoginMsg.FACTIONSELECT:
                    args1.NetPeer = peer;
                    args1.hasSelected = true;
                    args1.PlayerFaction = (Faction)loginPacket.Faction;
                    args1.PlayerName = loginPacket.PlayerName;
                    break;
            }
            //TODO: Change Debug
            //Console Output
            Console.WriteLine("Player {0} login successful. Faction: {1}", loginPacket.PlayerName, (Faction)loginPacket.Faction);

            //Invoke Event
            OnPlayerConnected(args1);
        }

        /// <summary>
        /// Approve connections and invoke PlayerjoinedEvent
        /// </summary>
        /// <param name="inc">Incoming Message</param>
        private void OnConnectionRequestEvent(ConnectionRequest request)
        {
            Console.WriteLine("CONNECTION REQUEST FROM: {0}", request.RemoteEndPoint.ToString());
            if (server.PeersCount < MAXPLAYERS)
            {
                request.AcceptIfKey(APPID);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("CONNECTION ACCEPTED.");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                request.Reject();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("CONNECTION REJECTED.");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        /// Handle Network Errors
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="socketError"></param>
        private void OnNetworkErrorEvent(System.Net.IPEndPoint endPoint, System.Net.Sockets.SocketError socketError)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("NETWORKERROR: {0}", socketError.GetTypeCode());
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Called whenever a new non-library packet is received
        /// </summary>
        /// <param name="peer">connected peer</param>
        /// <param name="reader">NetPacket</param>
        /// <param name="deliveryMethod">Deliverymethod of the received packet</param>
        private void OnNetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            PacketTypes packetType = (PacketTypes)reader.GetByte();

            switch (packetType)
            {
                case PacketTypes.LOGIN:
                    HandlePlayerLogin(peer, reader);
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("RECEIVED UNKNOWN PACKET.");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            reader.Recycle();
        }

        /// <summary>
        /// Called when a client successfully connected to the server
        /// </summary>
        /// <param name="peer">connected peer</param>
        private void OnPeerConnectedEvent(NetPeer peer)
        {
            Console.WriteLine("PLAYER CONNECTED: {0}", peer.EndPoint.ToString()); // Show peer ip
        }

        /// <summary>
        /// Called when a client disconnected from the server
        /// </summary>
        /// <param name="peer">disconnected peer</param>
        /// <param name="disconnectInfo"></param>
        private void OnPeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Console.WriteLine("PLAYER {0} DISCONNECTED. INFO: {1}", peer.EndPoint.ToString(), disconnectInfo.Reason);
            //Remove Player from PM
            PlayerLeftEventArgs args2 = new PlayerLeftEventArgs();
            args2.NetPeer = peer;
            OnPlayerLeft(args2);
        }

        /// <summary>
        /// Sends the Message to the clientconnection
        /// </summary>
        /// <param name="netConnection">clientConnection</param>
        /// <param name="msg">Outgoing Message</param>
        private void SendMessageToConnection(NetPeer netConnection, NetDataWriter msg)
        {
            netConnection.Send(msg, DeliveryMethod.ReliableOrdered);
        }

        #endregion Private Methods

        //EventInvoker

        #region Protected Methods

        protected virtual void OnPlayerConnected(PlayerConnectedEventArgs e)
        {
            PlayerConnected?.Invoke(this, e);
        }

        protected virtual void OnPlayerLeft(PlayerLeftEventArgs e)
        {
            PlayerLeft?.Invoke(this, e);
        }

        #endregion Protected Methods

        //Events

        #region Public Events

        public event EventHandler<PlayerConnectedEventArgs> PlayerConnected;

        public event EventHandler<PlayerLeftEventArgs> PlayerLeft;

        #endregion Public Events
    }

    //Custom EventArgs

    #region Public Classes

    /// <summary>
    /// Event Arguments for connected players. Now we are ready to send the map
    /// </summary>
    public class PlayerConnectedEventArgs : EventArgs
    {
        #region Public Properties

        public NetPeer NetPeer { get; set; }
        public bool hasSelected { get; set; }
        public Faction PlayerFaction { get; set; }
        public string PlayerName { get; set; }

        #endregion Public Properties
    }

    /// <summary>
    /// Event Arguments for disconnected players. Now we need to clean up the PlayerManager
    /// </summary>
    public class PlayerLeftEventArgs : EventArgs
    {
        #region Public Properties

        public NetPeer NetPeer { get; set; }

        #endregion Public Properties
    }

    #endregion Public Classes
}