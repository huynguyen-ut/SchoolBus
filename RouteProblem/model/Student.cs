using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteProblem.model
{
    class Student
    {
        private int id;
        private double lat;
        private string address;
        public double Lat
        {
            get { return lat; }
            set { lat = value; }
        }

        private double lon;

        public double Lon
        {
            get { return lon; }
            set { lon = value; }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        private Station station;
        public Station Station
        {
            get { return station; }
            set { station = value; }
        }
        private School school;

        internal School School
        {
            get { return school; }
            set { school = value; }
        }
        Bus bus;

        internal Bus Bus
        {
            get { return bus; }
            set { bus = value; }
        }
        public Student(int id,double lat,double lon,string address,School school, Station station) {
            this.id = id;
            this.lat = lat;
            this.lon = lon;
            this.station = station;
            this.school = school;
            this.bus = null;
            this.address = address;
        }


    }
}
