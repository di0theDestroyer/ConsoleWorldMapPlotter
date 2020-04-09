using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleWorldMapPlotter
{
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

                    AsyncConsoleWriter.Write(toDraw);

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

            AsyncConsoleWriter.Write(toPlot);
        }

        public static void PlotPoints(List<Tuple<int, int>> pointsToPlot)
        {
            foreach (var pointToPlot in pointsToPlot)
            {
                var toPlot =
                    new Tuple<string, int, int>(
                        AIRCRAFT_ICON,
                        pointToPlot.Item1,
                        pointToPlot.Item2
                    );

                AsyncConsoleWriter.Write(toPlot);
            }
        }
    }
}
