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

        public void OnPost(IFormFile file, string train, string predict)
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
                    { CustomerId = t.CustomerId.ToString(), R=t.R,F=t.F,M=t.M, Cluster = predictions[i] }).ToList();
                    ViewData["PredictedCustomers"] = predictedCustomers;

                }
            }
        }
    }
}
