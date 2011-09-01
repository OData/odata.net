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
    using System.Runtime.Serialization;
    #endregion Namespaces

    /// <summary>
    /// Class representing implementation specific debugging information to help determine the cause of the error.
    /// </summary>
    [DebuggerDisplay("{Message}")]
#if !WINDOWS_PHONE && !SILVERLIGHT
    [Serializable]
#endif
    public sealed class ODataInnerError : ODataItem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ODataInnerError()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> to create the inner error for.</param>
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

        /// <summary>The error message.</summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>The type name of this error, e.g., the type name of an exception.</summary>
        public string TypeName
        {
            get;
            set;
        }

        /// <summary>The stack trace for this error.</summary>
        public string StackTrace
        {
            get;
            set;
        }

        /// <summary>
        /// Nested implementation specific debugging information.
        /// </summary>
        public ODataInnerError InnerError
        {
            get;
            set;
        }
    }
}
