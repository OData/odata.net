//---------------------------------------------------------------------
// <copyright file="BatchReaderStreamTestUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Batch
{
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// Utilities for the various batch reader stream tests.
    /// </summary>
    public class BatchReaderStreamTestUtils
    {
        /// <summary>The length of two '-' characters to make the code easier to read.</summary>
        internal const int TwoDashesLength = 2;

        /// <summary>The default line feed characters.</summary>
        internal static readonly char[] DefaultLineFeedChars = new char[] { '\r', '\n' };

        /// <summary>
        /// Check whether the line feed characters consist of a single '\r'.
        /// </summary>
        /// <param name="lineFeed">The line feed characters to check.</param>
        /// <returns>true if the <paramref name="lineFeed"/> is a single carriage return; otherwise false.</returns>
        internal static bool IsSingleCarriageReturn(char[] lineFeed)
        {
            return lineFeed.Length == 1 && lineFeed[0] == '\r';
        }
    }
}
