//---------------------------------------------------------------------
// <copyright file="VsProfilerFunctionAllocations.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using ResultsComparer.Core;

namespace ResultsComparer.VsProfiler.Models
{
    /// <summary>
    /// Represents the type of entries in a
    /// VS .NET Object Allocation Profiler Function Allocations report.
    /// </summary>
    public class VsProfilerFunctionAllocations
    {
        [FieldName("Name")]
        public string Name { get; set; }

        [FieldName("Total (Allocations)")]
        public long TotalAllocations { get; set; }

        [FieldName("Self (Allocations)")]
        public long SelfAllocations { get; set; }

        [FieldName("Self Size (Bytes)")]
        public long Size { get; set; }
    }
}
