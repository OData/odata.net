//---------------------------------------------------------------------
// <copyright file="OpenTypesServiceClientTypes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.TestServices.OpenTypesServiceReference
{
    using System;

    /// <summary>
    /// Client side Row type with open properties.
    /// </summary>
    public partial class Row
    {
        public bool? OpenBoolean { get; set; }
        public ContactDetails OpenComplex { get; set; }
        public DateTimeOffset? OpenDateTimeOffset { get; set; }
        public decimal? OpenDecimal { get; set; }
        public double? OpenDouble { get; set; }
        public float? OpenFloat { get; set; }
        public Guid? OpenGuid { get; set; }
        public Int16? OpenInt16 { get; set; }
        public Int64? OpenInt64 { get; set; }
        public string OpenString { get; set; }
        public TimeSpan? OpenTime { get; set; }
    }

    /// <summary>
    /// Client side RowIndex type with open properties.
    /// </summary>
    public partial class RowIndex
    {
        public string IndexComments { get; set; }
    }
}