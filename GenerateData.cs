using System;
using System.Collections.Generic;

namespace ConsoleWorldMapPlotter
{
    class GenerateData
    {
        public static List<LatLong> Generate(double startLat, double startLong, int itemCnt, int randomizer)
        {
            List<LatLong> list = new List<LatLong>();
            list.Add(new LatLong(startLat, startLong));

            double currentLat = startLat;
            double currentLong = startLong;

            Random rnd = new Random(randomizer);

            for (int i = 0; i < itemCnt; i++)
            {
                currentLat += rnd.NextDouble();
                currentLong += rnd.NextDouble();

                list.Add(new LatLong(currentLat, currentLong));
            }

            return list;
        }

        public static void Print(List<LatLong> list)
        {
            foreach(LatLong position in list)
            {
                Console.WriteLine($"{position.Latitude} {position.Longitude}");
            }
        }
    }
}


