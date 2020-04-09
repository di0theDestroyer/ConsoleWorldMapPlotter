using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleWorldMapPlotter
{
    public static class Utilities
    {
        public static void ResetConsoleDefaults()
        {
            // get those nice defaults back
            Console.SetCursorPosition(0, 0);
            Console.ResetColor();
            Console.Clear();
        }
    }
}
