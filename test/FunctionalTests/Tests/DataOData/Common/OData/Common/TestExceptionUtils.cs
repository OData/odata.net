//---------------------------------------------------------------------
// <copyright file="TestExceptionUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Contracts;
    #endregion Namespaces

    /// <summary>
    /// Helper methods for testing exceptions and error cases
    /// </summary>
    public static class TestExceptionUtils
    {
        /// <summary>
        /// Runs the specified action and catches all exceptions
        /// </summary>
        /// <param name="action">The action to run.</param>
        /// <returns>The exception if one has been thrown, or null otherwise.</returns>
        public static Exception RunCatching(Action action)
        {
            try
            {
                action();
                return null;
            }
            catch (Exception e)
            {
                return e;
            }
        }

        /// <summary>
        /// Asserts that the given action throws the specified exception type.
        /// </summary>
        /// <typeparam name="TException">Exception type.</typeparam>
        /// <param name="action">Action to try.</param>
        /// <param name="expectedExceptionMessage">The expected exception message.</param>
        /// <param name="desciption">String to attach to all errors so that it's easier to locate what went wrong.</param>
        public static void ExpectedException<TException>(this AssertionHandler assert, Action action, string expectedExceptionMessage, string description = null)
        {
            Exception exception = RunCatching(action);
            exception = TestExceptionUtils.UnwrapAggregateException(exception, assert);
            IsExpectedException<TException>(assert, exception, expectedExceptionMessage, description);
        }

        /// <summary>
        /// Asserts that the given action throws the specified exception type.
        /// </summary>
        /// <param name="action">Action to try.</param>
        /// <param name="expectedException">The expected exception.</param>
        /// <param name="exceptionVerifier">The exception verifier.</param>
        public static void ExpectedException(this AssertionHandler assert, Action action, ExpectedException expectedException, IExceptionVerifier exceptionVerifier)
        {
            ExceptionUtilities.CheckArgumentNotNull(assert, "assert");
            ExceptionUtilities.CheckArgumentNotNull(action, "action");
            ExceptionUtilities.CheckArgumentNotNull(exceptionVerifier, "exceptionVerifier");

            Exception exception = RunCatching(action);

            if (exception == null && expectedException == null)
            {
                return;
            }
            else if (exception == null)
            {
                assert.IsNotNull(exception,
                    "Expected exception of type '{0}' with message resource ID '{1}' but none was thrown.",
                    expectedException.ExpectedExceptionType.ToString(),
                    expectedException.ExpectedMessage == null ? "<null>" : expectedException.ExpectedMessage.ResourceIdentifier);
            }
            else if (expectedException == null)
            {
                assert.IsNotNull(expectedException,
                    "Did not expect an exception but an exception of type '{0}' with message '{1}' was thrown.",
                    exception.GetType().ToString(),
                    exception.Message);
            }

            exception = TestExceptionUtils.UnwrapAggregateException(exception, assert);
            exceptionVerifier.VerifyExceptionResult(expectedException, exception);
        }

        /// <summary>
        /// Asserts that the given exception is the specified exception type.
        /// </summary>
        /// <typeparam name="TException">Exception type.</typeparam>
        /// <param name="exception">The exception instance to verify.</param>
        /// <param name="expectedExceptionMessage">The expected exception message. If this is null, the check will verify that no exception was thrown.</param>
        /// <param name="desciption">String to attach to all errors so that it's easier to locate what went wrong.</param>
        public static void IsExpectedException<TException>(this AssertionHandler assert, Exception exception, string expectedExceptionMessage, string description = null)
        {
            if (expectedExceptionMessage == null)
            {
                assert.IsNull(exception, "No exception was expected, but it occured. " + (description ?? string.Empty) + "\r\n" + (exception == null ? string.Empty : exception.ToString()));
            }
            else
            {
                assert.IsNotNull(exception, "Expected " + typeof(TException).FullName + " but it was not thrown. " + (description ?? string.Empty));
                assert.IsTrue(exception is TException, "Exception had unexpected type " + exception.GetType().FullName + ", expected type is " + typeof(TException).FullName + ". " + description);
                assert.AreEqual(expectedExceptionMessage, exception.Message, "Unexpected exception message. " + (description ?? string.Empty));
            }
        }

        /// <summary>
        /// Calls constructor and verifies expected exception.
        /// </summary>
        /// <param name="type">The type to construct.</param>
        /// <param name="errorMessage">The expected error message.</param>
        /// <param name="parameters">The parameters for the constructor.</param>
        public static void CheckInvalidConstructorParameters(AssertionHandler assert, Type type, string errorMessage, params object[] parameters)
        {
            try
            {
                ConstructorInfo c = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Single();
                c.Invoke(parameters);
                assert.Fail(errorMessage);
            }
            catch (TargetInvocationException e)
            {
                assert.IsTrue(e.InnerException is ArgumentException, "Expecting argument exception");
                assert.IsTrue(e.InnerException.Message.Contains(errorMessage), "The exception message doesn't contain the expected string '" + errorMessage + "'.");
            }
        }

        /// <summary>
        /// If an <see cref="AggregateException"/> is passed as <paramref name="exception"/> this method will check whether 
        /// more than one inner exceptions exist and throw if that is the case. Otherwise it returns the single inner exception.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> instance to check.</param>
        /// <param name="assert">The assertion handler.</param>
        /// <returns>
        /// If the <paramref name="exception"/> is an <see cref="AggregateException"/> a single inner exception is expected and returned;
        /// otherwise returns the <paramref name="exception"/> itself.
        /// </returns>
        public static Exception UnwrapAggregateException(Exception exception, AssertionHandler assert)
        {
            AggregateException ae = exception as AggregateException;
            if (ae == null)
            {
                return exception;
            }

            ae = ae.Flatten();
            assert.AreEqual(1, ae.InnerExceptions.Count, "Expected exception count does not match.");
            return ae.InnerExceptions[0];
        }

        /// <summary>
        /// Calls the specified action and if it throws it unwraps the aggregate exception (is any) and continues throwing the exception.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="assert">The assertion handler.</param>
        public static void UnwrapAggregateException(Action action, AssertionHandler assert)
        {
            try
            {
                action();
            }
            catch (AggregateException aggregateException)
            {
                throw UnwrapAggregateException(aggregateException, assert);
            }
        }
    }
}
