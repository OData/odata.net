//---------------------------------------------------------------------
// <copyright file="PassThroughPrimitiveValueConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// An implementation of primitive value converter that directly pass through the primitive value.
    /// </summary>
    internal class PassThroughPrimitiveValueConverter : IPrimitiveValueConverter
    {
        internal static readonly IPrimitiveValueConverter Instance = new PassThroughPrimitiveValueConverter();

        private PassThroughPrimitiveValueConverter()
        {
        }

        public object ConvertToUnderlyingType(object value)
        {
            return value;
        }

        public object ConvertFromUnderlyingType(object value)
        {
            return value;
        }
    }
}
