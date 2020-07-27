//*****************************************************************************************
//*                                                                                       *
//* This is an auto-generated file by Microsoft ML.NET CLI (Command-Line Interface) tool. *
//*                                                                                       *
//*****************************************************************************************

using Microsoft.ML.Data;

namespace SentimentAnalysisML.Model.DataModels
{
    public class ModelInput
{
    [ColumnName("Label"), LoadColumn(1)]
    public bool Label { get; set; }


        [ColumnName("rev_id"), LoadColumn(2)]
        public float Rev_id { get; set; }


    [ColumnName("Review"), LoadColumn(0)]
    public string Comment { get; set; }


        [ColumnName("year"), LoadColumn(3)]
        public float Year { get; set; }


        [ColumnName("logged_in"), LoadColumn(4)]
        public string Logged_in { get; set; }


        [ColumnName("ns"), LoadColumn(5)]
        public string Ns { get; set; }


        [ColumnName("sample"), LoadColumn(6)]
        public string Sample { get; set; }


        [ColumnName("split"), LoadColumn(7)]
        public string Split { get; set; }


    }
}
