//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#if ODATALIB_QUERY
namespace Microsoft.Data.Experimental.OData
#else
namespace Microsoft.Data.OData
#endif
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Data.Edm;

    #endregion Namespaces

    /// <summary>
    /// Class with utility methods to work with exceptions
    /// </summary>
    internal static class ExceptionUtils
    {
        /// <summary>Type of OutOfMemoryException.</summary>
        private static readonly Type OutOfMemoryType = typeof(System.OutOfMemoryException);

#if !PORTABLELIB
        /// <summary>Type of StackOverflowException.</summary>
        private static readonly Type StackOverflowType = typeof(System.StackOverflowException);

        /// <summary>Type of ThreadAbortException.</summary>
        private static readonly Type ThreadAbortType = typeof(System.Threading.ThreadAbortException);
#endif

        /// <summary>
        /// Determines whether the specified exception can be caught and 
        /// handled, or whether it should be allowed to continue unwinding.
        /// </summary>
        /// <param name="e"><see cref="Exception"/> to test.</param>
        /// <returns>
        /// true if the specified exception can be caught and handled; 
        /// false otherwise.
        /// </returns>
        internal static bool IsCatchableExceptionType(Exception e)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(e != null, "Unexpected null exception!");

            Type type = e.GetType();

            // a 'catchable' exception is defined by what it is not.
            return 
#if !PORTABLELIB
                    type != ThreadAbortType &&
                    type != StackOverflowType &&
#endif
                    type != OutOfMemoryType;
        }

        /// <summary>
        /// Checks the argument value for null and throws <see cref="ArgumentNullException"/> if it is null.
        /// </summary>
        /// <typeparam name="T">Type of the argument, used to force usage only for reference types.</typeparam>
        /// <param name="value">Argument whose value needs to be checked.</param>
        /// <param name="parameterName">Name of the argument, used for exception message.</param>
        internal static void CheckArgumentNotNull<T>([ValidatedNotNull] T value, string parameterName) where T : class
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "!string.IsNullOrEmpty(parameterName)");

            if (value == null)
            {
                throw Error.ArgumentNull(parameterName);
            }
        }

#if !ODATALIB_QUERY
        /// <summary>
        /// Checks the argument string value empty string and throws <see cref="ArgumentNullException"/> if it is empty. The value can be null though.
        /// </summary>
        /// <param name="value">Argument whose value needs to be checked.</param>
        /// <param name="parameterName">Name of the argument, used for exception message.</param>
        internal static void CheckArgumentStringNotEmpty(string value, string parameterName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "!string.IsNullOrEmpty(parameterName)");

            if (value != null && value.Length == 0)
            {
                throw new ArgumentException(Strings.ExceptionUtils_ArgumentStringEmpty, parameterName);
            }
        }
#endif

        /// <summary>
        /// Checks the argument string value for null or empty string and throws <see cref="ArgumentNullException"/> if it is null or empty.
        /// </summary>
        /// <param name="value">Argument whose value needs to be checked.</param>
        /// <param name="parameterName">Name of the argument, used for exception message.</param>
        internal static void CheckArgumentStringNotNullOrEmpty([ValidatedNotNull] string value, string parameterName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "!string.IsNullOrEmpty(parameterName)");

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(parameterName, Strings.ExceptionUtils_ArgumentStringNullOrEmpty);
            }
        }

        /// <summary>
        /// Checks the <paramref name="value"/> for not being negative and throws <see cref="ArgumentOutOfRangeException"/> if it is negative.
        /// </summary>
        /// <param name="value">Argument whose value needs to be checked.</param>
        /// <param name="parameterName">Name of the argument, used for exception message.</param>
        internal static void CheckIntegerNotNegative(int value, string parameterName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "!string.IsNullOrEmpty(parameterName)");

            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, Strings.ExceptionUtils_CheckIntegerNotNegative(value));
            }
        }

        /// <summary>
        /// Checks the <paramref name="value"/> for being greater than zero and throws <see cref="ArgumentOutOfRangeException"/> if it is not positive.
        /// </summary>
        /// <param name="value">Argument whose value needs to be checked.</param>
        /// <param name="parameterName">Name of the argument, used for exception message.</param>
        internal static void CheckIntegerPositive(int value, string parameterName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "!string.IsNullOrEmpty(parameterName)");

            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, Strings.ExceptionUtils_CheckIntegerPositive(value));
            }
        }

        /// <summary>
        /// Checks the <paramref name="value"/> for being greater than zero and throws <see cref="ArgumentOutOfRangeException"/> if it is not positive.
        /// </summary>
        /// <param name="value">Argument whose value needs to be checked.</param>
        /// <param name="parameterName">Name of the argument, used for exception message.</param>
        internal static void CheckLongPositive(long value, string parameterName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "!string.IsNullOrEmpty(parameterName)");

            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, Strings.ExceptionUtils_CheckLongPositive(value));
            }
        }

#if !ODATALIB_QUERY
        /// <summary>
        /// Checks the <paramref name="value"/> for not being empty.
        /// </summary>
        /// <param name="value">Argument whose value needs to be checked.</param>
        /// <param name="parameterName">Name of the argument, used for exception message.</param>
        /// <typeparam name="T">Type of the collection. It does not matter.</typeparam>
        internal static void CheckArgumentCollectionNotNullOrEmpty<T>(ICollection<T> value, string parameterName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "!string.IsNullOrEmpty(parameterName)");

            if (value == null)
            {
                throw Error.ArgumentNull(parameterName);
            }
            else if (value.Count == 0)
            {
                // TODO: STRINGS The string is fine; just rename it to just ArgumentEmpty
                throw new ArgumentException(Strings.ExceptionUtils_ArgumentStringEmpty, parameterName);
            }
        }
#endif

        /// <summary>
        /// A workaround to a problem with FxCop which does not recognize the CheckArgumentNotNull method
        /// as the one which validates the argument is not null.
        /// </summary>
        /// <remarks>This has been suggested as a workaround in msdn forums by the VS team. Note that even though this is production code
        /// the attribute has no effect on anything else.</remarks>
        private sealed class ValidatedNotNullAttribute : Attribute
        {
        }
    }
}
