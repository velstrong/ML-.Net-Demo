//*****************************************************************************************
//*                                                                                       *
//* This is an auto-generated file by Microsoft ML.NET CLI (Command-Line Interface) tool. *
//*                                                                                       *
//*****************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using MachineLearningDemo.Helpers;
using MachineLearningDemo.Models;

namespace MachineLearningDemo.ModelBuilders
{
    public static class RegressionModelBuilder
    {
        //private static string TRAIN_DATA_FILEPATH = @"../../../../Sentiment AnalysisML.Model/Restaurant_Reviews.tsv";
        // private static string MODEL_FILEPATH = @"../../../../Sentiment AnalysisML.Model/MLModel.zip";

        // Create MLContext to be shared across the model creation workflow objects 
        // Set a random seed for repeatable/deterministic results across multiple trainings.
        private static MLContext mlContext = new MLContext();

        public static string CreateModel(string trainingFilepath,string modelPath)
        {
            // Load Data
            IDataView trainingDataView = mlContext.Data.LoadFromTextFile<RegressionModelInput>(
                                            path: trainingFilepath,
                                            hasHeader: true,
                                            separatorChar: ',');

            // Build training pipeline
            IEstimator<ITransformer> trainingPipeline = BuildTrainingPipeline(mlContext);

            // Evaluate quality of Model
            var result = Evaluate(mlContext, trainingDataView, trainingPipeline);

            // Train Model
            ITransformer mlModel = TrainModel(mlContext, trainingDataView, trainingPipeline);

           

            // Save model
            SaveModel(mlContext, mlModel, modelPath, trainingDataView.Schema);
            return result;
        }

        public static IEstimator<ITransformer> BuildTrainingPipeline(MLContext mlContext)
        {
            var dataProcessPipeline =
                mlContext.Transforms.Categorical.OneHotEncoding(
                    new[]
                    {
            new InputOutputColumnPair("DayOfWeek", "DayOfWeek"),
            new InputOutputColumnPair("WeatherConditions", "WeatherConditions"),
            new InputOutputColumnPair("Hour", "Hour"),
            new InputOutputColumnPair("Month", "Month"),
                    })
                    .Append(
                        mlContext.Transforms.Concatenate(
                            "Features",
                            new[] { "DayOfWeek", "WeatherConditions", "Hour", "Month" }));

            // Set the training algorithm 
            var trainer = mlContext.Regression.Trainers.FastTreeTweedie(labelColumnName: "Label", featureColumnName: "Features");
            var trainingPipeline = dataProcessPipeline.Append(trainer);
            return trainingPipeline;
        }

        public static ITransformer TrainModel(MLContext mlContext, IDataView trainingDataView, IEstimator<ITransformer> trainingPipeline)
        {
            // Console.WriteLine("=============== Training  model ===============");

            ITransformer model = trainingPipeline.Fit(trainingDataView);

            // Console.WriteLine("=============== End of training process ===============");
            return model;
        }

        private static string Evaluate(MLContext mlContext, IDataView trainingDataView, IEstimator<ITransformer> trainingPipeline)
        {
            // Cross-Validate with single dataset (since we don't have two datasets, one for training and for evaluate)
            // in order to evaluate and get the model's accuracy metrics
            //  Console.WriteLine("=============== Cross-validating to get model's accuracy metrics ===============");
            var crossValidationResults = mlContext.Regression.CrossValidate(trainingDataView, trainingPipeline, numberOfFolds: 5, labelColumnName: "Label");
            return PrintBinaryClassificationFoldsAverageMetrics(crossValidationResults);
        }
        private static void SaveModel(MLContext mlContext, ITransformer mlModel, string modelRelativePath, DataViewSchema modelInputSchema)
        {
            // Save/persist the trained model to a .ZIP file
            //Console.WriteLine($"=============== Saving the model  ===============");
            mlContext.Model.Save(mlModel, modelInputSchema, modelRelativePath);
            //  Console.WriteLine("The model is saved to {0}", GetAbsolutePath(modelRelativePath));
        }

        //public static string GetAbsolutePath(string relativePath)
        //{
        //    FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
        //    string assemblyFolderPath = _dataRoot.Directory.FullName;

        //    string fullPath = Path.Combine(assemblyFolderPath, relativePath);

        //    return fullPath;
        //}

        public static void PrintBinaryClassificationMetrics(BinaryClassificationMetrics metrics)
        {
            Console.WriteLine($"************************************************************");
            Console.WriteLine($"*       Metrics for binary classification model      ");
            Console.WriteLine($"*-----------------------------------------------------------");
            Console.WriteLine($"*       Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"*       Auc:      {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"************************************************************");
        }


        public static string PrintBinaryClassificationFoldsAverageMetrics(IEnumerable<TrainCatalogBase.CrossValidationResult<RegressionMetrics>> crossValResults)
        {
            var L1 = crossValResults.Select(r => r.Metrics.MeanAbsoluteError);
            var L2 = crossValResults.Select(r => r.Metrics.MeanSquaredError);
            var RMS = crossValResults.Select(r => r.Metrics.RootMeanSquaredError);
            var lossFunction = crossValResults.Select(r => r.Metrics.LossFunction);
            var R2 = crossValResults.Select(r => r.Metrics.RSquared);

            //Console.WriteLine($"*************************************************************************************************************");
            //Console.WriteLine($"*       Metrics for Regression model      ");
            //Console.WriteLine($"*------------------------------------------------------------------------------------------------------------");
            //Console.WriteLine($"*       Average L1 Loss:       {L1.Average():0.###} ");
            //Console.WriteLine($"*       Average L2 Loss:       {L2.Average():0.###}  ");
            //Console.WriteLine($"*       Average RMS:           {RMS.Average():0.###}  ");
            //Console.WriteLine($"*       Average Loss Function: {lossFunction.Average():0.###}  ");
            //Console.WriteLine($"*       Average R-squared:     {R2.Average():0.###}  ");
            //Console.WriteLine($"*************************************************************************************************************");

            //Console.WriteLine($"*************************************************************************************************************");
            //Console.WriteLine($"*       Metrics for Binary Classification model      ");
            //Console.WriteLine($"*------------------------------------------------------------------------------------------------------------");
            return $"*       Average L1 Loss:       {L1.Average():0.###}  -Average L2 Loss:       {L2.Average():0.###}  - Average RMS:           {RMS.Average():0.###} - Average Loss Function: {lossFunction.Average():0.###} - Average R-squared:     {R2.Average():0.###} )";
            //Console.WriteLine($"*************************************************************************************************************");
        }

        public static double CalculateStandardDeviation(IEnumerable<double> values)
        {
            double average = values.Average();
            double sumOfSquaresOfDifferences = values.Select(val => (val - average) * (val - average)).Sum();
            double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / (values.Count() - 1));
            return standardDeviation;
        }

        public static double CalculateConfidenceInterval95(IEnumerable<double> values)
        {
            double confidenceInterval95 = 1.96 * CalculateStandardDeviation(values) / Math.Sqrt((values.Count() - 1));
            return confidenceInterval95;
        }
    }
}
