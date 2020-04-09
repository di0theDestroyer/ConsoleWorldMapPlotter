﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleWorldMapPlotter
{
    public static class WorldMapPlotter_GenerateData
    {
        public static void Generate(CancellationToken cancel)
        {
            Task.Run(() =>
            {
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
                    if (!cancel.IsCancellationRequested)
                    {
                        var pointsToPlot = new List<Tuple<int, int>>();

                        // 10 points why not
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
                }
            });
        }
    }
}
