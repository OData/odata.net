//   OData .NET Libraries ver. 6.9
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

#if !INTERNAL_DROP || ODATALIB

#if ASTORIA_SERVER
namespace Microsoft.OData.Service
#else
namespace Microsoft.OData.Core
#endif
{
    #region Namespaces
    #if ASTORIA_SERVER
    using Microsoft.OData.Core;
    #endif
    #endregion Namespaces

    /// <summary>
    /// Class with utility methods to deal with values in ODataLib.
    /// </summary>
    internal static class ODataValueUtils
    {
        /// <summary>
        /// Converts an object to an ODataValue. If the given object is already an ODataValue (such as an ODataCompleValue, ODataCollectionValue, etc.), the original object will be returned.
        /// </summary>
        /// <param name="objectToConvert">The object to convert to an ODataValue</param>
        /// <returns>The given object as an ODataValue.</returns>
        internal static ODataValue ToODataValue(this object objectToConvert)
        {
            if (objectToConvert == null)
            {
                return new ODataNullValue();
            }

            // If the given object is already an ODataValue, return it as is.
            ODataValue odataValue = objectToConvert as ODataValue;
            if (odataValue != null)
            {
                return odataValue;
            }

            // Otherwise treat it as a primitive and wrap in an ODataPrimitiveValue. This includes spatial types.
            return new ODataPrimitiveValue(objectToConvert);
        }

        /// <summary>
        /// Converts an ODataValue to the old style of representing values, where null values are null and primitive values are just the direct primitive (no longer wrapped by ODataPrimitiveValue).
        /// All other value types, such as ODataComplexValue and ODataCollectionValue are returned unchanged.
        /// </summary>
        /// <param name="odataValue">The value to convert.</param>
        /// <returns>The value behind the given ODataValue.</returns>
        internal static object FromODataValue(this ODataValue odataValue)
        {
            if (odataValue is ODataNullValue)
            {
                return null;
            }

            ODataPrimitiveValue primitiveValue = odataValue as ODataPrimitiveValue;
            if (primitiveValue != null)
            {
                return primitiveValue.Value;
            }

            return odataValue;
        }
    }
}

#endif
