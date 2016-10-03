using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace HashUtil
{
    internal static class DispatcherObjectExtensions
    {
        public static void SafeUpdate<T>(this T obj, Action<T> updater) where T : DispatcherObject
        {
            obj.Dispatcher.Invoke(() => updater(obj));
        }

        public static void SafeUpdate<T>(this T obj, Action updater) where T : DispatcherObject
        {
            obj.Dispatcher.Invoke(updater);
        }
    }
}
