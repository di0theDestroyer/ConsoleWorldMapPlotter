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
        private static bool _aircraftCallsignInputCursorSet;

        private static Task<string> _afTimeRangeInputTask;
        private static bool _afTimeRangeInputCursorSet;

        private static Task<string> _mappingInputTask;

        private static CancellationTokenSource _mapRunnerTaskCts;
        private static CancellationTokenSource _pointPlotterTaskCts;

        private static bool _userInputPromptDisplayed = false;

        static void Main(string[] args)
        {
            // DEBUG HACK -- MAXIMIZE CONSOLE WINDOW
            // TODO: Configure screen size on app startup
            //Console.SetWindowSize(150, 60);
            Maximize();

            // awww yea. doItSlow.
            SplashScreen.Display(doItSlow: true);

            // let's start things off right and get the callsign!
            // note: all user input after this is handled with flags
            //       in handleUserInput() and helper methods
            _aircraftCallsignInputTask = AsyncConsoleReader.ReadLine();

            //Infinite loop that allows the user to switch back and forth
            //  between the splash screen and mapping dialogs
            while (true)
            {
                handleUserInput();
            }
        }

        public static void handleUserInput()
        {
            displayUserInputPrompt();

            handleAircraftCallsignInput();

            handleAfTimeRangeInput();

            handleMappingInput();
        }

        public static void displayUserInputPrompt()
        {
            if (!_userInputPromptDisplayed)
            {
                var callsignPrompt = new Tuple<string, int, int>("Aircraft Callsign ----> ", 0, 45);
                AsyncConsoleWriter.Write(callsignPrompt);

                var afTimeRangePrompt = new Tuple<string, int, int>("AF Time Range --------> ", 0, 46);
                AsyncConsoleWriter.Write(afTimeRangePrompt);

                _userInputPromptDisplayed = true;
            }

            // put the cursor in the correct spot for user input
            if (!_aircraftCallsignInputTask.IsCompleted
                && !_aircraftCallsignInputCursorSet)
            {
                // TODO: hack to overcome race condition of AsyncConsoleWriter
                Thread.Sleep(200);

                Console.SetCursorPosition(24, 45);

                // make the cursor visible again after the splash screen loads
                //   and after we've already set the cursor position, otherwise
                //   the user will see it jump.
                Console.CursorVisible = true;

                _aircraftCallsignInputCursorSet = true;
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

            // put the cursor in the correct spot for user input
            if (_aircraftCallsignInputTask.IsCompleted
                && !_afTimeRangeInputTask.IsCompleted
                && !_afTimeRangeInputCursorSet)
            {
                // TODO: hack to overcome race condition of AsyncConsoleWriter
                Thread.Sleep(200);

                Console.SetCursorPosition(24, 46);

                // make the cursor visible again after the splash screen loads
                //   and after we've already set the cursor position, otherwise
                //   the user will see it jump.
                Console.CursorVisible = true;

                _afTimeRangeInputCursorSet = true;
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

                // let user hit [ENTER] to kill the map
                _mappingInputTask = AsyncConsoleReader.ReadLine();
            }
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
                    _mappingInputTask = null;

                    // redisplay the start screen, but let's not take our time about it
                    SplashScreen.Display(doItSlow: false);

                    // gotta display the user prompt again
                    _userInputPromptDisplayed = false;
                    _aircraftCallsignInputCursorSet = false;
                    _afTimeRangeInputCursorSet = false;
                    displayUserInputPrompt();

                    // need this to be null for above conditional to work
                    _afTimeRangeInputTask = null;

                    // capture user commands again
                    _aircraftCallsignInputTask = AsyncConsoleReader.ReadLine();
                }
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
            WorldMapPlotter_GenerateData.Generate(
                _pointPlotterTaskCts.Token, 
                useLatLongGenerator: true
            );
        }

        // EVERYTHING BELOW IS JUST A DEBUG HACK TO MAXIMIZE THE CONSOLE -- NOT NECESSARY IN PROD CODE

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(System.IntPtr hWnd, int cmdShow);

        private static void Maximize()
        {
            Process p = Process.GetCurrentProcess();
            ShowWindow(p.MainWindowHandle, 3); //SW_MAXIMIZE = 3
        }
    }
}
