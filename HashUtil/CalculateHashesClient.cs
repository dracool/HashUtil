using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashUtil
{
    class CalculatedEventArgs : EventArgs
    {
        public CalculatedEventArgs(Dictionary<string, byte[]> data)
        {
            Hashes = new ReadOnlyDictionary<string, byte[]>(data);
        }

        public IReadOnlyDictionary<string, byte[]> Hashes { get; }
    }

    class CalculateHashesClient
    {
        public CalculateHashesClient()
        {
            checker = new HashChecker();
            checker.Progress += (s, e) => Do(Progress, e);
        }

        private HashChecker checker;

        public event EventHandler<CalculatedEventArgs> Calculated;
        public event EventHandler<ProgressEventArgs> Progress;

        private void Do(EventHandler handler)
        {
            handler.Invoke(this, EventArgs.Empty);
        }

        private void Do<TEventArgs>(EventHandler<TEventArgs> handler, TEventArgs args)
            where TEventArgs : EventArgs
        {
            handler.Invoke(this, args);
        }

        public void Calculate()
        {
            using (var filestream = new FileStream(Runtime.Parameters.FilePath, FileMode.Open, FileAccess.Read))
            {
                checker.Data = filestream;
                var result = checker.CalculateHashes();
                Do(Calculated, new CalculatedEventArgs(result));
            }
        }
    }
}
