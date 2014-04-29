//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Class representing implementation specific debugging information to help determine the cause of the error.
    /// </summary>
    [DebuggerDisplay("{Message}")]
#if ORCAS
    [Serializable]
#endif
    public sealed class ODataInnerError
    {
        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Core.ODataInnerError" /> class with default values.</summary>
        public ODataInnerError()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Core.ODataInnerError" /> class with exception object.</summary>
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
