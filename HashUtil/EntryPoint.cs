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
            ConsoleUtils.EnableConsole();
            App.Main();
        }
    }
}
