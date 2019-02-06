using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using EmpireAttackServer.Players;
using EmpireAttackServer.Shared;

namespace EmpireAttackServer.Networking
{
    class ServerManager
    {
        #region Private Fields

        //App ID to connect the right versions
        private static string APPID;

        // Configuration object
        private static NetPeerConfiguration Config;

        //Max number of connections
        private static int MAXPLAYERS;

        //Holds player objects
        private static PlayerManager playerManager;

        //Port of the server
        private static int PORT;

        // Server object
        private static NetServer Server;

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
            // Create new instance of configs. Parameter is "application Id". It has to be same on client and server.
            Config = new NetPeerConfiguration(APPID);

            // Set server port
            Config.Port = PORT;

            // Max client amount
            Config.MaximumConnections = MAXPLAYERS;

            // Enable Timeout
            Config.ConnectionTimeout = 20.0f;

            // Enable New messagetype. Explained later
            Config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            //Set Timeout
            Config.ConnectionTimeout = 60.0f;

            // Create new server based on the configs just defined
            Server = new NetServer(Config);

            // Start it
            Server.Start();

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
            // Object that can be used to store and read messages
            NetIncomingMessage inc;

            //while loop for incoming packages
            while ((inc = Server.ReadMessage()) != null)
            {
                switch (inc.MessageType)
                {
                    case NetIncomingMessageType.ConnectionApproval:
                        HandleConnectionApproval(inc);
                        break;

                    case NetIncomingMessageType.Data:

                        if (inc.ReadByte() == (byte)PacketTypes.TEST)
                        {
                            //TODO: Find Player Obj
                            //TODO: Add Timer for timeout
                            //TODO: Send response
                            string msg = inc.ReadString();
                            Console.WriteLine("PLAYERMSG: " + msg + ", FROM: " + inc.SenderConnection);

                            NetOutgoingMessage outmsg = Server.CreateMessage();
                            outmsg.Write((byte)PacketTypes.TEST);
                            outmsg.Write("Server answer - ");

                            Server.SendMessage(outmsg, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                        }
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        HandleStatusChanged(inc);
                        break;

                    default:
                        Console.WriteLine("ERROR! --- THIS TYPE OF MESSAGE IS NOT SUPPORTED: " + inc.MessageType.ToString() + ", " + inc.ReadString());
                        break;
                }
            }
        }

        /// <summary>
        /// Sends the map to the specified Player
        /// </summary>
        /// <param name="netConnection">Player</param>
        /// <param name="tiles">map</param>
        public void SendMapToPlayer(NetConnection netConnection, Tile[][] tiles)
        {
            WorldUpdatePacket packet = new WorldUpdatePacket(tiles);
            NetOutgoingMessage msg = Server.CreateMessage();
            packet.Encode(msg);
            SendMessageToConnection(netConnection, msg);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Approve connections and invoke PlayerjoinedEvent
        /// </summary>
        /// <param name="inc">Incoming Message</param>
        private void HandleConnectionApproval(NetIncomingMessage inc)
        {
            // Read the first byte of the packet
            // ( Enums can be casted to bytes, so it be used to make bytes human readable )
            if (inc.ReadByte() == (byte)PacketTypes.LOGIN)
            {
                //TODO: Add Players to Custom Object/ Dictionary
                //TODO: Custom Timeout (Use systemtime)
                //TODO: Shoot answer back if msg is received
                LoginPacket lp = new LoginPacket(inc);
                inc.SenderConnection.Approve();
                Console.WriteLine("NEW PLAYER CONNECTING: " + lp.PlayerName + ", AS: " + (Faction)lp.Faction + ", IP: " + lp.IP);

                //Invoke Event for GameClass & PlayerManager
                PlayerJoinedEventArgs args = new PlayerJoinedEventArgs();
                args.NetConnection = lp.IP;
                args.PlayerName = lp.PlayerName;
                args.PlayerFaction = (Faction)lp.Faction;
                OnPlayerJoined(args);
            }
        }

        /// <summary>
        /// Handle Status updates initiated by Lidgren
        /// </summary>
        /// <param name="inc">Incoming Message</param>
        private void HandleStatusChanged(NetIncomingMessage inc)
        {
            NetConnectionStatus status = (NetConnectionStatus)inc.ReadByte();
            switch (status)
            {
                case NetConnectionStatus.Connected:
                    //Send Map to new Player
                    PlayerConnectedEventArgs args1 = new PlayerConnectedEventArgs();
                    args1.NetConnection = inc.SenderConnection;
                    OnPlayerConnected(args1);
                    break;

                case NetConnectionStatus.Disconnected:
                    //Remove Player from PM
                    PlayerLeftEventArgs args2 = new PlayerLeftEventArgs();
                    args2.NetConnection = inc.SenderConnection;
                    OnPlayerLeft(args2);
                    break;

                case NetConnectionStatus.Disconnecting:
                    //Remove Player from PM
                    PlayerLeftEventArgs args3 = new PlayerLeftEventArgs();
                    args3.NetConnection = inc.SenderConnection;
                    OnPlayerLeft(args3);
                    break;
            }
            Console.WriteLine("STATUS: " + status + ", FOR: " + inc.SenderConnection);
        }

        /// <summary>
        /// Sends the Message to the clientconnection
        /// </summary>
        /// <param name="netConnection">clientConnection</param>
        /// <param name="msg">Outgoing Message</param>
        private void SendMessageToConnection(NetConnection netConnection, NetOutgoingMessage msg)
        {
            Server.SendMessage(msg, netConnection, NetDeliveryMethod.ReliableOrdered);
        }

        #endregion Private Methods

        //EventInvoker

        #region Protected Methods

        protected virtual void OnPlayerConnected(PlayerConnectedEventArgs e)
        {
            PlayerConnected?.Invoke(this, e);
        }

        /// <summary>
        /// Delegate call
        /// </summary>
        /// <param name="e">PlayerJoined EventArgs</param>
        protected virtual void OnPlayerJoined(PlayerJoinedEventArgs e)
        {
            PlayerJoined?.Invoke(this, e);
        }

        protected virtual void OnPlayerLeft(PlayerLeftEventArgs e)
        {
            PlayerLeft?.Invoke(this, e);
        }

        #endregion Protected Methods

        //Events

        #region Public Events

        public event EventHandler<PlayerConnectedEventArgs> PlayerConnected;

        public event EventHandler<PlayerJoinedEventArgs> PlayerJoined;

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

        public NetConnection NetConnection { get; set; }

        #endregion Public Properties
    }

    /// <summary>
    /// Event Arguments for joining Players
    /// </summary>
    public class PlayerJoinedEventArgs : EventArgs
    {
        #region Public Properties

        public NetConnection NetConnection { get; set; }
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

        public NetConnection NetConnection { get; set; }

        #endregion Public Properties
    }

    #endregion Public Classes
}