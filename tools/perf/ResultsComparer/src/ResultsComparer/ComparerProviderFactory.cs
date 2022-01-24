//---------------------------------------------------------------------
// <copyright file="ComparerProviderFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
            provider.RegisterComparer("vsMem", new VsMemoryUsageComparer());
            provider.RegisterComparer("vsAllocs", new VsTypeAllocationsComparer());
            provider.RegisterComparer("vsFuncAllocs", new VsFunctionAllocationsComparer());
            return provider;
        }
    }
}
