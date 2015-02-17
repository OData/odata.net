//---------------------------------------------------------------------
// <copyright file="ExceptionUtilities.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData.Service;

    /// <summary>
    /// Common exception utilities. 
    /// </summary>
    internal static class ExceptionUtilities
    {
        /// <summary>
        /// Throws ArgumentNullException if specified argument is null.
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
        /// Throws InvalidOperationException if specified object is null.
        /// </summary>
        /// <param name="value">The object to check for null.</param>
        /// <param name="exceptionMessageFormatText">The exception message.</param>
        /// <param name="messageArguments">The format arguments (if any) for the exception message.</param>
        public static void CheckObjectNotNull(object value, string exceptionMessageFormatText, params object[] messageArguments)
        {
            ExceptionUtilities.Assert(exceptionMessageFormatText != null, "message cannnot be null");
            ExceptionUtilities.Assert(messageArguments != null, "messageArguments cannnot be null");

            if (value == null)
            {
                 throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, exceptionMessageFormatText, messageArguments));
            }
        }
        
        /// <summary>
        /// Throws ArgumentException if the given collection is null or empty.
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
        /// Throws a data service exception with the given status code and message if the condition is false
        /// </summary>
        /// <param name="condition">The condition to throw on if false</param>
        /// <param name="statusCode">The status code for the exception</param>
        /// <param name="errorMessage">The message for the exception</param>
        /// <param name="messageArguments">The arguments for the exception message</param>
        public static void ThrowDataServiceExceptionIfFalse(bool condition, int statusCode, string errorMessage, params object[] messageArguments)
        {
            if (!condition)
            {
                throw new DataServiceException(statusCode, string.Format(CultureInfo.InvariantCulture, errorMessage, messageArguments));
            }
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
