﻿//---------------------------------------------------------------------
// <copyright file="ExceptionUtilities.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Utils.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Common exception utilities. 
    /// </summary>
    public static class ExceptionUtilities
    {
        /// <summary>
        /// Gets the error message thrown for types which implement IEnumerable only for cleaner construction.
        /// </summary>
        public static string EnumerableNotImplementedExceptionMessage
        {
            get { return "IEnumerable is only implemented for cleaner construction."; }
        }

        /// <summary>
        /// Throws TaupoArgumentNullException if specified argument is null.
        /// </summary>
        /// <param name="argument">Argument to check for null.</param>
        /// <param name="argumentName">Argument name.</param>
        public static void CheckArgumentNotNull(object argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        /// Throws TaupoArgumentException if the range is invalid.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be comparable.</typeparam>
        /// <param name="leftValue">The left value.</param>
        /// <param name="leftParameterName">Name of the left parameter.</param>
        /// <param name="rightValue">The right value.</param>
        /// <param name="rightParameterName">Name of the right parameter.</param>
        public static void CheckValidRange<TValue>(TValue leftValue, string leftParameterName, TValue rightValue, string rightParameterName)
            where TValue : struct, IComparable<TValue>
        {
            if (leftValue.CompareTo(rightValue) > 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid range specified - '{0}' must be greater than or equal to '{1}'.", rightParameterName, leftParameterName));
            }
        }

        /// <summary>
        /// Throws TaupoArgumentException if string argument is empty and TaupoArgumentNullException if string argument is null.
        /// </summary>
        /// <param name="argument">String argument for check.</param>
        /// <param name="argumentName">Argument name.</param>
        public static void CheckStringArgumentIsNotNullOrEmpty(string argument, string argumentName)
        {
            CheckArgumentNotNull(argument, argumentName);

            if (string.IsNullOrEmpty(argument))
            {
                string message = string.Format(CultureInfo.InvariantCulture, "Argument '{0}' cannot be empty.", argumentName);
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// Throws TaupoInvalidOperationException if specified string is null or empty
        /// </summary>
        /// <param name="value">The string to check for null/empty</param>
        /// <param name="exceptionMessageFormatText">The exception message.</param>
        /// <param name="messageArguments">The format arguments (if any) for the exception message.</param>
        public static void CheckStringNotNullOrEmpty(string value, string exceptionMessageFormatText, params object[] messageArguments)
        {
            Assert(exceptionMessageFormatText != null, "message cannnot be null");
            Assert(messageArguments != null, "messageArguments cannnot be null");

            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, exceptionMessageFormatText, messageArguments));
            }
        }

        /// <summary>
        /// Throws TaupoInvalidOperationException if specified object is null.
        /// </summary>
        /// <param name="value">The object to check for null.</param>
        /// <param name="exceptionMessageFormatText">The exception message.</param>
        /// <param name="messageArguments">The format arguments (if any) for the exception message.</param>
        public static void CheckObjectNotNull(object value, string exceptionMessageFormatText, params object[] messageArguments)
        {
            Assert(exceptionMessageFormatText != null, "message cannnot be null");
            Assert(messageArguments != null, "messageArguments cannnot be null");

            if (value == null)
            {
                var message = exceptionMessageFormatText;
                if (messageArguments.Length > 0)
                {
                    message = string.Format(CultureInfo.InvariantCulture, exceptionMessageFormatText, messageArguments);
                }

                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Throws TaupoArgumentException if the given collection is null or empty.
        /// </summary>
        /// <typeparam name="TElement">Type of the element type.</typeparam>
        /// <param name="argument">The argument.</param>
        /// <param name="argumentName">Name of the argument.</param>
        public static void CheckCollectionNotEmpty<TElement>(IEnumerable<TElement> argument, string argumentName)
        {
            CheckArgumentNotNull(argument, argumentName);

            if (!argument.Any())
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "Collection argument '{0}' must have at least one element.", argumentName));
            }
        }

        /// <summary>
        /// Throws TaupoArgumentException if the given collection is null or contains null elements.
        /// </summary>
        /// <typeparam name="TElement">Type of the element type.</typeparam>
        /// <param name="argument">The argument.</param>
        /// <param name="argumentName">Name of the argument.</param>
        public static void CheckCollectionDoesNotContainNulls<TElement>(IEnumerable<TElement> argument, string argumentName)
        {
            CheckArgumentNotNull(argument, argumentName);

            if (argument.Any(e => e == null))
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "Collection argument '{0}' cannot contain null elements.", argumentName));
            }
        }

        /// <summary>
        /// Throws TaupoNotSupportedException saying that IEnumerable is only implemented for cleaner construction.
        /// </summary>
        /// <returns>TaupoNotSupportedException with appropriate message</returns>
        public static NotSupportedException CreateIEnumerableNotImplementedException()
        {
            return new NotSupportedException(EnumerableNotImplementedExceptionMessage);
        }

        /// <summary>
        /// Determines whether the specified exception is catchable.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>
        /// A value <c>true</c> if the specified exception is catchable; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCatchable(Exception exception)
        {
#if !WIN8 && !NETCOREAPP1_0
            if (exception is ThreadAbortException)
            {
                return false;
            }
#endif

            return true;
        }

        /// <summary>
        /// Asserts the specified condition to be true and throws exception if it is not.
        /// </summary>
        /// <param name="condition">If set to <c>true</c>, the exception will not be thrown.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="messageArguments">Arguments for the error message.</param>
        public static void Assert(bool condition, string errorMessage, params object[] messageArguments)
        {
            if (!condition)
            {
                throw new InvalidOperationException(
                    "ASSERTION FAILED: " + string.Format(CultureInfo.InvariantCulture, errorMessage, messageArguments));
            }
        }
    }
}