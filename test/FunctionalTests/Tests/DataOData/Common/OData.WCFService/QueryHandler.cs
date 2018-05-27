//---------------------------------------------------------------------
// <copyright file="QueryHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.WCFService
{
    using System;
    using System.IO;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    /// <summary>
    /// Class for handling incoming client query requests.
    /// </summary>
    public class QueryHandler : RequestHandler
    {
        /// <summary>
        /// Parses incoming feed/entry/property queries, resolves against the data store and formulates the response.
        /// </summary>
        /// <returns>Stream containing the query results if successful, otherwise an error.</returns>
        public Stream ProcessGetQuery(string requestUri)
        {
            object queryResults = null;
            QueryContext queryContext;

            try
            {
                queryContext = this.GetDefaultQueryContext();
                queryResults = queryContext.ResolveQuery(this.Model, this.DataContext);
            }
            catch (Exception error)
            {
                return this.WriteErrorResponse(400, error);
            }

            return this.WriteResponse(200, (messageWriter, writerSettings, message) =>
            {
                IEdmEntitySet entitySet = queryContext.ResolveEntitySet();
                ODataPathSegment lastSegment = queryContext.QueryPath.LastSegment;
                var expandedProperties = Enumerable.Empty<string>();

                if (lastSegment is EntitySetSegment)
                {
                    ODataWriter resultWriter = messageWriter.CreateODataResourceSetWriter(entitySet);
                    ResponseWriter.WriteFeed(resultWriter, queryResults as IQueryable, entitySet, this.Model, writerSettings.Version.GetValueOrDefault(), expandedProperties);
                    resultWriter.Flush();
                }
                else if (lastSegment is KeySegment)
                {
                    ODataWriter resultWriter = messageWriter.CreateODataResourceWriter(entitySet);
                    ResponseWriter.WriteEntry(resultWriter, queryResults, entitySet, this.Model, writerSettings.Version.GetValueOrDefault(), expandedProperties);
                    resultWriter.Flush();
                }
                else if (lastSegment is PropertySegment)
                {
                    ODataProperty property = ODataObjectModelConverter.CreateODataProperty(queryResults, (lastSegment as PropertySegment).Property.Name);
                    messageWriter.WriteProperty(property);
                }
                else
                {
                    throw new ODataErrorException("Unsupported URI segment " + lastSegment.GetType());
                }
            });
        }
    }
}