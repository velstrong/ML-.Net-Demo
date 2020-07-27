using System;

namespace MachineLearning.Models
{
    public class Transaction
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public int TransactionID { get; set; }
       // public int CustomerId { get; set; }
        public DateTime TimeStamp { get; set; }
        //public string Country { get; set; }

        public string ExpenseType { get; set; }
    }

}