//   OData .NET Libraries ver. 6.8.1
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

namespace Microsoft.Spatial
{
    using System;

    /// <summary>
    /// Util class
    /// </summary>
    internal class Util
    {
#if !PORTABLELIB
        /// <summary>StackOverFlow exception type</summary>
        private static readonly Type StackOverflowType = typeof(System.StackOverflowException);

        /// <summary>ThreadAbortException exception type</summary>
        private static readonly Type ThreadAbortType = typeof(System.Threading.ThreadAbortException);

        /// <summary>AccessViolationException exception type</summary>
        private static readonly Type AccessViolationType = typeof(System.AccessViolationException);
#endif

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
#if !PORTABLELIB
                    (type != StackOverflowType) &&
                    (type != ThreadAbortType) &&
                    (type != AccessViolationType) &&
#endif
                    (type != NullReferenceType) &&
                    !SecurityType.IsAssignableFrom(type));
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
