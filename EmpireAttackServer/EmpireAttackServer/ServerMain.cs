using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using EmpireAttackServer.TileMap;
using EmpireAttackServer.Networking;

namespace EmpireAttackServer
{
    class ServerMain
    {
        #region Private Fields

        private static Game gameInstance;
        private static Timer gameTimer;
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

            GameTicks = 0;
            gameInstance.Initialize(1);

            //Setup Game Update Routine timer to update every 500ms
            gameTimer = new System.Timers.Timer(500);
            gameTimer.Elapsed += OnGameUpdate;
            gameTimer.AutoReset = true;
            gameTimer.Enabled = true;

            //Setup Server Update Routine timer to update every 50ms
            serverTimer = new System.Timers.Timer(500);
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
            MapPNGImport map = new MapPNGImport(AppDomain.CurrentDomain.BaseDirectory + @"/Map.png");

            //Add Handler for application exit etc to free bound ressources
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            //TODO: Specify args for startup -> Import Config
            gameInstance = new Game(10);
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

        private static void OnServerUpdate(Object source, ElapsedEventArgs )
        {
        }

        #endregion Private Methods
    }
}