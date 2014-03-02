using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventDetection.Model
{
    class DbCategory
    {
        [System.ComponentModel.DataAnnotations.Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string ParrentId { get; set; }
    }
}
