using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleWorldMapPlotter
{
    public static class WorldMapPlotter_GenerateData
    {
        public static void GenerateRealData(
            CancellationToken cancel,
            PlaneDataProvider planeDataProvider, 
            string aircraftCallsign,
            string afStartTime, 
            string afEndTime
        )
        {
            //
            // for PoC, just do lat, longs, on map and in metadatatable
            //
            // then later, add more cool metadata
            //

            // MUST change default Data Archive server in PSE >> File >> Connections,
            //   or this will fail
            IDictionary<DateTime, Dictionary<string,string>> planeData = 
                planeDataProvider.GetPlaneDataInInterval(
                    aircraftCallsign, 
                    afStartTime, 
                    afEndTime
                );

            var worldMapPositionConverter =
                new WorldMapPositionConverter(
                    WorldMapPlotter.MAP_COLUMNS,
                    WorldMapPlotter.MAP_ROWS
                );

            var planeDataPackets = new List<PlaneDataPacket>();

            // convert raw planData to PlaneDataPackets
            foreach (var rawEntry in planeData)
            {
                // need this for conversion class
                var latLong = new List<LatLong>()
                {
                    //TODO: error handle bad parse
                    new LatLong(
                        double.Parse(rawEntry.Value["Latitude"]), 
                        double.Parse(rawEntry.Value["Longitude"])
                    )
                };

                // TODO: why do i need these?
                double centerLat = 0;
                double centerLong = 0;

                // need this for conversion class
                List<Coordinates> coordinatesPair =
                    worldMapPositionConverter.Convert(
                        latLong,
                        out centerLat,
                        out centerLong
                    );

                // create our PlaneDataPacket object!!!
                var planeDataPacket =
                    new PlaneDataPacket(
                        aircraftCallsign,
                        afStartTime,
                        afEndTime,
                        rawEntry.Value["Latitude"],
                        rawEntry.Value["Longitude"],
                        rawEntry.Key.ToString("MM/dd/yyyy h:mm tt"),
                        coordinatesPair[0].xCoord,
                        coordinatesPair[0].yCoord,
                        rawEntry.Value["Altitude"],
                        rawEntry.Value["GroundSpeed"],
                        rawEntry.Value["VerticalSpeed"]
                    );

                planeDataPackets.Add(planeDataPacket);
            }


            foreach (var planeDataPacket in planeDataPackets)
            {
                if (!cancel.IsCancellationRequested)
                {
                    WorldMapPlotter.PlotPoint(
                        planeDataPacket.PlotX,
                        planeDataPacket.PlotY
                    );

                    // update the lower part of the screen
                    WorldMapPlotter.UpdateMapMetaData(
                        planeDataPacket.AircraftCallsign, 
                        planeDataPacket.AfStartTime, 
                        planeDataPacket.AfEndTime,
                        double.Parse(planeDataPacket.Latitude),
                        double.Parse(planeDataPacket.Longitude)
                    );
                }

                Thread.Sleep(700);
            }

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
                                WorldMapPlotter.UpdateMapMetaData("somecallsign", "someafstarttime", "someafendtime", 10, -10);
                            }
                            
                            Thread.Sleep(700);
                        }
                    }
                }
            });
        }
    }
}
