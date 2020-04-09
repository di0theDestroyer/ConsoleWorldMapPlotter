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

            Random rnd = new Random();

            /*
            // COMING FROM ANOTHER THREAD
            // this simulates plotting single points
            while (true)
            {
                // maptext is 150x34
                var x = rnd.Next(0, 150);
                var y = rnd.Next(0, 34);

                WorldMapPlotter.PlotPoint(x, y);

                Thread.Sleep(100);
            }
            */

            // COMING FROM ANOTHER THREAD
            // this simulates plotting multiple points
            while (true)
            {
                var pointsToPlot = new List<Tuple<int, int>>();

                // 30 points why not
                for (int i = 0; i < 10; i++)
                {
                    // maptext is 150x34
                    var x = rnd.Next(0, 150);
                    var y = rnd.Next(0, 33);

                    pointsToPlot.Add(new Tuple<int, int>(x, y));
                }

                WorldMapPlotter.PlotPoints(pointsToPlot);

                Thread.Sleep(100);
            }

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
