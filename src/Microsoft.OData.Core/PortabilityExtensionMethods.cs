//---------------------------------------------------------------------
// <copyright file="PortabilityExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.OData
{
    #region Namespaces
    using System.Data.OData.Staging;
    #endregion Namespaces

#if WINDOWS_PHONE || ORCAS
    /// <summary>
    /// Helper extension methods to hide differences in platforms.
    /// </summary>
    internal static class PortabilityExtensionMethods
    {
        /// <summary>
        /// Returns true if the specified kind enum has the specified flags set.
        /// </summary>
        /// <param name="kind">The kind enum to test.</param>
        /// <param name="flag">The flag to look for.</param>
        /// <returns>true if the flag is set; false otherwise.</returns>
        internal static bool HasFlag(this ODataResourcePropertyKind kind, ODataResourcePropertyKind flag)
        {
            DebugUtils.CheckNoExternalCallers();
            return (kind & flag) == flag;
        }
    }
#endif
}
