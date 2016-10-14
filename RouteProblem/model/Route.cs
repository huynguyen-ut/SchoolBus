using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteProblem.model
{
    class Route
    {
        private int id;
        private List<Bus> buses;
        private Path path;
        public Route(int id,Path path) {
            this.id = id;
            this.path = path;
            Buses = new List<Bus>();
        }
        public  int Id
        {
            get
            {
                return id;
            }
                     
        }

        internal List<Bus> Buses
        {
            get
            {
                return buses;
            }

            set
            {
                buses = value;
            }
        }

        internal Path Path
        {
            get
            {
                return path;
            }

            set
            {
                path = value;
            }
        }

        public void addBus(Bus p)
        {
            this.Buses.Add(p);
        }
        public bool isBelongToRoute(Path path) {
            if (this.path.CheckIdentity(path))
                return true;
            else return false;
        }
       

    }
}
