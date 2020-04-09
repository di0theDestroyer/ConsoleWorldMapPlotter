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

        private static Task _mapRunnerTask;
        private static CancellationTokenSource _mapRunnerTaskCts;

        static void Main(string[] args)
        {
            // DEBUG HACK -- MAXIMIZE CONSOLE WINDOW
            Maximize();
            //Console.SetWindowSize(150, 60);

            _userInputTask = AsyncConsoleReader.ReadLine();

            //SplashScreen.Display(doItSlow: true);

            createMapRunnerTask();
            WorldMapPlotter_GenerateData.Generate();

            //Infinite loop to contiously keep reading in user input for callsign and time interval.
            while (false)
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
                        // cancel mapping tasks
                        _mapRunnerTaskCts.Cancel();

                        // important to set task to null since we check below
                        _mapRunnerTask = null;

                        // redisplay the start screen, but let's not take our time about it
                        SplashScreen.Display(doItSlow: false);
                    }
                    else
                    {
                        // user must want a map
                        createMapRunnerTask();

                        // DEBUG -- leave it commented.
                        WorldMapPlotter_GenerateData.Generate();
                    }

                }
                else
                {
                    // if mapping tasks are !null, then don't create them again
                    //   because the mapping feature is probably already running,
                    //   but, otherwise create them
                    if (_mapRunnerTask != null)
                    {
                        createMapRunnerTask();
                    }


                }



                //Console.Clear();
            }



            
            //WorldMapPositionConverter_GenerateData.Generate();

            Console.Read();
        }

        public static void createMapRunnerTask()
        {
            _mapRunnerTaskCts = new CancellationTokenSource();

            // reload the map from file (basically initializes the static class -- bleh. TODO: fix that)
            WorldMapPlotter.LoadMapFromFile();

            // run that map
            WorldMapPlotter.RunMap(_mapRunnerTaskCts.Token);
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
