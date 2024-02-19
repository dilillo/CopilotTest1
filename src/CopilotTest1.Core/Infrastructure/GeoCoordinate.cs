namespace CopilotTest1.Core.Infrastructure
{
    [GenerateSerializer]
    public struct GeoCoordinate
    {
        public GeoCoordinate()
        {
        }

        public GeoCoordinate(double latitude, double longitude)
        {
            // Validate latitude and longitude values if needed
            if (latitude < -90.0 || latitude > 90.0)
                throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90 degrees.");

            if (longitude < -180.0 || longitude > 180.0)
                throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180 degrees.");

            Latitude = latitude;
            Longitude = longitude;
        }

        [Id(0)]
        public double Latitude { get; set; }

        [Id(1)]
        public double Longitude { get; set; }
    }
}
