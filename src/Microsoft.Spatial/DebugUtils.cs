//---------------------------------------------------------------------
// <copyright file="DebugUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    /// <summary>
    /// Dummy class for code that is shared with ODataLib.
    /// The ODataLib version of this class has an implementation, but this version is just provided
    /// so that we don't have to conditionally compile all references to it in the shared code.
    /// Since it is debug-only anyway, there is no harm in leaving this no-op version so that the shared code is cleaner.
    /// </summary>
    internal static class DebugUtils
    {
        /// <summary>
        /// Dummy method to allow shared code to compile.
        /// </summary>
        [System.Diagnostics.Conditional("DEBUG")]
        internal static void CheckNoExternalCallers()
        {
        }
    }
}