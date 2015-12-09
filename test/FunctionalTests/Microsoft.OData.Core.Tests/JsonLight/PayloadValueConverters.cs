using System;
using System.Globalization;
using Microsoft.OData.Core.Metadata;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.Tests.JsonLight
{
    internal class BinaryFieldAsStringPrimitivePayloadValueConverter : ODataPayloadValueConverter
    {
        public override object ConvertToPayloadValue(object value, IEdmTypeReference edmTypeReference)
        {
            // Bypass the conversion to Byte[] in case of Explicit Binary Properties
            if (IsStringPayloadValueAndTypeBinary(value, edmTypeReference))
            {
                return value;
            }
            else
            {
                return base.ConvertToPayloadValue(value, edmTypeReference);
            }
        }

        public override object ConvertFromPayloadValue(object value, IEdmTypeReference edmTypeReference)
        {
            // Skip Base implementation in case of Explicit Binary Properties
            if (IsStringPayloadValueAndTypeBinary(value, edmTypeReference))
            {
                return value;
            }

            return base.ConvertFromPayloadValue(value, edmTypeReference);
        }

        private static bool IsStringPayloadValueAndTypeBinary(object value, IEdmTypeReference edmTypeReference)
        {
            IEdmPrimitiveTypeReference actualTypeReference = EdmLibraryExtensions.GetPrimitiveTypeReference(value.GetType());

            return actualTypeReference.IsString() && edmTypeReference.IsBinary();
        }
    }

    internal class DateTimeOffsetCustomFormatPrimitivePayloadValueConverter : ODataPayloadValueConverter
    {
        public override object ConvertToPayloadValue(object value, IEdmTypeReference edmTypeReference)
        {
            if (value is DateTimeOffset)
            {
                return ((DateTimeOffset)value).ToString("R", CultureInfo.InvariantCulture);
            }

            return base.ConvertToPayloadValue(value, edmTypeReference);
        }

        public override object ConvertFromPayloadValue(object value, IEdmTypeReference edmTypeReference)
        {
            if (edmTypeReference.IsDateTimeOffset() && value is string)
            {
                return DateTimeOffset.Parse((string)value, CultureInfo.InvariantCulture);
            }

            return base.ConvertFromPayloadValue(value, edmTypeReference);
        }
    }
}
