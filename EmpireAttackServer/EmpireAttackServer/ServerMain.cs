using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using EmpireAttackServer.TileMap;
using EmpireAttackServer.Networking;
using EmpireAttackServer.Shared;
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
        private static ServerManager Server;

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
            Server = new ServerManager("EA2", 14242, 100);
            Server.Initialize();

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
            //Fast running Loop
            //Fetches Messages to the server and processes them
            Server.OnUpdate();
        }

        #endregion Private Methods
    }
}