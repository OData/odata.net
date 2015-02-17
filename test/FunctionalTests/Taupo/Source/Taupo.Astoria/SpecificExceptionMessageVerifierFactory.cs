//---------------------------------------------------------------------
// <copyright file="SpecificExceptionMessageVerifierFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System;
    using System.Globalization;
    using System.Threading;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Default implementation of the contract for building verifiers for specific exceptions for which resource identifiers are unknown.
    /// </summary>
    [ImplementationName(typeof(ISpecificExceptionMessageVerifierFactory), "Default")]
    public class SpecificExceptionMessageVerifierFactory : ISpecificExceptionMessageVerifierFactory
    {
        /// <summary>
        /// Creates a verifier for the given exception.
        /// </summary>
        /// <param name="actualException">The actual exception.</param>
        /// <returns>
        /// A verifier specific to the exception.
        /// </returns>
        public IStringResourceVerifier CreateVerifier(Exception actualException)
        {
            ExceptionUtilities.CheckArgumentNotNull(actualException, "actualException");
            return new ActualExceptionResourceStringVerifier(actualException);
        }

        /// <summary>
        /// Resource string verifier that performs strict matching against an exception that was caught at runtime
        /// </summary>
        private class ActualExceptionResourceStringVerifier : IStringResourceVerifier
        {
            private readonly string actualExceptionMessage;

            /// <summary>
            /// Initializes a new instance of the <see cref="ActualExceptionResourceStringVerifier"/> class.
            /// </summary>
            /// <param name="actualException">The actual exception.</param>
            public ActualExceptionResourceStringVerifier(Exception actualException)
            {
                ExceptionUtilities.CheckArgumentNotNull(actualException, "actualException");
                this.actualExceptionMessage = actualException.Message;
            }

            /// <summary>
            /// Determines if the supplied string is an instance of the string defined in localized resources
            /// </summary>
            /// <param name="expectedResourceKey">The key of the resource string to match against</param>
            /// <param name="actualMessage">String value to be verified</param>
            /// <param name="stringParameters">Expected values for string.Format placeholders in the resource string
            /// If none are supplied then any values for placeholders in the resource string will count as a match</param>
            /// <returns>
            /// True if the string matches, false otherwise
            /// </returns>
            public bool IsMatch(string expectedResourceKey, string actualMessage, params object[] stringParameters)
            {
                return this.IsMatch(expectedResourceKey, actualMessage, true, stringParameters);
            }

            /// <summary>
            /// Determines if the supplied string is an instance of the string defined in localized resources
            /// </summary>
            /// <param name="expectedResourceKey">The key of the resource string to match against</param>
            /// <param name="actualMessage">String value to be verified</param>
            /// <param name="isExactMatch">Determines whether the exception message must be exact match of the message in the resource file, or just contain it.</param>
            /// <param name="stringParameters">Expected values for string.Format placeholders in the resource string
            /// If none are supplied then any values for placeholders in the resource string will count as a match</param>
            /// <returns>
            /// True if the string matches, false otherwise
            /// </returns>
            public bool IsMatch(string expectedResourceKey, string actualMessage, bool isExactMatch, params object[] stringParameters)
            {
                ExceptionUtilities.Assert(isExactMatch, "Only exact match supported");
                return this.actualExceptionMessage == actualMessage;
            }

            /// <summary>
            /// Determines if the supplied string is an instance of the string defined in localized resources
            /// If the string in the resource file contains string.Format place holders then the actual message can contain any values for these
            /// </summary>
            /// <param name="expectedResourceKey">The key of the resource string to match against</param>
            /// <param name="actualMessage">String value to be verified</param>
            /// <param name="stringParameters">Expected values for string.Format placeholders in the resource string
            /// If none are supplied then any values for placeholders in the resource string will count as a match</param>
            public void VerifyMatch(string expectedResourceKey, string actualMessage, params object[] stringParameters)
            {
                this.VerifyMatch(expectedResourceKey, actualMessage, true, stringParameters);
            }

            /// <summary>
            /// Determines if the supplied string is an instance of the string defined in localized resources
            /// If the string in the resource file contains string.Format place holders then the actual message can contain any values for these
            /// </summary>
            /// <param name="expectedResourceKey">The key of the resource string to match against</param>
            /// <param name="actualMessage">String value to be verified</param>
            /// <param name="isExactMatch">Determines whether the exception message must be exact match of the message in the resource file, or just contain it.</param>
            /// <param name="stringParameters">Expected values for string.Format placeholders in the resource string
            /// If none are supplied then any values for placeholders in the resource string will count as a match</param>
            public void VerifyMatch(string expectedResourceKey, string actualMessage, bool isExactMatch, params object[] stringParameters)
            {
                ExceptionUtilities.Assert(isExactMatch, "Only exact match supported");

                if (this.actualExceptionMessage != actualMessage)
                {
                    throw new DataComparisonException("Actual string does not match the expected exception message.")
                    {
                        ExpectedValue = this.actualExceptionMessage,
                        ActualValue = actualMessage
                    };
                }
            }
        }
    }
}
