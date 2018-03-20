using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using System.IO;

namespace WarehouseManagementSystem.Models
{
    public class Excel
    {

        internal static ExcelPackage CreateExcelPackage()
        {
            ExcelPackage package = new ExcelPackage();
            ExcelWorkbook workbook = package.Workbook;
            workbook.Worksheets.Add("Sheet1");

            return package;
        }
        internal static string ConvertPackageToBase64(ExcelPackage package)
        {
            MemoryStream stream = new System.IO.MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;
            byte[] excelArray = stream.ToArray();
            string base64Excel = Convert.ToBase64String(excelArray);

            return base64Excel;
        }

        public class Workbook
        {
            public class Worksheet
            {
                internal static void WriteRowsToWorksheet(ExcelWorksheet worksheet, List<Excel.Workbook.Worksheet.Row> rows)
                {
                    foreach(Excel.Workbook.Worksheet.Row row in rows)
                    {
                        foreach(Excel.Workbook.Worksheet.Cell cell in row.Cells)
                        {
                            worksheet.Cells[row.RowNumber, cell.ColumnNumber].Value = cell.Value;
                        }
                    }
                }
                public class Row
                {
                    public int RowNumber { get; set; }
                    public List<Cell> Cells { get; set; }

                    public Row()
                    {
                        this.Cells = new List<Cell>();
                    }
                }
                public class Cell
                {
                    public int RowNumber { get; set; }
                    public int ColumnNumber { get; set; }
                    public string Value { get; set; }
                }
            }
        }
    }
}