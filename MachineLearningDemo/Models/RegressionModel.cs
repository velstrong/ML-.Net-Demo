using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML.Data;
namespace MachineLearningDemo.Models
{
    public class RegressionModelInput
    {
        [LoadColumn(1)]
        public float Hour { get; set; }
        [LoadColumn(2)]
        public float Month { get; set; }
        [LoadColumn(3)]
        public string DayOfWeek { get; set; }
        [LoadColumn(4)]
        public string WeatherConditions { get; set; }
        [ColumnName("Label"),LoadColumn(5)]
        public float Calls { get; set; }
    }

    public class RegressionModelOutput
    {
        public float Score { get; set; }
    }
    public class CallInput
    {
        public float Hour { get; set; }
        public float Month { get; set; }
        public string DayOfWeek { get; set; }
        public DateTime Date { get; set; }
        public string WeatherConditions { get; set; }
        public float Calls { get; set; }
    }
    public class ChartOutput
    {
        public int x { get; set; }
        public double y { get; set; }
    }
}
