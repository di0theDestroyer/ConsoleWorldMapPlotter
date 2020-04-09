namespace ConsoleWorldMapPlotter
{
    public class PlaneDataPacket
    {
        public string AircraftCallsign { get; set; }

        public string AfStartTime { get; set; }

        public string AfEndTime { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Timestamp { get; set; }

        public int PlotX { get; set; }

        public int PlotY { get; set; }

        public string Altitude { get; set; }

        public string GroundSpeed { get; set; }

        public string VerticalSpeed { get; set; }

        public PlaneDataPacket(
            string aircraftCallsign,
            string afStartTime,
            string afEndTime,
            string latitude,
            string longitude,
            string timestamp,
            int plotX,
            int plotY,
            string altitude,
            string groundSpeed,
            string verticalSpeed
        )
        {
            AircraftCallsign = aircraftCallsign;
            AfStartTime = afStartTime;
            AfEndTime = afEndTime;
            Latitude = latitude;
            Longitude = longitude;
            Timestamp = timestamp;
            PlotX = plotX;
            PlotY = plotY;
            Altitude = altitude;
            GroundSpeed = groundSpeed;
            VerticalSpeed = verticalSpeed;
        }
    }
}
