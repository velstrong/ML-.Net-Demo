using System.Collections.Generic;

namespace MachineLearning.Models
{
    public class Customer : CustomerBusinessValue
    {
        public Customer()
        {
            Transactions = new List<Transaction>();
        }
        public int CustomerId { get; set; }
        public IList<Transaction> Transactions { get; set; }
    }
}
