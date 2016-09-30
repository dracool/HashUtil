using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace HashUtil
{
    class HashUtils
    {
        public static string BytesToHex(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static byte[] HexToBytes(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }

    static class RuntimeUtils
    {

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        private static Lazy<bool> _isCommandLine = new Lazy<bool>(() => GetConsoleWindow() != IntPtr.Zero, true);
        public static bool IsCommandLine => _isCommandLine.Value;


        [DllImport("Kernel32.dll")]
        private static extern bool AttachConsole(int processId);
        public static bool EnableConsole()
        {
            return AttachConsole(-1);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeConsole();
    }
    
    static class DispatcherUtils
    {
        public static void Synchronous<T>(this T obj, Action<T> updater)
            where T: DispatcherObject
        {
            obj.Dispatcher.Invoke(() => updater(obj));
        }

        public static void Asynchronous<T>(this T obj, Action<T> updater)
            where T : DispatcherObject
        {
            obj.Dispatcher.BeginInvoke((Action)(() => updater(obj)));
        }

        public static ItemContainer GetContainerAtPoint<ItemContainer>(this ItemsControl control, Point p)
                            where ItemContainer : DependencyObject
        {
            HitTestResult result = VisualTreeHelper.HitTest(control, p);
            DependencyObject obj = result?.VisualHit;

            while (VisualTreeHelper.GetParent(obj) != null && !(obj is ItemContainer))
            {
                obj = VisualTreeHelper.GetParent(obj);
            }

            // Will return null if not found
            return obj as ItemContainer;
        }
    }
}
