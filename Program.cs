using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleWorldMapPlotter
{
    class Program
    {
        private static string CallSign = string.Empty;
        private static string TimeInterval = string.Empty;

        private static Task<string> _userInputTask;

        private static CancellationTokenSource _mapRunnerTaskCts;
        private static CancellationTokenSource _pointPlotterTaskCts;

        private static bool _mapIsRunning = false;
        private static bool _atSplashScreen = true;

        static void Main(string[] args)
        {
            // DEBUG HACK -- MAXIMIZE CONSOLE WINDOW
            Maximize();
            //Console.SetWindowSize(150, 60);

            _userInputTask = AsyncConsoleReader.ReadLine();

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

                if (_userInputTask.IsCompleted)
                {
                    // only [ENTER] key was pressed without any input
                    // so let's redisplay the splash screen, and cancel mapping tasks
                    if (_userInputTask.Result == "")
                    {
                        // cancel mapping and point plotting tasks
                        _mapRunnerTaskCts.Cancel();
                        _pointPlotterTaskCts.Cancel();
                        _mapIsRunning = false;

                        // redisplay the start screen, but let's not take our time about it
                        SplashScreen.Display(doItSlow: false);

                        _atSplashScreen = true;

                        // capture user commands again
                        _userInputTask = AsyncConsoleReader.ReadLine();
                    }
                    else
                    {
                        // user must want a map
                        createMapRunnerTask();

                        _atSplashScreen = false;

                        // let user hit [ENTER] to kill the map
                        _userInputTask = AsyncConsoleReader.ReadLine();
                    }

                }
                else
                {
                    if (!_mapIsRunning && !_atSplashScreen)
                    {
                        // if mapping tasks are !null, then don't create them again
                        //   because the mapping feature is probably already running,
                        //   but, otherwise create them
                        createMapRunnerTask();

                        _atSplashScreen = false;
                    }

                }



                //Console.Clear();
            }



            
            //WorldMapPositionConverter_GenerateData.Generate();

            Console.Read();
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
