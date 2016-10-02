using System;
using System.Windows;

namespace HashUtil.Console.GUI
{
    public class TextBlock : CliElement
    {
        private string _text;
        private TextAlignment _alignment;
        private VerticalAlignment _verticalAlignment;
        private TextTrimming _trimming;

        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                _text = value;
                Invalidate();
            }
        }
        public TextAlignment Alignment
        {
            get
            {
                return _alignment;
            }

            set
            {
                _alignment = value;
                Invalidate();
            }
        }
        public VerticalAlignment VerticalAlignment
        {
            get
            {
                return _verticalAlignment;
            }

            set
            {
                _verticalAlignment = value;
                Invalidate();
            }
        }

        public TextTrimming Trimming
        {
            get
            {
                return _trimming;
            }

            set
            {
                _trimming = value;
                Invalidate();
            }
        }

        protected override void DoUpdate(bool recreate)
        {
            switch (VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    DrawHelper.WriteText(Left, Top, Width, Text, Alignment, Trimming);
                    break;
                case VerticalAlignment.Bottom:
                    DrawHelper.WriteText(Left, Top + Height - 1, Width, Text, Alignment, Trimming);
                    break;
                case VerticalAlignment.Center:
                case VerticalAlignment.Stretch:
                    DrawHelper.WriteText(Left, Top + Height / 2, Width, Text, Alignment, Trimming);
                    break;
                default:
                    throw new Exception("Invalid VerticalAlignment value (should never happen)");
            }
        }
    }
}