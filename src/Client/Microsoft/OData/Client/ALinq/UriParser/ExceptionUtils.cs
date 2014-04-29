//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

#if ASTORIA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.Core
#endif
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;

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
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "!string.IsNullOrEmpty(parameterName)");

            if (value == null)
            {
#if !ASTORIA_CLIENT
                throw Error.ArgumentNull(parameterName);
#endif 
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
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "!string.IsNullOrEmpty(parameterName)");

            if (value != null && value.Length == 0)
            {
#if !ASTORIA_CLIENT
                throw new ArgumentException(Strings.ExceptionUtils_ArgumentStringEmpty, parameterName);
#endif
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
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "!string.IsNullOrEmpty(parameterName)");

            if (string.IsNullOrEmpty(value))
            {
#if !ASTORIA_CLIENT
                throw new ArgumentNullException(parameterName, Strings.ExceptionUtils_ArgumentStringNullOrEmpty);
#endif
            }
        }

        /// <summary>
        /// Checks the <paramref name="value"/> for not being negative and throws <see cref="ArgumentOutOfRangeException"/> if it is negative.
        /// </summary>
        /// <param name="value">Argument whose value needs to be checked.</param>
        /// <param name="parameterName">Name of the argument, used for exception message.</param>
        internal static void CheckIntegerNotNegative(int value, string parameterName)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "!string.IsNullOrEmpty(parameterName)");

            if (value < 0)
            {
#if !ASTORIA_CLIENT
                throw new ArgumentOutOfRangeException(parameterName, Strings.ExceptionUtils_CheckIntegerNotNegative(value));
#endif
            }
        }

        /// <summary>
        /// Checks the <paramref name="value"/> for being greater than zero and throws <see cref="ArgumentOutOfRangeException"/> if it is not positive.
        /// </summary>
        /// <param name="value">Argument whose value needs to be checked.</param>
        /// <param name="parameterName">Name of the argument, used for exception message.</param>
        internal static void CheckIntegerPositive(int value, string parameterName)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "!string.IsNullOrEmpty(parameterName)");

            if (value <= 0)
            {
#if !ASTORIA_CLIENT
                throw new ArgumentOutOfRangeException(parameterName, Strings.ExceptionUtils_CheckIntegerPositive(value));
#endif
            }
        }

        /// <summary>
        /// Checks the <paramref name="value"/> for being greater than zero and throws <see cref="ArgumentOutOfRangeException"/> if it is not positive.
        /// </summary>
        /// <param name="value">Argument whose value needs to be checked.</param>
        /// <param name="parameterName">Name of the argument, used for exception message.</param>
        internal static void CheckLongPositive(long value, string parameterName)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "!string.IsNullOrEmpty(parameterName)");

            if (value <= 0)
            {
#if !ASTORIA_CLIENT
                throw new ArgumentOutOfRangeException(parameterName, Strings.ExceptionUtils_CheckLongPositive(value));
#endif
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
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "!string.IsNullOrEmpty(parameterName)");

            if (value == null)
            {
#if !ASTORIA_CLIENT
                throw Error.ArgumentNull(parameterName);
#endif
            }
            else if (value.Count == 0)
            {
#if !ASTORIA_CLIENT
                // TODO: STRINGS The string is fine; just rename it to just ArgumentEmpty
                throw new ArgumentException(Strings.ExceptionUtils_ArgumentStringEmpty, parameterName);
#endif
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
