using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchNearby
{
    class GPlace
    {
        public String ID { get; set; }
        public String Name { get; set; }
        public String Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Distance { get; set; }
    }
}
