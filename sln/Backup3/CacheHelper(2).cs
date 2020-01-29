//---------------------------------------------------------------------
// <copyright file="CacheHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Helper for Cache class.
    /// </summary>
    internal static class CacheHelper
    {
        internal static readonly object Unknown = new object();
        internal static readonly object CycleSentinel = new object();
        internal static readonly object SecondPassCycleSentinel = new object();

        private static readonly object BoxedTrue = true;
        private static readonly object BoxedFalse = false;

        internal static object BoxedBool(bool value)
        {
            return value ? BoxedTrue : BoxedFalse;
        }
    }
}
