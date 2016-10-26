using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteProblem.model
{
    class Bus:IComparable
    {
       
        private int id;
        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }
        int orderBus;
        private List<StateBus> states;

        internal List<StateBus> States
        {
            get { return states; }
            set { states = value; }
        }

       
        private int capacity;

        public int Capacity
        {
            get { return capacity; }
            set { capacity = value; }
        }
        private Station curStation;

        public Station CurStation
        {
            get { return curStation; }
            set { curStation = value; }
        }
        private int distance;

        public int Distance
        {
            get { return distance; }
            set { distance = value; }
        }
        private int runningTime;

        public int RunningTime
        {
            get { return runningTime; }
            set { runningTime = value; }
        }
        private bool isComplete;

        public bool IsComplete
        {
            get { return isComplete; }
            set {     
                 isComplete = value; }
        }
        private List<Student> students;

        internal List<Student> Students
        {
            get { return students; }
           
        }
        public void addStudent(Student student) {
            this.students.Add(student);
        }
        private RouteProblem.Path path;

        internal Path Path
        {
            get { return path; }
            
        }

        public int OrderBus
        {
            get
            {
                return orderBus;
            }

            set
            {
                orderBus = value;
            }
        }

        public Bus(int id,int capacity) {

            this.Id = id;
            this.capacity = capacity;
            this.path = new RouteProblem.Path(id);
            this.isComplete = false;
            this.curStation = null;
            this.runningTime = 0;
            this.distance = 0;
            this.students = new List<Student>();
            this.states = new List<StateBus>();
        }
        public void ReSet()
        {
            this.path = new RouteProblem.Path(id);
            this.isComplete = false;
            this.curStation = null;
            this.runningTime = 0;
            this.distance = 0;
            this.students = new List<Student>();
            this.states = new List<StateBus>();
        }
        public int nStudentInStation(Station station){
            int count=0;
            foreach (Student student in this.students)
                if (student.Station.Id == station.Id)
                    count++;
            return count;

        }
        public void addStation(Station station){

            StateBus state;
            if (this.path.Stations.Count != 0)
            {
                this.runningTime += this.curStation.GetDuration(station) + this.curStation.Stoptime;
                this.distance += this.curStation.GetDistance(station);
                state = new StateBus(station.Id);
                state.RunningTime = this.runningTime;
                state.Distance = this.distance;
                this.states.Add(state);
                this.getState(curStation).TimeToBack = this.curStation.GetDuration(station);

            }
            else
            {
                state = new StateBus(station.Id);
                state.RunningTime = this.runningTime;
                state.Distance = this.distance;
                this.states.Add(state);
            }
            this.curStation = station;
            this.path.addStation(station);
          
            int nsit=this.getNumberSit() ;
            int nstudent=station.Students.Count;
            if(station.Id!=0)
            if (nsit > nstudent)
            {
                for (int i = 0; i < nstudent; i++)
                {
                    this.students.Add(station.Students[0]);
                    station.Students[0].Bus = this;
                    station.Students.RemoveAt(0);
                }
                state.StdentUP = nstudent;
            }
            else {
                for (int i = 0; i < nsit; i++)
                {
                    this.students.Add(station.Students[0]);
                    station.Students[0].Bus = this;
                    station.Students.RemoveAt(0);
                }
                state.StdentUP = nsit;
            }
           
   
          

        }
        public StateBus getState(Station station) { 
            foreach(StateBus state in this.states)
                if(state.Id==station.Id)
                    return state;
            return null;
        }
        public int getNumberSit() {
            return this.capacity-this.students.Count;
        }
        public bool isFull() {
            return this.capacity == this.students.Count;
        }
        public bool isAvailable() {
            if (!this.isComplete && !this.isFull())
                return true;
            else return false;
        }

        public int CompareTo(object obj)
        {
            if (this.runningTime> ((Bus)obj).runningTime)
                return 1;
            else if (this.runningTime < ((Bus)obj).runningTime) return -1;
            else return 0;
        }
    }
}
