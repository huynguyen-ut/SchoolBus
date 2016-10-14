using RouteProblem.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RouteProblem.routing;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
namespace RouteProblem.heuristic
{
     class  Heuristic
    {
        private int mode;

        public int Mode
        {
            get { return mode; }
            set { mode = value; }
        }
        private int LimitTime;

        public int Limittime
        {
            get { return LimitTime; }
            set { LimitTime = value; }
        }
        private School school;
        private List<Station> stations;
        private List<Bus> buses;
        private List<Route> routes;
         public Heuristic(School school, List<Station> stations, List<Bus> buses)
        {
            this.school =school;
            this.stations = stations;
            this.buses = buses;
            this.Limittime = 3000;
            this.routes = new List<Route>();
        }
        public void Run()
        {
            switch(mode){
                case 1: this.Solve();
                        this.Routing();
                        this.PostProcessing();
                        break;
                default: break;
            }
        }
        protected Bus SelectBus() {
            Bus bus = null;
            foreach(Bus i in this.buses)// Check Bus and select bus
                if (!i.IsComplete)
                {
                    bus = i;
                    break;
                }               
            return bus;
        }
        protected Station SelectStation() {
            Station station = null;
            foreach (Station si in this.stations)// check station and select station
                if (!si.isEmpty())
                {
                    station = si;
                    break;
                }
            return station;
        }
        public void Solve() {
            this.stations.Sort();
            Bus bus=null;
            Station station=null;
            int time;
            while (true) { // initial 1 route
                if ((bus=SelectBus() )== null)
                    break;
                if ((station = this.SelectStation()) == null)
                    break;
               
                    bus.addStation(station);
                     time=bus.RunningTime+bus.CurStation.GetDuration(this.school);
                    while(bus.isAvailable()&&time<this.LimitTime)// Check Bus
                    {
                         int Min = this.Limittime;
                         time=0;
                         station = null;
                        foreach (Station si in this.stations) {
                            if (si.Id != bus.CurStation.Id && !si.isEmpty())// determine next station
                            {
                                time = bus.CurStation.GetDuration(si);
                                if ( time< Min)
                                {
                                    if (bus.RunningTime + bus.CurStation.GetDuration(si) + si.GetDuration(this.school) < this.Limittime)
                                    {
                                        station = si;
                                        Min = time;
                                    }
                                 }
                            }
                        }
                        if (station != null)
                            bus.addStation(station);
                        else break;
                    }
                    bus.IsComplete = true;
                    bus.addStation(this.school);
                   // bus.RunningTime += bus.CurStation.GetDuration(this.school);
                   // bus.Distance+= bus.CurStation.GetDistance(this.school);
                // this.stations.Sort();
            }
           // this.Routing();
        }
        public void Routing() {
            bool flag = false;
            int id=0;
            foreach (Bus bus in this.buses)
            if(bus.RunningTime>0){
                flag = false;
                if (this.routes.Count>0)
                foreach (Route route in this.routes) {
                    if (route.isBelongToRoute(bus.Path))
                    {   route.addBus(bus);
                        flag = true;
                        break;
                    }
                }
                if (flag == false) {
                    id++;
                    Route r = new Route(id,bus.Path);
                    this.routes.Add(r);
                    r.addBus(bus);
                   
                }
            }
        }
        public void PostProcessing() {
            this.buses.Sort();
            int dura = 0;
            bool flag=true;// first route
            int startTime = 0;
            int orderBus=1;
            foreach (Bus bus in this.buses)
            if(bus.RunningTime>0){
                if (flag) {
                    dura +=bus.RunningTime;
                    flag = false;
                    }
                    else {
                        startTime = dura+this.school.GetDuration(bus.Path.getFirstStation());
                        dura += this.school.GetDuration(bus.Path.getFirstStation()) + bus.RunningTime;
                   }
                if (dura <= this.LimitTime)
                {
                        foreach (Station station in bus.Path.Stations) {
                            bus.getState(station).RunningTime+=startTime;
                        }
                        bus.OrderBus= orderBus;
                }
                else {
                        dura = 0;
                        startTime = 0;
                        bus.OrderBus= ++orderBus;
                        dura += bus.RunningTime;
                    }
            }
        }
        public void PrintSolution()
        {
            foreach(Route route in this.routes){
			  if(route.Path.Stations.Count!=0)
			  {Console.Write(route.Id+":");

			  foreach(Station s in route.Path.Stations){
				  Console.Write(s.Id+"->");
				  
			    }
              Console.WriteLine("School");
			  }}
        }
        
        public void PrintFileSolution() {
            String filePath = "../../data/output/solution.xls";
            String sheetName ;
            String fileExt = System.IO.Path.GetExtension(filePath);
            Stream file = new FileStream(filePath, FileMode.OpenOrCreate);
            IWorkbook workbook = null;
            if (fileExt == ".xls") workbook = new HSSFWorkbook();
            else if (fileExt == ".xlsx") workbook = new XSSFWorkbook();
            if (workbook != null)
            {
                ISheet sheet ;
                sheetName = "students_route";
                sheet = workbook.CreateSheet(sheetName);
                if (sheet != null)
                {

                    IRow header = sheet.CreateRow(0);
                    header.CreateCell(0).SetCellValue("Id");
                    header.CreateCell(1).SetCellValue("lat");
                    header.CreateCell(2).SetCellValue("lon");
                    header.CreateCell(3).SetCellValue("IdStattion");
                    header.CreateCell(4).SetCellValue("Name of Staion");
                    header.CreateCell(5).SetCellValue("Address");
                    header.CreateCell(6).SetCellValue("IdRoute");
                    header.CreateCell(7).SetCellValue("IdBus");
                    header.CreateCell(8).SetCellValue("Arrival Time");
                    int i = 1;
                    foreach(Route route in this.routes)
                        foreach(Bus bus in route.Buses)
                     foreach (Student student in bus.Students)
                     {
                         header = sheet.CreateRow(i++);
                         header.CreateCell(0).SetCellValue(student.Id);
                         header.CreateCell(1).SetCellValue(student.Lat);
                         header.CreateCell(2).SetCellValue(student.Lon);
                         header.CreateCell(3).SetCellValue(student.Station.Id);
                         header.CreateCell(4).SetCellValue(student.Station.Name);
                         header.CreateCell(5).SetCellValue(student.Station.Address);
                         header.CreateCell(6).SetCellValue(route.Id);
                         header.CreateCell(7).SetCellValue(bus.OrderBus);
                         header.CreateCell(8).SetCellValue("6h"+bus.getState(student.Station).RunningTime/60);

                            }

                }

                sheetName = "route";
                sheet = workbook.CreateSheet(sheetName);
                if (sheet != null)
                {
                    IRow header = sheet.CreateRow(0);
                    header.CreateCell(0).SetCellValue("IdRoute");
                    header.CreateCell(1).SetCellValue("lat");
                    header.CreateCell(2).SetCellValue("lon");
                    header.CreateCell(3).SetCellValue("IdStattion");
                   // header.CreateCell(4).SetCellValue("Name");
                  //  header.CreateCell(5).SetCellValue("Address");
                    int i=0;
                    int j = 1;
                 
                    foreach (Route route in this.routes)
                    {
                        if(route.Buses.Count>0)
                        for(int index=0; index <route.Path.Stations.Count; index++)
                        {
                            header = sheet.CreateRow(j++);
                            header.CreateCell(0).SetCellValue(route.Id);
                            header.CreateCell(1).SetCellValue(route.Path.Stations[index].Lat);
                            header.CreateCell(2).SetCellValue(route.Path.Stations[index].Lon);
                            header.CreateCell(3).SetCellValue(route.Path.Stations[index].Id);
                            header.CreateCell(4).SetCellValue(route.Path.Stations[index].NStudent);
                            i = 5;
                            if(index< route.Path.Stations.Count-1)
                            foreach(GoogleMapsApi.Entities.Common.Location location in route.Path.Stations[index].getPolyline(route.Path.Stations[index+1]))
                            header.CreateCell(i++).SetCellValue(location.Latitude.ToString()+";"+location.Longitude.ToString());
                           
                            // header.CreateCell(i++).SetCellValue(route.Path.Stations[index].Name);
                           // header.CreateCell(i++).SetCellValue(route.Path.Stations[index].Address);
                        }
                        header = sheet.CreateRow(j++);
                        header.CreateCell(0).SetCellValue("*");
                    }
                        

                }
                    //////////////////////////////////////////////////////////
                    sheetName = "summary_route";
              sheet = workbook.CreateSheet(sheetName);
              if (sheet != null)
              {

                  IRow header = sheet.CreateRow(0);
                  int i = 1;
                  foreach(Route route in this.routes)
                    {
                        header = sheet.CreateRow(i++);
                        header.CreateCell(0).SetCellValue("Route: "+ route.Id);
                        foreach (Bus b in route.Buses)
                        {
                            if (b.Path.Stations.Count != 0)
                            {
                                header = sheet.CreateRow(i++);
                                header.CreateCell(0).SetCellValue("Bus:"+b.OrderBus);
                                int nP = b.Path.Stations.Count;
                             
                                for (int index = 0; index < nP; index++)
                                {
                                   header.CreateCell(index+1).SetCellValue("Station:"+b.Path.Stations[index].Id);
                                                  

                                }
                                header = sheet.CreateRow(i++);
                                header.CreateCell(0).SetCellValue("Arrival time:");
                                for (int index = 0; index < nP; index++)
                                {
                                    header.CreateCell(index + 1).SetCellValue(b.getState(b.Path.Stations[index]).RunningTime);


                                }
                                header = sheet.CreateRow(i++);
                                header.CreateCell(0).SetCellValue("Student in station:" );
                                for (int index = 0; index < nP; index++)
                                {
                                    header.CreateCell(index + 1).SetCellValue(b.getState(b.Path.Stations[index]).StdentUP);
                                }

                                
                            }
                            header = sheet.CreateRow(i++);
                            header.CreateCell(0).SetCellValue("*");
                        }
                 
                  }
                  
              }
                //////////////////////////////////////
                sheetName = "summary_total";
                sheet = workbook.CreateSheet(sheetName);
                if (sheet != null)
                {

                    IRow header = sheet.CreateRow(0);
                    header.CreateCell(0).SetCellValue("IdStation");
                    header.CreateCell(1).SetCellValue("lat");
                    header.CreateCell(2).SetCellValue("lon");
                    header.CreateCell(3).SetCellValue("Name of station");

                    int i = 1;
                    foreach (Route route in this.routes)
                    {
                        if (route.Path.Stations.Count != 0)
                        {
                            header = sheet.CreateRow(i++);
                            header.CreateCell(0).SetCellValue(route.Id);
                            int nP = route.Path.Stations.Count;

                            for (int index = 0; index < nP; index++)
                            {
                                header = sheet.CreateRow(i++);
                                header.CreateCell(0).SetCellValue(route.Path.Stations[index].Id);
                                header.CreateCell(1).SetCellValue(route.Path.Stations[index].Lat);
                                header.CreateCell(2).SetCellValue(route.Path.Stations[index].Lon);
                                header.CreateCell(3).SetCellValue(route.Path.Stations[index].Address);
                               
                                int j = 7;
                                if (index < route.Path.Stations.Count - 1)
                                    foreach (string dir in route.Path.Stations[index].getDirection(route.Path.Stations[index + 1]))
                                    {
                                        header.CreateCell(j++).SetCellValue(dir);
                                    }
                                else
                                    foreach (string dir in route.Path.Stations[index].getDirection(this.school))
                                    {
                                        header.CreateCell(j++).SetCellValue(dir);
                                    }

                            }
                                                    
                            header = sheet.CreateRow(i++);
                            header.CreateCell(0).SetCellValue("*");
                        }
                    }
                   
                }
                workbook.Write(file);
            }

            file.Close();
        }

    }
}
