using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using Microsoft.ML.Data;
using Newtonsoft.Json;

namespace MachineLearningDemo.Controllers
{
    [Route("/{Controller}/{Action}")]
    public class ImageClassificationController : Controller
    {
        // <SnippetDeclareGlobalVariables>
        static readonly string _assetsPath = Path.Combine("assets");
        static readonly string _imagesFolder = Path.Combine(_assetsPath, "images/passports");
        //static readonly string _trainTagsTsv = Path.Combine(_imagesFolder, "tags.tsv");
        static readonly string _trainTagsTsv = Path.Combine(_imagesFolder, "passport_tags.tsv");
        static readonly string _testTagsTsv = Path.Combine(_imagesFolder, "test-tags.tsv");
        //static readonly string _testTagsTsv = Path.Combine(_imagesFolder, "test-tags.tsv");
        //static readonly string _predictSingleImage = Path.Combine(_imagesFolder, "p4.PNG");
        //static readonly string _predictSingleImage = Path.Combine(_imagesFolder, "Capture12.PNG");
        //static readonly string _predictSingleImage = Path.Combine(_imagesFolder, "teddy4.jpg");
        static readonly string _inceptionTensorFlowModel = Path.Combine(_assetsPath, "inception", "tensorflow_inception_graph.pb");
        //</SnippetDeclareGlobalVariables>

        public ActionResult Index()
        {
            ImageModel imageModel = new ImageModel();

            return View(imageModel);
        }

        [HttpPost]
        public ActionResult Index(ImageModel imageModel,string trainResult)
        {
            //ImageModel imageModel = new ImageModel();
            if (imageModel != null)
            {
                string pic = System.IO.Path.GetFileName(imageModel.formFile.FileName);
                string filePath = System.IO.Path.Combine("assets/temp", pic);

                using (var stream = System.IO.File.Create(filePath))
                {
                    imageModel.formFile.CopyTo(stream);
                }

                if (System.IO.File.Exists(filePath))
                {
                    // Create MLContext to be shared across the model creation workflow objects
                    // <SnippetCreateMLContext>
                    MLContext mlContext = new MLContext();
                    // </SnippetCreateMLContext>

                    //string _assetsPath = System.IO.Path.Combine(Server.MapPath("~/assets"));
                    // <SnippetCallGenerateModel>
                    ITransformer model = GenerateModel(mlContext);
                    // </SnippetCallGenerateModel>

                    //var value = HttpContext.Session.GetString("TrainModel");

                    
                    // <SnippetCallClassifySingleImage>
                    imageModel = ClassifySingleImage(mlContext, model, filePath);
                    // </SnippetCallClassifySingleImage>
                    
                    imageModel.ImagePath = filePath;
                }
            }
            return View(imageModel);
        }

        //[HttpPost]
        //public ActionResult TrainModel(IFormFile file)
        //{
        //    if (file == null)
        //    {
        //        return Json(new
        //        {
        //            Status = 0
        //        });
        //    }
        //    string fileName = System.IO.Path.GetFileName(file.FileName);
        //    string filePath = System.IO.Path.Combine("assets/images/passports", fileName);

        //    using (var stream = System.IO.File.Create(filePath))
        //    {
        //        file.CopyTo(stream);
        //    }
        //    if (System.IO.File.Exists(filePath))
        //    {
        //        // Create MLContext to be shared across the model creation workflow objects
        //        // <SnippetCreateMLContext>
        //        MLContext mlContext = new MLContext();
        //        // </SnippetCreateMLContext>

        //        //string _assetsPath = System.IO.Path.Combine(Server.MapPath("~/assets"));
        //        // <SnippetCallGenerateModel>
        //        ITransformer model = GenerateModel(mlContext, filePath);
        //        // </SnippetCallGenerateModel>


        //        //HttpContext.Session.SetComplexData("TrainModel", model);
        //        //HttpContext.Session.SetString("TrainModel", JsonConvert.SerializeObject(model));

        //        return Json(new
        //        {
        //            Status = 1,
        //            result = JsonConvert.SerializeObject(model)
        //        });
        //    }

        //    return Json(new
        //    {
        //        Status = 0
        //    });
        //}

        // Build and train model
        public static ITransformer GenerateModel(MLContext mlContext)
        {

            // <SnippetImageTransforms>
            IEstimator<ITransformer> pipeline = mlContext.Transforms.LoadImages(outputColumnName: "input", imageFolder: _imagesFolder, inputColumnName: nameof(ImageData.ImagePath))
                            // The image transforms transform the images into the model's expected format.
                            .Append(mlContext.Transforms.ResizeImages(outputColumnName: "input", imageWidth: InceptionSettings.ImageWidth, imageHeight: InceptionSettings.ImageHeight, inputColumnName: "input"))
                            .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "input", interleavePixelColors: InceptionSettings.ChannelsLast, offsetImage: InceptionSettings.Mean))
                            // </SnippetImageTransforms>
                            // The ScoreTensorFlowModel transform scores the TensorFlow model and allows communication
                            // <SnippetScoreTensorFlowModel>
                            .Append(mlContext.Model.LoadTensorFlowModel(_inceptionTensorFlowModel).
                                ScoreTensorFlowModel(outputColumnNames: new[] { "softmax2_pre_activation" }, inputColumnNames: new[] { "input" }, addBatchDimensionInput: true))
                            // </SnippetScoreTensorFlowModel>
                            // <SnippetMapValueToKey>
                            .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "LabelKey", inputColumnName: "Label"))
                            // </SnippetMapValueToKey>
                            // <SnippetAddTrainer>
                            .Append(mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(labelColumnName: "LabelKey", featureColumnName: "softmax2_pre_activation"))
                            // </SnippetAddTrainer>
                            // <SnippetMapKeyToValue>
                            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabelValue", "PredictedLabel"))
                            .AppendCacheCheckpoint(mlContext);
            // </SnippetMapKeyToValue>

            // <SnippetLoadData>
            IDataView trainingData = mlContext.Data.LoadFromTextFile<ImageData>(path: _trainTagsTsv, hasHeader: false);
            // </SnippetLoadData>

            // Train the model
            //Console.WriteLine("=============== Training classification model ===============");
            // Create and train the model
            // <SnippetTrainModel>
            ITransformer model = pipeline.Fit(trainingData);
            // </SnippetTrainModel>

            // Generate predictions from the test data, to be evaluated
            // <SnippetLoadAndTransformTestData>
            IDataView testData = mlContext.Data.LoadFromTextFile<ImageData>(path: _testTagsTsv, hasHeader: false);
            IDataView predictions = model.Transform(testData);

            // Create an IEnumerable for the predictions for displaying results
            IEnumerable<ImagePrediction> imagePredictionData = mlContext.Data.CreateEnumerable<ImagePrediction>(predictions, true);
            DisplayResults(imagePredictionData);
            // </SnippetLoadAndTransformTestData>

            // Get performance metrics on the model using training data
            //Console.WriteLine("=============== Classification metrics ===============");

            // <SnippetEvaluate>
            MulticlassClassificationMetrics metrics =
                mlContext.MulticlassClassification.Evaluate(predictions,
                  labelColumnName: "LabelKey",
                  predictedLabelColumnName: "PredictedLabel");
            // </SnippetEvaluate>

            //<SnippetDisplayMetrics>
            //Console.WriteLine($"LogLoss is: {metrics.LogLoss}");
            //Console.WriteLine($"PerClassLogLoss is: {String.Join(" , ", metrics.PerClassLogLoss.Select(c => c.ToString()))}");
            //</SnippetDisplayMetrics>

            // <SnippetReturnModel>
            return model;
            // </SnippetReturnModel>
        }

        public static ImageModel ClassifySingleImage(MLContext mlContext, ITransformer model, string predictSingleImage)
        {
            var rootFolder = Directory.GetCurrentDirectory();
            string _predictSingleImage = System.IO.Path.Combine(rootFolder, predictSingleImage);
            // load the fully qualified image file name into ImageData
            // <SnippetLoadImageData>
            var imageData = new ImageData()
            {
                ImagePath = _predictSingleImage
            };
            // </SnippetLoadImageData>

            // <SnippetPredictSingle>
            // Make prediction function (input = ImageData, output = ImagePrediction)
            var predictor = mlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(model);
            var prediction = predictor.Predict(imageData);
            // </SnippetPredictSingle>

            ImageModel _image = new ImageModel();

            _image.Predicted = prediction.PredictedLabelValue;
            _image.Score = prediction.Score.Max();

            return _image;
            //Console.WriteLine("=============== Making single image classification ==============="); 
            // <SnippetDisplayPrediction>
            //Console.WriteLine($"Image: {Path.GetFileName(imageData.ImagePath)} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score.Max()} ");
            // </SnippetDisplayPrediction>i
        }

        private static void DisplayResults(IEnumerable<ImagePrediction> imagePredictionData)
        {
            // <SnippetDisplayPredictions>
            foreach (ImagePrediction prediction in imagePredictionData)
            {
                //Console.WriteLine($"Image: {Path.GetFileName(prediction.ImagePath)} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score.Max()} ");
            }
            // </SnippetDisplayPredictions>
        }

        //public static IEnumerable<ImageData> ReadFromTsv(string file, string folder)
        //{
        //    //Need to parse through the tags.tsv file to combine the file path to the
        //    // image name for the ImagePath property so that the image file can be found.

        //    // <SnippetReadFromTsv>
        //    return File.ReadAllLines(file)
        //     .Select(line => line.Split('\t'))
        //     .Select(line => new ImageData()
        //     {
        //         ImagePath = Path.Combine(folder, line[0])
        //     });
        //    // </SnippetReadFromTsv>
        //}

        // <SnippetInceptionSettings>
        private struct InceptionSettings
        {
            public const int ImageHeight = 224;
            public const int ImageWidth = 224;
            public const float Mean = 117;
            public const float Scale = 1;
            public const bool ChannelsLast = true;
        }
        // </SnippetInceptionSettings>

        // <SnippetDeclareImageData>
        public class ImageData
        {
            [LoadColumn(0)]
            public string ImagePath;

            [LoadColumn(1)]
            public string Label;
        }
        // </SnippetDeclareImageData>

        // <SnippetDeclareImagePrediction>
        public class ImagePrediction : ImageData
        {
            public float[] Score;

            public string PredictedLabelValue;
        }
        // </SnippetDeclareImagePrediction>

    }
    public class ImageModel
    {
        public string Predicted { get; set; }

        public float Score { get; set; }

        public string ImagePath { get; set; }

        public IFormFile formFile { get; set; }
    }
}