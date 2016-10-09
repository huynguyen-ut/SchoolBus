using RouteProblem.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RouteProblem.routing;


using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
namespace RouteProblem.heuristic
{
     class  Heuristic:Route
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
         public Heuristic(School school, List<Station> stations, List<Bus> buses)
        {
            this.school =school;
            this.stations = stations;
            this.buses = buses;
            this.Limittime = 3000;
        }
        public void Run()
        {
            switch(mode){
                case 1: this.Heuristic1(); break;
               default: break;
            }
        }
        protected Bus SelectBus() {
            Bus bus = null;
            foreach (Bus i in this.buses)// Check Bus and select bus
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
        public void Heuristic1() {
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
                   // this.stations.Sort();
           }
        }
        public void PrintSolution()
        {
            foreach(Bus b in this.buses){
			  if(b.Path.Stations.Count!=0)
			  {Console.Write(b.Id+":");

			  foreach(Station s in b.Path.Stations){
				  Console.Write(s.Id+"->");
				  
			    }
              Console.WriteLine("School");
			  }}
        }
        public void PrintFileSolution() {
            String filePath = "../../data/output/solution.xls";
            String sheetName = "route";
            String fileExt = System.IO.Path.GetExtension(filePath);
            Stream file = new FileStream(filePath, FileMode.OpenOrCreate);
            IWorkbook workbook = null;
            if (fileExt == ".xls") workbook = new HSSFWorkbook();
            else if (fileExt == ".xlsx") workbook = new XSSFWorkbook();
            if (workbook != null)
            {
                ISheet sheet = workbook.CreateSheet(sheetName);
                if (sheet != null)
                {
                    
                    IRow header = sheet.CreateRow(0);
                    header.CreateCell(0).SetCellValue("Id");
                    header.CreateCell(1).SetCellValue("lat");
                    header.CreateCell(2).SetCellValue("lon");
                    header.CreateCell(3).SetCellValue("nStudent");
                    int i = 1;
                    foreach (Bus b in this.buses)
                    {
                        if (b.Path.Stations.Count != 0)
                        {
                          foreach (Station s in b.Path.Stations)
                            {
                                header = sheet.CreateRow(i++);
                                header.CreateCell(0).SetCellValue(b.Id);
                                header.CreateCell(1).SetCellValue(s.Lat);
                                header.CreateCell(2).SetCellValue(s.Lon);
                                header.CreateCell(3).SetCellValue(b.nStudentInStation(s));

                            }
                          header = sheet.CreateRow(i++);
                          header.CreateCell(0).SetCellValue(b.Id);
                          header.CreateCell(1).SetCellValue(this.school.Lat);
                          header.CreateCell(2).SetCellValue(this.school.Lon);
                        
                          header = sheet.CreateRow(i++);
                          header.CreateCell(0).SetCellValue("*");
                        }
                    }
                   
                }
                sheetName = "students";
                sheet = workbook.CreateSheet(sheetName);
                if (sheet != null)
                {

                    IRow header = sheet.CreateRow(0);
                    header.CreateCell(0).SetCellValue("Id");
                    header.CreateCell(1).SetCellValue("lat");
                    header.CreateCell(2).SetCellValue("lon");
                    header.CreateCell(3).SetCellValue("IdStattion");
                    header.CreateCell(4).SetCellValue("Name");
                    header.CreateCell(5).SetCellValue("Address");
                    header.CreateCell(6).SetCellValue("IdBus");
                     int i = 1;
                     foreach (Student student in this.school.Students)
                     {
                         header = sheet.CreateRow(i++);
                         header.CreateCell(0).SetCellValue(student.Id);
                         header.CreateCell(1).SetCellValue(student.Lat);
                         header.CreateCell(2).SetCellValue(student.Lon);
                         header.CreateCell(3).SetCellValue(student.Station.Id);
                         header.CreateCell(4).SetCellValue(student.Station.Name);
                         header.CreateCell(5).SetCellValue(student.Station.Address);
                         header.CreateCell(6).SetCellValue(student.Bus.Id);
                        
                     }

                }
              

                //////////////////////////////////////////////////////////
              sheetName = "summary";
              sheet = workbook.CreateSheet(sheetName);
              if (sheet != null)
              {

                  IRow header = sheet.CreateRow(0);
                  header.CreateCell(0).SetCellValue("Id");
                  header.CreateCell(1).SetCellValue("lat");
                  header.CreateCell(2).SetCellValue("lon");
                  header.CreateCell(3).SetCellValue("Name of station");
                  header.CreateCell(4).SetCellValue("nStudent");
                  header.CreateCell(5).SetCellValue("duration(s)");
                  header.CreateCell(6).SetCellValue("distance(m)");

                  int i = 1;
                  foreach (Bus b in this.buses)
                  {
                      if (b.Path.Stations.Count != 0)
                      {
                          header = sheet.CreateRow(i++);
                          header.CreateCell(0).SetCellValue(b.Id);
                          header.CreateCell(4).SetCellValue(b.Students.Count);
                          int nP=b.Path.Stations.Count;
                         
                          for (int index=0;index<nP;index++)
                          {   
                              header = sheet.CreateRow(i++);
                              header.CreateCell(0).SetCellValue(b.Path.Stations[index].Id);
                              header.CreateCell(1).SetCellValue(b.Path.Stations[index].Lat);
                              header.CreateCell(2).SetCellValue(b.Path.Stations[index].Lon);
                              header.CreateCell(3).SetCellValue(b.Path.Stations[index].Address);
                              header.CreateCell(4).SetCellValue(b.nStudentInStation(b.Path.Stations[index]));
                              header.CreateCell(5).SetCellValue( b.getState(b.Path.Stations[index]).RunningTime);
                              header.CreateCell(6).SetCellValue(b.getState(b.Path.Stations[index]).Distance);
                              int j=7;
                              if(index<b.Path.Stations.Count-1)
                              foreach (string dir in b.Path.Stations[index].getDirection(b.Path.Stations[index+1]))
                              {
                                  header.CreateCell(j++).SetCellValue(dir);
                              }else
                                  foreach (string dir in b.Path.Stations[index].getDirection(this.school))
                                  {
                                      header.CreateCell(j++).SetCellValue(dir);
                                  }

                          }
                          header = sheet.CreateRow(i++);
                        
                          header.CreateCell(1).SetCellValue(this.school.Lat);
                          header.CreateCell(2).SetCellValue(this.school.Lon);
                          header.CreateCell(5).SetCellValue(b.CurStation.GetDuration(this.school) + b.RunningTime);
                          header.CreateCell(6).SetCellValue(b.CurStation.GetDistance(this.school) + b.Distance);
                          header = sheet.CreateRow(i++);
                          header.CreateCell(0).SetCellValue("*");
                      }
                  }
                  workbook.Write(file);
              }
            }

            file.Close();
        }

    }
}
