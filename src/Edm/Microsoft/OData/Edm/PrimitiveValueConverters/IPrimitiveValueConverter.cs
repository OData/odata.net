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
    /// <summary>
    /// Class for defining a primitive value conversion for a type definition.
    /// Suppose a type definition defines a primitive type X (underlying type) as a new type Y,
    /// and the type Y has a logically corresponding CLR type Z,
    /// the ConvertToUnderlyingType method converts value from Z to X
    /// and the ConvertFromUnderlyingType method converts value from X to Z.
    /// </summary>
    public interface IPrimitiveValueConverter
    {
        /// <summary>
        /// Converts the given primitive value from the CLR type to the underlying type defined in a type definition.
        /// </summary>
        /// <param name="value">The given CLR value.</param>
        /// <returns>The converted value of the underlying type.</returns>
        object ConvertToUnderlyingType(object value);

        /// <summary>
        /// Converts the given primitive value from the underlying type to the CLR type defined in a type definition.
        /// </summary>
        /// <param name="value">The given value of the CLR type.</param>
        /// <returns>The converted CLR value.</returns>
        object ConvertFromUnderlyingType(object value);
    }
}
