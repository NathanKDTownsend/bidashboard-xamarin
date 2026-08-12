using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;

namespace BIDashboard
{
    public static class ExcelReader
    {
        public static List<Sale> ReadSales(string filePath)
        {
            var sales = new List<Sale>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed();

                bool firstRow = true;
                foreach (var row in rows)
                {
                    if (firstRow) { firstRow = false; continue; }

                    sales.Add(new Sale
                    {
                        Year = row.Cell(1).GetValue<int>(),
                        QTR = row.Cell(2).GetValue<int>(),
                        Region = row.Cell(3).GetValue<string>(),
                        Vehicle = row.Cell(4).GetValue<string>(),
                        Quantity = row.Cell(5).GetValue<int>()
                    });
                }
            }

            return sales;
        }

        public static List<Sale> ReadSalesStream(Stream excelStream)
        {
            var sales = new List<Sale>();

            using (var workbook = new XLWorkbook(excelStream))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed();

                bool firstRow = true;
                foreach (var row in rows)
                {
                    if (firstRow) { firstRow = false; continue; }

                    sales.Add(new Sale
                    {
                        Year = row.Cell(1).GetValue<int>(),
                        QTR = row.Cell(2).GetValue<int>(),
                        Region = row.Cell(3).GetValue<string>(),
                        Vehicle = row.Cell(4).GetValue<string>(),
                        Quantity = row.Cell(5).GetValue<int>()
                    });
                }
            }

            return sales;
        }
    }
}
