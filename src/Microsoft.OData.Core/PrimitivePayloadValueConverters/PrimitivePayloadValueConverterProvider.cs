//---------------------------------------------------------------------
// <copyright file="DefaultPrimitiveValueConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.PrimitivePayloadValueConverters
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Provides the default of primitive payload value converter
    /// </summary>
    public static class PrimitivePayloadValueConverterProvider
    {
        private static IPrimitivePayloadValueConverter _primitivePayloadValueConverter = DefaultPrimitivePayloadValueConverter.Instance;

        public static void SetPrimitivePayloadValueConverter(IPrimitivePayloadValueConverter primitivePayloadValueConverter)
        {
            _primitivePayloadValueConverter = primitivePayloadValueConverter;
        }

        public static IPrimitivePayloadValueConverter GetPrimitivePayloadValueConverter()
        {
            return _primitivePayloadValueConverter;
        }
    }
}
