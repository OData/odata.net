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

namespace System.Data.Services
{
    using System;

    /// <summary>
    /// Strongly-typed and parameterized exception factory.
    /// </summary>
    internal static partial class Error
    {
        /// <summary>
        /// create and trace a HttpHeaderFailure
        /// </summary>
        /// <param name="errorCode">error code</param>
        /// <param name="message">error message</param>
        /// <returns>DataServiceException</returns>
        internal static DataServiceException HttpHeaderFailure(int errorCode, string message)
        {
            return Trace(new DataServiceException(errorCode, message));
        }

        /// <summary>
        /// Trace the exception
        /// </summary>
        /// <typeparam name="T">type of the exception</typeparam>
        /// <param name="exception">exception object to trace</param>
        /// <returns>the exception parameter</returns>
        private static T Trace<T>(T exception) where T : Exception
        {
            return exception;
        }
    }
}
