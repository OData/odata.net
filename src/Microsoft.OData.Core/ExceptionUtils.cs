//---------------------------------------------------------------------
// <copyright file="ExceptionUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData
#endif
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    #endregion Namespaces

    /// <summary>
    /// Class with utility methods to work with exceptions
    /// </summary>
    internal static class ExceptionUtils
    {
        /// <summary>Type of OutOfMemoryException.</summary>
        private static readonly Type OutOfMemoryType = typeof(System.OutOfMemoryException);

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
            return type != OutOfMemoryType;
        }

        /// <summary>
        /// Checks the argument value for null and throws <see cref="ArgumentNullException"/> if it is null.
        /// </summary>
        /// <typeparam name="T">Type of the argument, used to force usage only for reference types.</typeparam>
        /// <param name="value">Argument whose value needs to be checked.</param>
        /// <param name="parameterName">Name of the argument, used for exception message.</param>
        /// <returns>The value</returns>
        internal static T CheckArgumentNotNull<T>([ValidatedNotNull] T value, string parameterName) where T : class
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "!string.IsNullOrEmpty(parameterName)");

            if (value == null)
            {
#if !ODATA_CLIENT
                throw Error.ArgumentNull(parameterName);
#endif
            }

            return value;
        }

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
#if !ODATA_CLIENT
                throw new ArgumentException(Strings.ExceptionUtils_ArgumentStringEmpty, parameterName);
#endif
            }
        }

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
#if !ODATA_CLIENT
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
#if !ODATA_CLIENT
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
#if !ODATA_CLIENT
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
#if !ODATA_CLIENT
                throw new ArgumentOutOfRangeException(parameterName, Strings.ExceptionUtils_CheckLongPositive(value));
#endif
            }
        }

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
#if !ODATA_CLIENT
                throw Error.ArgumentNull(parameterName);
#endif
            }
            else if (value.Count == 0)
            {
#if !ODATA_CLIENT
                // TODO: STRINGS The string is fine; just rename it to just ArgumentEmpty
                throw new ArgumentException(Strings.ExceptionUtils_ArgumentStringEmpty, parameterName);
#endif
            }
        }

        /// <summary>
        /// A workaround to a problem with FxCop which does not recognize the CheckArgumentNotNull method
        /// as the one which validates the argument is not null.
        /// </summary>
        /// <remarks>This has been suggested as a workaround in msdn forums by the VS team. Note that even though this is production code
        /// the attribute has no effect on anything else.</remarks>
        [AttributeUsage(AttributeTargets.Parameter)]
        private sealed class ValidatedNotNullAttribute : Attribute
        {
        }
    }
}
