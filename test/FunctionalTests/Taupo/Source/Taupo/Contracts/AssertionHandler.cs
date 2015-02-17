//---------------------------------------------------------------------
// <copyright file="AssertionHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Assertion handling utilities.
    /// </summary>
    [ImplementationSelector("AssertionHandler", DefaultImplementation = "Default")]
    public abstract class AssertionHandler
    {
        /// <summary>
        /// Asserts that two objects are the same.
        /// </summary>
        /// <typeparam name="T">Type of the argument</typeparam>
        /// <param name="expected">Expected object reference.</param>
        /// <param name="actual">Actual object reference.</param>
        /// <param name="errorMessage">Error message when two objects are not the same.</param>
        /// <param name="errorMessageArguments">The error message arguments.</param>
        public void AreSame<T>(T expected, T actual, string errorMessage, params object[] errorMessageArguments)
            where T : class
        {
            if (!ReferenceEquals(expected, actual))
            {
                this.OnDataComparisonFailure(expected, actual, FormatErrorMessage(errorMessage, errorMessageArguments));
            }
        }

        /// <summary>
        /// Asserts that two objects are not the same.
        /// </summary>
        /// <typeparam name="T">Type of the argument</typeparam>
        /// <param name="expected">Expected object reference.</param>
        /// <param name="actual">Actual object reference.</param>
        /// <param name="errorMessage">Error message when two objects are the same.</param>
        /// <param name="errorMessageArguments">The error message arguments.</param>
        public void AreNotSame<T>(T expected, T actual, string errorMessage, params object[] errorMessageArguments)
            where T : class
        {
            if (ReferenceEquals(expected, actual))
            {
                this.OnDataComparisonFailure(expected, actual, FormatErrorMessage(errorMessage, errorMessageArguments));
            }
        }

        /// <summary>
        /// Asserts that two values of the same type are equal.
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        /// <param name="errorMessage">Error message when two values are different.</param>
        /// <param name="errorMessageArguments">The error message arguments.</param>
        public void AreEqual<T>(T expected, T actual, string errorMessage, params object[] errorMessageArguments)
        {
            this.AreEqual<T>(expected, actual, EqualityComparer<T>.Default, errorMessage, errorMessageArguments);
        }

        /// <summary>
        /// Asserts that two values of the same type are equal.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        /// <param name="errorMessage">Error message when two values are different.</param>
        /// <param name="errorMessageArguments">The error message arguments.</param>
        public void AreEqual(object expected, object actual, string errorMessage, params object[] errorMessageArguments)
        {
            if (!Equals(expected, actual, ValueComparer.Instance))
            {
                this.OnDataComparisonFailure(expected, actual, FormatErrorMessage(errorMessage, errorMessageArguments));
            }
        }

        /// <summary>
        /// Asserts that two values are equal using specified comparer.
        /// </summary>
        /// <typeparam name="T">The type of the values</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="comparer">Equality comparer to use when comparing two values.</param>
        /// <param name="errorMessage">The error message when two values are different.</param>
        /// <param name="errorMessageArguments">The error message arguments.</param>
        public void AreEqual<T>(T expected, T actual, IEqualityComparer<T> comparer, string errorMessage, params object[] errorMessageArguments)
        {
            ExceptionUtilities.CheckArgumentNotNull(comparer, "comparer");

            if (!Equals<T>(expected, actual, comparer))
            {
                this.OnDataComparisonFailure(expected, actual, FormatErrorMessage(errorMessage, errorMessageArguments));
            }
        }

        /// <summary>
        /// Asserts that two values of the same type are not equal.
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        /// <param name="errorMessage">Error message when to values are equal.</param>
        /// <param name="errorMessageArguments">The error message arguments.</param>
        public void AreNotEqual<T>(T expected, T actual, string errorMessage, params object[] errorMessageArguments)
        {
            if (Equals(expected, actual, EqualityComparer<T>.Default))
            {
                this.OnDataComparisonFailure(expected, actual, FormatErrorMessage(errorMessage, errorMessageArguments));
            }
        }

        /// <summary>
        /// Asserts that two values of the same type are not equal.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        /// <param name="errorMessage">Error message when to values are equal.</param>
        /// <param name="errorMessageArguments">The error message arguments.</param>
        public void AreNotEqual(object expected, object actual, string errorMessage, params object[] errorMessageArguments)
        {
            if (Equals(expected, actual, ValueComparer.Instance))
            {
                this.OnDataComparisonFailure(expected, actual, FormatErrorMessage(errorMessage, errorMessageArguments));
            }
        }

        /// <summary>
        /// Asserts that the given boolean value is true.
        /// </summary>
        /// <param name="condition">Boolean value.</param>
        /// <param name="errorMessage">Error message when the value is not true.</param>
        /// <param name="errorMessageArguments">The error message arguments.</param>
        public void IsTrue(bool condition, string errorMessage, params object[] errorMessageArguments)
        {
            if (condition != true)
            {
                this.OnAssertionFailure(FormatErrorMessage(errorMessage, errorMessageArguments));
            }
        }

        /// <summary>
        /// Asserts that the given boolean value is false.
        /// </summary>
        /// <param name="condition">Boolean value.</param>
        /// <param name="errorMessage">Error message when the value is not false.</param>
        /// <param name="errorMessageArguments">The error message arguments.</param>
        public void IsFalse(bool condition, string errorMessage, params object[] errorMessageArguments)
        {
            if (condition)
            {
                this.OnAssertionFailure(FormatErrorMessage(errorMessage, errorMessageArguments));
            }
        }

        /// <summary>
        /// Asserts that the given reference is null.
        /// </summary>
        /// <param name="value">Reference value.</param>
        /// <param name="errorMessage">Error message when the reference is not null.</param>
        /// <param name="errorMessageArguments">The error message arguments.</param>
        public void IsNull(object value, string errorMessage, params object[] errorMessageArguments)
        {
            if (value != null)
            {
                this.OnAssertionFailure(FormatErrorMessage(errorMessage, errorMessageArguments));
            }
        }

        /// <summary>
        /// Asserts that the given reference is not null.
        /// </summary>
        /// <param name="value">Reference value.</param>
        /// <param name="errorMessage">Error message when the reference is null.</param>
        /// <param name="errorMessageArguments">The error message arguments.</param>
        public void IsNotNull(object value, string errorMessage, params object[] errorMessageArguments)
        {
            if (value == null)
            {
                this.OnAssertionFailure(FormatErrorMessage(errorMessage, errorMessageArguments));
            }
        }

        /// <summary>
        /// Emits a warning message.
        /// </summary>
        /// <param name="warningMessage">Warning message to be emitted.</param>
        /// <param name="warningMessageArguments">The error message arguments.</param>
        public void Warn(string warningMessage, params object[] warningMessageArguments)
        {
            this.OnWarning(FormatErrorMessage(warningMessage, warningMessageArguments));
        }

        /// <summary>
        /// Emits an error message and terminates test.
        /// </summary>
        /// <param name="errorMessage">Error message to be emitted.</param>
        /// <param name="errorMessageArguments">The error message arguments.</param>
        public void Fail(string errorMessage, params object[] errorMessageArguments)
        {
            this.OnAssertionFailure(FormatErrorMessage(errorMessage, errorMessageArguments));
        }

        /// <summary>
        /// Emits a message and skips the test.
        /// </summary>
        /// <param name="skipMessage">Message to be emitted.</param>
        /// <param name="skipMessageArguments">The error message arguments.</param>
        public void Skip(string skipMessage, params object[] skipMessageArguments)
        {
            this.OnSkipped(FormatErrorMessage(skipMessage, skipMessageArguments));
        }

        /// <summary>
        /// Emits a message and skips the test if the given condition is false
        /// </summary>
        /// <param name="condition">the condition</param>
        /// <param name="skipMessage">Message to be emitted if condition is false.</param>
        /// <param name="skipMessageArguments">The error message arguments.</param>
        public void SkipIfFalse(bool condition, string skipMessage, params object[] skipMessageArguments)
        {
            if (!condition)
            {
                this.OnSkipped(FormatErrorMessage(skipMessage, skipMessageArguments));
            }
        }

        /// <summary>
        /// Asserts that the given action throws the specified exception type.
        /// </summary>
        /// <typeparam name="TException">Exception type.</typeparam>
        /// <param name="action">Action to try.</param>
        /// <param name="errorMessage">Error message when the action throws a different type of exception.</param>
        /// <param name="errorMessageArguments">The error message arguments.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exceptions here.")]
        public void ThrowsException<TException>(Action action, string errorMessage, params object[] errorMessageArguments)
            where TException : Exception
        {
            bool correctFailure;
            bool gotException;

            try
            {
                action();
                correctFailure = false;
                gotException = false;
            }
            catch (Exception ex)
            {
                gotException = true;
                if (ex is TException)
                {
                    correctFailure = true;
                }
                else
                {
                    correctFailure = false;
                }
            }

            if (!gotException)
            {
                this.Fail("Expected " + typeof(TException).FullName + " but it was not thrown.");
                return;
            }

            this.IsTrue(correctFailure, FormatErrorMessage(errorMessage, errorMessageArguments));
        }

        /// <summary>
        /// Invoked whenever there's an assertion failure.
        /// </summary>
        /// <param name="text">Assertion failure text.</param>
        protected abstract void OnAssertionFailure(string text);

        /// <summary>
        /// Invoked whenever there's data comparison failure.
        /// </summary>
        /// <param name="expected">Expected value</param>
        /// <param name="actual">Actual value</param>
        /// <param name="text">Error text</param>
        protected abstract void OnDataComparisonFailure(object expected, object actual, string text);

        /// <summary>
        /// Invoked whenever <see cref="Skip"/> method is called.
        /// </summary>
        /// <param name="text">Skip message</param>
        protected abstract void OnSkipped(string text);

        /// <summary>
        /// Invoked whenever <see cref="Warn"/> method is called.
        /// </summary>
        /// <param name="text">Warning text.</param>
        protected abstract void OnWarning(string text);

        private static bool Equals<T>(T expected, T actual, IEqualityComparer<T> comparer)
        {
            return comparer.Equals(expected, actual);
        }

        private static string FormatErrorMessage(string errorMessage, params object[] errorMessageArguments)
        {
            string text = errorMessage;
            if (errorMessageArguments != null && errorMessageArguments.Length > 0)
            {
                text = string.Format(CultureInfo.InvariantCulture, errorMessage, errorMessageArguments);
            }

            return text;
        }
    }
}