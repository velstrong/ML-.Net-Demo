using System;
using Microsoft.ML.Data;

namespace CreditCardFraudDetection.Models
{
    public class ModelTrainResponse
    {
        public double Accuracy { get; set; }

        public double StandardDeviation { get; set; }

        public double ConfidenceInterval95 { get; set; }
    }
}
