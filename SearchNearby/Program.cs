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
using System.Device.Location; // Assembly: System.Device (in System.Device.dll)

namespace SearchNearby
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("PROCESSING ...");
            Task.Run(async () =>
            {
                List<Cluster> clusters = await ReadExcel();
                SaveExcel(clusters);
                Console.WriteLine("OK BABY !!!");
            }).Wait();
        }

        static async Task<List<Cluster>> ReadExcel()
        {
            List<Cluster> clusters = new List<Cluster>();

            String filePath = "../../data/data.xls";
            String sheetName = "Cluster";
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
                            Cluster cluster = new Cluster();
                            cluster.ID = int.Parse(row.GetCell(0).ToString().Trim());
                            cluster.Latitude = double.Parse(row.GetCell(1).ToString().Trim());
                            cluster.Longitude = double.Parse(row.GetCell(2).ToString().Trim());
                            cluster.Nb = int.Parse(row.GetCell(3).ToString().Trim());

                            int radius = 500;
                            String result = await GoogleAPI.NearbySearchPlaces(cluster.Latitude.ToString(), cluster.Longitude.ToString(), radius);
                            if (result != null)
                            {
                                List<GPlace> places = new List<GPlace>();
                                result = result.Trim();
                                String[] items = result.Split('\n');
                                foreach (String item in items)
                                {
                                    String[] items2 = item.Split('$');
                                    if (items2.Length >= 5)
                                    {
                                        GPlace place = new GPlace();
                                        place.ID = items2[0].Trim();
                                        place.Name = items2[1].Trim();
                                        place.Address = items2[2].Trim();
                                        place.Latitude = double.Parse(items2[3].Trim());
                                        place.Longitude = double.Parse(items2[4].Trim());
                                        GeoCoordinate sCoord = new GeoCoordinate(cluster.Latitude, cluster.Longitude);
                                        GeoCoordinate eCoord = new GeoCoordinate(place.Latitude, place.Longitude);
                                        place.Distance = sCoord.GetDistanceTo(eCoord);
                                        places.Add(place);
                                    }
                                }
                                List<GPlace> sortedPlaces = places.OrderBy(o => o.Distance).ToList(); // sort by Distance
                                cluster.Places = sortedPlaces;
                            }
                            clusters.Add(cluster);
                        }
                    }
                }
            }
            file.Close();
            return clusters;
        }

        static void SaveExcel(List<Cluster> clusters)
        {
            String filePath = "../../data/data_output.xls";
            String sheetName1 = "Places";
            String sheetName2 = "Station";
            String fileExt = System.IO.Path.GetExtension(filePath);
            Stream file = new FileStream(filePath, FileMode.OpenOrCreate);

            IWorkbook workbook = null;
            if (fileExt == ".xls") workbook = new HSSFWorkbook();
            else if (fileExt == ".xlsx") workbook = new XSSFWorkbook();
            if (workbook != null)
            {
                // create sheetName1
                ISheet sheet = workbook.CreateSheet(sheetName1);
                if (sheet != null)
                {
                    IRow header = sheet.CreateRow(0);
                    header.CreateCell(0).SetCellValue("ClusterID");
                    header.CreateCell(1).SetCellValue("PlaceID");
                    header.CreateCell(2).SetCellValue("Name");
                    header.CreateCell(3).SetCellValue("Address");
                    header.CreateCell(4).SetCellValue("Latitude");
                    header.CreateCell(5).SetCellValue("Longitude");
                    header.CreateCell(6).SetCellValue("Distance");
                    int index = 1;
                    foreach (Cluster cluster in clusters)
                    {
                        foreach (GPlace place in cluster.Places)
                        {
                            IRow row = sheet.CreateRow(index++);
                            row.CreateCell(0).SetCellValue(cluster.ID);
                            row.CreateCell(1).SetCellValue(place.ID);
                            row.CreateCell(2).SetCellValue(place.Name);
                            row.CreateCell(3).SetCellValue(place.Address);
                            row.CreateCell(4).SetCellValue(place.Latitude);
                            row.CreateCell(5).SetCellValue(place.Longitude);
                            row.CreateCell(6).SetCellValue(place.Distance);
                        }
                    }
                }

                // create sheetName2
                sheet = workbook.CreateSheet(sheetName2);
                if (sheet != null)
                {
                    IRow header = sheet.CreateRow(0);
                    header.CreateCell(0).SetCellValue("StationID");
                    header.CreateCell(1).SetCellValue("Latitude");
                    header.CreateCell(2).SetCellValue("Longitude");
                    header.CreateCell(3).SetCellValue("Nb");
                    header.CreateCell(4).SetCellValue("Name");
                    header.CreateCell(5).SetCellValue("Address");
                    header.CreateCell(6).SetCellValue("Distance");
                    int index = 1;
                    foreach (Cluster cluster in clusters)
                    {
                        foreach (GPlace place in cluster.Places)
                        {
                            if (place.Distance <= 500 && !place.Address.Contains("/") && !place.Address.Contains("{"))
                            {
                                IRow row = sheet.CreateRow(index++);
                                row.CreateCell(0).SetCellValue(cluster.ID);
                                row.CreateCell(1).SetCellValue(place.Latitude);
                                row.CreateCell(2).SetCellValue(place.Longitude);
                                row.CreateCell(3).SetCellValue(cluster.Nb);
                                row.CreateCell(4).SetCellValue(place.Name);
                                row.CreateCell(5).SetCellValue(place.Address);
                                row.CreateCell(6).SetCellValue(place.Distance);
                                break;
                            }
                        }
                    }
                }
                workbook.Write(file);
            }
            file.Close();
        }
    }
}