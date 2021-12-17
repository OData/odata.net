using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultsComparer.Core
{
    interface IReader<T> : IDisposable, IEnumerable<T> where T : new()
    {
        bool ReadNext(out T value);
    }
}
