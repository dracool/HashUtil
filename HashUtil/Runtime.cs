using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashUtil
{
    enum Interface
    {
        GUI = 0,
        Console = 1,
    }

    enum HashingMode
    {
        Select = 0,
        Match = 1,
        Calculate = 2,
    }

    enum HashSource
    {
        None = 0,
        Argument = 1,
        Clipbard = 2,
        File = 3,
    }

    class Runtime
    {
        public class RuntimeBuilder
        {
            protected RuntimeBuilder() { }

            public string FilePath { get; set; }
            public string Hash { get; set; }
            public Interface Interface { get; set; }
            public HashingMode Mode { get; set; }
            public HashSource HashSource { get; set; }

            public void Build()
            {
                if(Parameters != null) return;
                Parameters = new Runtime(this);
            }
        }
        private class BuilderInstance : RuntimeBuilder
        {
            public BuilderInstance() : base() { }
        }

        private Runtime(RuntimeBuilder b)
        {
            FilePath = b.FilePath;
            Hash = b.Hash;
            Interface = b.Interface;
            Mode = b.Mode;
            HashSource = b.HashSource;
        }
        public static RuntimeBuilder Builder { get; } = new BuilderInstance();

        public static Runtime Parameters { get; private set; }

        public string FilePath { get; }
        public string Hash { get; }
        public Interface Interface { get; }
        public HashingMode Mode { get; }
        public HashSource HashSource { get; }
    }
}
