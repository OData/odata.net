//---------------------------------------------------------------------
// <copyright file="ExpectedExceptionsComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Common;
    
    /// <summary>
    /// Compares an ExpectedExceptions object with an exception.
    /// </summary>
    public static class ExpectedExceptionsComparer
    {
        /// <summary>
        /// Compares an ExpectedExceptions object with an exception.
        /// </summary>
        /// <param name="expectedExceptions">The expected exceptions.</param>
        /// <param name="actualException">The actual exception.</param>
        /// <returns>Success if the exception matches, skipped if there was no exception and none was expected, or failed if the exception does not match.</returns>
        public static ComparisonResult CompareException(ExpectedExceptions expectedExceptions, Exception actualException)
        {
            if (expectedExceptions == null)
            {
                if (actualException != null)
                {
                    throw actualException;
                }

                return ComparisonResult.Skipped;
            }
            else
            {
                ComparisonResult result = Verify(expectedExceptions, actualException);
                if (result != ComparisonResult.Failure)
                {
                    return result;
                }
                else
                {
                    if (actualException == null)
                    {
                        throw new TaupoInvalidOperationException("No exception was thrown but one was expected.");
                    }
                    else if (expectedExceptions.ExpectedException != null)
                    {
                        throw new TaupoInvalidOperationException("A different exception was thrown than expected.", actualException);
                    }
                    else
                    {
                        // bug: remove once SqlCeExceptions can be expected.
                        if (actualException.GetType().FullName == "System.Data.SqlServerCe.SqlCeException")
                        {
                            return ComparisonResult.Success;
                        }

                        throw actualException;
                    }
                }
            }
        }

        /// <summary>
        /// Verifies that the <paramref name="actualException"/> matches at least one of the expected exception, or the ignorable exceptions.
        /// </summary>
        /// <param name="expectedExceptions">The expected exceptions.</param>
        /// <param name="actualException">The actual exception.</param>
        /// <returns>Success if the exception matches, skipped if there was no exception and none was expected, or failed if the exception does not match.</returns>
        internal static ComparisonResult Verify(ExpectedExceptions expectedExceptions, Exception actualException)
        {
            if (actualException == null)
            {
                // return skipped only if all are ignored
                if (expectedExceptions.ExpectedException == null)
                {
                    return ComparisonResult.Skipped;
                }
                else
                {
                    return ComparisonResult.Failure;
                }
            }
            else
            {
                // return success if exception matches
                if (expectedExceptions.ExpectedException != null && Verify(expectedExceptions.ExpectedException, actualException))
                {
                    return ComparisonResult.Success;
                }

                // return success if an ignored exception is matched
                foreach (var expected in expectedExceptions.IgnoredExceptions)
                {
                    if (Verify(expected, actualException))
                    {
                        return ComparisonResult.Success;
                    }
                }

                return ComparisonResult.Failure;
            }
        }

        /// <summary>
        /// Verifies whether the actual exception matches the expected exception.
        /// </summary>
        /// <param name="expectedException">The expected exception.</param>
        /// <param name="exception">The actual exception.</param>
        /// <returns>Whether the <paramref name="actualException"/> matches the <paramref name="expectedException"/>.</returns>
        internal static bool Verify(ExpectedExceptionVerifier expectedException, Exception exception)
        {
            if (!expectedException.VerifyException(exception))
            {
                return false;
            }

            if (expectedException.InnerExpectedException != null)
            {
                return Verify(expectedException.InnerExpectedException, exception.InnerException);
            }

            return true;
        }
    }
}
