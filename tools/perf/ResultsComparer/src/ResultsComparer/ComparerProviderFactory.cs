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
    /// <summary>
    /// A class that provides helpers to set up and create
    /// instances of <see cref="IResultsComparerProvider"/>
    /// </summary>
    public class ComparerProviderFactory
    {
        /// <summary>
        /// This creates the default <see cref="IResultsComparerProvider"/>
        /// initialized with the default supported <see cref="IResultsComparer"/>.
        /// </summary>
        /// <returns>The default <see cref="IResultsComparerProvider"/></returns>
        public static IResultsComparerProvider CreateDefaultProvider()
        {
            ResultsComparerProvider provider = new();
            provider.RegisterComparer("bdn", new BdnComparer());
            provider.RegisterComparer("vsMem", new VsMemoryUsageComparer());
            provider.RegisterComparer("vsTypeAllocs", new VsTypeAllocationsComparer());
            provider.RegisterComparer("vsFuncAllocs", new VsFunctionAllocationsComparer());
            return provider;
        }
    }
}
