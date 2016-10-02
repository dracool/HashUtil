using System.Windows;

namespace HashUtil.Console.GUI
{
    public class TextBox : TextBlock
    {
        protected override void DoUpdate(bool recreate)
        {
            DrawHelper.Box(Left, Top, Width, Height);

            switch (VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    DrawHelper.WriteText(Left + 1, Top + 1, Width - 2, Text, Alignment, Trimming);
                    break;
                case VerticalAlignment.Bottom:
                    DrawHelper.WriteText(Left + 1, (Top + 1) + Height - 3, Width - 2, Text, Alignment, Trimming);
                    break;
                case VerticalAlignment.Center:
                case VerticalAlignment.Stretch:
                    DrawHelper.WriteText(Left + 1, (Top + 1) + (Height - 2) / 2, Width - 2, Text, Alignment, Trimming);
                    break;
                default:
                    break;
            }
        }
    }
}