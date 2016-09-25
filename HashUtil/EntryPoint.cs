using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HashUtil
{
    public class EntryPoint
    {
        [STAThread]
        public static void Main(string[] args)
        {
            bool enabled = RuntimeUtils.EnableConsole() || RuntimeUtils.IsCommandLine;
            Runtime.Builder.Interface = enabled ? Interface.Console : Interface.GUI;

            App.Main();
        }
    }
}
