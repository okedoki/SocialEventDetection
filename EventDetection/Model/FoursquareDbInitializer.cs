using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventDetection.Model
{
    class FoursquareDbInitializer : DropCreateDatabaseAlways<FoursquareContext>// DropCreateDatabaseIfModelChanges<FoursquareContext>
    {
        protected override void Seed(FoursquareContext context)
        {
            context.SaveChanges();
       
        }
    }
}
