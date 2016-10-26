using RouteProblem.data;
using RouteProblem.heuristic;
using System;


namespace RouteProblem
{
    class Program
    {
        static void Main(string[] args)
        {
            ReadData data;
            Heuristic heuristic;
            data = new ReadData("../../data/input/stations.txt", "../../data/input/students_di_s.txt", "../../data/input/Buses.txt", "../../data/input/distance.txt");
            heuristic = new Heuristic(data.School, data.Stations, data.Buses);

            heuristic.Mode = 1;
            heuristic.Limittime = 1800;
            heuristic.StartTime = 21600;
            heuristic.Run();
            heuristic.PrintSolution();

            data = new ReadData("../../data/input/stations.txt", "../../data/input/students_di_s.txt", "../../data/input/Buses.txt", "../../data/input/distance.txt");
            heuristic = new Heuristic(data.School, data.Stations, data.Buses);
            heuristic.Mode = 3;
           // heuristic.ReSet();
            heuristic.Limittime = 1800;
            heuristic.StartTime = 21600;
            heuristic.Run();
            heuristic.PrintSolution();
           // heuristic.PrintFileSolution("Solution_Arrival_06h");

            //data = new ReadData("../../data/input/stations.txt", "../../data/input/students_ve_s.txt", "../../data/input/Buses.txt", "../../data/input/distance.txt");
            //heuristic = new Heuristic(data.School, data.Stations, data.Buses);
            //heuristic.Mode = 2;
            //heuristic.Limittime = 1800;
            //heuristic.StartTime = 41400;
            //heuristic.Run();
            //heuristic.PrintSolution();
            //heuristic.PrintFileSolution("Solution_Back_11h30");


            //data = new ReadData("../../data/input/stations.txt", "../../data/input/students_di_c.txt", "../../data/input/Buses.txt", "../../data/input/distance.txt");
            //heuristic = new Heuristic(data.School, data.Stations, data.Buses);
            //heuristic.Mode = 1;
            //heuristic.Limittime = 1800;
            //heuristic.StartTime = 43200;
            //heuristic.Mode = 1;
            //heuristic.Run();
            //heuristic.PrintSolution();
            //heuristic.PrintFileSolution("Solution_Arrival_12h");

            //data = new ReadData("../../data/input/stations.txt", "../../data/input/students_ve_c.txt", "../../data/input/Buses.txt", "../../data/input/distance.txt");
            //heuristic = new Heuristic(data.School, data.Stations, data.Buses);
            //heuristic.Mode = 2;
            //heuristic.Limittime = 1800;
            //heuristic.StartTime = 58680;
            //heuristic.Run();
            //heuristic.PrintSolution();
            //heuristic.PrintFileSolution("Solution_Back_16h30");
            // Route f = new Route();
            // f.Show();
            Console.ReadKey();


        }
    }
}
