using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HashUtil
{
    class GuiRunner
    {
        public void Run()
        {
            switch(Runtime.Parameters.Mode)
            {
                case HashingMode.Match:
                    new MatchHashDialog().Match();
                    break;
                case HashingMode.Calculate:
                    new CalculateHashesDialog().Calculate();
                    break;
                case HashingMode.Select:
                    new SelectDataDialog().Show();
                    break;
                default:
                    throw new Exception("Invalid value for Mode Parameter (should never happen)");
            }
        }
    }
}
