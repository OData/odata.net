//-----------------------------------------------------------------------------
// <copyright file="PrimitiveKeyValuesDataModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using EfKey = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace Microsoft.OData.Client.E2E.Tests.Common.Server.PrimitiveKeys
{
    [Key("Id")]
    public class EdmBinary
    {
        public Byte[] Id { get; set; }
    }

    [Key("Id")]
    public class EdmBoolean
    {
        public Boolean Id { get; set; }
    }

    [Key("Id")]
    public class EdmByte
    {
        public Byte Id { get; set; }
    }

    [Key("Id")]
    public class EdmDecimal
    {
        public Decimal Id { get; set; }
    }

    [Key("Id")]
    public class EdmDouble
    {
        public Double Id { get; set; }
    }

    [Key("Id")]
    public class EdmSingle
    {
        public Single Id { get; set; }
    }

    [Key("Id")]
    public class EdmGuid
    {
        public Guid Id { get; set; }
    }

    [Key("Id")]
    public class EdmInt16
    {
        public Int16 Id { get; set; }
    }

    [Key("Id")]
    public class EdmInt32
    {
        public Int32 Id { get; set; }
    }

    [Key("Id")]
    public class EdmInt64
    {
        public Int64 Id { get; set; }
    }

    [Key("Id")]
    public class EdmString
    {
        public String Id { get; set; }
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
