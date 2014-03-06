using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventDetection
{
    class Requests
    {
        private const string _apiVersion = "20140223";

        RestClient client = new RestClient();
        private string _accessToken;

        public Requests(string accessToken)
        {
            this._accessToken = accessToken;
        }

        public VenueExploreType VenueWholeCity(int offset = 0)
        {
            RestRequest venuesSearchRequest = new RestRequest("https://api.foursquare.com/v2/venues/explore");

            venuesSearchRequest.AddParameter("near", "Russia, St.-Petersburg");
            venuesSearchRequest.AddParameter("limit", "50");
            venuesSearchRequest.AddParameter("oauth_token", _accessToken);
            venuesSearchRequest.AddParameter("v", _apiVersion);
            venuesSearchRequest.AddParameter("intent", "browse");
            venuesSearchRequest.AddParameter("openNow", "0");
            venuesSearchRequest.AddParameter("specials", "0");
            if (offset != 0) venuesSearchRequest.AddParameter("offset", offset);

            return client.Execute<VenueExploreType>(venuesSearchRequest).Data;

        }
        public VenueExploreType VenueExploreBySquare(ExploreInfo info, int offset = 0)
        {
            RestRequest venuesSearchRequest = new RestRequest("https://api.foursquare.com/v2/venues/explore");


            venuesSearchRequest.AddParameter("limit", "50");
            venuesSearchRequest.AddParameter("oauth_token", _accessToken);
            venuesSearchRequest.AddParameter("v", _apiVersion);
            venuesSearchRequest.AddParameter("day", "any");
            venuesSearchRequest.AddParameter("time", "any");

            venuesSearchRequest.AddParameter("ll", info.LatitudeAndLongitude());

            //   venuesSearchRequest.AddParameter("radius", info.Radius * 100);
            if (offset != 0) venuesSearchRequest.AddParameter("offset", offset);

            return client.Execute<VenueExploreType>(venuesSearchRequest).Data;

        }
        public VenueExploreType VenueBySquare(ExploreInfo swInfo, ExploreInfo neInfo)
        {
            RestRequest venuesSearchRequest = new RestRequest("https://api.foursquare.com/v2/venues/explore");


            venuesSearchRequest.AddParameter("limit", "50");
            venuesSearchRequest.AddParameter("oauth_token", _accessToken);
            venuesSearchRequest.AddParameter("v", _apiVersion);

            venuesSearchRequest.AddParameter("sw", swInfo.LatitudeAndLongitude());
            venuesSearchRequest.AddParameter("ne", neInfo.LatitudeAndLongitude()); 
 

            //   venuesSearchRequest.AddParameter("radius", info.Radius * 100);

            return client.Execute<VenueExploreType>(venuesSearchRequest).Data;
        }
  
        public void UpdateVenue(List<Venue> d)
        {
            RestRequest venuesSearchRequest = new RestRequest("https://api.foursquare.com/v2/venues/timeseries");
 
            venuesSearchRequest.AddParameter("oauth_token", _accessToken);
            venuesSearchRequest.AddParameter("v", _apiVersion);
            venuesSearchRequest.AddParameter("venueId", _apiVersion); 
 
        }

        public List<Category> GetCategories()
        {
            RestRequest venuesSearchRequest = new RestRequest("https://api.foursquare.com/v2/venues/categories");
            venuesSearchRequest.AddParameter("oauth_token", _accessToken);
            venuesSearchRequest.AddParameter("v", _apiVersion);
            return client.Execute<Categories>(venuesSearchRequest).Data.response.categories;
        }
    }
}
