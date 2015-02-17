//---------------------------------------------------------------------
// <copyright file="IDataServicePagingProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
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
