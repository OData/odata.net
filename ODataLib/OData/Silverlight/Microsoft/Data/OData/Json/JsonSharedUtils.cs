//   OData .NET Libraries ver. 5.6.3
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

namespace Microsoft.Data.OData.Json
{
    #region Namespaces
    using System;
    using Microsoft.Data.Edm;
    #endregion

    /// <summary>
    /// Shared JSON util code for ODataLib and Server.
    /// </summary>
    internal static class JsonSharedUtils
    {
        /// <summary>
        /// Determines if the given double is serialized as a string in JSON.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>true if the value should be written as a string, false if should be written as a JSON number.</returns>
        internal static bool IsDoubleValueSerializedAsString(double value)
        {
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
            return Double.IsInfinity(value) || Double.IsNaN(value);
        }

        /// <summary>
        /// Determines if the given primitive value is of a basic type where we can rely on just the JSON representation to convey type information.
        /// If so, we don't have to write the type name.
        /// </summary>
        /// <param name="primitiveValue">The primitive value in question.</param>
        /// <param name="valueTypeReference">The type of the primitive value.</param>
        /// <returns>true if the given primitive value is of a basic JSON type, false otherwise.</returns>
        internal static bool ValueTypeMatchesJsonType(ODataPrimitiveValue primitiveValue, IEdmPrimitiveTypeReference valueTypeReference)
        {
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif

            switch (valueTypeReference.PrimitiveKind())
            {
                // If the value being serialized is of a basic type where we can rely on just the JSON representation to convey type information, then never write the type name.
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.String:
                case EdmPrimitiveTypeKind.Boolean:
                    return true;

                case EdmPrimitiveTypeKind.Double:
                    double doubleValue = (double)primitiveValue.Value;

                    // If a double value is positive infinity, negative infinity, or NaN, we serialize the double as a string.
                    // Thus the reader can't infer the type from the JSON representation, and we must write the type name explicitly 
                    // (i.e., if the property is open or the property type is assumed to be unknown, as is the case when writing in full metadata mode).
                    return !IsDoubleValueSerializedAsString(doubleValue);
                
                default:
                    return false;
            }
        }
    }
}

#endif
