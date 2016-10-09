using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchNearby
{
    class Cluster
    {
        public int ID { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Nb { get; set; }
        public List<GPlace> Places { get; set; }
    }
}
