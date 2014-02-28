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
            string clientId = configFile.AppSettings.Settings["clientId"].Value;//"ENP3KAMJTYXGH5GSETVPVOIBHSPFUGPHQB0IZ2BNJRDL0FCT";
            string clientSecret = configFile.AppSettings.Settings["clientSecret"].Value;// "FQLMHNVBVHVNGI03AQJICDPO1IQZ5RNOSUIFRYG4QUFDIFA5";
            string redirectUri = configFile.AppSettings.Settings["redirectUri"].Value; //"http://localhost/app";
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
                //r.AddParameter("client_id",clientId);
                //r.AddParameter("client_secret",clientSecret);
                //  r.AddParameter("grant_type","authorization_code");
                //  r.AddParameter("redirect_uri",redirectUri);
                //           r.AddParameter("code",code);

                IRestResponse wr = new RestClient().Execute(r);

                configFile.AppSettings.Settings["code"].Value = code;
                configFile.AppSettings.Settings["accessToken"].Value = accessToken;
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }

            #endregion
            Requests requests = new Requests(accessToken);
            List<Item2> venueList;
            int resultedOccassion = 0;
            int offset = 0;

 
            //foreach (Item2 g in venueList)
            //{
            //    Console.WriteLine(g.venue.id + ". " + g.venue.name);
            //    Console.WriteLine(g.venue.location.address);

            //    Console.WriteLine("Category:");
            //    foreach (Category cat in g.venue.categories)
            //        Console.WriteLine(cat.name);

            //    Console.WriteLine("Tips:");
            //    foreach (Tip tip in g.tips)
            //    {
            //        Console.WriteLine(tip.text);
            //    }


            //}

            System.Data.Entity.Database.SetInitializer(new EventDetection.Model.FoursquareDbInitializer());
             VenueExploreType  t;
            //  System.Data.Entity.Database.SetInitializer<EventDetection.Model.FoursquareContext>(initializer);
            // System.Data.Entity.Database.SetInitializer(new EventDetection.Model.FoursquareDbInitializer());
            using (EventDetection.Model.FoursquareContext foursquareContext = new Model.FoursquareContext())
            {
                do
                {
                
                    offset += 50;
                      t = requests.VenueExplore(offset);
                    venueList = t.response[0].groups[0].items;
                    resultedOccassion += foursquareContext.setNewVenueAndCheckinList(venueList, DateTime.Now);
                    foursquareContext.SaveChanges();
                } while (venueList.Count != 0 && venueList != null && offset <= 1000);
            }

            Console.WriteLine("Number of occasion:" + resultedOccassion);
            Console.ReadKey();
        }
    }


}
