using System;
using System.Collections.Generic;

namespace ConsoleWorldMapPlotter
{
    public class WorldMapPositionConverter
    {
        //longitude transfers to columns (x axis)
        //latitude transfers to rows (y axis)

        public int Columns { get; set; }
        public int Rows { get; set; }

        private double minLat, maxLat, minLong, maxLong;
        private double cLat, cLong;

        public WorldMapPositionConverter(int columns, int rows)
        {
            Rows = rows;
            Columns = columns;
        }


        public List<Coordinates> Convert(List<LatLong> list, out double centerLat, out double centerLong)
        {
            List<Coordinates> converted = new List<Coordinates>();

            FindCenter(list);

            double xScale = (maxLong - minLong) / Columns;
            double yScale = (maxLat - minLat) / Rows;


            foreach(LatLong pos in list)
            {
                int x = (int)((pos.Longitude - minLong) / xScale);
                int y = (int)((pos.Latitude - minLat) / yScale);

                converted.Add(new Coordinates(x, y));
            }

            centerLat = cLat;
            centerLong = cLong;
            return converted;
        }

        public void Print(List<Coordinates> list)
        {
            foreach (Coordinates position in list)
            {
                Console.WriteLine($"{position.xCoord} {position.yCoord}");
            }
        }

        private void FindCenter(List<LatLong> list)
        {
            minLat = list[0].Latitude;
            maxLat = list[0].Latitude;
            minLong = list[0].Longitude;
            maxLong = list[0].Longitude;

            foreach (LatLong loc in list)
            {
                if (loc.Latitude < minLat) minLat = loc.Latitude;
                if (loc.Latitude > maxLat) maxLat = loc.Latitude;
                if (loc.Longitude < minLong) minLong = loc.Longitude;
                if (loc.Longitude > maxLong) maxLong = loc.Longitude;
            }

            cLat = (minLat + maxLat) / 2;
            cLong = (minLong + maxLong) / 2;
        }
    }
}

