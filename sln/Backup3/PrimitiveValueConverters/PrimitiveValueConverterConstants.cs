//---------------------------------------------------------------------
// <copyright file="PrimitiveValueConverterConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
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
