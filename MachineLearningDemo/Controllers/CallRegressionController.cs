using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using MachineLearningDemo.Helpers;
using MachineLearningDemo.ModelBuilders;
using MachineLearningDemo.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using SentimentAnalysisML.Model.DataModels;

namespace MachineLearningDemo.Controllers
{
    public class CallRegressionController : Controller
    {
        IHostingEnvironment _hostingEnvironment;
        public CallRegressionController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet]
        public IActionResult Analysis()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Train(IFormFile file)
        {
            if (file == null)
            {
                return Json(new
                {
                    Status = 0
                });
            }

            var fileName = file.FileName;
            var fullPath = _hostingEnvironment.ContentRootPath + FileHelper.UploadPath + fileName;

            //using (FileStream fs = System.IO.File.Create(fullPath))
            //{
            //    file.CopyTo(fs);
            //    fs.Flush();
            //}
            var resultData = RegressionModelBuilder.CreateModel(fullPath, _hostingEnvironment.ContentRootPath + FileHelper.RegressionModelPath);
            // TODO: train model here

            return Json(new
            {
                Status = 1,
                result = resultData
            });
        }
        [HttpGet]
        public IActionResult RemoveModel()
        {
            System.IO.File.Delete(_hostingEnvironment.ContentRootPath + FileHelper.RegressionModelPath);

            return Json(new
            {
                Status = !System.IO.File.Exists(_hostingEnvironment.ContentRootPath + FileHelper.RegressionModelPath)
            });
        }

        [HttpPost]
        public IActionResult Analysis(RegressionModelInput input)
        {
            // Load the model
            MLContext mlContext = new MLContext();

            ITransformer mlModel = mlContext.Model.Load(_hostingEnvironment.ContentRootPath + FileHelper.RegressionModelPath, out var modelInputSchema);
            var predictionEngine = mlContext.Model.CreatePredictionEngine<RegressionModelInput, RegressionModelOutput>(mlModel);
            var inputdate = DateTime.Parse(input.DayOfWeek);
            //Input
            // Try model on sample data
            // Predict values for each hour of this day
            var calls = GetCalls();
            var actual = calls.Where(x => x.Date == new DateTime(2019, inputdate.Month, inputdate.Day)).Select(x => new ChartOutput() { x = Convert.ToInt32(x.Hour), y = x.Calls });
            var predictedValues = new List<ChartOutput>();

            for (int i = 0; i < 24; i++)
            {
                var prediction = predictionEngine.Predict(new RegressionModelInput()
                {
                    Hour = (float)i,
                    Month = inputdate.Month,
                    DayOfWeek = inputdate.DayOfWeek.ToString(),
                    WeatherConditions = input.WeatherConditions
                });

                predictedValues.Add(new ChartOutput { x = i, y = Math.Round(prediction.Score) });
            }
            return Json(new
            {
                Status = 1,
                actual = actual.ToArray(),
                predicted = predictedValues.ToArray()
            });
        }

        [HttpPost]
        public IActionResult YearAnalysis(RegressionModelInput input)
        {
            // Load the model
            MLContext mlContext = new MLContext();

            ITransformer mlModel = mlContext.Model.Load(_hostingEnvironment.ContentRootPath + FileHelper.RegressionModelPath, out var modelInputSchema);
            var predictionEngine = mlContext.Model.CreatePredictionEngine<RegressionModelInput, RegressionModelOutput>(mlModel);
            //Input
            // Try model on sample data
            // Predict a whole year, starting with startDate
            DateTime startDate = new DateTime(2019, 1, 1);
            var predictedValues = new List<(string date, float calls)>();
            var calls = GetCalls();
            for (DateTime date = startDate; date < startDate.AddYears(1); date = date.AddDays(1))
            {
                var dateString = date.ToString("MM'/'dd'/'yyyy 00:00:00");

                var row = calls.Where(x => x.Date == date);

                string weatherConditions = row.FirstOrDefault().WeatherConditions;

                float callCount = 0;
                for (int i = 0; i < 24; i++)
                {
                    var prediction = predictionEngine.Predict(new RegressionModelInput()
                    {
                        Hour = (float)i,
                        Month = (float)date.Month,
                        DayOfWeek = date.DayOfWeek.ToString(),
                        WeatherConditions = weatherConditions
                    });

                    callCount += prediction.Score;
                }

                predictedValues.Add((dateString, callCount));
            }
            return Json(new
            {
                Status = 1,
                result = predictedValues
            });
        }

        public List<CallInput> GetCalls()
        {
            var callList = new List<CallInput>();
            using (var reader = new StreamReader(_hostingEnvironment.ContentRootPath + FileHelper.UploadPath + "calls.csv"))
            {
                int index = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (index > 0)
                    {
                        var values = line.Split(',');

                        callList.Add(new CallInput()
                        {
                            Date = DateTime.ParseExact(values[0], "MM/dd/yyyy hh:mm:ss", new CultureInfo("en-US")),
                            Hour = float.Parse(values[1]),
                            Month = float.Parse(values[2]),
                            DayOfWeek = values[3],
                            WeatherConditions = values[4],
                            Calls = float.Parse(values[5])
                        });
                    }
                    index++;
                }
            }
            return callList;
        }
    }
}
