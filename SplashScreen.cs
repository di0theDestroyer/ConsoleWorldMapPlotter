using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleWorldMapPlotter
{
    public static class SplashScreen
    {
        public static void Display(bool doItSlow)
        {
            // get those nice defaults back
            Utilities.ResetConsoleDefaults();

            // make the cursor invisible until after splash screen load
            Console.CursorVisible = false;

            List<string> piInSkyLogo = new List<string>()
                {
                    @"                          ___                                          ___     ",
                    @"                         /  /\      ___                  ___          /__/\    ",
                    @"                        /  /::\    /  /\                /  /\         \  \:\   ",
                    @"                       /  /:/\:\  /  /:/               /  /:/          \  \:\  ",
                    @"                      /  /:/~/:/ /__/::\              /__/::\      _____\__\:\ ",
                    @"                     /__/:/ /:/  \__\/\:\__           \__\/\:\__  /__/::::::::\",
                    @"                     \  \:\/:/      \  \:\/\             \  \:\/\ \  \:\~~\~~\/",
                    @"                      \  \::/        \__\::/              \__\::/  \  \:\  ~~~ ",
                    @"                       \  \:\        /__/:/               /__/:/    \  \:\     ",
                    @"                        \  \:\       \__\/                \__\/      \  \:\    ",
                    @"                         \__\/                                        \__\/    ",
                    @"                       ___           ___                    ___           ___                 ",
                    @"           ___        /__/\         /  /\                  /  /\         /__/|          ___   ",
                    @"          /  /\       \  \:\       /  /:/_                /  /:/_       |  |:|         /__/|  ",
                    @"         /  /:/        \__\:\     /  /:/ /\              /  /:/ /\      |  |:|        |  |:|  ",
                    @"        /  /:/     ___ /  /::\   /  /:/ /:/_            /  /:/ /::\   __|  |:|        |  |:|  ",
                    @"       /  /::\    /__/\  /:/\:\ /__/:/ /:/ /\          /__/:/ /:/\:\ /__/\_|:|____  __|__|:|  ",
                    @"      /__/:/\:\   \  \:\/:/__\/ \  \:\/:/ /:/          \  \:\/:/~/:/ \  \:\/:::::/ /__/::::\  ",
                    @"      \__\/  \:\   \  \::/       \  \::/ /:/            \  \::/ /:/   \  \::/~~~~     ~\~~\:\ ",
                    @"           \  \:\   \  \:\        \  \:\/:/              \__\/ /:/     \  \:\           \  \:\",
                    @"            \__\/    \  \:\        \  \::/                 /__/:/       \  \:\           \__\/",
                    @"                      \__\/         \__\/                  \__\/         \__\/                "
                };

            List<string> teamMembersList = new List<string>()
                {
                    "                             Joy Wang                  Sang Yun Lee",
                    "                             Maxim Podkolzin           Roland Poklemba",
                    "                             Gurpreet Singh            Dustin Johnson\n\n",
                };

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("                                   _________________________________________");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("    _________________________________________");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n    WW              WW                                             EEEEEEEE ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("    WW              WW                                            EE ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("    WW              WW EEEEEEE LL         CCCCCC OOOOOO MMM   MMM  EE ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("     WW     WW     WW  EE      LL     =+  CC     O    O MM M M MM   EEE");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("      WW   WWWW   WW   EEEE    LL     +=  CC     O    O MM  M  MM  EE");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("       WW WW  WW WW    EE      LL         CC     O    O MM     MM EE");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("        WW     WW      EEEEEEE LLLLLL     CCCCCC OOOOOO MM     MM  EEEEEEEE");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("    _________________________________________");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("                                   _________________________________________");
            Console.WriteLine("");

            //Slowly animate the Pi in the Sky logo.
            Console.ForegroundColor = ConsoleColor.Cyan;
            foreach (string line in piInSkyLogo)
            {
                Console.WriteLine(line);

                if (doItSlow)
                {
                    Thread.Sleep(100);
                }
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n\n\n           PROJECT BY:");
            Console.ForegroundColor = ConsoleColor.Cyan;
            //Slowly animate the team members list.
            foreach (string member in teamMembersList)
            {
                Console.WriteLine(member);
                Thread.Sleep(100);
            }
        }
    }
}
