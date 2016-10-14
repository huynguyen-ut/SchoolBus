using GoogleMaps.LocationServices;
using GoogleMapsApi.Entities.Directions.Request;
using GoogleMapsApi.Entities.Directions.Response;
using GoogleMapsApi.Entities.Places.Request;
using GoogleMapsApi.Entities.Places.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RouteProblem
{

    abstract class Location 
     {
         protected int id;
         private string API_KEY = "AIzaSyCI23HUoG4zqAvSwheflDAXaoYDX0cB96c";
         public int Id
         {
             get { return id; }
         }
         private double lat;

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
        private string address;

public string Address
{
  get { return address; }
  set { address = value; }
}
         private List<int> distanceToX;

         public List<int> DistanceToX
         {
             get { return distanceToX; }
             set { distanceToX = value; }
         }
         private List<int> durationToX;

         public List<int> DurationToX
         {
             get { return durationToX; }
             set { durationToX = value; }
         }
         public Location(int id, string address,double lat,double lon) {
             this.id=id;
             this.lat = lat;
             this.lon = lon;
             this.distanceToX = new List<int>();
             this.durationToX = new List<int>();
             this.address = address;
            // this.address=this.getAdress();
         }
         public void AddDurationToX(int x) {
             this.durationToX.Add(x);
         }
         public int GetDistance(Location x) {
             return this.distanceToX[x.Id];
         }
         public int GetDuration(Location x)
         {
             return this.durationToX[x.Id];
         }
       
        // https://github.com/maximn/google-maps
         public List<string> getDirection(Location L) {

             List<string> steps = new List<string>();
             DirectionsRequest directionsRequest = new DirectionsRequest()
               {
                  Origin = this.lat+","+this.lon,
                  Destination = L.lat + "," + L.lon
              };
             directionsRequest.ApiKey = this.API_KEY;
             DirectionsResponse directionsResponse = GoogleMapsApi.GoogleMaps.Directions.Query(directionsRequest);
             GoogleMapsApi.Entities.Directions.Response.Leg leg = directionsResponse.Routes.ElementAt(0).Legs.ElementAt(0);
             foreach (var step in leg.Steps)
             {
                 steps.Add(Regex.Replace(step.HtmlInstructions, @"<(.|\n)*?>", string.Empty));
             }
             return steps;
         }
        public List<GoogleMapsApi.Entities.Common.Location> getPolyline(Location L)
        {
          
            List<GoogleMapsApi.Entities.Common.Location> steps = new List<GoogleMapsApi.Entities.Common.Location>();
            DirectionsRequest directionsRequest = new DirectionsRequest()
            {
                Origin = this.lat + "," + this.lon,
                Destination = L.lat + "," + L.lon
            };
            directionsRequest.ApiKey = this.API_KEY;
            DirectionsResponse directionsResponse = GoogleMapsApi.GoogleMaps.Directions.Query(directionsRequest);
            GoogleMapsApi.Entities.Directions.Response.Leg leg = directionsResponse.Routes.ElementAt(0).Legs.ElementAt(0);
            foreach (var step in leg.Steps)
            {
                foreach (GoogleMapsApi.Entities.Common.Location point in step.PolyLine.Points)
                {
                    steps.Add(point);
                }
            }
            return steps;
        }
        public string getAdress() {
             PlacesRequest place = new PlacesRequest()
             {
                 Location=new GoogleMapsApi.Entities.Common.Location(this.lat,this.lon),
                 Radius=20
             };
             place.ApiKey=this.API_KEY;
            var response= GoogleMapsApi.GoogleMaps.Places.Query(place);
            return response.Results.First().Name;
         }
    }
}
