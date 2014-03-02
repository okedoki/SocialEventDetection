using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventDetection.Model
{
    class FoursquareContext : DbContext
    {
        public DbSet<Venue> Venues { get; set; }
        public DbSet<CheckinStatistic> Checkins { get; set; }
        public DbSet<DbCategory> Categories { get; set; }

        public void setCategory(List<Category> cat)
        {
            foreach(Category cat1 in cat)
            {
                Categories.Add(new DbCategory() { Id = cat1.id, Name = cat1.name, ParrentId = "" });
                foreach (Category cat2 in cat1.categories)
                {
                    Categories.Add(new DbCategory() { Id = cat2.id, Name = cat2.name, ParrentId = cat1.id });
                }
            }


        }


        public int setNewVenueAndCheckinList(List<Item2> itemList, DateTime dt)
        {
            int occassionNumber = 0;
            foreach(Item2 item in itemList)
            {
                if (Venues.Any(venue => venue.Id == item.venue.id))
                {
                    occassionNumber++;
                    continue;
                }
                Venues.Add(new Venue
                 {
                     Id = item.venue.id,
                     Address = item.venue.location.address,
                     Lat = item.venue.location.lat,
                     Lng = item.venue.location.lng,
                     CategoryId = item.venue.categories.FirstOrDefault().id,
                     Name = item.venue.name
                 });
                Checkins.Add(new CheckinStatistic
                         {
                             Venue_FK = item.venue.id,
                             DateTime = dt,
                             CheckinNumber = item.venue.stats.checkinsCount,
                             UserNumber = item.venue.stats.usersCount
                         }
                         );

            }
            return occassionNumber;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
        }
    }
}
