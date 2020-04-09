using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleWorldMapPlotter
{
    public static class WorldMapPlotter
    {
        public static int MAP_ROWS = 34;
        public static int MAP_COLUMNS = 150;
        public const string AIRCRAFT_ICON = "@";
        public const int MAP_REDRAW_FREQUENCY = 1100; // milliseconds

        private static string _mapContents = string.Empty;

        public static void LoadMapFromFile()
        {
            // read map from .txt file
            string filePath = @"../../worldmap.txt";
            _mapContents = File.ReadAllText(filePath);
        }

        public static void RunMap(CancellationToken cancelToken)
        {
            bool mapDrawn = false;

            // get those nice defaults back
            Utilities.ResetConsoleDefaults();

            Task.Run(() =>
            {

                Random rnd = new Random();

                while (true)
                {
                    if (!cancelToken.IsCancellationRequested)
                    {

                        if (!mapDrawn)
                        {
                            // draw the map at 0,0
                            var toDraw = new Tuple<string, int, int>(_mapContents, 0, 0);

                            AsyncConsoleWriter.Write(toDraw);
                        }

                        Thread.Sleep(MAP_REDRAW_FREQUENCY);

                        // go back to home screen if user hits enter
                    }
                    else
                    {
                        // TODO: cleanup task or it runs forever without doing anything
                        // return and break give error
                        return;
                    }
                }
            });
        }

        public static void PlotPoint(int x, int y)
        {
            // fly that lil plane
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
                
                /*
                //TO PLOT ALL POINTS INSTEAD MAKING THE PLANE FLYYYYYY
                // COMMENT THESE TWO LINES 
                // draw the map at 0,0
                var toDraw = new Tuple<string, int, int>(_mapContents, 0, 0);
                AsyncConsoleWriter.Write(toDraw);
                */
            }
        }
    }
}
