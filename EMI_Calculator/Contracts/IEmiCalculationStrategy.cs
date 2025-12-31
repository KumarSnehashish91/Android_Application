using EMI_Calculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMI_Calculator.Contracts
{
    public interface IEmiCalculationStrategy
    {
        decimal Calculate(EmiInput input);
    }
}
