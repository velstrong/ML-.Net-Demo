using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MachineLearning.Import;
using MachineLearning.Models;
using MachineLearning.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.ML.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace MachineLearning.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        public ClusteringMetrics _clusteringMetrics { get; set; }
        public IndexModel(ILogger<IndexModel> logger, IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost(IFormFile file, string train, string predict, string download)
        {
            // var fileName = file.FileName;
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                var customers = new ExcelImportProcessor().GetImportData(stream);
                var rfmCalculateService = new RfmCalculateService();
                var calculatedScores = rfmCalculateService.CalculateRfmScores(customers);

                var segmentator = new CustomersSegmentator(_hostingEnvironment.ContentRootPath);
                if (!string.IsNullOrEmpty(train))
                {
                    var businessData = calculatedScores.Select(x => new Rfm
                    {
                        R = x.R,
                        F = x.F,
                        M = x.M
                    }).ToList();
                    _clusteringMetrics = segmentator.Train(businessData);
                    ViewData["ClusteringMetrics"] = _clusteringMetrics.AvgMinScore.ToString();
                }
                else if (!string.IsNullOrEmpty(predict))
                {
                    var businessData = calculatedScores.Select(x => new ClusteringData
                    {
                        R = x.R,
                        F = x.F,
                        M = x.M
                    }).ToList();
                    var predictions = segmentator.Predict(businessData);
                    var predictedCustomers = calculatedScores.Select((t, i) => new PredictionResult
                    { CustomerId = t.CustomerId.ToString(), R = t.R, F = t.F, M = t.M, Cluster = predictions[i] }).ToList();
                    ViewData["PredictedCustomers"] = predictedCustomers;

                }
                else if (!string.IsNullOrEmpty(download))
                {
                    var businessData = calculatedScores.Select(x => new ClusteringData
                    {
                        R = x.R,
                        F = x.F,
                        M = x.M
                    }).ToList();
                    var predictions = segmentator.Predict(businessData);
                    var predictedCustomers = calculatedScores.Select((t, i) => new PredictionResult
                    { CustomerId = t.CustomerId.ToString(), R = t.R, F = t.F, M = t.M, Cluster = predictions[i] }).ToList();
                    ViewData["PredictedCustomers"] = predictedCustomers;
                    var groupedCustomers = predictedCustomers.GroupBy(x => x.Cluster);
                    ExcelPackage excel = new ExcelPackage();
                    foreach (var group in groupedCustomers)
                    {
                        var workSheet = excel.Workbook.Worksheets.Add("Cluster" + group.Key.ToString());
                        workSheet.TabColor = System.Drawing.Color.Black;
                        workSheet.DefaultRowHeight = 12;
                        //Header of table  
                        //  
                        workSheet.Row(1).Height = 20;
                        workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Row(1).Style.Font.Bold = true;
                        workSheet.Cells[1, 1].Value = "CustomerID";
                        workSheet.Cells[1, 2].Value = "R";
                        workSheet.Cells[1, 3].Value = "F";
                        workSheet.Cells[1, 4].Value = "M";
                        //Body of table  
                        //  
                        int recordIndex = 2;
                        foreach (var customer in group)
                        {
                            workSheet.Cells[recordIndex, 1].Value = customer.CustomerId;
                            workSheet.Cells[recordIndex, 2].Value = customer.R;
                            workSheet.Cells[recordIndex, 3].Value = customer.F;
                            workSheet.Cells[recordIndex, 4].Value = customer.M;
                            recordIndex++;
                        }
                        workSheet.Column(1).AutoFit();
                        workSheet.Column(2).AutoFit();
                        workSheet.Column(3).AutoFit();
                        workSheet.Column(4).AutoFit();
                    }
                    string excelName = "ClusteredData.xlsx";
                    using (var memoryStream = new MemoryStream())
                    {
                        excel.SaveAs(memoryStream);
                        var content = memoryStream.ToArray();

                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                           excelName);
                    }
                    //using (var memoryStream = new MemoryStream())
                    //{
                    //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //    Response.Headers.Add("content-disposition", "attachment; filename=" + excelName + ".xlsx");
                    //    excel.SaveAs(memoryStream);
                    //    Response.OutputStream. memoryStream.WriteTo();
                    //    Response.Flush();
                    //    Response.End();
                    //}
                }
            }
            return null;
        }
    }
}
