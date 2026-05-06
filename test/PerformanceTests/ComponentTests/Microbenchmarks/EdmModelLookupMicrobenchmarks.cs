//---------------------------------------------------------------------
// <copyright file="EdmModelLookupMicrobenchmarks.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance.Microbenchmarks
{
    using System.Linq;
    using BenchmarkDotNet.Attributes;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Targets repeated EDM model lookups: <see cref="IEdmType"/> resolution,
    /// <see cref="IEdmModel.FindBoundOperations"/>, and FullName comparisons.
    /// See REPORT.md findings #17, #18, #19, #45.
    /// </summary>
    [MemoryDiagnoser]
    public class EdmModelLookupMicrobenchmarks
    {
        private IEdmModel _model;
        private IEdmStructuredType _bindingType;
        private string[] _typeFullNames;

        [GlobalSetup]
        public void Setup()
        {
            _model = TestUtils.GetAdventureWorksModel();
            _bindingType = (IEdmStructuredType)_model.FindDeclaredType("PerformanceServices.Edm.AdventureWorks.Product");
            _typeFullNames = _model.SchemaElements
                .OfType<IEdmSchemaType>()
                .Select(t => t.FullName())
                .ToArray();
        }

        [Benchmark]
        public int FindDeclaredType_Bulk()
        {
            int hits = 0;
            for (int round = 0; round < 100; round++)
            {
                foreach (string name in _typeFullNames)
                {
                    if (_model.FindDeclaredType(name) != null)
                    {
                        hits++;
                    }
                }
            }
            return hits;
        }

        [Benchmark]
        public int FindBoundOperations_Bulk()
        {
            int total = 0;
            for (int round = 0; round < 1000; round++)
            {
                foreach (var _ in _model.FindBoundOperations(_bindingType))
                {
                    total++;
                }
            }
            return total;
        }
    }
}
