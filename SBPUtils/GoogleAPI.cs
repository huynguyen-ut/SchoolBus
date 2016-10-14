using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SBPUtils
{
    /* ref: https://developers.google.com/maps/documentation */
    public class GoogleAPI
    {
        /* ref: https://developers.google.com/maps/documentation/geocoding/intro */
        public static async Task<String> Geocode(String address)
        {
            String returnValue = null;
            WebClient client = new WebClient();
            client.Headers["Accept"] = "application/json";
            client.DownloadStringCompleted += (s1, e1) =>
            {
                String json = e1.Result.ToString();
                JObject jsResult = JObject.Parse(json);
                String status = (String)jsResult["status"];
                if ("OK".Equals(status))
                {
                    JArray results = (JArray)jsResult["results"];
                    JObject top1 = (JObject)results[0];
                    String formatted_address = (String)top1["formatted_address"];
                    formatted_address = Encoding.UTF8.GetString(Encoding.Default.GetBytes(formatted_address)); // convert to UTF8
                    JObject geometry = (JObject)top1["geometry"];
                    JObject location = (JObject)geometry["location"];
                    String lat = (String)location["lat"];
                    String lng = (String)location["lng"];
                    returnValue = lat + "\n" + lng + "\n" + formatted_address;
                }
            };
            String uri = String.Format("http://maps.googleapis.com/maps/api/geocode/json?address={0}", address);
            await client.DownloadStringTaskAsync(new Uri(uri));
            return returnValue;
        }
        public static async Task<String> ReverseGeocode(String lat, String lng)
        {
            String returnValue = null;
            WebClient client = new WebClient();
            client.Headers["Accept"] = "application/json";
            client.DownloadStringCompleted += (s1, e1) =>
            {
                String json = e1.Result.ToString();
                JObject jsResult = JObject.Parse(json);
                String status = (String)jsResult["status"];
                if ("OK".Equals(status))
                {
                    JArray results = (JArray)jsResult["results"];
                    JObject top1 = (JObject)results[0];
                    String formatted_address = (String)top1["formatted_address"];
                    formatted_address = Encoding.UTF8.GetString(Encoding.Default.GetBytes(formatted_address)); // convert to UTF8
                    returnValue = formatted_address;
                }
            };
            String uri = String.Format("http://maps.googleapis.com/maps/api/geocode/json?latlng={0},{1}", lat, lng);
            await client.DownloadStringTaskAsync(new Uri(uri));
            return returnValue;
        }

        /* ref: https://developers.google.com/maps/documentation/distance-matrix/start */
        public static async Task<String> GetDistanceDuration(String from, String to)
        {
            String returnValue = null;
            WebClient client = new WebClient();
            client.Headers["Accept"] = "application/json";
            client.DownloadStringCompleted += (s1, e1) =>
            {
                String json = e1.Result.ToString();
                JObject result = JObject.Parse(json);
                String status = (String)result["status"];
                if ("OK".Equals(status))
                {
                    JArray rows = (JArray)result["rows"];
                    JObject row = (JObject)rows[0];
                    JArray elements = (JArray)row["elements"];
                    JObject element = (JObject)elements[0];
                    String status2 = (String)element["status"];
                    if ("OK".Equals(status2))
                    {
                        JObject distance = (JObject)element["distance"];
                        long dist = (long)distance["value"];
                        JObject duration = (JObject)element["duration"];
                        long dur = (long)duration["value"];
                        returnValue = dist + "\n" + dur;
                    }
                }
            };
            String uri = String.Format("http://maps.googleapis.com/maps/api/distancematrix/json?origins={0}&destinations={1}", from, to);
            await client.DownloadStringTaskAsync(new Uri(uri));
            return returnValue;
        }
        public static async Task<String> GetDistanceDuration(String lat1, String lng1, String lat2, String lng2)
        {
            String API_KEY = "AIzaSyCI23HUoG4zqAvSwheflDAXaoYDX0cB96c";
            String returnValue = null;
            WebClient client = new WebClient();
            client.Headers["Accept"] = "application/json";
            client.DownloadStringCompleted += (s1, e1) =>
            {
                String json = e1.Result.ToString();
                JObject result = JObject.Parse(json);
                String status = (String)result["status"];
                if ("OK".Equals(status))
                {
                    JArray rows = (JArray)result["rows"];
                    JObject row = (JObject)rows[0];
                    JArray elements = (JArray)row["elements"];
                    JObject element = (JObject)elements[0];
                    String status2 = (String)element["status"];
                    if ("OK".Equals(status2))
                    {
                        JObject distance = (JObject)element["distance"];
                        long dist = (long)distance["value"];
                        JObject duration = (JObject)element["duration"];
                        long dur = (long)duration["value"];
                        returnValue = dist + "\n" + dur;
                    }
                }
            };
            String uri = String.Format("https://maps.googleapis.com/maps/api/distancematrix/json?origins={0},{1}&destinations={2},{3}&key={4}", lat1, lng1, lat2, lng2, API_KEY);
            await client.DownloadStringTaskAsync(new Uri(uri));
            return returnValue;
        }

        /* ref: https://developers.google.com/maps/documentation/roads/nearest */
        public static async Task<String> GetNearestRoad(String lat, String lng)
        {
            String returnValue = null;
            String API_KEY = "AIzaSyCMixsUHVQxCI1lRjeuYlnw44rguIIVu78"; // enable Google Maps Roads API
            WebClient client = new WebClient();
            client.Headers["Accept"] = "application/json";
            client.DownloadStringCompleted += (s1, e1) =>
            {
                String json = e1.Result.ToString();
                JObject jsResult = JObject.Parse(json);
                JArray snappedPoints = (JArray)jsResult["snappedPoints"];
                if (snappedPoints.Count > 0)
                {
                    JObject top1 = (JObject)snappedPoints[0];
                    JObject location = (JObject)top1["location"];
                    String latitude = (String)location["latitude"];
                    String longitude = (String)location["longitude"];
                    returnValue = latitude + "\n" + longitude;
                }
            };
            String uri = String.Format("https://roads.googleapis.com/v1/nearestRoads?points={0},{1}&key={2}", lat, lng, API_KEY);
            await client.DownloadStringTaskAsync(new Uri(uri));
            return returnValue;
        }

        /* ref: https://developers.google.com/places/web-service/search */
        public static async Task<String> NearbySearchPlaces(String lat, String lng, int radius)
        {
            String returnValue = null;
            String API_KEY = "AIzaSyCMixsUHVQxCI1lRjeuYlnw44rguIIVu78"; // enable Google Places API Web Service
            WebClient client = new WebClient();
            client.Headers["Accept"] = "application/json";
            client.DownloadStringCompleted += (s1, e1) =>
            {
                String json = e1.Result.ToString();
                JObject jsResult = JObject.Parse(json);
                JArray results = (JArray)jsResult["results"];
                for(int i = 0; i < results.Count; i++)
                {
                    JObject place = (JObject)results[i];
                    String name = (String)place["name"];
                    name = Encoding.UTF8.GetString(Encoding.Default.GetBytes(name)); // convert to UTF8
                    String address = (String)place["vicinity"];
                    address = Encoding.UTF8.GetString(Encoding.Default.GetBytes(address)); // convert to UTF8
                    String id = (String)place["place_id"];
                    JObject geometry = (JObject)place["geometry"];
                    JObject location = (JObject)geometry["location"];
                    String latitude = (String)location["lat"];
                    String longitude = (String)location["lng"];
                    returnValue += id + "$" + name + "$" + address + "$" + latitude + "$" + longitude + "\n";
                }
            };
            String uri = String.Format("https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={0},{1}&radius={2}&key={3}", lat, lng, radius, API_KEY);
            await client.DownloadStringTaskAsync(new Uri(uri));
            return returnValue;
        }
    }
}