//---------------------------------------------------------------------
// <copyright file="MemoryExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Runtime.InteropServices;

    #endregion Namespaces

    internal static class MemoryExtensions
    {
        /// <summary>
        /// Returns a string representation of the specified character memory without unnecessary allocation.
        /// </summary>
        /// <param name="memory">The character memory to convert to a string.</param>
        /// <returns>
        /// The underlying string when <paramref name="memory"/> is backed by a string slice that starts at index 0 and spans the full length; 
        /// otherwise, a new string created from <paramref name="memory"/>.
        /// </returns>
        /// <remarks>
        /// Uses MemoryMarshal.TryGetString to avoid allocation when possible; falls back to ReadOnlyMemory&lt;char&gt;.ToString().
        /// </remarks>
        public static string GetString(this ReadOnlyMemory<char> memory)
        {
            if (MemoryMarshal.TryGetString(memory, out string value, out int length, out int start) && start == 0 && length == memory.Length)
            {
                return value;
            }

            return memory.ToString();
        }
    }
}
