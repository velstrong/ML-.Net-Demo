namespace MachineLearning.Models
{
    public class PredictionResult
    {
        public string CustomerId { get; set; }
        public int Cluster { get; set; }
        public float M;

        public float F;

        public float R;
    }
}
