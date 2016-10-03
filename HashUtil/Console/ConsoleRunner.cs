using System;
using System.Text;
using System.IO;
using System.Threading;
using System.Collections.ObjectModel;
using System.Windows;
using static System.Console;

namespace HashUtil.Console
{    
    internal class ConsoleRunner
    {
        public void Run(ExecutionInfo info)
        {
            Clear();
            CursorVisible = false;
            switch(info.Mode)
            {
                case HashingMode.Match:
                    new MatchHashDialog { Title = "HashUtil" }.Match(info);
                    break;
                case HashingMode.Calculate:
                    new CalculateHashDialog {Title = "HashUtil"}.Calculate(info.FilePath);
                    break;
                case HashingMode.Select:
                    WriteLine("Interactive mode is not supported on command line");
                    Application.Current.Shutdown();
                    break;
                default:
                    throw new Exception("Invalid Value for Mode Parameter (should never happen)");
            }
        }
    }
}
