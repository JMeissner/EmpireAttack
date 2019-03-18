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
using EmpireAttackServer.Players;

namespace EmpireAttackServer
{
    class ServerMain
    {
        #region Private Fields

        private static Game gameInstance;
        private static PlayerManager playerManager;
        private static Timer gameTimer;

        // Server object
        private static ServerManager Server;

        private static Timer serverTimer;

        #endregion Private Fields

        #region Public Fields

        public static int GameTicks;
        public static int NumberOfFactions = 2;
        public static List<Faction> AvailableFactions;

        #endregion Public Fields

        #region Private Methods

        private static void Initialize()
        {
            //TODO: Map initialization
            AvailableFactions = new List<Faction>();
            AvailableFactions.Add(Faction.Eagles);
            AvailableFactions.Add(Faction.Baguette);

            //TODO: Server startup sequence
            Server = new ServerManager("EA2", 14242, 10);
            Server.PlayerConnected += OnPlayerConnected;
            Server.PlayerLeft += OnPlayerLeft;
            Server.Initialize();

            //Initialize GameInstance
            GameTicks = 0;
            gameInstance.Initialize(1);

            //Initialize PlayerManager
            playerManager = new PlayerManager();

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
            gameInstance = new Game();
            Initialize();

            Console.ReadLine();
        }

        private static void OnGameUpdate(Object sender, ElapsedEventArgs e)
        {
            GameTicks += 1;
            //Console.WriteLine("Update at {0:HH:mm:ss.fff}", e.SignalTime);
            //gameInstance.Update();
            //Console.WriteLine("Update took {0} ms", (DateTime.Now - e.SignalTime));
        }

        private static void OnPlayerConnected(Object sender, PlayerConnectedEventArgs e)
        {
            //INIT Phase
            if(!e.hasSelected)
            {
                Faction playerFaction = playerManager.GetFactionFromName(e.PlayerName);
                if (playerFaction == Faction.NONE)
                {
                    //Send Selection
                    Server.SendLogInInfoToPlayer(e.NetPeer, e.PlayerName, AvailableFactions.ToArray());
                } else
                {
                    //Send Previous Faction
                    Server.SendLogInInfoToPlayer(e.NetPeer, e.PlayerName, new Faction[1] { playerFaction });
                }
            }
            //FACTIONSELECT Phase
            else
            {
                playerManager.AddPlayer(e.PlayerName, e.PlayerFaction, e.NetPeer);
                Console.WriteLine("Sending map to player: {0}...", e.PlayerName);

                Server.SendMapToPlayer(e.NetPeer, gameInstance.GetTileMap());
            }
        }

        private static void OnPlayerLeft(Object sender, PlayerLeftEventArgs e)
        {
            playerManager.RemovePlayer(e.NetPeer);
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            Console.WriteLine("Shutting down Server...");
        }

        private static void OnServerUpdate(Object sender, ElapsedEventArgs e)
        {
            //Fast running Loop
            //Fetches Messages to the server and processes them
            Server.OnUpdate();
        }

        #endregion Private Methods
    }
}