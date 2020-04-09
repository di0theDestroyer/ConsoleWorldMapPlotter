using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleWorldMapPlotter
{
    public static class WorldMapPlotter_GenerateData
    {
        public static void GenerateRealData(
            PlaneDataProvider planeDataProvider, 
            string aircraftCallsign, 
            string afStartTime, 
            string afEndTime
        )
        {
            // for PoC, just do lat, longs, on map and in metadatatable

        }

        public static void GenerateDummyData(CancellationToken cancel, bool useLatLongGenerator)
        {
            var pointsToPlot = new List<Tuple<int, int>>();

            if (useLatLongGenerator)
            {
                List<LatLong> latLongs = 
                    WorldMapPositionConverter_GenerateData.Generate(
                            startLat: 10, 
                            startLong: 10, 
                            itemCnt: 30, 
                            randomizer: 100
                        );

                var worldMapPositionConverter = 
                    new WorldMapPositionConverter(
                        WorldMapPlotter.MAP_COLUMNS, 
                        WorldMapPlotter.MAP_ROWS
                    );

                // TODO: why do i need these?
                double centerLat = 0;
                double centerLong = 0;

                var coordinates = 
                    worldMapPositionConverter.Convert(
                        latLongs, 
                        out centerLat, 
                        out centerLong
                    );


                foreach (var coordinate in coordinates)
                {
                    var convertedCoords = 
                        new Tuple<int, int>(
                            coordinate.xCoord, 
                            coordinate.yCoord
                        );

                    pointsToPlot.Add(convertedCoords);
                }
            }

            Task.Run(() =>
            {
                // used in commented code as well -- keep it here
                Random rnd = new Random();

                /*
                // COMING FROM ANOTHER THREAD
                // this simulates plotting _single_ points
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
                // this simulates plotting _multiple_ points
                while (true)
                {
                    if (!cancel.IsCancellationRequested)
                    {
                        if (!useLatLongGenerator)
                        {
                            // let's just get random

                            // 10 points why not
                            for (int i = 0; i < 10; i++)
                            {
                                // maptext is 150x34
                                var x = rnd.Next(0, 150);
                                var y = rnd.Next(0, 33);

                                pointsToPlot.Add(new Tuple<int, int>(x, y));
                            }
                        }

                        //DEBUG --- this plots all points instead of making the plane "fly"
                        //WorldMapPlotter.PlotPoints(pointsToPlot);

                        foreach (var pointToPlot in pointsToPlot)
                        {
                            if (!cancel.IsCancellationRequested)
                            {
                                WorldMapPlotter.PlotPoint(
                                    pointToPlot.Item1,
                                    pointToPlot.Item2
                                );


                                // update the lower part of the screen
                                WorldMapPlotter.UpdateMapMetaData("somecallsign", "sometimerange", 10.0000, -10.0000);
                            }
                            
                            Thread.Sleep(700);
                        }
                    }
                }
            });
        }
    }
}
