using RouteProblem.model;

using System.Collections.Generic;

namespace RouteProblem
{
    class Path
    {
        private int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        private int duration;
        private double distance;
        private List<Station> stations;

        internal List<Station> Stations
        {
            get { return stations; }
            set { stations = value; }
        }

        public int Duration
        {
            get
            {
                return duration;
            }            
        }

        public double Distance
        {
            get
            {
                return distance;
            }          
        }

        public void addStation(Station s,int duration,int distance)
        {
            this.stations.Add(s);
            this.duration = duration;
            this.distance = distance;
        }
        public Path(int id) {
            this.id = id;
            this.stations = new List<Station>();
        }
        public Station getFirstStation() {
            return this.stations[0];
        }
        public Station getLastStation() {
            return this.stations[this.stations.Count - 1];
        }
      }
}
