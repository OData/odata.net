using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultsComparer.Core
{
    public interface IResultsComparer
    {
        Task<bool> CanReadFile(string path);
        Task<ComparerResults> CompareResults(string basePath, string diffPath, ComparerOptions options);
    }
}
