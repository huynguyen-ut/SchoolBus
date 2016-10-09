using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SBPUtils;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace GetDistance
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("PROCESSING ...");
            Task.Run(async () =>
            {
                List<Station> stations = ReadExcel();
                int[,] distance = CreateMatrix(stations.Count);
                int[,] duration = CreateMatrix(stations.Count);
                await GetDistanceDuration(stations, distance, duration);
                SaveExcel(stations, distance, duration);
                Console.WriteLine("OK BABY !!!");
            }).Wait();
        }

        static List<Station> ReadExcel()
        {
            List<Station> stations = new List<Station>();

            String filePath = "../../data/data.xls";
            String sheetName = "Station";
            String fileExt = System.IO.Path.GetExtension(filePath);
            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            IWorkbook workbook = null;
            if (fileExt == ".xls") workbook = new HSSFWorkbook(file);
            else if (fileExt == ".xlsx") workbook = new XSSFWorkbook(file);
            if (workbook != null)
            {
                ISheet sheet = workbook.GetSheet(sheetName);
                if (sheet != null)
                {
                    for (int i = 1; i <= sheet.LastRowNum; i++) // skip header
                    {
                        IRow row = sheet.GetRow(i);
                        if (row != null) //null is when the row only contains empty cells 
                        {
                            Station station = new Station();
                            station.ID = int.Parse(row.GetCell(0).ToString().Trim());
                            station.Latitude = double.Parse(row.GetCell(1).ToString().Trim());
                            station.Longitude = double.Parse(row.GetCell(2).ToString().Trim());
                            stations.Add(station);
                        }
                    }
                }
            }
            file.Close();
            return stations;
        }

        static void SaveExcel(List<Station> stations, int[,] distance, int[,] duration)
        {
            String filePath = "../../data/data_output.xls";
            String sheetName = "Distance";
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
                    header.CreateCell(0).SetCellValue("Id1");
                    header.CreateCell(1).SetCellValue("Id2");
                    header.CreateCell(2).SetCellValue("Distance");
                    header.CreateCell(3).SetCellValue("Duration");

                    int index = 1;
                    int size = stations.Count;
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            IRow row = sheet.CreateRow(index++);
                            row.CreateCell(0).SetCellValue(i);
                            row.CreateCell(1).SetCellValue(j);
                            row.CreateCell(2).SetCellValue(distance[i, j]);
                            row.CreateCell(3).SetCellValue(duration[i, j]);
                        }
                    }
                }
                workbook.Write(file);
            }
            file.Close();
        }

        static async Task<int> GetDistanceDuration(List<Station> stations, int[,] distance, int[,] duration)
        {
            int size = stations.Count;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i == j)
                    {
                        distance[i, j] = 0;
                        duration[i, j] = 0;
                    }
                    else if (distance[i, j] == -1 && duration[i, j] == -1)
                    {
                        Station stationI = stations[i];
                        Station stationJ = stations[j];
                        String result = await GoogleAPI.GetDistanceDuration(stationI.Latitude.ToString(), stationI.Longitude.ToString(), stationJ.Latitude.ToString(), stationJ.Longitude.ToString());
                        String[] items = result.Split('\n');
                        distance[i, j] = int.Parse(items[0]);
                        duration[i, j] = int.Parse(items[1]);
                        distance[j, i] = distance[i, j];
                        duration[j, i] = duration[i, j];
                    }
                }
            }
            // for DEBUG
            /*for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Console.WriteLine(i + " - " + j + ": " + distance[i, j] + " - " + duration[i, j]);
                }
            }*/
            return 0;
        }

        static int[,] CreateMatrix(int size)
        {
            int[,] matrix = new int[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    matrix[i, j] = -1;
            return matrix;
        }
    }
}