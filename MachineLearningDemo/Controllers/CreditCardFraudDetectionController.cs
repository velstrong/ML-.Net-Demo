using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using System.Collections.Generic;
using System;
using CreditCardFraudDetection.Models;
using CreditCardFraudDetection.Helpers;
using CreditCardFraudDetection.Trainer;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace MachineLearningDemo.Controllers
{
    public class CreditCardFraudDetectionController : Controller
    {
        [HttpGet]
        public IActionResult Default()
        {
            //Dataset to use for predictions 
            var dataFilePath = FileHelper.TestDataPath;

            // Create MLContext
            MLContext mlContext = new MLContext();

            // Load dataset
            IDataView dataView = mlContext.Data.LoadFromTextFile<ModelInput>(
                path: dataFilePath,
                hasHeader: true,
                separatorChar: ',',
                allowQuoting: true,
                allowSparse: false
            );

            // Use first line of dataset as model input
            // You can replace this with new test data (hardcoded or from end-user application)
            /*List<ModelInput> sampleForPredictions = mlContext.Data.CreateEnumerable<ModelInput>(dataView, false).Where(x => x.Class == true).Take(5).ToList();

            var random = new Random();
            var index = random.Next(sampleForPredictions.Count - 1);

            ModelInput sampleData = sampleForPredictions[index];*/

            IEnumerable<ModelInput> input = mlContext.Data.CreateEnumerable<ModelInput>(dataView, false);

            var viewModel = new TestDataViewModel()
            {
                FraudList = input.Where(x => x.Class == true && x.Amount > 1).Take(5).ToList(),
                ValidList = input.Where(x => x.Class == false && x.Amount > 1).Take(5).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Predict(ModelInput input)
        {
            // Load model & create prediction engine
            string modelPath = FileHelper.ModelPath;

            // Create new MLContext
            MLContext mlContext = new MLContext();

            ITransformer mlModel = mlContext.Model.Load(modelPath, out var modelInputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

            // Use model to make prediction on input data
            var prediction = predEngine.Predict(input);

            return Json(prediction);
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
            var fullPath = FileHelper.UploadPath + fileName;

            using (FileStream fs = System.IO.File.Create(fullPath))
            {
                file.CopyTo(fs);
                fs.Flush();
            }

            ModelBuilder.TrainModel(fullPath);

            return Json(new
            {
                Status = 1
            });
        }

        [HttpGet]
        public IActionResult RemoveModel()
        {
            System.IO.File.Delete(FileHelper.ModelPath);

            return Json(new
            {
                Status = !System.IO.File.Exists(FileHelper.ModelPath)
            });
        }
    }
}
