using EmpireAttackServer.Networking;
using EmpireAttackServer.Players;
using EmpireAttackServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpireAttackServer
{
    /// <summary>
    /// Connects Game and Server and handles crossover commands
    /// </summary>
    class Root
    {
        private Game GameInstance;
        private ServerManager Server;
        private PlayerManager PlayerManager;

        public int GameTicks;
        public Status GameStatus;

        public Root()
        {
            GameTicks = 0;
            GameStatus = Status.LOBBY;
            WriteLogo();
        }

        #region STARTUP METHODS
        public void StartServer(String ServerName, int Port, int MaxPlayers)
        {
            Server = new ServerManager(this, ServerName, Port, MaxPlayers);
            Server.Initialize();
        }

        public void StartGame(int MapLoadType, int NumberOfFactions)
        {
            GameInstance = new Game(this);
            GameInstance.Initialize(MapLoadType, NumberOfFactions);
        }

        public void StartPlayerManager()
        {
            PlayerManager = new PlayerManager();
        }
        #endregion

        #region UPDATE METHODS

        public void ServerUpdate()
        {
            //Fast running Loop
            //Fetches Messages to the server and processes them
            Server.OnUpdate();
        }

        /// <summary>
        /// Runs the GameUpdate Loop
        /// </summary>
        public void GameUpdate()
        {
            GameTicks++;
            GameInstance.Update();
            //TODO: Update Population from here
        }

        /// <summary>
        /// Runs the Game LateUpdate Loop
        /// </summary>
        public void GameLateUpdate(DateTime startTime)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("GAME>>UPDATING MAP...");
            Console.ForegroundColor = ConsoleColor.White;
            GameInstance.LateUpdate();
            //Sync map to all players after LateUpdate. Might want to use DELTA instead on large/sparse maps
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("RESYNC PLAYERS... Update took: {0}ms", (DateTime.Now - startTime));
            Console.ForegroundColor = ConsoleColor.White;
            Server.SyncAllPlayers(GameInstance.GetTileMap());
        }

        public void MatchEnded()
        {
            //TODO: Send Info who won etc, then wait for restart
        }

        #endregion

        #region REMOTEPLAYER METHODS

        public void PlayerConnected()
        {

        }

        public void PlayerJoined()
        {

        }

        public void PlayerLeft()
        {

        }

        #endregion

        /// <summary>
        /// Checks validity of DeltaUpdate and Send response if valid
        /// </summary>
        /// <param name="FieldX">coord X</param>
        /// <param name="FieldY">coord Y</param>
        /// <param name="faction">attacking faction</param>
        /// <param name="Population">attacking population</param>
        public void ProcessDelta(int FieldX, int FieldY, Faction faction, int Population)
        {
            //Send deltaupdate to all connected clients, if the move was legal
            if (GameInstance.OccupyFromDelta(faction, FieldX, FieldY, Population))
            {
                //Tile is now occupied by attacking force
                Server.SendDeltaUpdateToAll(faction, FieldX, FieldY, GameInstance.GetPopulationOnTile(FieldX, FieldY));
                return;
            }
            if (GameInstance.PopulationFromDelta(faction, FieldX, FieldY, Population))
            {
                //Tile is not occupied by attacking force => update population on tile
                Server.SendDeltaUpdateToAll(GameInstance.GetFactionOnTile(FieldX, FieldY), FieldX, FieldY, GameInstance.GetPopulationOnTile(FieldX, FieldY));
            }
        }

        /// <summary>
        /// TODO: Checks to see if enough players are in the game to start it
        /// </summary>
        private void CheckGameStart()
        {

        }

        /// <summary>
        /// Writes a Logo in ASCII to the console
        /// </summary>
        private void WriteLogo()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(@"                 _____  ____              _____                  /\                    ");
            Console.WriteLine(@"               _/ ____\/_   |__  __ _____/ ____\____  __ ________)/ ______             ");
            Console.WriteLine(@"               \   __\  |   \  \/ // __ \   __\/  _ \|  |  \_  __ \/  ___/             ");
            Console.WriteLine(@"                |  |    |   |\   /\  ___/|  | (  <_> )  |  /|  | \/\___ \              ");
            Console.WriteLine(@"                |__|    |___| \_/  \___  >__|  \____/|____/ |__|  /____  >             ");
            Console.WriteLine(@"                                       \/                              \/              ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(@"___________              .__                    _____   __    __                 __    ");
            Console.WriteLine(@"\_   _____/ _____ ______ |__|______   ____     /  _  \_/  |__/  |______    ____ |  | __");
            Console.WriteLine(@" |    __)_ /     \\____ \|  \_  __ \_/ __ \   /  /_\  \   __\   __\__  \ _/ ___\|  |/ /");
            Console.WriteLine(@" |        \  Y Y  \  |_> >  ||  | \/\  ___/  /    |    \  |  |  |  / __ \\  \___|    < ");
            Console.WriteLine(@"/_______  /__|_|  /   __/|__||__|    \___  > \____|__  /__|  |__| (____  /\___  >__|_ \");
            Console.WriteLine(@"        \/      \/|__|                   \/          \/                \/     \/     \/");
            Console.WriteLine(@"_______________________________________________________________________________________");
        }
    }
}
