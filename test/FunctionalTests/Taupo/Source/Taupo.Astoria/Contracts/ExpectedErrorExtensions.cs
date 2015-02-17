//---------------------------------------------------------------------
// <copyright file="ExpectedErrorExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Extensions to the expected error message API
    /// </summary>
    public static class ExpectedErrorExtensions
    {
        /// <summary>
        /// Verifies the ExceptionMessage recursively
        /// </summary>
        /// <param name="expectedErrorMessage">Error Message to Verify</param>
        /// <param name="resourceVerifier">Resource Verifier to use</param>
        /// <param name="errorPayload">Error payload to verify</param>
        /// <param name="assert">assert call to make</param>
        public static void VerifyExceptionMessage(this ExpectedErrorMessage expectedErrorMessage, IStringResourceVerifier resourceVerifier, ODataErrorPayload errorPayload, Action<bool, string> assert)
        {
            ExceptionUtilities.CheckArgumentNotNull(expectedErrorMessage, "expectedErrorMessage");
            ExceptionUtilities.CheckArgumentNotNull(resourceVerifier, "resourceVerifier");
            ExceptionUtilities.CheckArgumentNotNull(errorPayload, "errorPayload");
            ExceptionUtilities.CheckArgumentNotNull(assert, "assert");

            // Verify messages are the same
            // TODO: Need to also verify the language and code
            expectedErrorMessage.VerifyMatch(resourceVerifier, errorPayload.Message, true);

            ExpectedErrorMessage expectedInnerError = expectedErrorMessage.InnerError;
            ODataInternalExceptionPayload innerError = errorPayload.InnerError;

            // Set it to null so comparison won't occur
            if (expectedErrorMessage.SkipInnerErrorVerification)
            {
                innerError = null;
                expectedInnerError = null;
            }

            while (innerError != null)
            {
                // TODO: better error message
                assert(expectedInnerError != null, "Did not expect inner error");

                // Verify messages are the same
                expectedInnerError.VerifyMatch(resourceVerifier, innerError.Message, true);

                if (expectedInnerError.SkipInnerErrorVerification)
                {
                    innerError = null;
                    expectedInnerError = null;
                }
                else
                {
                    // TODO: verify stack trace, type name, etc
                    innerError = innerError.InternalException;
                    expectedInnerError = expectedInnerError.InnerError;
                }
            }
        }

        /// <summary>
        /// Verifies that the error matches
        /// </summary>
        /// <param name="expected">The expected error message</param>
        /// <param name="defaultVerifier">The verifier to use if the message does not have one set</param>
        /// <param name="actual">The actual error string</param>
        /// <param name="isExactMatch">Whether to perform an exact match</param>
        public static void VerifyMatch(this ExpectedErrorMessage expected, IStringResourceVerifier defaultVerifier, string actual, bool isExactMatch)
        {
            ExceptionUtilities.CheckArgumentNotNull(expected, "expected");

            var verifier = expected.Verifier ?? defaultVerifier;
            ExceptionUtilities.CheckObjectNotNull(verifier, "Either a default verifier or specific message verifier must be provided");

            verifier.VerifyMatch(expected.ResourceIdentifier, actual, isExactMatch, expected.Arguments.ToArray());
        }
        
        /// <summary>
        /// Creates an expected error with the given information
        /// </summary>
        /// <param name="verifier">The verifier to use</param>
        /// <param name="identifier">The identifier for the error string</param>
        /// <param name="args">The arguments to the error string</param>
        /// <returns>The expected error</returns>
        public static ExpectedErrorMessage CreateExpectedError(this IStringResourceVerifier verifier, string identifier, params string[] args)
        {
            return new ExpectedErrorMessage(identifier, args) { Verifier = verifier };
        }
    }
}
