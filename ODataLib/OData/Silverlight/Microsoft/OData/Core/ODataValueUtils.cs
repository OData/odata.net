//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
