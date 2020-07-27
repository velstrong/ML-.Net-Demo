using MachineLearning.Extensions;
using MachineLearning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
namespace MachineLearning.Services
{
    public class RfmCalculateService
    {
        // RFM from 111 to 333
        private int MaxValue = 3;
      
        public List<Customer> CalculateRfmScores(List<Customer> list)
        {
            // Calculate pre-values

            foreach (Customer customer in list)
            {
                // Monetary

                decimal m = 0;
                foreach (Transaction invoice in customer.Transactions)
                {
                    m = m + Math.Abs(invoice.Amount);
                }

                if (m <= 0) // broken data
                    m = 1;

                customer.Monetary = m;

                // Recency

                DateTime? minDate = customer.Transactions.Where(x => x.TimeStamp.Year != 1).DefaultIfEmpty().Min(x => x?.TimeStamp);
                DateTime? maxDate = customer.Transactions.Where(x => x.TimeStamp.Year != 1).DefaultIfEmpty().Max(x => x?.TimeStamp);

                customer.Recency = minDate.HasValue && maxDate.HasValue ? (maxDate - minDate).Value.TotalDays + 1 : 1;

                // Frequency

                customer.Frequency = customer.Transactions.Count;

                if (customer.Frequency == 0) // broken data
                    customer.Frequency = 1;
            }

            // Calculate 

            int maxScore = MaxValue;

            var rList = list.OrderByDescending(x => x.Recency).ToList().Partition(maxScore);
            int rValue = maxScore;
            foreach (var rPart in rList)
            {
                foreach (Customer customer in rPart)
                {
                    customer.R = rValue;
                }

                rValue = rValue - 1;
            }

            var mList = list.OrderByDescending(x => x.Monetary).ToList().Partition(maxScore);

            int mValue = maxScore;
            foreach (var mPart in mList)
            {
                foreach (Customer customer in mPart)
                {
                    customer.M = mValue;
                }

                mValue = mValue - 1;
            }

            var fList = list.OrderByDescending(x => x.Frequency).ToList().Partition(maxScore);
            int fValue = maxScore;
            foreach (var fPart in fList)
            {
                foreach (Customer customer in fPart)
                {
                    customer.F = fValue;
                }

                fValue = fValue - 1;
            }

            return list;

        }
    }
}
