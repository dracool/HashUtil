using System;

namespace HashUtil.Hashing
{
    [Serializable]
    public class FilterNameNotFoundException : Exception
    {
        public FilterNameNotFoundException(string filter) : base($"Specified filter not found: {filter}")
        {
            Filter = filter;
        }
        public FilterNameNotFoundException(string filter, Exception inner) : base($"Specified filter not found: {filter}", inner)
        {
            Filter = filter;
        }

        protected FilterNameNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public string Filter { get; }
    }
}
