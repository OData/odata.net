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

namespace System.Data.OData
{
    #region Namespaces.
    using System.Diagnostics;
    #endregion Namespaces.

    /// <summary>
    /// Class representing an error payload.
    /// </summary>
    [DebuggerDisplay("{errorCode}: {message}")]
#if INTERNAL_DROP
    internal sealed class ODataError : ODataItem
#else
    public sealed class ODataError : ODataItem
#endif
    {
        #region Public properties.

        /// <summary>Error code to be used in payloads.</summary>
        public string ErrorCode
        {
            get;
            set;
        }

        /// <summary>The error message.</summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>Language for the exception Message.</summary>
        public string MessageLanguage
        {
            get;
            set;
        }

        /// <summary>
        /// Implementation specific debugging information to help determine the cause of the error.
        /// </summary>
        public string InnerError
        {
            get;
            set;
        }

        #endregion Public properties.
    }
}
