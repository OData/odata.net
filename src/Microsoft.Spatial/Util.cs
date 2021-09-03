//---------------------------------------------------------------------
// <copyright file="Util.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Util class
    /// </summary>
    internal class Util
    {
        /// <summary>OutOfMemoryException exception type</summary>
        private static readonly Type OutOfMemoryType = typeof(System.OutOfMemoryException);

        /// <summary>NullReferenceException exception type</summary>
        private static readonly Type NullReferenceType = typeof(System.NullReferenceException);

        /// <summary>SecurityException exception type</summary>
        private static readonly Type SecurityType = typeof(System.Security.SecurityException);

        /// <summary>
        /// Check if input is null, throw an ArgumentNullException if it is.
        /// </summary>
        /// <param name="arg">The input argument</param>
        /// <param name="errorMessage">The error to throw</param>
        [DebuggerStepThrough]
        internal static void CheckArgumentNull([ValidatedNotNull] object arg, string errorMessage)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(errorMessage);
            }
        }

        /// <summary>
        /// Determines if the exception is one of the prohibited types that should not be caught.
        /// </summary>
        /// <param name="e">The exception to be checked against the prohibited list.</param>
        /// <returns>True if the exception is ok to be caught, false otherwise.</returns>
        internal static bool IsCatchableExceptionType(Exception e)
        {
            // a 'catchable' exception is defined by what it is not.
            Type type = e.GetType();

            return ((type != OutOfMemoryType) &&
                    (type != NullReferenceType) &&
                    !SecurityType.IsAssignableFrom(type));
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
