using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HashUtil
{
    internal static class DependencyObjectExtensions
    {
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