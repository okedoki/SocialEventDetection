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
            CultureInfo  culture =  new CultureInfo("en-US");
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
  public class RequestRegionCalculator
    {
        private GeoCoordinate _leftBottom;
        private GeoCoordinate _rightUpper;

        private double _latitudeDistance;
        private double _longitudeDistance;

        private double _hLatitude;
        private double _hLongitude;

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
            if (pieceByLatitude < 0|| pieceByLatitude > 1|| pieceByLongitude < 0|| pieceByLongitude > 1) throw new Exception("Wrong devider by coordiate. Must be between 0 and 1");

            _hLatitude = _latitudeDistance * pieceByLatitude;
            _hLongitude = _longitudeDistance * pieceByLongitude;

            _currentLatitude = _leftBottom.Latitude;
            _currentLongitude = _leftBottom.Longitude;
            _nextLongitude = _leftBottom.Longitude + _hLongitude;
            _nextLatitude = _leftBottom.Latitude + _hLatitude;
         
         }
        private int i = 0;
        private const double eps = 0.00001;
        public ExploreInfo CalculateNextParameters()
        {

            if (_nextLongitude + eps >= _rightUpper.Longitude)
            {
                if (_nextLatitude + eps >= _rightUpper.Latitude) return new ExploreInfo();

                _currentLatitude = _nextLatitude;
                _nextLatitude = _currentLatitude + _hLatitude;

                _currentLongitude = _leftBottom.Longitude;
                _nextLongitude = _currentLongitude + _hLongitude;
                return new ExploreInfo(1, 1, 1);

            }
            else
            {
                i++;
                _currentLongitude = _nextLongitude;
                _nextLongitude = _currentLongitude + _hLongitude;
            }
                return new ExploreInfo(1,1,1);
            
        }

    }
}
