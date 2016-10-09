using RouteProblem.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using GoogleMapsApi.Entities.Directions.Request;
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
        private List<Station> stations;

        internal List<Station> Stations
        {
            get { return stations; }
            set { stations = value; }
        }
        public void addStation(Station s)
        {
            this.stations.Add(s);

        }
        public Path(int id) {
            this.id = id;
            this.stations = new List<Station>();
        }
      }
}
