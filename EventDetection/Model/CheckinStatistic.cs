using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventDetection.Model
{
    class CheckinStatistic
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }
        public  string Venue_FK { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public virtual int CheckinNumber { get; set; }

        public int UserNumber { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public DateTime DateTime { get; set; }


    }
}
