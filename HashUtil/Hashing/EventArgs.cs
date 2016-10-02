using System;

namespace HashUtil.Hashing
{
    [Serializable]
    public class StatusChangedEventArgs : EventArgs
    {
        public StatusChangedEventArgs(HashStatus value)
        {
            Value = value;
        }

        public HashStatus Value { get; }
    }

    [Serializable]
    public class ProgressChangedEventArgs : EventArgs
    {
        public ProgressChangedEventArgs(int current, int upperBound)
        {
            IsBoundUnknown = upperBound < current || upperBound < 0;
            Current = current;
            UpperBound = upperBound;
        }

        public static ProgressChangedEventArgs Unknown { get; } = new ProgressChangedEventArgs(-1, -1);

        public bool IsBoundUnknown { get; }
        public int Current { get; }
        public int UpperBound { get; }
    }
}
