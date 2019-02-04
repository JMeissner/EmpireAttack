using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using EmpireAttackServer.Networking.NetworkMessages;
using EmpireAttackServer.Players;

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

        public ServerManager(PlayerManager pm, string APPID, int PORT, int MAXPLAYERS)
        {
            playerManager = pm;
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
                            //outmsg.Write("Server answer - " + GameTicks);

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

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Approve connections and add Players to PM
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
                Console.WriteLine("NEW PLAYER CONNECTION: " + lp.PlayerName + ", AS: " + (Faction)lp.Faction + ", IP: " + lp.IP);
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
                    break;

                case NetConnectionStatus.Disconnected:
                    //Remove Player from PM
                    break;

                case NetConnectionStatus.Disconnecting:
                    //Remove Player from PM
                    break;
            }
            Console.WriteLine("STATUS: " + (NetConnectionStatus)inc.ReadByte() + ", FOR: " + inc.SenderConnection);
        }

        #endregion Private Methods
    }
}