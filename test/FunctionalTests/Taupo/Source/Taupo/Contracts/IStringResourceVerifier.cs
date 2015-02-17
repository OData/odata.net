//---------------------------------------------------------------------
// <copyright file="IStringResourceVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    /// <summary>
    /// Helpers for verifying strings that are pulled from a localized resources
    /// </summary>
    public interface IStringResourceVerifier
    {
        /// <summary>
        /// Determines if the supplied string is an instance of the string defined in localized resources
        /// </summary>
        /// <param name="expectedResourceKey">The key of the resource string to match against</param>
        /// <param name="actualMessage">String value to be verified</param>
        /// <param name="stringParameters">
        /// Expected values for string.Format placeholders in the resource string
        /// If none are supplied then any values for placeholders in the resource string will count as a match
        /// </param>
        /// <returns>True if the string matches, false otherwise</returns>
        bool IsMatch(string expectedResourceKey, string actualMessage, params object[] stringParameters);

        /// <summary>
        /// Determines if the supplied string is an instance of the string defined in localized resources
        /// </summary>
        /// <param name="expectedResourceKey">The key of the resource string to match against</param>
        /// <param name="actualMessage">String value to be verified</param>
        /// <param name="isExactMatch">Determines whether the exception message must be exact match of the message in the resource file, or just contain it.</param>
        /// <param name="stringParameters">
        /// Expected values for string.Format placeholders in the resource string
        /// If none are supplied then any values for placeholders in the resource string will count as a match
        /// </param>
        /// <returns>True if the string matches, false otherwise</returns>
        bool IsMatch(string expectedResourceKey, string actualMessage, bool isExactMatch, params object[] stringParameters);

        /// <summary>
        /// Determines if the supplied string is an instance of the string defined in localized resources
        /// If the string in the resource file contains string.Format place holders then the actual message can contain any values for these
        /// </summary>
        /// <param name="expectedResourceKey">The key of the resource string to match against</param>
        /// <param name="actualMessage">String value to be verified</param>
        /// <param name="stringParameters">
        /// Expected values for string.Format placeholders in the resource string
        /// If none are supplied then any values for placeholders in the resource string will count as a match
        /// </param>
        void VerifyMatch(string expectedResourceKey, string actualMessage, params object[] stringParameters);

        /// <summary>
        /// Determines if the supplied string is an instance of the string defined in localized resources
        /// If the string in the resource file contains string.Format place holders then the actual message can contain any values for these
        /// </summary>
        /// <param name="expectedResourceKey">The key of the resource string to match against</param>
        /// <param name="actualMessage">String value to be verified</param>
        /// <param name="isExactMatch">Determines whether the exception message must be exact match of the message in the resource file, or just contain it.</param>
        /// <param name="stringParameters">
        /// Expected values for string.Format placeholders in the resource string
        /// If none are supplied then any values for placeholders in the resource string will count as a match
        /// </param>
        void VerifyMatch(string expectedResourceKey, string actualMessage, bool isExactMatch, params object[] stringParameters);
    }
}
