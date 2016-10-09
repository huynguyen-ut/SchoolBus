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

namespace GetAddress
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("PROCESSING ...");
            Task.Run(async () =>
            {
                IWorkbook workbook = await ReadExcel();
                SaveExcel(workbook);
                Console.WriteLine("OK BABY !!!");
            }).Wait();
        }

        static async Task<IWorkbook> ReadExcel()
        {
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
                            String lat = row.GetCell(1).ToString().Trim();
                            String lng = row.GetCell(2).ToString().Trim();
                            String address = await GoogleAPI.ReverseGeocode(lat, lng);
                            if (address != null)
                            {
                                ICell cell = row.GetCell(3) != null ? row.GetCell(3) : row.CreateCell(3);
                                cell.SetCellValue(address);
                            }
                        }
                    }
                }
            }
            file.Close();
            return workbook;
        }

        static void SaveExcel(IWorkbook workbook)
        {
            String filePath = "../../data/data_output.xls";
            FileStream file = new FileStream(filePath, FileMode.OpenOrCreate);
            workbook.Write(file);
            file.Close();
        }
    }
}
