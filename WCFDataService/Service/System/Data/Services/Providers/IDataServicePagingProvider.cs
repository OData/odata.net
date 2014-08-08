//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace System.Data.Services.Providers
{
    #region Namespaces
    using System.Collections;
    using System.Linq;
    #endregion

    /// <summary>
    /// When exposed by a provider, this interface is used to provide custom paging for the clients.
    /// </summary>
    public interface IDataServicePagingProvider
    {
        /// <summary>Returns the next-page token to put in the $skiptoken query option.</summary>
        /// <returns>The next-page token as a collection of primitive types.</returns>
        /// <param name="enumerator">Enumerator for which the continuation token is being requested.</param>
        object[] GetContinuationToken(IEnumerator enumerator);

        /// <summary>Gets the next-page token from the $skiptoken query option in the request URI.</summary>
        /// <param name="query">Query for which the continuation token is being provided.</param>
        /// <param name="resourceType">Resource type of the result on which the $skip token is to be applied.</param>
        /// <param name="continuationToken">Continuation token parsed into primitive type values.</param>
        void SetContinuationToken(IQueryable query, ResourceType resourceType, object[] continuationToken);
    }
}
