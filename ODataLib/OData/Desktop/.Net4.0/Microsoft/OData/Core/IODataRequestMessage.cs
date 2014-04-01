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
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    #endregion Namespaces

    /// <summary>
    /// Interface for synchronous OData request messages.
    /// </summary>
    public interface IODataRequestMessage
    {
        /// <summary>Gets an enumerable over all the headers for this message.</summary>
        /// <returns>An enumerable over all the headers for this message.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Users will never have to instantiate these; the rule does not apply.")]
        IEnumerable<KeyValuePair<string, string>> Headers
        {
            // TODO: do we want to impose a certain order of the headers?
            get;
        }

        /// <summary>Gets or sets the request URL for this request message.</summary>
        /// <returns>The request URL for this request message.</returns>
        Uri Url
        {
            get;
            set;
        }

        /// <summary>Gets or sets the HTTP method used for this request message.</summary>
        /// <returns>The HTTP method used for this request message.</returns>
        string Method
        {
            get;
            set;
        }

        /// <summary>Returns a value of an HTTP header.</summary>
        /// <returns>The value of the HTTP header, or null if no such header was present on the message.</returns>
        /// <param name="headerName">The name of the header to get.</param>
        string GetHeader(string headerName);

        /// <summary>Sets the value of an HTTP header.</summary>
        /// <param name="headerName">The name of the header to set.</param>
        /// <param name="headerValue">The value of the HTTP header or 'null' if the header should be removed.</param>
        void SetHeader(string headerName, string headerValue);

        /// <summary>Gets the stream backing for this message.</summary>
        /// <returns>The stream backing for this message.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is intentionally a method.")]
        Stream GetStream();
    }
}
