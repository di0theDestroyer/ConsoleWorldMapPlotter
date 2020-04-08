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

            WorldMapPlotter.DrawMap();
            WorldMapPlotter.StatusUpdate();

            // 2 options



        }

        public static class WorldMapPlotter
        {
            public static void DrawMap()
            {
                // read map from .txt file

                // output to console writer

                string filePath = @"../../worldmap.txt";
                string fileText = File.ReadAllText(filePath);

                ConsoleWriter.WriteLine(fileText);
            }

            public static void StatusUpdate()
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
            }

            public static void PlotPoint()
            {

            }
        }

        public static class ConsoleWriter
        {
            private static BlockingCollection<string> blockingCollection = new BlockingCollection<string>();

            static ConsoleWriter()
            {
                Task.Run(() =>
                {
                    while (true)
                    {
                        System.Console.WriteLine(blockingCollection.Take());
                    }

                });
            }

            public static void WriteLine(string value)
            {
                blockingCollection.Add(value);
            }

            public static void Write(string value)
            {
                blockingCollection.Add(value);
            }

        }
    }
}
