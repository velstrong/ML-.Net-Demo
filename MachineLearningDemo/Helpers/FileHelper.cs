using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MachineLearningDemo.Helpers
{
    public static class FileHelper
    {
        public static string BasePath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + "../../../";
            }
        }

        public static string TestDataPath
        {
            get
            {
                return BasePath + "assets/input/testData.csv";
            }
        }

        public static string TrainDataPath
        {
            get
            {
                return BasePath + "assets/input/trainData.csv";
            }
        }

        public static string ModelPath
        {
            get
            {
                return BasePath + "assets/output/MLModel.zip";
            }
        }
        public static string SentimentModelPath
        {
            get
            {
                return "/assets/output/SentimentMLModel.zip";
            }
        }
        public static string UploadPath
        {
            get
            {
                return  "/assets/upload/";
            }
        }

        public static bool ModelExists
        {
            get
            {
                return File.Exists(ModelPath);
            }
        }

        public static string ImageMLModellPath
        {
            get
            {
                return "/assets/output/ImageMLModel.zip";
            }
        }
    }
}
