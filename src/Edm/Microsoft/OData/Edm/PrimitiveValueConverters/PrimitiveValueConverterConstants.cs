//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
