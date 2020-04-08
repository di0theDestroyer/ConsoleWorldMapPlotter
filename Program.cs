using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleWorldMapPlotter
{
    class Program
    {
        static void Main(string[] args)
        {
            //WorldMapPlotter worldMapPlotter = new WorldMapPlotter();

            WorldMapPlotter.LoadMapFromFile();
            WorldMapPlotter.RunMap();
            //WorldMapPlotter.StatusUpdate();

            Random rnd = new Random();

            // 2 options
            while (true)
            {
                var x = rnd.Next(0, 30);
                var y = rnd.Next(0, 30);

                WorldMapPlotter.PlotPoint(x, y);

                Thread.Sleep(1000);
            }

            Console.Read();
        }

        public static class WorldMapPlotter
        {

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
                        PlotPoint(x , y);

                        Thread.Sleep(1000);
                    }
                });
            }

            /*
            public static void StatusUpdate()
            {
                Task.Run(() =>
                {

                    var whiteSpace = new StringBuilder();
                    whiteSpace.Append(' ', 10);
                    var random = new Random();
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    var randomWord = new string(Enumerable.Repeat(chars, random.Next(10)).Select(s => s[random.Next(s.Length)]).ToArray());

                    while (true)
                    {
                        Console.SetCursorPosition(0, 0);

                        var sb = new StringBuilder();
                        sb.AppendLine($"Program Status:{whiteSpace}");
                        sb.AppendLine("-------------------------------");
                        sb.AppendLine($"Last Updated: {DateTime.Now}{whiteSpace}");
                        sb.AppendLine($"Random Word: {randomWord}{whiteSpace}");
                        sb.AppendLine("-------------------------------");

                        ConsoleWriter.Write(sb.ToString());

                        Thread.Sleep(1000);
                    }
                });
            }
            */

            public static void PlotPoint(int x, int y)
            {
                var toPlot = new Tuple<string, int, int>("@", x, y);

                ConsoleWriter.Write(toPlot);
            }
        }

        public static class ConsoleWriter
        {
            //private static BlockingCollection<string> writeLines = 
            //    new BlockingCollection<string>();

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

                        Console.Write(toWrite.Item1);
                    }

                });

                /*
                Task.Run(() =>
                {
                    while (true)
                    {
                        Console.WriteLine(writes.Take());
                    }

                });
                */
            }

            /*
            public static void WriteLine(string value)
            {
                writeLines.Add(value);
            }
            */

            public static void Write(Tuple<string, int, int> value)
            {
                writes.Add(value);
            }

        }
    }
}
