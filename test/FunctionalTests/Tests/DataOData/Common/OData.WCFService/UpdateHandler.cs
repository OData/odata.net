//---------------------------------------------------------------------
// <copyright file="UpdateHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.WCFService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData;

    /// <summary>
    /// Class for handling requests to update entry & property values.
    /// </summary>
    public class UpdateHandler : RequestHandler
    {
        /// <summary>
        /// Parses a request to update an entry and makes the corresponding change to the 
        /// data store.
        /// </summary>
        /// <param name="messageBody">Message body containing the changes to make to the entry.</param>
        /// <returns>Stream containg the updated entry if successful, otherwise an error.</returns>
        public Stream ProcessEntryUpdateRequest(Stream messageBody)
        {
            object result;
            IEdmEntitySet targetEntitySet;

            try
            {
                var queryContext = this.GetDefaultQueryContext();
                targetEntitySet = queryContext.ResolveEntitySet();

                var message = this.GetIncomingRequestMessage(messageBody);
                result = ProcessPutBody(message, targetEntitySet, queryContext.ResolveKeyValues());
            }
            catch (Exception error)
            {
                return this.WriteErrorResponse(400, error);
            }

            return this.WriteResponse(
               200,
               (writer, writerSettings, responseMessage) =>
               {
                   ODataVersion targetVersion = writerSettings.Version.GetValueOrDefault();
                   responseMessage.SetHeader("Location", ODataObjectModelConverter.BuildEntryUri(result, targetEntitySet, targetVersion).OriginalString);
                   ResponseWriter.WriteEntry(writer.CreateODataResourceWriter(targetEntitySet), result, targetEntitySet, this.Model, targetVersion, Enumerable.Empty<string>());
               });
        }

        /// <summary>
        /// Parses a request to update a property and makes the corresponding change to the 
        /// data store.
        /// </summary>
        /// <param name="messageBody">Message body containing the new property value.</param>
        /// <param name="propertyName">The name of the property to modify.</param>
        /// <returns>Stream containg the updated entry if successful, otherwise an error.</returns>
        public Stream ProcessTopLevelPropertyRequest(Stream messageBody, string propertyName)
        {
            object result;
            IEdmEntitySet targetEntitySet;

            try
            {
                // ODL Query fails to parse EntitySet(key)/Property so we remove the property name and parse the rest of the URI.
                var requestUriWithoutPropertyName = new Uri(this.IncomingRequestUri.OriginalString.Replace(propertyName, string.Empty));
                QueryContext queryContext = QueryContext.ParseUri(requestUriWithoutPropertyName, this.Model);

                targetEntitySet = queryContext.ResolveEntitySet();
                var keyValues = queryContext.ResolveKeyValues();

                var message = this.GetIncomingRequestMessage(messageBody);
                using (var reader = new ODataMessageReader(message, this.GetDefaultReaderSettings(), this.Model))
                {
                    ODataProperty property = reader.ReadProperty();
                    this.DataContext.UpdateItem(targetEntitySet, keyValues, property.Name, property.Value);
                }

                result = this.DataContext.GetItem(targetEntitySet, keyValues);
            }
            catch (Exception error)
            {
                return this.WriteErrorResponse(400, error);
            }

            return this.WriteResponse(
               200,
               (writer, writerSettings, responseMessage) =>
               {
                   var targetVersion = writerSettings.Version.GetValueOrDefault();
                   responseMessage.SetHeader("Location", ODataObjectModelConverter.BuildEntryUri(result, targetEntitySet, targetVersion).OriginalString);
                   ResponseWriter.WriteEntry(writer.CreateODataResourceWriter(targetEntitySet), result, targetEntitySet, this.Model, targetVersion, Enumerable.Empty<string>());
               });
        }

        private object ProcessPutBody(IncomingRequestMessage message, IEdmEntitySet entitySet, IDictionary<string, object> entityKeys) 
        {
            using (var messageReader = new ODataMessageReader(message, this.GetDefaultReaderSettings(), this.Model))
            {
                var entryReader = messageReader.CreateODataResourceReader(entitySet.EntityType());

                while (entryReader.Read())
                {
                    switch(entryReader.State)
                    {
                        case ODataReaderState.ResourceEnd:
                            var entry = (ODataResource)entryReader.Item;
                            foreach (var property in entry.Properties)
                            {
                                this.DataContext.UpdateItem(entitySet, entityKeys, property.Name, property.Value);
                            }

                            break;
                    }
                }
            }

            return this.DataContext.GetItem(entitySet, entityKeys);
        }
    }
}