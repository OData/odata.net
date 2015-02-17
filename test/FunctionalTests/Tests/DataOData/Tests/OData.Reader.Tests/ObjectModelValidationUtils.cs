//---------------------------------------------------------------------
// <copyright file="ObjectModelValidationUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using Microsoft.OData.Core;
    #endregion Namespaces

    /// <summary>
    /// Helper methods to compare OData OM instances.
    /// </summary>
    internal static class ObjectModelValidationUtils
    {
        /// <summary>
        /// Compares two <see cref="ODataError"/> instances and returns true if they are equal. Otherwise returns false.
        /// </summary>
        /// <param name="first">The first <see cref="ODataError"/> to use in the comparison.</param>
        /// <param name="second">The second <see cref="ODataError"/> to use in the comparison.</param>
        /// <returns>true if the <paramref name="first"/> and <paramref name="second"/> error instances are equal; otherwise false.</returns>
        internal static bool AreEqual(ODataError first, ODataError second)
        {
            if (first == null && second == null) return true;
            if (first == null || second == null) return false;

            if (string.CompareOrdinal(first.ErrorCode, second.ErrorCode) != 0) return false;
            if (string.CompareOrdinal(first.Message, second.Message) != 0) return false;
            if (string.CompareOrdinal(first.MessageLanguage, second.MessageLanguage) != 0) return false;
            if (!AreEqual(first.InnerError, second.InnerError)) return false;

            return true;
        }

        /// <summary>
        /// Compares two <see cref="ODataInnerError"/> instances and returns true if they are equal. Otherwise returns false.
        /// </summary>
        /// <param name="first">The first <see cref="ODataInnerError"/> to use in the comparison.</param>
        /// <param name="second">The second <see cref="ODataInnerError"/> to use in the comparison.</param>
        /// <returns>true if the <paramref name="first"/> and <paramref name="second"/> inner error instances are equal; otherwise false.</returns>
        internal static bool AreEqual(ODataInnerError first, ODataInnerError second)
        {
            if (first == null && second == null) return true;
            if (first == null || second == null) return false;

            if (string.CompareOrdinal(first.Message, second.Message) != 0) return false;
            if (string.CompareOrdinal(first.TypeName, second.TypeName) != 0) return false;
            if (string.CompareOrdinal(first.StackTrace, second.StackTrace) != 0) return false;
            if (!AreEqual(first.InnerError, second.InnerError)) return false;

            return true;
        }
    }
}
