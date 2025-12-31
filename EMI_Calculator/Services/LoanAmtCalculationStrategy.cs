using EMI_Calculator.Contracts;
using EMI_Calculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMI_Calculator.Services
{
    public class LoanAmtCalculationStrategy : IEmiCalculationStrategy
    {
        public decimal Calculate(EmiInput input)
        {
            if (input.InterestRate == 0)
                return input.Emi * input.PeriodInYears * 12;

            decimal monthlyRate = input.InterestRate / (12m * 100m);
            int months = input.PeriodInYears * 12;

            decimal factor = (decimal)Math.Pow(
                (double)(1 + monthlyRate), months);

            decimal loanAmount =
                input.Emi * (factor - 1) / (monthlyRate * factor);

            return Math.Round(loanAmount, 0);
        }
    }
}
