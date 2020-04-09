using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleWorldMapPlotter
{
    class Program
    {
        static void Main(string[] args)
        {
            // DEBUG HACK -- MAXIMIZE CONSOLE WINDOW
            Maximize();

            WorldMapPlotter.LoadMapFromFile();
            WorldMapPlotter.RunMap();

            WorldMapPlotter_GenerateData.Generate();
            //WorldMapPositionConverter_GenerateData.Generate();

            Console.Read();
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
