using System;
using System.IO;
using MachineLearningDemo.Helpers;
using MachineLearningDemo.ModelBuilders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using SentimentAnalysisML.Model.DataModels;

namespace MachineLearningDemo.Controllers
{
    public class SemanticController : Controller
    {
        IHostingEnvironment _hostingEnvironment;
        public SemanticController(IHostingEnvironment hostingEnvironment)
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
            var resultData = SentimentModelBuilder.CreateModel(fullPath);
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
            System.IO.File.Delete(_hostingEnvironment.ContentRootPath + FileHelper.SentimentModelPath);

            return Json(new
            {
                Status = !System.IO.File.Exists(_hostingEnvironment.ContentRootPath + FileHelper.SentimentModelPath)
            });
        }

        [HttpPost]
        public IActionResult Analysis(ModelInput input)
        {
            // Load the model
            MLContext mlContext = new MLContext();

            ITransformer mlModel = mlContext.Model.Load(_hostingEnvironment.ContentRootPath + FileHelper.SentimentModelPath, out var modelInputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
            //Input
            input.Year = DateTime.Now.Year;
            // Try model on sample data
            ModelOutput resultData = predEngine.Predict(input);
            return Json(new
            {
                Status = 1,
                result = resultData
            });
        }
    }
}
