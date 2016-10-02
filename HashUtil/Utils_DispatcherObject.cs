using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace HashUtil
{
    static class DispatcherUtils
    {
        public static void SafeUpdate<T>(this T obj, Action<T> updater) where T : DispatcherObject
        {
            obj.Dispatcher.Invoke(() => updater(obj));
        }

        public static void SafeUpdate<T>(this T obj, Action updater) where T : DispatcherObject
        {
            obj.Dispatcher.Invoke(updater);
        }


        public static TItemContainer GetContainerAtPoint<TItemContainer>(this ItemsControl control, Point p)
                            where TItemContainer : DependencyObject
        {
            var result = VisualTreeHelper.HitTest(control, p);
            var obj = result?.VisualHit;

            while (obj != null && (VisualTreeHelper.GetParent(obj) != null && !(obj is TItemContainer)))
            {
                obj = VisualTreeHelper.GetParent(obj);
            }

            // Will return null if not found
            return obj as TItemContainer;
        }
    }
}
