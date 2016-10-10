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
        private static int count=0;
        private List<Bus> buses;
        public Route() {
            count++;
            id =count;
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

        public void addBus(Bus p)
        {
            this.Buses.Add(p);
        }
    

    }
}
