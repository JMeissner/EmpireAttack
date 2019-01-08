using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using EmpireAttackServer.TileMap;
using EmpireAttackServer.Networking;
using EmpireAttackServer.Networking.NetworkMessages;
using Lidgren.Network;

namespace EmpireAttackServer
{
    class ServerMain
    {
        #region Private Fields

        // Configuration object
        private static NetPeerConfiguration Config;

        private static Game gameInstance;
        private static Timer gameTimer;

        // Server object
        private static NetServer Server;

        private static Timer serverTimer;

        #endregion Private Fields

        #region Public Fields

        public static int GameTicks;

        #endregion Public Fields

        #region Private Methods

        private static void Initialize()
        {
            //TODO: Map initialization
            //TODO: Server startup sequence
            InitializeServer();

            GameTicks = 0;
            //gameInstance.Initialize(1);

            //Setup Game Update Routine timer to update every 500ms
            gameTimer = new System.Timers.Timer(500);
            gameTimer.Elapsed += OnGameUpdate;
            gameTimer.AutoReset = true;
            gameTimer.Enabled = true;

            //Setup Server Update Routine timer to update every 50ms
            serverTimer = new System.Timers.Timer(50);
            serverTimer.Elapsed += OnServerUpdate;
            serverTimer.AutoReset = true;
            serverTimer.Enabled = true;
        }

        private static void InitializeServer()
        {
            // Create new instance of configs. Parameter is "application Id". It has to be same on client and server.
            Config = new NetPeerConfiguration("game");

            // Set server port
            Config.Port = 14242;

            // Max client amount
            Config.MaximumConnections = 200;

            // Enable New messagetype. Explained later
            Config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            // Create new server based on the configs just defined
            Server = new NetServer(Config);

            // Start it
            Server.Start();

            // Console
            Console.WriteLine("Server Started");
            Console.WriteLine("---  CONFIGURATION  ---");
            Console.WriteLine("> Type: game");
            Console.WriteLine("> Port: 14242");
            Console.WriteLine("> Max. Players: 200");
            Console.WriteLine("--- /CONFIGURATION  ---");
        }

        /// <summary>
        /// Programm entry
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            //TEST REMOVE ASAP
            //MapPNGImport map = new MapPNGImport(AppDomain.CurrentDomain.BaseDirectory + @"/Map.png");

            //Add Handler for application exit etc to free bound ressources
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            //TODO: Specify args for startup -> Import Config
            //gameInstance = new Game(10);
            Initialize();

            Console.ReadLine();
        }

        private static void OnGameUpdate(Object source, ElapsedEventArgs e)
        {
            GameTicks += 1;
            //Console.WriteLine("Update at {0:HH:mm:ss.fff}", e.SignalTime);
            //gameInstance.Update();
            //Console.WriteLine("Update took {0} ms", (DateTime.Now - e.SignalTime));
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            Console.WriteLine("Shutting down Server...");
        }

        private static void OnServerUpdate(Object source, ElapsedEventArgs e)
        {
            // Object that can be used to store and read messages
            NetIncomingMessage inc;

            //while loop for incoming packages
            while ((inc = Server.ReadMessage()) != null)
            {
                switch (inc.MessageType)
                {
                    case NetIncomingMessageType.ConnectionApproval:

                        // Read the first byte of the packet
                        // ( Enums can be casted to bytes, so it be used to make bytes human readable )
                        if (inc.ReadByte() == (byte)PacketTypes.LOGIN)
                        {
                            //TODO: Add Players to Custom Object/ Dictionary
                            //TODO: Custom Timeout (Use systemtime)
                            //TODO: Shoot answer back if msg is received
                            inc.SenderConnection.Approve();
                            string pName = inc.ReadString();
                            Console.WriteLine("NEW PLAYER CONNECTION: " + pName + ", IP: " + inc.SenderConnection);

                            //NetOutgoingMessage outmsg = Server.CreateMessage();
                            //outmsg.Write((byte)PacketTypes.TEST);
                            //outmsg.Write("Server says Hi!");

                            //Server.SendMessage(outmsg, inc.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                        }
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
                            outmsg.Write("Server answer - " + GameTicks);

                            Server.SendMessage(outmsg, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                        }
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        Console.WriteLine("STATUS: " + (NetConnectionStatus)inc.ReadByte() + ", FOR: " + inc.SenderConnection);
                        break;

                    default:
                        Console.WriteLine("ERROR! --- THIS TYPE OF MESSAGE IS NOT SUPPORTED: " + inc.MessageType.ToString() + ", " + inc.ReadString());
                        break;
                }
            }
        }

        #endregion Private Methods
    }
}