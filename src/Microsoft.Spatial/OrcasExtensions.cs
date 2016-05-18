//---------------------------------------------------------------------
// <copyright file="OrcasExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Text;

    /// <summary>
    /// This class holds extension methods for objects that have new capabilities
    /// in newer versions of .net, and this lets us make the calls look the same and reduces the #if noise
    /// </summary>
    internal static class OrcasExtensions
    {
        /// <summary>
        /// StringBuilder didn't have a clear method in Orcas, so we added and extension method to give it one.
        /// </summary>
        /// <param name="builder">The StringBuilder instance to clear.</param>
        internal static void Clear(this StringBuilder builder)
         {
             builder.Length = 0;
             builder.Capacity = 0;
         }
    }
}
