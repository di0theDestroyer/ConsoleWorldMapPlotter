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

        public static class WorldMapPlotter
        {
            public const string AIRCRAFT_ICON = "@";

            private static string _mapContents = string.Empty;

            public static void LoadMapFromFile()
            {
                // read map from .txt file
                string filePath = @"../../worldmap.txt";
                _mapContents = File.ReadAllText(filePath);
            }


            public static void DrawMap()
            {
                //ConsoleWriter.WriteLine(_mapContents);
            }

            public static void RunMap()
            {
                Task.Run(() =>
                {
                    Random rnd = new Random();

                    while (true)
                    {
                        var toDraw = new Tuple<string, int, int>(_mapContents, 0, 0);

                        ConsoleWriter.Write(toDraw);

                        var x = rnd.Next(0, 30);
                        var y = rnd.Next(0, 30);
                        //PlotPoint(x , y);

                        Thread.Sleep(1000);
                    }
                });
            }

            public static void PlotPoint(int x, int y)
            {
                var toPlot = new Tuple<string, int, int>(AIRCRAFT_ICON, x, y);

                ConsoleWriter.Write(toPlot);
            }

            public static void PlotPoints(List<Tuple<int,int>> pointsToPlot)
            {
                foreach (var pointToPlot in pointsToPlot)
                {
                    var toPlot = 
                        new Tuple<string, int, int>(
                            AIRCRAFT_ICON, 
                            pointToPlot.Item1, 
                            pointToPlot.Item2
                        );

                    ConsoleWriter.Write(toPlot);
                }
            }
        }

        public static class ConsoleWriter
        {
            private static BlockingCollection<Tuple<string, int, int>> writes =
                new BlockingCollection<Tuple<string, int, int>>();

            static ConsoleWriter()
            {
                Task.Run(() =>
                {
                    while (true)
                    {
                        var toWrite = writes.Take();

                        Console.SetCursorPosition(toWrite.Item2, toWrite.Item3);

                        // change aircraft icon red
                        if (toWrite.Item1 == WorldMapPlotter.AIRCRAFT_ICON)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;

                            Console.Write(toWrite.Item1);

                            Console.ResetColor();

                            continue;
                        }

                        Console.Write(toWrite.Item1);
                    }

                });
            }

            public static void Write(Tuple<string, int, int> value)
            {
                writes.Add(value);
            }

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
