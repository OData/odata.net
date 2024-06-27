//-----------------------------------------------------------------------------
// <copyright file="OpenTypesServiceClientTestDataModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.OData.Client.E2E.Tests.ClientTests.Server
{
    public partial class Row
    {
        public bool? OpenBoolean { get; set; }
        public ContactDetails? OpenComplex { get; set; }
        public DateTimeOffset? OpenDateTimeOffset { get; set; }
        public decimal? OpenDecimal { get; set; }
        public double? OpenDouble { get; set; }
        public float? OpenFloat { get; set; }
        public Guid? OpenGuid { get; set; }
        public Int16? OpenInt16 { get; set; }
        public Int64? OpenInt64 { get; set; }
        public string? OpenString { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public IDictionary<string, object>? DynamicProperties { get; set; }
    }

    public partial class RowIndex
    {
        public string? IndexComments { get; set; }
    }

    public class ContactDetails
    {
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
}
