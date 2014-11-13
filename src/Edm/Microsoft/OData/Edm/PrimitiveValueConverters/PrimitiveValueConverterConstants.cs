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
    /// The constants for primitive value converters.
    /// </summary>
    internal static class PrimitiveValueConverterConstants
    {
        /// <summary>
        /// The name of the type definition for UInt16.
        /// </summary>
        internal const string UInt16TypeName = "UInt16";

        /// <summary>
        /// The name of the type definition for UInt32.
        /// </summary>
        internal const string UInt32TypeName = "UInt32";

        /// <summary>
        /// The name of the type definition for UInt64.
        /// </summary>
        internal const string UInt64TypeName = "UInt64";

        /// <summary>
        /// The default underlying type of the type definition for UInt16.
        /// </summary>
        internal const string DefaultUInt16UnderlyingType = "Edm.Int32";

        /// <summary>
        /// The default underlying type of the type definition for UInt32.
        /// </summary>
        internal const string DefaultUInt32UnderlyingType = "Edm.Int64";

        /// <summary>
        /// The default underlying type of the type definition for UInt64.
        /// </summary>
        internal const string DefaultUInt64UnderlyingType = "Edm.Decimal";
    }
}
