using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CreditCardFraudDetection.Helpers
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
                return BasePath + "wwwroot/training-files/testData.csv";
            }
        }

        public static string TrainDataPath
        {
            get
            {
                return BasePath + "wwwroot/training-files/trainData.csv";
            }
        }

        public static string ModelPath
        {
            get
            {
                return BasePath + "assets/output/MLModel.zip";
            }
        }

        public static string UploadPath
        {
            get
            {
                return BasePath + "assets/upload/";
            }
        }

        public static bool ModelExists
        {
            get
            {
                return File.Exists(ModelPath);
            }
        }
    }
}
