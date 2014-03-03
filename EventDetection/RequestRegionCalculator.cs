using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventDetection
{
    public struct GeoCoordinate
    {
        private readonly double latitude;
        private readonly double longitude;

        public double Latitude { get { return latitude; } }
        public double Longitude { get { return longitude; } }

        public GeoCoordinate(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }
        public GeoCoordinate(string latitudecommalongitude)
        {
            try
            {
                CultureInfo culture = new CultureInfo("en-US");
                string[] coordinata = latitudecommalongitude.Split(',');
                latitude = Convert.ToDouble(coordinata[0], culture);
                longitude = Convert.ToDouble(coordinata[1], culture);
            }
            catch (Exception fe)
            {
                throw new Exception("Problem with string representation of coordinate", fe);
            }
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", Latitude, Longitude);
        }
    }
    public struct ExploreInfo
    {
        private double latitude;

        public double Latitude
        {
            get { return latitude; }
        }
        private double longitude;

        public double Longitude
        {
            get { return longitude; }
        }

        public string LatitudeAndLongitude()
        {
            System.Globalization.CultureInfo culture = CultureInfo.GetCultureInfo("en-GB");
             return Latitude.ToString(culture) + "," + Longitude.ToString(culture);
        }

        private double radius;

        public double Radius
        {
            get { return radius; }
        }
        public ExploreInfo(double latitude, double longitude, double radius)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.radius = radius;
        }



    }

    public static class CoordinatesUtil
{
    // http://www.geodatasource.com/developers/c-sharp  Автор
    public static double Distance(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
    {
        double theta = lon1 - lon2;
        double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
        dist = Math.Acos(dist);
        dist = rad2deg(dist);
        dist = dist * 60 * 1.1515;
        if (unit == 'K')
        {
            dist = dist * 1.609344;
        }
        else if (unit == 'N')
        {
            dist = dist * 0.8684;
        }
        return (dist);
    }
    private static double deg2rad(double deg)
    {
        return (deg * Math.PI / 180.0);
    }
    private static double rad2deg(double rad)
    {
        return (rad / Math.PI * 180.0);
    }

}
    public class RequestRegionCalculator
    {
        private readonly GeoCoordinate _leftBottom;
        private readonly GeoCoordinate _rightUpper;

        private readonly double _latitudeDistance;
        private readonly double _longitudeDistance;

        private readonly double _hLatitude;
        private readonly double _hLongitude;

        private readonly int pieceNumber;

        public int PieceNumber { get { return pieceNumber; } } 

        private double _nextLatitude;
        private double _nextLongitude;
        private double _currentLatitude;
        private double _currentLongitude;

     

        public RequestRegionCalculator(string leftBottomCoordinate, string rightUpperCoordinate, double pieceByLatitude = 0.05, double pieceByLongitude = 0.05)
        {
            _leftBottom = new GeoCoordinate(leftBottomCoordinate);
            _rightUpper = new GeoCoordinate(rightUpperCoordinate);

            _latitudeDistance = _rightUpper.Latitude - _leftBottom.Latitude;
            _longitudeDistance = _rightUpper.Longitude - _leftBottom.Longitude;

            if (_latitudeDistance < 0 || _longitudeDistance < 0) throw new Exception("Wrong locations for bottom and upper corner");
            if (pieceByLatitude < 0 || pieceByLatitude > 1 || pieceByLongitude < 0 || pieceByLongitude > 1) throw new Exception("Wrong devider by coordiate. Must be between 0 and 1");

            _hLatitude = _latitudeDistance * pieceByLatitude;
            _hLongitude = _longitudeDistance * pieceByLongitude;

            _currentLatitude = _leftBottom.Latitude;
            _currentLongitude = _leftBottom.Longitude;
            _nextLongitude = _leftBottom.Longitude + _hLongitude;
            _nextLatitude = _leftBottom.Latitude + _hLatitude;

            pieceNumber = Convert.ToInt32((1.0 / pieceByLatitude) * (1.0 / pieceByLongitude));

        }
        private int i = 0;
        private const double eps = 0.00001;
        public ExploreInfo CalculateNextParameters()
        {
            ExploreInfo exploreInfo = CalculateRadius(_currentLatitude, _nextLatitude, _currentLongitude, _nextLongitude);
            if (_nextLongitude + eps >= _rightUpper.Longitude)
            {
                if (_nextLatitude + eps >= _rightUpper.Latitude) return new ExploreInfo();


                _currentLatitude = _nextLatitude;
                _nextLatitude = _currentLatitude + _hLatitude;

                _currentLongitude = _leftBottom.Longitude;
                _nextLongitude = _currentLongitude + _hLongitude;
                return exploreInfo ;

            }
            else
            {
                i++;
                _currentLongitude = _nextLongitude;
                _nextLongitude = _currentLongitude + _hLongitude;
            }
            return exploreInfo;

        }
        private ExploreInfo CalculateRadius(double bottomLatitude, double upperLatitude, double leftLongitude, double rightLongitude)
        {
            double latitudeDifference = upperLatitude - bottomLatitude;
            double longitudeDifference = rightLongitude - leftLongitude;

            double radius = Math.Sqrt(Math.Pow(latitudeDifference, 2) + Math.Pow(longitudeDifference, 2)) / 2;
            radius = CoordinatesUtil.Distance(bottomLatitude, leftLongitude, upperLatitude, rightLongitude)*1000;
            return new ExploreInfo(bottomLatitude + latitudeDifference, leftLongitude + longitudeDifference, radius);
        }



    }
}
