using RouteProblem.data;
using RouteProblem.heuristic;
using System;


namespace RouteProblem
{
    class Program
    {
        static void Main(string[] args)
        {
            ReadData data = new ReadData("../../data/input/stations.txt", "../../data/input/students.txt", "../../data/input/Buses.txt", "../../data/input/distance.txt");
            Heuristic heuristic = new Heuristic(data.School,data.Stations,data.Buses);
            heuristic.Mode = 1;
            heuristic.Limittime = 2400;
            heuristic.Run();
            heuristic.PrintSolution();
            heuristic.PrintFileSolution();
           
           // Route f = new Route();
           // f.Show();
            Console.ReadKey();


        }
    }
}
