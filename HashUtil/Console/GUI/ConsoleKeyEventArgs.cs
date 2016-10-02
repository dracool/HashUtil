using System;

namespace HashUtil.Console.GUI
{
    public class ConsoleKeyEventArgs : EventArgs
    {
        public ConsoleKeyEventArgs(ConsoleKeyInfo info)
        {
            Key = info.Key;
            KeyChar = info.KeyChar;
            Modifiers = info.Modifiers;
        }

        public ConsoleKey Key { get; }
        public ConsoleModifiers Modifiers { get; }
        public char KeyChar { get; }
    }
}