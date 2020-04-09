using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleWorldMapPlotter
{
    class Program
    {
        private static string _aircraftCallsign = string.Empty;
        private static string _afTimeRange = string.Empty;

        private static Task<string> _aircraftCallsignInputTask;
        private static Task<string> _afTimeRangeInputTask;
        private static Task<string> _mappingInputTask;

        private static CancellationTokenSource _mapRunnerTaskCts;
        private static CancellationTokenSource _pointPlotterTaskCts;

        private static bool _mapIsRunning = false;
        private static bool _atSplashScreen = true;

        static void Main(string[] args)
        {
            // DEBUG HACK -- MAXIMIZE CONSOLE WINDOW
            Maximize();
            //Console.SetWindowSize(150, 60);

            // start off with the callsign!
            _aircraftCallsignInputTask = AsyncConsoleReader.ReadLine();

            SplashScreen.Display(doItSlow: true);

            //createMapRunnerTask();
            //WorldMapPlotter_GenerateData.Generate();

            //Infinite loop to contiously keep reading in user input for callsign and time interval.
            while (true)
            {

                /*
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("============================================================================");
                Console.WriteLine("Aircraft Callsign: ");
                CallSign = Console.ReadLine();
                Console.WriteLine("AFTime Range: ");
                TimeInterval = Console.ReadLine();
                Console.WriteLine("============================================================================");
                */

                //_userInputTask = AsyncConsoleReader.ReadLine();

                // ONLY RUN THE SPLASH SCREEN IF WE'VE PRESSED [ENTER] DURING MAPPING

                handleUserInput();

                if (!_mapIsRunning && !_atSplashScreen)
                {
                    // if mapping tasks are !null, then don't create them again
                    //   because the mapping feature is probably already running,
                    //   but, otherwise create them
                    createMapRunnerTask();
                }

            }



            
            //WorldMapPositionConverter_GenerateData.Generate();

            Console.Read();
        }

        public static void handleUserInput()
        {
            handleAircraftCallsignInput();

            handleAfTimeRangeInput();

            handleMappingInput();
        }

        public static void handleMappingInput()
        {
            // HANDLE MAP INPUT
            if (_mappingInputTask != null
                && _mappingInputTask.IsCompleted)
            {
                // EXIT MAP LOGIC
                // -- only [ENTER] key was pressed without any textual input,
                // so let's redisplay the splash screen, and cancel mapping tasks
                if (_mappingInputTask.Result == "")
                {
                    // cancel mapping and point plotting tasks
                    _mapRunnerTaskCts.Cancel();
                    _pointPlotterTaskCts.Cancel();
                    _mapIsRunning = false;
                    _mappingInputTask = null;

                    // redisplay the start screen, but let's not take our time about it
                    SplashScreen.Display(doItSlow: false);

                    _atSplashScreen = true;

                    // need this to be null for above conditional to work
                    _afTimeRangeInputTask = null;

                    // capture user commands again
                    _aircraftCallsignInputTask = AsyncConsoleReader.ReadLine();
                }
            }
        }

        public static void handleAircraftCallsignInput()
        {
            if (_aircraftCallsignInputTask.IsCompleted
                && _afTimeRangeInputTask == null)
            {
                // looks like we just got the callsign from the user
                _aircraftCallsign = _aircraftCallsignInputTask.Result;

                // now, let's get the afTimeRange
                _afTimeRangeInputTask = AsyncConsoleReader.ReadLine();
            }
        }

        public static void handleAfTimeRangeInput()
        {
            if (_aircraftCallsignInputTask.IsCompleted
                && _afTimeRangeInputTask != null
                && _afTimeRangeInputTask.IsCompleted
                && _mappingInputTask == null)
            {

                //START MAP LOGIC

                // looks like we just got the afTimeRange from the user
                _afTimeRange = _afTimeRangeInputTask.Result;

                // that completes our input requirements, sooooooo

                // user must want a map
                createMapRunnerTask();

                _atSplashScreen = false;

                // let user hit [ENTER] to kill the map
                _mappingInputTask = AsyncConsoleReader.ReadLine();
            }
        }

        public static void createMapRunnerTask()
        {
            // initialize the cancellation routine, so we can make it all stop
            _mapRunnerTaskCts = new CancellationTokenSource();
            _pointPlotterTaskCts = new CancellationTokenSource();

            // reload the map from file (basically initializes the static class -- bleh. TODO: fix that)
            WorldMapPlotter.LoadMapFromFile();

            // run that map
            WorldMapPlotter.RunMap(_mapRunnerTaskCts.Token);

            // DEBUG -- leave it commented.
            WorldMapPlotter_GenerateData.Generate(_pointPlotterTaskCts.Token);

            _mapIsRunning = true;
        }



        // BELOW IS JUST A DEBUG HACK TO MAXIMIZE THE CONSOLE

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(System.IntPtr hWnd, int cmdShow);

        private static void Maximize()
        {
            Process p = Process.GetCurrentProcess();
            ShowWindow(p.MainWindowHandle, 3); //SW_MAXIMIZE = 3
        }
    }
}
