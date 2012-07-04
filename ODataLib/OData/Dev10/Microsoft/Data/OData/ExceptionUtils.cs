//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Class with utility methods to work with exceptions
    /// </summary>
    internal static class ExceptionUtils
    {
        /// <summary>Type of OutOfMemoryException.</summary>
        private static readonly Type OutOfMemoryType = typeof(System.OutOfMemoryException);

        /// <summary>Type of StackOverflowException.</summary>
        private static readonly Type StackOverflowType = typeof(System.StackOverflowException);

        /// <summary>Type of ThreadAbortException.</summary>
        private static readonly Type ThreadAbortType = typeof(System.Threading.ThreadAbortException);

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
                    type != ThreadAbortType &&
                    type != StackOverflowType &&
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
