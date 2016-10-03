using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HashUtil
{
    /// <summary>
    /// Overwritten entry point of the application to enable console before WPF makes it impossible
    /// </summary>
    public class EntryPoint
    {
        /// <summary>
        /// Overwritten entry point of the application to enable console before WPF makes it impossible
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            ConsoleUtils.EnableConsole();
            App.Main();
        }
    }
}
