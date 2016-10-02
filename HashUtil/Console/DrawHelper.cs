using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Windows;

namespace HashUtil.Console
{
    using static System.Console;
    internal static class DrawHelper
    {
        public static void Safe(Action a)
        {
            var fg = ForegroundColor;
            var bg = BackgroundColor;
            var x = CursorLeft;
            var y = CursorTop;
            a();
            ForegroundColor = fg;
            BackgroundColor = bg;
            SetCursorPosition(x, y);
        }

        public static void InvertColors()
        {
            var temp = BackgroundColor;
            BackgroundColor = ForegroundColor;
            ForegroundColor = temp;
        }

        public static void Inverted(Action a)
        {
            InvertColors();
            a();
            InvertColors();
        }

        public static void Color(ConsoleColor foreground, Action a)
        {
            Color(foreground, BackgroundColor, a);
        }

        public static void Color(ConsoleColor foreground, ConsoleColor background, Action a)
        {
            var fg = ForegroundColor;
            var bg = BackgroundColor;
            ForegroundColor = foreground;
            BackgroundColor = background;
            a();
            ForegroundColor = fg;
            BackgroundColor = bg;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MoveAndWrite(int x, int y, char c)
        {
            SetCursorPosition(x , y);
            Write(c);
        }

        private static void MoveAndWrite(int x, int y, string s)
        {
            SetCursorPosition(x, y);
            Write(s);
        }

        public static void HorzLine(int x, int y, int w, char c = '=')
        {
            for (var i = 0; i < w; i++)
            {
                MoveAndWrite(x + i, y, c);
            }
        }

        public static void VertLine(int x, int y, int h, char c = '|')
        {
            for (var i = 0; i < h; i++)
            {
                MoveAndWrite(x, y + i, c);
            }
        }

        public static void Dot(int x, int y, char c = '+')
        {
            MoveAndWrite(x, y, c);
        }

        public static void Box(int x, int y, int w, int h)
        {
            HorzLine(x, y, w);
            VertLine(x, y, h);
            HorzLine(x, y + h - 1, w);
            VertLine(x + w - 1, y, h);
            Dot(x, y);
            Dot(x + w - 1, y);
            Dot(x, y + h - 1);
            Dot(x + w - 1, y + h - 1);
        }

        public static void WriteText(int x, int y, int w, string text, TextAlignment alignment = TextAlignment.Left, TextTrimming trimming = TextTrimming.CharacterEllipsis)
        {
            if (string.IsNullOrEmpty(text)) return;

            if (text.Length > w && trimming != TextTrimming.None)
            {
                text = text.Substring(0, w - 3) + "...";
            }
            switch (alignment)
            {
                case TextAlignment.Justify:
                case TextAlignment.Left:
                    break;
                case TextAlignment.Right:
                    x = x + (w - text.Length);
                    break;
                case TextAlignment.Center:
                    x = x + (w - text.Length) / 2;
                    break;
                default:
                    throw new Exception("Invalid TextAlignment value (should never happen)");
            }
            MoveAndWrite(x, y, text);
        }
    }
}
