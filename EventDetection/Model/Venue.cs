using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventDetection.Model
{
    class Venue
    {
        [System.ComponentModel.DataAnnotations.Key]
        public virtual string Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }

        public string Address { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
