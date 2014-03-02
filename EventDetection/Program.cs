#define DbCreate

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows.Forms;
using RestSharp;


namespace EventDetection
{
    class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Console.WriteLine("Приложения загрузки информации из foursquare");
            #region Authorization

            System.Configuration.Configuration configFile =
                         ConfigurationManager.OpenExeConfiguration(
                               ConfigurationUserLevel.None);
            if (configFile == null) throw new Exception("Have no configuration file. Goodbye!");
            string clientId = configFile.AppSettings.Settings["clientId"].Value;
            string clientSecret = configFile.AppSettings.Settings["clientSecret"].Value;
            string redirectUri = configFile.AppSettings.Settings["redirectUri"].Value;
            string code = configFile.AppSettings.Settings["code"].Value;
            string accessToken = configFile.AppSettings.Settings["accessToken"].Value;

            string authenticateUrl = "https://foursquare.com/oauth2/authenticate";
            string accessTokenUrl = "https://foursquare.com/oauth2/access_token";



            if (String.IsNullOrEmpty(code) || String.IsNullOrEmpty(accessToken))
            {

                Uri address = new Uri(string.Format("{0}?client_id={1}&response_type=code&redirect_uri={2}", authenticateUrl, clientId, redirectUri));

                CodeForm form = CodeFormFactory.FormCreate(address, redirectUri);
                Application.Run(form);
 
                if (string.IsNullOrEmpty(form.Code)) throw new Exception("Returned code is equal to null. Goodbye");
                code = form.Code;
                string url = string.Format("{0}?client_id={1}&client_secret={2}&grant_type=authorization_code&redirect_uri={3}&code={4}", accessTokenUrl, clientId, clientSecret, redirectUri, code);
                //  sharpSquare.GetAccessToken(redirectUri, code);
                RestRequest r = new RestRequest(url);
                AccessToken aToken = new RestClient().Execute<AccessToken>(r).Data;

                configFile.AppSettings.Settings["code"].Value = code;
                configFile.AppSettings.Settings["accessToken"].Value = aToken.access_token;
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }

            #endregion

            Requests requests = new Requests(accessToken);
            System.Data.Entity.Database.SetInitializer(new EventDetection.Model.FoursquareDbInitializer());
#if(DbCreate)
            #region  GetCategorities
            List<Category> cat =  requests.GetCategories();

            using (EventDetection.Model.FoursquareContext foursquareContext = new Model.FoursquareContext())
            {
                foursquareContext.setCategory(cat);
                foursquareContext.SaveChanges();
            }

            #endregion
            #region GetFirstVenueInfo
            List<Item2> venueList;
            int resultedOccassion = 0;
 



       
            VenueExploreType  venueReturns;

            string leftBottomMap = configFile.AppSettings.Settings["leftBottomMapCorner"].Value;
            string rightUpperMap =  configFile.AppSettings.Settings["rightUpperMapCorner"].Value;

            RequestRegionCalculator reqCalculator = new RequestRegionCalculator(leftBottomMap, rightUpperMap, 0.25, 0.25);
            ExploreInfo exploreInfo;
            int currentPieceNumber = 0;
            Console.WriteLine("Total number of pieces:{0}", reqCalculator.PieceNumber);


 
            using (EventDetection.Model.FoursquareContext foursquareContext = new Model.FoursquareContext())
            {
                do
                {
                    exploreInfo = reqCalculator.CalculateNextParameters(); 
                    Console.WriteLine("Piece #{0} ", currentPieceNumber++);

                    venueReturns = requests.VenueExploreBySquare(exploreInfo);
                    try
                    {
                        venueList = venueReturns.response[0].groups[0].items;
                    }
                    catch (NullReferenceException)
                    {
                        venueList = new List<Item2>();
                    }
                    foreach (Item2 g in venueList)
                    {
                        Console.WriteLine(g.venue.id + ". " + g.venue.name);
                    }

                    resultedOccassion += foursquareContext.setNewVenueAndCheckinList(venueList, DateTime.Now);
                    foursquareContext.SaveChanges();
                } while (venueList != null  && exploreInfo.Radius != 0);
            }
            #endregion
#endif
            Console.WriteLine("Number of occasion:" + resultedOccassion);
            Console.ReadKey();
        }
    }


}
