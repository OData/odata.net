//   OData .NET Libraries ver. 6.8.1
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
