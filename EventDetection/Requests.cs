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
        RestClient client = new RestClient();

        string _accessToken;
        public Requests(string accessToken)
        {
            this._accessToken = accessToken;
        }

        public VenueExploreType VenueExplore(int offset=0)
        {
            RestRequest venuesSearchRequest = new RestRequest("https://api.foursquare.com/v2/venues/explore");

            venuesSearchRequest.AddParameter("near", "Russia, St.-Petersburg");
            venuesSearchRequest.AddParameter("limit", "50");
            venuesSearchRequest.AddParameter("oauth_token", _accessToken);
            venuesSearchRequest.AddParameter("v", "20140223");
            venuesSearchRequest.AddParameter("intent", "browse");
            venuesSearchRequest.AddParameter("openNow", "0");
               venuesSearchRequest.AddParameter("specials", "0"); 
            if (offset != 0) venuesSearchRequest.AddParameter("offset", offset);

        //   IRestResponse r = client.Execute(venuesSearchRequest);

            return  client.Execute<VenueExploreType>(venuesSearchRequest).Data;
       
        }
    }
}
