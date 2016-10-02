using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace HashUtil
{
    static class ConsoleUtils
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        private static Lazy<bool> _isCommandLine = new Lazy<bool>(() => GetConsoleWindow() != IntPtr.Zero, true);
        private static bool _isConsoleEnabled = false;
        public static bool IsCommandLine => _isCommandLine.Value || _isConsoleEnabled;
        
        [DllImport("Kernel32.dll")]
        private static extern bool AttachConsole(int processId);
        public static bool EnableConsole()
        {
            var enabled = AttachConsole(-1);
            _isConsoleEnabled = enabled;
            return enabled;
        }

        [DllImport("kernel32.dll", EntryPoint = "FreeConsole", SetLastError = true)]
        private static extern bool freeConsole();
        public static bool FreeConsole()
        {
            var enabled = !freeConsole();
            _isConsoleEnabled = enabled;
            return enabled;
        }
    }
}
