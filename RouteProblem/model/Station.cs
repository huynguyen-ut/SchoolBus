using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteProblem.model
{
    class Station: Location, IComparable 
    {

        private int stoptime;
        private int nStudent;
        private School school;
        public int Stoptime
        {
            get { return stoptime; }
            set { stoptime = value; }
        }
        List<StateStation> states;

        internal List<StateStation> States
        {
            get { return states; }
       
        }
        List<Student> students;

        internal List<Student> Students
        {
            get { return students; }
            set { students = value; }
        }
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int NStudent
        {
            get
            {
                return nStudent;
            }

            set
            {
                nStudent = value;
            }
        }

        public int Priority
        {
            get
            {
                return priority;
            }

            set
            {
                priority = value;
            }
        }

        private int priority;

        public bool isEmpty() { 
            return this.students.Count==0?true:false;
        }
        public Station(int id,string name,int priority,string address,double lat,double lon,School school):base(id,address,lat,lon){
            this.students =new List<Student>();
            this.states = new List<StateStation>();
            this.Stoptime = 30;
            this.name = name;
            this.nStudent = 0;
            this.priority = priority;
            this.school = school;
        }
        public void Reset() {
            this.students.Clear();
            this.states.Clear();
            this.Stoptime = 30;
           // this.nStudent = 0;
       }
        
        public void addStudent(Student student) {
            this.students.Add(student);
            this.nStudent++;
        }
        public void addState(StateStation state) {
            this.states.Add(state);
        }
        public double DistanceEuclidToX(Station x) {
            double theta = this.Lon - x.Lon;
            double dist = Math.Sin(deg2rad(this.Lat)) * Math.Sin(deg2rad(x.Lat)) + Math.Cos(deg2rad(this.Lat)) * Math.Cos(deg2rad(x.Lat)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            //if (unit == "K") {
            dist = dist * 1.609344;
            //} else if (unit == "N") {
            //	dist = dist * 0.8684;
            //	}
            return (dist);
        }
        private static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }
        private static double rad2deg(double rad)
        {
            return (rad * 180 / Math.PI);
        }

        public int CompareTo(Object location)
        {
            Station loca = (Station)location;
           // if (this.students.Count >= loca.students.Count && this.students.Count >= 30)
           //     return -1;
           // else
            {
                /* if (this.Priority == 1 && loca.priority == 0)
                     return -1;
                 else
                 if (this.Priority == 0 && loca.priority == 1)
                     return 1;
                 else*/
                // if (this.DistanceToX[0] > loca.DistanceToX[0])
                if (this.DistanceEuclidToX(school) > loca.DistanceEuclidToX(school))
                    return -1;
                else return 1;
            }
        }

    }
}
