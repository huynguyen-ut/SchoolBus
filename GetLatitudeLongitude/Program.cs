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

namespace GetLatitudeLongitude
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("PROCESSING ...");
            Task.Run(async () =>
            {
                List<Student> students = await ReadExcel();
                SaveExcel(students);
                Console.WriteLine("OK BABY !!!");
            }).Wait();
        }

        static async Task<List<Student>> ReadExcel()
        {
            List<Student> students = new List<Student>();

            String filePath = "../../data/data.xls";
            String sheetName = "TH KIM DONG";
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
                    for (int i = 2; i <= sheet.LastRowNum; i++) // skip header
                    {
                        IRow row = sheet.GetRow(i);
                        if (row != null) //null is when the row only contains empty cells 
                        {
                            Student student = new Student();
                            student.ID = int.Parse(row.GetCell(0).ToString().Trim());
                            student.Address = row.GetCell(4).ToString().Trim();
                            if (student.Address != "")
                            {
                                // parse Address
                                String[] items = student.Address.Split(',');
                                foreach (String item in items)
                                {
                                    if (item.Trim().Contains("Q. ")) student.Quan = item.Trim().Substring(item.Trim().IndexOf("Q. "));
                                    if (item.Trim().Contains("P. ")) student.Phuong = item.Trim().Substring(item.Trim().IndexOf("P. "));
                                    if (item.Trim().Contains("KP. ")) student.KhuPho = item.Trim().Substring(item.Trim().IndexOf("KP. "));
                                    if (item.Trim().Contains("Tổ ")) student.To = item.Trim().Substring(item.Trim().IndexOf("Tổ "));
                                }

                                // geocode Address
                                String geocode = await GoogleAPI.Geocode(student.Address);
                                if (geocode != null)
                                {
                                    items = geocode.Split('\n');
                                    student.Latitude = double.Parse(items[0]);
                                    student.Longitude = double.Parse(items[1]);
                                    student.GoogleAddress = items[2];
                                }
                            }
                            students.Add(student);
                        }
                    }
                }
            }
            file.Close();
            return students;
        }

        static void SaveExcel(List<Student> students)
        {
            String filePath = "../../data/data_output.xls";
            String sheetName = "Student";
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
                    header.CreateCell(0).SetCellValue("ID");
                    header.CreateCell(1).SetCellValue("Address");
                    header.CreateCell(2).SetCellValue("Quan");
                    header.CreateCell(3).SetCellValue("Phuong");
                    header.CreateCell(4).SetCellValue("KhuPho");
                    header.CreateCell(5).SetCellValue("To");
                    header.CreateCell(6).SetCellValue("Latitude");
                    header.CreateCell(7).SetCellValue("Longitude");
                    header.CreateCell(8).SetCellValue("GoogleAddress");

                    int index = 1;
                    foreach (Student student in students)
                    {
                        IRow row = sheet.CreateRow(index++);
                        row.CreateCell(0).SetCellValue(student.ID);
                        row.CreateCell(1).SetCellValue(student.Address);
                        row.CreateCell(2).SetCellValue(student.Quan);
                        row.CreateCell(3).SetCellValue(student.Phuong);
                        row.CreateCell(4).SetCellValue(student.KhuPho);
                        row.CreateCell(5).SetCellValue(student.To);
                        row.CreateCell(6).SetCellValue(student.Latitude);
                        row.CreateCell(7).SetCellValue(student.Longitude);
                        row.CreateCell(8).SetCellValue(student.GoogleAddress);
                    }
                }
                workbook.Write(file);
            }
            file.Close();
        }
    }
}