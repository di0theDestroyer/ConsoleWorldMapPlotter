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
        private static string _afStartTime = string.Empty;
        private static string _afEndTime = string.Empty;

        private static bool _userInputPromptDisplayed = false;

        private static Task<string> _aircraftCallsignInputTask;
        private static bool _aircraftCallsignInputCursorSet;

        private static Task<string> _afStartTimeInputTask;
        private static bool _afStartTimeInputCursorSet;

        private static Task<string> _afEndTimeInputTask;
        private static bool _afEndTimeInputCursorSet;

        private static Task<string> _mappingInputTask;
        private static CancellationTokenSource _mapRunnerTaskCts;
        private static CancellationTokenSource _pointPlotterTaskCts;
        private static bool _isMappingStarted;

        private static PlaneDataProvider _planeDataProvider;

        private const bool GENERATE_REAL_DATA = false;

        static void Main(string[] args)
        {
            //PlaneDataProvider_Test.Go();

            startTheApp();
        }

        private static void startTheApp()
        {
            // initialize PlaneDataProvider (this is expensive ~2s, so do it first)
            if (GENERATE_REAL_DATA)
            {
                _planeDataProvider = new PlaneDataProvider();
            }
            
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

            handleAfStartTimeInput();

            handleAfEndTimeInput();

            handleMappingInput();
        }

        public static void displayUserInputPrompt()
        {
            if (!_userInputPromptDisplayed)
            {
                var callsignPrompt = new Tuple<string, int, int>("Aircraft Callsign ----> ", 0, 45);
                AsyncConsoleWriter.Write(callsignPrompt);

                var afStartTimePrompt = new Tuple<string, int, int>("AF Start Time --------> ", 0, 46);
                AsyncConsoleWriter.Write(afStartTimePrompt);

                var afEndTimePrompt = new Tuple<string, int, int>("AF End Time ----------> ", 0, 47);
                AsyncConsoleWriter.Write(afEndTimePrompt);

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
                && _afStartTimeInputTask == null
                && !_afStartTimeInputCursorSet)
            {
                // TODO: hack to overcome race condition of AsyncConsoleWriter
                Thread.Sleep(200);

                // looks like we just got the callsign from the user
                _aircraftCallsign = _aircraftCallsignInputTask.Result;

                // now, let's get the afStartTime
                _afStartTimeInputTask = AsyncConsoleReader.ReadLine();

                // put the cursor in the correct spot for user input (afStartTime)
                Console.SetCursorPosition(24, 46);

                // make the cursor visible again after the splash screen loads
                //   and after we've already set the cursor position, otherwise
                //   the user will see it jump.
                Console.CursorVisible = true;

                _afStartTimeInputCursorSet = true;
            }
        }

        public static void handleAfStartTimeInput()
        {
            if (_aircraftCallsignInputTask.IsCompleted
                && _afStartTimeInputTask.IsCompleted
                && _afEndTimeInputTask == null
                && !_afEndTimeInputCursorSet
            )
            {
                // looks like we just got the afStartTime from the user
                //   -- inside conditional so it only assigns once
                _afStartTime = _afStartTimeInputTask.Result;

                // now, let's get the afEndTime
                _afEndTimeInputTask = AsyncConsoleReader.ReadLine();

                // put the cursor in the correct spot for user input (afEndTime)
                Console.SetCursorPosition(24, 47);

                // make the cursor visible again after the splash screen loads
                //   and after we've already set the cursor position, otherwise
                //   the user will see it jump.
                Console.CursorVisible = true;

                _afEndTimeInputCursorSet = true;
            }
        }

        public static void handleAfEndTimeInput()
        {
            if (_aircraftCallsignInputTask.IsCompleted
                && _afStartTimeInputTask != null
                && _afStartTimeInputTask.IsCompleted
                && _afEndTimeInputTask != null
                && _afEndTimeInputTask.IsCompleted
            )
            {
                //START MAP LOGIC
                if (!_isMappingStarted)
                {
                    // looks like we just got the afTimeRange from the user
                    _afEndTime = _afEndTimeInputTask.Result;

                    // that completes our input requirements, sooooooo

                    // user must want a map
                    createMapRunnerTask();

                    // let user hit [ENTER] to kill the map
                    _mappingInputTask = AsyncConsoleReader.ReadLine();

                    _isMappingStarted = true;
                }
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

                    // gotta display the user prompt again, and reset flags
                    resetUserInputPrompt();
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

            if (GENERATE_REAL_DATA)
            {
                // real data

                WorldMapPlotter_GenerateData.GenerateRealData(
                    _pointPlotterTaskCts.Token,
                    _planeDataProvider, 
                    _aircraftCallsign, 
                    _afStartTime, 
                    _afEndTime
                );
            }
            else
            {
                // dummy data

                WorldMapPlotter_GenerateData.GenerateDummyData(
                    _pointPlotterTaskCts.Token,
                    useLatLongGenerator: true
                );
            }
        }

        public static void resetUserInputPrompt()
        {
            // reset all the input flow flags
            _userInputPromptDisplayed = false;
            _aircraftCallsignInputCursorSet = false;
            _afStartTimeInputCursorSet = false;
            _afEndTimeInputCursorSet = false;
            _isMappingStarted = false;

            // display the prompt
            displayUserInputPrompt();

            // reset these since conditionals depend on null
            _aircraftCallsignInputTask = null;
            _afStartTimeInputTask = null;
            _afEndTimeInputTask = null;
            _mappingInputTask = null;

            // capture user commands again
            _aircraftCallsignInputTask = AsyncConsoleReader.ReadLine();
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
