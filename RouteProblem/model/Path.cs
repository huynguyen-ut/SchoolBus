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

        public void addStation(Station s)
        {
            this.stations.Add(s);
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
        public int NumberStation() {
            return this.stations.Count;
        }
        public bool CheckIdentity(Path path) {
            if (this.stations.Count!= path.NumberStation())
                return false;
            else { 
                for (int i = 0; i < this.NumberStation(); i++)
                    if (this.Stations[i].Id != path.stations[i].Id)
                        return false;
                 }
           return true;
        }
        public bool IsContain(Station station) {
            foreach (Station st in this.stations) {
                if (st.Id == station.Id)
                    return true;
            }
            return false;
        }
      }
}
