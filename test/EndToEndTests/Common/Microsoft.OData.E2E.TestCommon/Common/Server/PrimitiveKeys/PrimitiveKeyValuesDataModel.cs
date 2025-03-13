//-----------------------------------------------------------------------------
// <copyright file="PrimitiveKeyValuesDataModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.OData.Client;
using EfKey = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.PrimitiveKeys
{
    [Key("Id")]
    public class EdmBinary
    {
        public byte[] Id { get; set; }
    }

    [Key("Id")]
    public class EdmBoolean
    {
        public bool Id { get; set; }
    }

    [Key("Id")]
    public class EdmByte
    {
        public byte Id { get; set; }
    }

    [Key("Id")]
    public class EdmDecimal
    {
        public decimal Id { get; set; }
    }

    [Key("Id")]
    public class EdmDouble
    {
        public double Id { get; set; }
    }

    [Key("Id")]
    public class EdmSingle
    {
        public float Id { get; set; }
    }

    [Key("Id")]
    public class EdmGuid
    {
        public Guid Id { get; set; }
    }

    [Key("Id")]
    public class EdmInt16
    {
        public short Id { get; set; }
    }

    [Key("Id")]
    public class EdmInt32
    {
        public int Id { get; set; }
    }

    [Key("Id")]
    public class EdmInt64
    {
        public long Id { get; set; }
    }

    [Key("Id")]
    public class EdmString
    {
        public string Id { get; set; }
    }

    [Key("Id")]
    public class EdmTime
    {
        public TimeSpan Id { get; set; }
    }

    [Key("Id")]
    public class EdmDateTimeOffset
    {
        public DateTimeOffset Id { get; set; }
    }
}
