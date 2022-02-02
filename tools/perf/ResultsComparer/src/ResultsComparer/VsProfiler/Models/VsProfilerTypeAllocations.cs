//---------------------------------------------------------------------
// <copyright file="VsProfilerTypeAllocations.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using ResultsComparer.Core;

namespace ResultsComparer.VsProfiler
{
    /// <summary>
    /// Represents the type of entries in the "Allocations" tab of a
    /// VS .NET Object Allocation profiler report.
    /// </summary>
    public class VsProfilerTypeAllocations
    {
        [FieldName("Type")]
        public string Type { get; set; }

        [FieldName("Allocations")]
        public long Allocations { get; set; }

        [FieldName("Bytes")]
        public long Bytes { get; set; }
    }
}
