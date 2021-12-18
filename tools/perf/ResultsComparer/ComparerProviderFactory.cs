using ResultsComparer.Bdn;
using ResultsComparer.Core;
using ResultsComparer.VsProfiler;

namespace ResultsComparer
{
    public class ComparerProviderFactory
    {
        public static IResultsComparerProvider CreateDefaultProvider()
        {
            ResultsComparerProvider provider = new();
            provider.RegisterComparer("bdn", new BdnComparer());
            provider.RegisterComparer("vsAllocs", new VsAllocationsComparer());
            return provider;
        }
    }
}
