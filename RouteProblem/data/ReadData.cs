using RouteProblem.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteProblem.data
{
    class ReadData
    {
        private School school;

        internal School School
        {
            get { return school; }
            set { school = value; }
        }
        private List<Student> students;

        internal List<Student> Students
        {
            get { return students; }
            set { students = value; }
        }
        private List<Station> stations;

        internal List<Station> Stations
        {
            get { return stations; }
            set { stations = value; }
        }
        private List<Bus> buses;

        internal List<Bus> Buses
        {
            get { return buses; }
            set { buses = value; }
        }
        string FileStations;
        string FileStudents;
        string FileDistance;
        string FileBuses;
        
        public ReadData(string FileStations, string FileStudents, string FileBuses, string FileDistance)
        {
            this.stations = new List<Station>();
            this.students = new List<Student>();
            this.buses = new List<Bus>();
            this.FileStations = FileStations;
            this.FileStudents = FileStudents;
            this.FileDistance = FileDistance;
            this.FileBuses = FileBuses;
            this.ReadStation();
            this.ReadStudents();
            this.ReadDistance();
            this.ReadBus();
        }
        public void ReadBus() {
            using (StreamReader sr = new StreamReader(this.FileBuses))
            {
                string line;
                string[] tokens;
              
                while ((line = sr.ReadLine()) != null)
                {
                    tokens = line.Split('\t');
                    this.buses.Add(new Bus(int.Parse(tokens[0]), int.Parse(tokens[1])));

                }
            }
        }
        public void ReadStation() {
   
            using (StreamReader sr = new StreamReader(this.FileStations))
            {
                string line=sr.ReadLine();
                string[] tokens;  
                tokens = line.Split('\t');
                this.school = new School(int.Parse(tokens[0]),tokens[4],tokens[5], Double.Parse(tokens[1]), Double.Parse(tokens[2]),this.students);
                while ((line = sr.ReadLine()) != null)
                {
                    tokens = line.Split('\t');
                    this.stations.Add(new Station(int.Parse(tokens[0]),tokens[4], 1,tokens[4], Double.Parse(tokens[1]), Double.Parse(tokens[2]),school));
                    
                }
            }
           
            
        }
        public void ReadDistance()
        {
            using (StreamReader sr = new StreamReader(this.FileDistance))
            {
                string line;
       
                string[] tokens ;
                int i =1;
                int x = -1;
                while ((line = sr.ReadLine()) != null)
                {
                    tokens = line.Split('\t');
                    if (i <= (this.stations.Count+1))
                    {
                        this.school.DistanceToX.Add(int.Parse(tokens[0]));
                        this.school.DurationToX.Add(int.Parse(tokens[1]));
                    }
                 
                    else
                     {
                        this.stations[x].DistanceToX.Add(int.Parse(tokens[0]));
                        this.stations[x].DurationToX.Add(int.Parse(tokens[1]));
                      
                    }
                    if (i % (this.stations.Count+1) == 0)
                    {
                        x++;
                    } 
                    i++;
                }
             
            }

        }
        public void ReadStudents()
        {
            
            using (StreamReader sr = new StreamReader(this.FileStudents))
            {
                string line;
                string[] tokens;
                Student student;
                Station station;
                while ((line = sr.ReadLine()) != null)
                {
                    tokens = line.Split('\t');
                    if (tokens[1] !="")
                    {
                        station = this.getStation(int.Parse(tokens[3]));
                        student = new Student(int.Parse(tokens[0]), double.Parse(tokens[1]), double.Parse(tokens[2]), tokens[4], this.school, station);
                        this.students.Add(student);
                        station.addStudent(student);
                    }
                    
                   
                   
                }
            }

        }
        private Station getStation(int id) {
            foreach (Station station in this.stations)
                if (station.Id == id)
                    return station;
            return null;
         }

    }
}
