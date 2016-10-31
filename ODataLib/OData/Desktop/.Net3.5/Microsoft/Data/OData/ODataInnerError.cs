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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Class representing implementation specific debugging information to help determine the cause of the error.
    /// </summary>
    [DebuggerDisplay("{Message}")]
#if !WINDOWS_PHONE && !SILVERLIGHT && !PORTABLELIB
    [Serializable]
#endif
    public sealed class ODataInnerError
    {
        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.Data.OData.ODataInnerError" /> class with default values.</summary>
        public ODataInnerError()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.Data.OData.ODataInnerError" /> class with exception object.</summary>
        /// <param name="exception">The <see cref="T:System.Exception" /> used to create the inner error.</param>
        public ODataInnerError(Exception exception)
        {
            ExceptionUtils.CheckArgumentNotNull(exception, "exception");

            this.Message = exception.Message ?? string.Empty;
            this.TypeName = exception.GetType().FullName;
            this.StackTrace = exception.StackTrace;

            if (exception.InnerException != null)
            {
                this.InnerError = new ODataInnerError(exception.InnerException);
            }
        }

        /// <summary>Gets or sets the error message.</summary>
        /// <returns>The error message.</returns>
        public string Message
        {
            get;
            set;
        }

        /// <summary>Gets or sets the type name of this error, for example, the type name of an exception.</summary>
        /// <returns>The type name of this error.</returns>
        public string TypeName
        {
            get;
            set;
        }

        /// <summary>Gets or sets the stack trace for this error.</summary>
        /// <returns>The stack trace for this error.</returns>
        public string StackTrace
        {
            get;
            set;
        }

        /// <summary>Gets or sets the nested implementation specific debugging information. </summary>
        /// <returns>The nested implementation specific debugging information.</returns>
        public ODataInnerError InnerError
        {
            get;
            set;
        }
    }
}
