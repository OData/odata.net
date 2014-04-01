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
    using System.Diagnostics.CodeAnalysis;
    #endregion Namespaces

    /// <summary>
    /// Uri utility methods.
    /// </summary>
    internal static class UriUtilsCommon
    {
        /// <summary>
        /// Returns the unescaped string representation of the Uri; if the Uri is absolute returns the absolute Uri otherwise the original string.
        /// </summary>
        /// <param name="uri">The Uri to convert to a string.</param>
        /// <returns>For absolute Uris the string representation of the absolute Uri; otherwise the Uri's original string.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0010", Justification = "Usage of OriginalString is safe in this context")]
        internal static string UriToString(Uri uri)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(uri != null, "uri != null");

            return uri.IsAbsoluteUri ? uri.AbsoluteUri : uri.OriginalString;
        }
    }
}
