namespace ConsoleWorldMapPlotter
{
    public struct LatLong
    {
        public LatLong(double lat, double lng)
        {
            Latitude = lat;
            Longitude = lng;
        }

        public double Latitude;
        public double Longitude;
    }
}
