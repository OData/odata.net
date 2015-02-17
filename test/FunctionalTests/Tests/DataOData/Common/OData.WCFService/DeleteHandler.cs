//---------------------------------------------------------------------
// <copyright file="DeleteHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.WCFService
{
    using System;
    using System.IO;

    /// <summary>
    /// Class for handling delete requests.
    /// </summary>
    public class DeleteHandler : RequestHandler
    {
        /// <summary>
        /// Parses the request and removes the specified item from the data store.
        /// </summary>
        /// <returns>An empty stream if successful, otherwise an error.</returns>
        public Stream ProcessDeleteRequest()
        {
            try
            {
                var queryContext = this.GetDefaultQueryContext();
                var targetEntitySet = queryContext.ResolveEntitySet();
                var keyValues = queryContext.ResolveKeyValues();

                this.DataContext.DeleteItem(targetEntitySet, keyValues);
                
                return new MemoryStream();
            }
            catch (Exception error)
            {
                return this.WriteErrorResponse(400, error);
            }
        }
    }
}