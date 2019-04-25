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
using static EmpireAttackServer.Game;

namespace EmpireAttackServer
{
    class ServerMain
    {
        #region Private Fields

        private static Game gameInstance;
        private static PlayerManager playerManager;
        private static Timer gameTimer;
        private static Timer lateGameTimer;

        // Server object
        private static ServerManager Server;

        private static Timer serverTimer;
        private static Timer syncTimer;
        private static Timer matchTimer;

        #endregion Private Fields

        #region Public Fields

        //STATIC GAME PARAMETERS
        public static int GameTicks;
        public static int NumberOfFactions = 2;
        public static List<Faction> AvailableFactions;
        public static int GameTime = 900;
        public static int GameTickRate = 500;
        public static int ServerTickRate = 50;
        public static int SyncRate = 600000;

        #endregion Public Fields

        #region Private Methods

        private static void Initialize()
        {
            //TODO: Map initialization
            AvailableFactions = new List<Faction>();
            for(int i = 0; i < NumberOfFactions; i++)
            {
                int f = 0;
                do
                {
                    Random r = new Random();
                    f = r.Next(1, Enum.GetNames(typeof(Faction)).Length);
                } while (AvailableFactions.Contains((Faction)f));
                AvailableFactions.Add((Faction)f);
            }

            //TODO: Server startup sequence
            Server = new ServerManager("EA2", 14242, 10);
            Server.PlayerConnected += OnPlayerConnected;
            Server.PlayerLeft += OnPlayerLeft;
            Server.DeltaUpdateReceived += OnDeltaUpdateReceived;
            Server.Initialize();

            //Initialize GameInstance
            gameInstance = new Game();
            GameTicks = 0;
            gameInstance.Initialize(1, AvailableFactions);
            gameInstance.UpdatePopulation += OnUpdatePopulation;
            gameInstance.ReSync += OnReSync;

            //Initialize PlayerManager
            playerManager = new PlayerManager();

            //Setup Game Update Routine timer to update every 500ms
            gameTimer = new System.Timers.Timer(GameTickRate);
            gameTimer.Elapsed += OnGameUpdate;
            gameTimer.AutoReset = true;
            gameTimer.Enabled = true;

            //Setup Server Update Routine timer to update every 50ms
            serverTimer = new System.Timers.Timer(ServerTickRate);
            serverTimer.Elapsed += OnServerUpdate;
            serverTimer.AutoReset = true;
            serverTimer.Enabled = true;

            //Setup LateGame Update Routine timer to update every 25th GameTick
            lateGameTimer = new System.Timers.Timer(GameTickRate * 25);
            lateGameTimer.Elapsed += OnLateUpdate;
            lateGameTimer.AutoReset = true;
            lateGameTimer.Enabled = true;

            //Setup Sync Routine timer to update on Syncrate
            syncTimer = new System.Timers.Timer(SyncRate);
            syncTimer.Elapsed += OnSync;
            syncTimer.AutoReset = true;
            syncTimer.Enabled = true;

            //Setup Match End timer to end the match after the given amount of time
            matchTimer = new System.Timers.Timer(GameTime);
            matchTimer.Elapsed += OnMatchEnd;
            matchTimer.Enabled = true;
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
            Initialize();

            Console.ReadLine();
        }

        private static void OnGameUpdate(Object sender, ElapsedEventArgs e)
        {
            GameTicks += 1;
            //Console.WriteLine("Update at {0:HH:mm:ss.fff}", e.SignalTime);
            gameInstance.Update();
            //Console.WriteLine("Update took {0} ms", (DateTime.Now - e.SignalTime));
        }

        private static void OnLateUpdate(Object sender, ElapsedEventArgs e)
        {
            gameInstance.LateUpdate();
        }

        private static void OnReSync(Object sender, EventArgs e)
        {
            //Sync map to all players after LateUpdate. Might want to use DELTA instead on large/sparse maps
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("RESYNC PLAYERS...");
            Console.ForegroundColor = ConsoleColor.White;
            Server.SyncAllPlayers(gameInstance.GetTileMap());
        }

        private static void OnSync(Object sender, ElapsedEventArgs e)
        {
            //Not in use right now. Use ReSync instead
        }

        private static void OnMatchEnd(Object sender, ElapsedEventArgs e)
        {

        }

        private static void OnPlayerConnected(Object sender, PlayerConnectedEventArgs e)
        {
            //TODO: rework Login Behavior
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

        private static void OnUpdatePopulation(Object sender, UpdatePopulationEventArgs e)
        {
            //Population update triggered by the game
            Server.SendPopulationUpdateToAll(e.faction, e.amount);
        }

        private static void OnDeltaUpdateReceived(Object sender, DeltaUpdateReceivedArgs e)
        {
            //Send deltaupdate to all connected clients, if the move was legal
            if(gameInstance.OccupyFromDelta(e.PlayerFaction, e.FieldX, e.FieldY, e.Population))
            {
                //Tile is now occupied by attacking force
                Server.SendDeltaUpdateToAll(e.PlayerFaction, e.FieldX, e.FieldY, gameInstance.GetPopulationOnTile(e.FieldX, e.FieldY));
                return;
            }
            if(gameInstance.PopulationFromDelta(e.PlayerFaction, e.FieldX, e.FieldY, e.Population))
            {
                //Tile is not occupied by attacking force => update population on tile
                Server.SendDeltaUpdateToAll(gameInstance.GetFactionOnTile(e.FieldX, e.FieldY), e.FieldX, e.FieldY, gameInstance.GetPopulationOnTile(e.FieldX, e.FieldY));
            }
        }

        #endregion Private Methods
    }
}