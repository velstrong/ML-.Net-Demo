
using MachineLearning.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace MachineLearning.Import
{
    public class ExcelImportProcessor
    {
        public List<Customer> GetImportData(Stream fileStream)
        {
            return BuildObjectModel(ReadExcel(fileStream));
        }

        //public List<Transaction> GetImportProducts(Stream fileStream)
        //{
        //    return BuildProductModel(ReadExcel(fileStream));
        //}

        private List<Customer> BuildObjectModel(DataTable dataTable)
        {
            if (dataTable == null)
                throw new ArgumentNullException(nameof(dataTable));

            List<Customer> customers = new List<Customer>();

            var groupedData = dataTable.AsEnumerable().GroupBy(x => x.Field<string>("CustomerID"));
            foreach (IGrouping<string, DataRow> data in groupedData)
            {
                if (!string.IsNullOrEmpty(data.Key))
                {
                    int customerId = int.Parse(data.Key);
                    if (customerId <= 0)
                        continue;

                    Customer customer = new Customer
                    {
                        CustomerId = customerId,
                        Transactions = new List<Transaction>(),
                    };

                    foreach (DataRow record in data.ToList())
                    {
                        decimal.TryParse(record["Amount"].ToString(), out var amount);
                        DateTime.TryParse(record["TransactionDate"].ToString(), out var dt);
                        int.TryParse(record["TransactionID"].ToString(), out var transactionId);
                       // int.TryParse(record["CustomerID"].ToString(), out var CustomerID);
                        if (transactionId > 0)
                        {
                            customer.Transactions.Add(new Transaction()
                            {
                                Amount = amount,
                               // CustomerId = CustomerID,
                                ExpenseType = record["ExpenseType"].ToString(),
                                TransactionID = transactionId,
                                TimeStamp = dt,
                                Currency = "AED"
                            });
                        }


                    }
                    if (customer.Transactions.Any())
                        customers.Add(customer);
                }
            }

            return customers;
        }

        //private List<Transaction> BuildProductModel(DataTable dataTable)
        //{
        //    if (dataTable == null)
        //        throw new ArgumentNullException(nameof(dataTable));

        //    List<Transaction> products = new List<Transaction>();

        //    var groupedData = dataTable.AsEnumerable().GroupBy(x => x.Field<string>("StockCode"));
        //    foreach (IGrouping<string, DataRow> data in groupedData)
        //    {
        //        if (!string.IsNullOrEmpty(data.Key))
        //        {
        //            var found = false;
        //            foreach (var record in data)
        //            {
        //                var description = record["Description"].ToString();
        //                int.TryParse(record["Quantity"].ToString(), out var quantity);
        //                decimal.TryParse(record["UnitPrice"].ToString(), out var unitPrice);
        //                int.TryParse(record["InvoiceNo"].ToString(), out var number);



        //                if (number > 0 && quantity > 0 && !string.IsNullOrEmpty(description))
        //                {
        //                    found = true;
        //                    products.Add(new Transaction
        //                    {
        //                        Quantity = quantity,
        //                        Number = number,
        //                        StockCode = data.Key,
        //                        Price = unitPrice,
        //                        Description = description
        //                    });
        //                }

        //                if (found) break;
        //            }
        //        }
        //    }

        //    return products;
        //}

        private DataTable ReadExcel(Stream stream)
        {
            DataTable dt = new DataTable();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(stream))
            {
                var ws = excelPackage.Workbook.Worksheets[0];

                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    dt.Columns.Add(firstRowCell.Text);
                }

                for (int rowNum = 2; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = dt.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }
            }

            return dt;
        }
    }
}
