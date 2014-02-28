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
                     CategoryName = item.venue.categories.FirstOrDefault().name,
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
