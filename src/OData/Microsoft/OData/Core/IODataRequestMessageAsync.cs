//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

#if ODATALIB_ASYNC
namespace Microsoft.OData.Core
{
    #region Namespaces
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Threading.Tasks;
    #endregion Namespaces

    /// <summary>
    /// Interface for asynchronous OData request messages.
    /// </summary>
    public interface IODataRequestMessageAsync : IODataRequestMessage
    {
        /// <summary>Asynchronously get the stream backing for this message.</summary>
        /// <returns>The stream for this message.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is intentionally a method.")]
        Task<Stream> GetStreamAsync();
    }
}
#endif
