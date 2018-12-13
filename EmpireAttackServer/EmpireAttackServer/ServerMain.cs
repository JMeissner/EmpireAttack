using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace EmpireAttackServer
{
    class ServerMain
    {
        #region Private Fields

        private static Game gameInstance;
        private static Timer updateTimer;

        #endregion Private Fields

        #region Private Methods

        private static void Main(string[] args)
        {
            //TODO: Specify args for startup
            gameInstance = new Game(10);
            SetUpUpdate();

            Console.ReadLine();
        }

        private static void OnUpdate(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("Update at {0:HH:mm:ss.fff}", e.SignalTime);
            gameInstance.Update();
            Console.WriteLine("Update took {0} ms", (DateTime.Now - e.SignalTime));
        }

        private static void SetUpUpdate()
        {
            //TODO: Map initialization
            //TODO: Server startup sequence

            //Setup Update Routine timer to update every 500ms
            updateTimer = new System.Timers.Timer(500);
            updateTimer.Elapsed += OnUpdate;
            updateTimer.AutoReset = true;
            updateTimer.Enabled = true;
        }

        #endregion Private Methods
    }
}