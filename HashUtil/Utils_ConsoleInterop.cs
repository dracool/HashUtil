using System;
using System.Runtime.InteropServices;

namespace HashUtil
{
    /// <summary>
    /// Utilities used to distinguish and use console mode
    /// </summary>
    internal static class ConsoleUtils
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        // ReSharper disable once InconsistentNaming (backing field)
        private static readonly Lazy<bool> isCommandLine = new Lazy<bool>(() => GetConsoleWindow() != IntPtr.Zero, true);
        private static bool _isConsoleEnabled;
        public static bool IsCommandLine => isCommandLine.Value || _isConsoleEnabled;
        
        [DllImport("Kernel32.dll")]
        private static extern bool AttachConsole(int processId);

        /// <summary>
        /// Attaches the application to the console if it was started from command line
        /// </summary>
        /// <returns><value>true</value> if the console was attached, <value>false</value> otherwise</returns>
        public static bool EnableConsole()
        {
            var enabled = AttachConsole(-1);
            _isConsoleEnabled = enabled;
            return enabled;
        }

        [DllImport("kernel32.dll", EntryPoint = "FreeConsole", SetLastError = true)]
        private static extern bool freeConsole();

        /// <summary>
        /// Detaches a previously attached console
        /// </summary>
        /// <returns><c>true</c> if detaching was sucessful, <value>false</value> otherwise</returns>
        public static bool FreeConsole()
        {
            var enabled = freeConsole();
            _isConsoleEnabled = !enabled;
            return enabled;
        }
    }
}
