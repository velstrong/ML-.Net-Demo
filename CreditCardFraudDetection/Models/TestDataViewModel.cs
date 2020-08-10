using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreditCardFraudDetection.Models
{
    public class TestDataViewModel
    {
        public List<ModelInput> TransactionList { get; set; }

        public List<ModelInput> ValidList { get; set; }
    }
}
