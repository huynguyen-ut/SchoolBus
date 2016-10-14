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

        public bool isEmpty() { 
            return this.students.Count==0?true:false;
        }
        public Station(int id,string name,string address,double lat,double lon):base(id,address,lat,lon){
            this.students =new List<Student>();
            this.states = new List<StateStation>();
            this.Stoptime = 30;
            this.name = name;
            this.nStudent = 0;
        }
        public void addStudent(Student student) {
            this.students.Add(student);
            this.nStudent++;
        }
        public void addState(StateStation state) {
            this.states.Add(state);
        }
        public int CompareTo(Object location)
        {
            Station loca = (Station)location;
            if (this.students.Count >= loca.students.Count && this.students.Count >= 30)
                return -1;
            else
            {
                if (loca.students.Count >= 30)
                    return 1;
                else
                if (this.DistanceToX[0] > loca.DistanceToX[0])
                    return -1;
                else return 1;
            }
        }

    }
}
