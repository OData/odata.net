//---------------------------------------------------------------------
// <copyright file="DefaultPrimitiveValueConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.PrimitiveValueConverters
{
    using System;
    using System.Globalization;

    /// <summary>
    /// The default implementation of primitive value converter for unsigned ints, which:
    ///     converts UInt16 to Int32,
    ///     converts UInt32 to Int64,
    ///     converts UInt64 to Decimal.
    /// </summary>
    internal class DefaultPrimitiveValueConverter : IPrimitiveValueConverter
    {
        internal static readonly IPrimitiveValueConverter Instance = new DefaultPrimitiveValueConverter();

        private DefaultPrimitiveValueConverter()
        {
        }

        public object ConvertToUnderlyingType(object value)
        {
            var type = value.GetType();
            var typeCode = Type.GetTypeCode(type);

            switch (typeCode)
            {
                case TypeCode.UInt16:
                    return Convert.ToInt32(value, CultureInfo.InvariantCulture);
                case TypeCode.UInt32:
                    return Convert.ToInt64(value, CultureInfo.InvariantCulture);
                case TypeCode.UInt64:
                    return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
            }

            return value;
        }

        public object ConvertFromUnderlyingType(object value)
        {
            var type = value.GetType();
            var typeCode = Type.GetTypeCode(type);

            switch (typeCode)
            {
                case TypeCode.Int32:
                    return Convert.ToUInt16(value, CultureInfo.InvariantCulture);
                case TypeCode.Int64:
                    return Convert.ToUInt32(value, CultureInfo.InvariantCulture);
                case TypeCode.Decimal:
                    return Convert.ToUInt64(value, CultureInfo.InvariantCulture);
            }

            return value;
        }
    }
}
