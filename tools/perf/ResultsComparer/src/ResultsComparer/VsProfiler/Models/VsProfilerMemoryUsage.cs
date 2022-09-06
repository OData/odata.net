//---------------------------------------------------------------------
// <copyright file="VsProfilerMemoryUsage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using ResultsComparer.Core;

namespace ResultsComparer.VsProfiler
{
    /// <summary>
    /// Represents the type of entries in a
    /// VS Memory Usage report.
    /// </summary>
    public class VsProfilerMemoryUsage
    {
        [FieldName("Object Type")]
        public string Type { get; set; }
        [FieldName("Count")]
        public long Count { get; set; }
        [FieldName("Size (Bytes)")]
        public long Size { get; set; }
        [FieldName("Inclusive Size (Bytes)")]
        public long InclusiveSize { get; set; }
        [FieldName("Module")]
        public string Module { get; set; }
    }
}
