//-----------------------------------------------------------------------------
// <copyright file="OpenTypesDataModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.OData.Client.E2E.Tests.Common.Server.OpenTypes
{
    public class ContactDetails
    {
        public byte[]? FirstContacted { get; set; }
        public DateTimeOffset LastContacted { get; set; }
        public DateTimeOffset Contacted { get; set; }
        public Guid GUID { get; set; }
        public TimeSpan PreferedContactTime { get; set; }
        public byte Byte { get; set; }
        public sbyte SignedByte { get; set; }
        public double Double { get; set; }
        public float Single { get; set; }
        public short Short { get; set; }
        public int Int { get; set; }
        public long Long { get; set; }
    }

    public class Row
    {
        public Guid Id { get; set; }
        public Dictionary<string, object>? DynamicProperties { get; set; }
    }

    public class IndexedRow : Row
    {
    }

    public class RowIndex
    {
        public int Id { get; set; }
        public ICollection<IndexedRow>? Rows { get; set; }
        public Dictionary<string, object>? DynamicProperties { get; set; }
    }
}
