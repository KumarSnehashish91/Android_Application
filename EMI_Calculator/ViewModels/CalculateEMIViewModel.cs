using EMI_Calculator.Contracts;
using EMI_Calculator.Models;
using EMI_Calculator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMI_Calculator.ViewModels
{
    public class CalculateEMIViewModel
    {
        private readonly Dictionary<CalculationType, IEmiCalculationStrategy> _strategies;

        public CalculationType SelectedCalculation { get; set; } = CalculationType.None;

        public CalculateEMIViewModel()
        {
            _strategies = new Dictionary<CalculationType, IEmiCalculationStrategy>
            {
                { CalculationType.Emi, new EmiCalculationStrategy() },
                { CalculationType.LoanAmount, new LoanAmtCalculationStrategy() }
                // later add others
            };
        }

        public decimal Calculate(EmiInput input)
        {
            if (SelectedCalculation == CalculationType.None)
                throw new InvalidOperationException("No calculation type selected");

            if (!_strategies.ContainsKey(SelectedCalculation))
                throw new InvalidOperationException("Calculation not supported");

            return _strategies[SelectedCalculation].Calculate(input);
        }
    }
}
