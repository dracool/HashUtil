using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HashUtil.Graphical
{
    class GuiRunner
    {
        public void Run(ExecutionInfo info)
        {
            switch(info.Mode)
            {
                case HashingMode.Match:
                    new MatchHashDialog().Match(info);
                    break;
                case HashingMode.Calculate:
                    new CalculateHashesDialog().Calculate(info.FilePath);
                    break;
                case HashingMode.Select:
                    new SelectDataDialog().Initialize();
                    break;
                default:
                    throw new Exception("Invalid value for Mode Parameter (should never happen)");
            }
        }
    }
}
