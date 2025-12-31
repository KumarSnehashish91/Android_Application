using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMI_Calculator.Models
{
    public class EmiInput
    {
        public decimal LoanAmount { get; init; }
        public decimal InterestRate { get; init; }
        public int PeriodInYears { get; init; }
        public decimal Emi { get; init; }
    }
}
