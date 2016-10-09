using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGNES
{
    class Program
    {
        static void Main(string[] args)
        {
            int limitDistance = 500;
            HSSFWorkbook wb = null;
            // create xls if not exists
            if (File.Exists("../../data/data.xls"))
            {
                FileStream file = new FileStream("../../data/data.xls", FileMode.Open, FileAccess.Read);
                wb = new HSSFWorkbook(file);
            }
            List<Point> points = InitPoints(wb);
            HashSet<Cluster> clusters = new HashSet<Cluster>();
            // assign each point to each cluster
            foreach (var point in points)
            {
                List<Point> tempList = new List<Point>();
                tempList.Add(point);
                clusters.Add(new Cluster() { Id = point.Id, CenterGPS = point.GPS, Points = tempList });
            }
            List<Cluster> minClusterPair = new List<Cluster>();
            double min = double.MaxValue;
            while (clusters.Count > 1)
            {
                // find two groups which are the nearest pair use linkable avarage
                for (int i = 0; i < clusters.Count-1; i++)
                {
                    for (int j = i+1; j < clusters.Count; j++)
                    {
                        if (i != j)
                        {
                            if (clusters.ElementAt(i).CenterGPS.GetDistanceTo(clusters.ElementAt(j).CenterGPS) < min)
                            {
                                minClusterPair = new List<Cluster>();
                                minClusterPair.Add(clusters.ElementAt(i));
                                minClusterPair.Add(clusters.ElementAt(j));
                                min = clusters.ElementAt(i).CenterGPS.GetDistanceTo(clusters.ElementAt(j).CenterGPS);
                            }
                        }
                    }
                }
             
                // merge two nearest groups become once group
                Cluster mergeCluster = new Cluster();
                mergeCluster.Id = minClusterPair.FirstOrDefault().Id;
                mergeCluster.Points = minClusterPair.FirstOrDefault().Points.Concat(minClusterPair.LastOrDefault().Points).ToList();
                mergeCluster.CenterGPS = new GeoCoordinate();
                mergeCluster.CenterGPS.Latitude = mergeCluster.Points.Sum(x => x.GPS.Latitude) / mergeCluster.Points.Count;
                mergeCluster.CenterGPS.Longitude = mergeCluster.Points.Sum(x => x.GPS.Longitude) / mergeCluster.Points.Count;
                if (CheckAvailableCluster(mergeCluster, limitDistance))
                {
                    clusters.Remove(minClusterPair.FirstOrDefault());
                    clusters.Remove(minClusterPair.LastOrDefault());
                    clusters.Add(mergeCluster);
                    minClusterPair = new List<Cluster>();
                    min = double.MaxValue;
                }
                else
                {
                    Debug.WriteLine("min: " + min);
                    foreach (var cluster in clusters)
                    {
                        Debug.WriteLine(cluster.Id);
                        foreach (var point in cluster.Points)
                        {
                            Debug.Write(point.Id + "    ");
                        }
                        Debug.WriteLine("");
                    }
                    break;
                }
               
            }            

            StoreBusStop(wb, clusters);
            StoreStation(wb, clusters);
            SaveExcel(wb);
        }

        static bool CheckAvailableCluster(Cluster cluster, double limitDistance){
            double max = -1;
            foreach (var point in cluster.Points)
            {
                if (point.GPS.GetDistanceTo(cluster.CenterGPS) > max)
                {
                    max = point.GPS.GetDistanceTo(cluster.CenterGPS);
                }
            }
            if (max > limitDistance)
                return false;
            else
                return true;
        }

        static List<Point> InitPoints(HSSFWorkbook wb)
        {
           
            ISheet sheet = wb.GetSheetAt(0);
            List<Point> points = new List<Point>();
            int index = 0;
            for (int i = 2; i <= sheet.LastRowNum; i++)
            {
                Point geo = new Point();
                IRow row = sheet.GetRow(i);
                geo.GPS = new GeoCoordinate();
                if (row.Cells[9] != null && row.Cells[9].CellType != CellType.Blank)
                {
                    if (row.Cells[9].CellType == CellType.String && !string.IsNullOrEmpty(row.Cells[9].StringCellValue))
                    {
                        geo.GPS.Latitude = Convert.ToDouble(row.Cells[9].StringCellValue);
                    }
                    else if (row.Cells[9].CellType == CellType.Numeric)
                    {
                        geo.GPS.Latitude = Convert.ToDouble(row.Cells[9].NumericCellValue);
                    }
                    if (row.Cells[10].CellType == CellType.String && !string.IsNullOrEmpty(row.Cells[10].StringCellValue))
                    {
                        geo.GPS.Longitude = Convert.ToDouble(row.Cells[10].StringCellValue);
                    }
                    else if (row.Cells[10].CellType == CellType.Numeric)
                    {
                        geo.GPS.Longitude = Convert.ToDouble(row.Cells[10].NumericCellValue);
                    }
                    geo.Id = index++;
                    geo.Address = row.Cells[4].StringCellValue;
                    geo.Gaddress = row.Cells[11].StringCellValue;
                    points.Add(geo);
                }
            }
            return points;
        }

        static void StoreBusStop(HSSFWorkbook wb, HashSet<Cluster> clusters)
        {
            // Output
            if (wb == null) return;

            //Create new Excel sheet
            ISheet sheet = null;
            sheet = wb.CreateSheet("BusStop");
            ////(Optional) set the width of the columns
            sheet.SetColumnWidth(0, 10 * 256);
            sheet.SetColumnWidth(1, 20 * 256);
            sheet.SetColumnWidth(2, 50 * 256);
            sheet.SetColumnWidth(3, 15 * 256);
            sheet.SetColumnWidth(4, 15 * 256);
            sheet.SetColumnWidth(5, 15 * 256);
            sheet.SetColumnWidth(6, 15 * 256);
            sheet.SetColumnWidth(7, 15 * 256);

            //Create a header row
            var headerRow = sheet.CreateRow(0);
            //Set the column names in the header row
            headerRow.CreateCell(0).SetCellValue("StudentId");
            headerRow.CreateCell(1).SetCellValue("Addr");
            headerRow.CreateCell(2).SetCellValue("Google Addr");
            headerRow.CreateCell(3).SetCellValue("RawLat");
            headerRow.CreateCell(4).SetCellValue("RawLong");
            headerRow.CreateCell(5).SetCellValue("ClusterId");
            headerRow.CreateCell(6).SetCellValue("CentroidsLat");
            headerRow.CreateCell(7).SetCellValue("CentroidsLong");

            int j = 1;
            var indexCluster = 1;
            for (int k = 0; k < clusters.Count; ++k) // Each cluster
            {
                for (int i = 0; i < clusters.ElementAt(k).Points.Count; ++i)
                { // Each tuple                
                    var row = sheet.CreateRow(j++);
                    row.CreateCell(0).SetCellValue(clusters.ElementAt(k).Points[i].Id);
                    row.CreateCell(1).SetCellValue(clusters.ElementAt(k).Points[i].Address);
                    row.CreateCell(2).SetCellValue(clusters.ElementAt(k).Points[i].Gaddress);
                    row.CreateCell(3).SetCellValue(clusters.ElementAt(k).Points[i].GPS.Latitude);
                    row.CreateCell(4).SetCellValue(clusters.ElementAt(k).Points[i].GPS.Longitude);
                    row.CreateCell(5).SetCellValue(indexCluster.ToString());
                    row.CreateCell(6).SetCellValue(clusters.ElementAt(k).CenterGPS.Latitude);
                    row.CreateCell(7).SetCellValue(clusters.ElementAt(k).CenterGPS.Longitude);                 
                }
                indexCluster++;
            }
        }
           
        static void StoreStation(HSSFWorkbook wb, HashSet<Cluster> clusters)
        {
            // Output
            if (wb == null) return;

            //Create new Excel sheet
            ISheet sheet = null;
            sheet = wb.CreateSheet("Station");
            ////(Optional) set the width of the columns
            sheet.SetColumnWidth(0, 10 * 256);
            sheet.SetColumnWidth(1, 20 * 256);
            sheet.SetColumnWidth(2, 20 * 256);
            sheet.SetColumnWidth(3, 10 * 256);

            //Create a header row
            var headerRow = sheet.CreateRow(0);
            //Set the column names in the header row
            headerRow.CreateCell(0).SetCellValue("Clusterid");
            headerRow.CreateCell(1).SetCellValue("Lat");
            headerRow.CreateCell(2).SetCellValue("Long");
            headerRow.CreateCell(3).SetCellValue("Nb");

            int j = 1;
            var indexCluster = 1;
            for (int k = 0; k < clusters.Count; ++k) // Each cluster
            {
                var row = sheet.CreateRow(j++);
                row.CreateCell(0).SetCellValue(indexCluster.ToString());
                row.CreateCell(1).SetCellValue(clusters.ElementAt(k).CenterGPS.Latitude);
                row.CreateCell(2).SetCellValue(clusters.ElementAt(k).CenterGPS.Longitude);
                row.CreateCell(3).SetCellValue(clusters.ElementAt(k).Points.Count.ToString());
                indexCluster++;
            }
        }

        static void SaveExcel(HSSFWorkbook hssfworkbook)
        {
            String filePath_output = "../../data/data_output.xls";
            FileStream fs = new FileStream(filePath_output, FileMode.OpenOrCreate);
            hssfworkbook.Write(fs);
            fs.Close();
        }

    }

    public class Cluster
    {
        public int Id { get; set; }
        public List<Point> Points { get; set; }
        public GeoCoordinate CenterGPS { get; set; }
    }
    public class Point
    {
        public int Id { get; set; }
        public GeoCoordinate GPS { get; set; }
        public string Address {get;set;}
        public string Gaddress {get;set;}
    }
}
