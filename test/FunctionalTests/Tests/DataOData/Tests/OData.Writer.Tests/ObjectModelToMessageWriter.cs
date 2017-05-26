//---------------------------------------------------------------------
// <copyright file="ObjectModelToMessageWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests
{
    using System;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;

    public class ObjectModelToMessageWriter
    {
        /// <summary>
        /// Public constructor
        /// </summary>
        public ObjectModelToMessageWriter()
        {
        }

        /// <summary>
        /// Write payload kind to message.
        /// </summary>
        /// <param name="messageWriter">Message writer to write payload to.</param>
        /// <param name="payloadKind">The kind of payload we are writing.</param>
        /// <param name="payload">The payload to write.</param>
        /// <param name="model">The model used for writing the payloads.</param>
        /// <param name="functionImport">Function import whose parameters are to be written when the payload kind is Parameters.</param>
        public virtual void WriteMessage(
            ODataMessageWriterTestWrapper messageWriter,
            ODataPayloadKind payloadKind,
            object payload,
            IEdmModel model = null,
            IEdmOperationImport functionImport = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(messageWriter, "messageReader");

            switch (payloadKind)
            {
                case ODataPayloadKind.Property:
                    messageWriter.WriteProperty((ODataProperty)payload);
                    break;

                case ODataPayloadKind.ResourceSet:
                    this.WriteTopLevelFeed(messageWriter, (ODataResourceSet)payload);
                    break;

                case ODataPayloadKind.Resource:
                    this.WriteTopLevelEntry(messageWriter, (ODataResource)payload);
                    break;

                case ODataPayloadKind.Collection:
                    this.WriteCollection(messageWriter, (ODataCollectionStart)payload);
                    break;

                case ODataPayloadKind.ServiceDocument:
                    this.WriteServiceDocument(messageWriter, (ODataServiceDocument)payload);
                    break;

                case ODataPayloadKind.MetadataDocument:
                    this.WriteMetadataDocument(messageWriter);
                    break;

                case ODataPayloadKind.Error:
                    this.WriteError(messageWriter, (ODataError)payload, true);
                    break;

                case ODataPayloadKind.EntityReferenceLink:
                    this.WriteEntityReferenceLink(messageWriter, (ODataEntityReferenceLink)payload);
                    break;

                case ODataPayloadKind.EntityReferenceLinks:
                    this.WriteEntityReferenceLinks(messageWriter, (ODataEntityReferenceLinks)payload);
                    break;

                case ODataPayloadKind.Value:
                    this.WriteValue(messageWriter, payload);
                    break;

                case ODataPayloadKind.Batch:
                    // TODO: Have to figure out product representation of batch payloads or perhaps keep it separate
                    throw new NotSupportedException("Batch not supported in ObjectModelToMessageWriter");

                case ODataPayloadKind.Parameter:
                    this.WriteParameters(messageWriter, (ODataParameters)payload, functionImport);
                    break;

                default:
                    ExceptionUtilities.Assert(false, "The payload kind '{0}' is not yet supported by MessageToObjectModelReader.", payloadKind);
                    break;
            }
        }

        private void WriteTopLevelFeed(ODataMessageWriterTestWrapper messageWriter, ODataResourceSet feed)
        {
            ExceptionUtilities.CheckArgumentNotNull(messageWriter, "messageWriter");

            var feedWriter = messageWriter.CreateODataResourceSetWriter();
            this.WriteFeed(feedWriter, feed);

            feedWriter.Flush();

        }

        private void WriteTopLevelEntry(ODataMessageWriterTestWrapper messageWriter, ODataResource entry)
        {
            ExceptionUtilities.CheckArgumentNotNull(messageWriter, "messageWriter");
            ODataWriter entryWriter = messageWriter.CreateODataResourceWriter();

            this.WriteEntry(entryWriter, entry);

            entryWriter.Flush();

        }

        public void WriteEntityReferenceLink(ODataMessageWriterTestWrapper messageWriter, ODataEntityReferenceLink referenceLink)
        {
            messageWriter.WriteEntityReferenceLink(referenceLink);
        }

        private void WriteEntityReferenceLinks(ODataMessageWriterTestWrapper messageWriter, ODataEntityReferenceLinks referenceLinks)
        {
            messageWriter.WriteEntityReferenceLinks(referenceLinks);
        }

        private void WriteValue(ODataMessageWriterTestWrapper messageWriter, object value)
        {
            messageWriter.WriteValue(value);
        }

        private void WriteCollection(ODataMessageWriterTestWrapper messageWriter, ODataCollectionStart collection)
        {
            ODataCollectionWriter collectionWriter = messageWriter.CreateODataCollectionWriter();
            this.WriteCollection(collectionWriter, collection);

            collectionWriter.Flush();
        }

        private void WriteParameters(ODataMessageWriterTestWrapper messageWriter, ODataParameters parameters, IEdmOperationImport functionImport)
        {
            ODataParameterWriter parameterWriter = messageWriter.CreateODataParameterWriter(functionImport);
            parameterWriter.WriteStart();
            foreach (var parameter in parameters)
            {
                ODataCollectionStart collectionStart = parameter.Value as ODataCollectionStart;
                ODataResourceSet feed;
                ODataResource entry;
                if (collectionStart != null)
                {
                    ODataCollectionWriter collectionWriter = parameterWriter.CreateCollectionWriter(parameter.Key);
                    this.WriteCollection(collectionWriter, collectionStart);
                    collectionWriter.Flush();
                }
                else if ((feed = parameter.Value as ODataResourceSet) != null)
                {
                    this.WriteFeed(parameterWriter.CreateResourceSetWriter(parameter.Key), feed);
                }
                else if ((entry = parameter.Value as ODataResource) != null)
                {
                    this.WriteEntry(parameterWriter.CreateResourceWriter(parameter.Key), entry);
                }
                else
                {
                    parameterWriter.WriteValue(parameter.Key, parameter.Value);
                }
            }

            parameterWriter.WriteEnd();
            parameterWriter.Flush();
        }

        private void WriteServiceDocument(ODataMessageWriterTestWrapper messageWriter, ODataServiceDocument serviceDocument)
        {
            messageWriter.WriteServiceDocument(serviceDocument);
        }

        private void WriteMetadataDocument(ODataMessageWriterTestWrapper messageWriter)
        {
            messageWriter.WriteMetadataDocument();
        }

        private void WriteCollection(ODataCollectionWriter collectionWriter, ODataCollectionStart collection)
        {
            collectionWriter.WriteStart(collection);
            var annotation = collection.GetAnnotation<ODataCollectionItemsObjectModelAnnotation>();

            if (annotation != null)
            {
                foreach (var item in annotation)
                {
                    collectionWriter.WriteItem(item);
                }
            }

            collectionWriter.WriteEnd();
        }

        private void WriteError(ODataMessageWriterTestWrapper messageWriter, ODataError error, bool debug)
        {
            messageWriter.WriteError(error, debug);
        }

        private void WriteFeed(ODataWriter writer, ODataResourceSet resourceCollection)
        {
            writer.WriteStart(resourceCollection);
            var annotation = resourceCollection.GetAnnotation<ODataFeedEntriesObjectModelAnnotation>();
            if (annotation != null)
            {
                foreach (var entry in annotation)
                {
                    this.WriteEntry(writer, entry);
                }
            }

            writer.WriteEnd();
        }

        private void WriteEntry(ODataWriter writer, ODataResource entry)
        {
            writer.WriteStart(entry);
            var annotation = entry.GetAnnotation<ODataEntryNavigationLinksObjectModelAnnotation>();
            ODataNestedResourceInfo navLink = null;
            if (annotation != null)
            {
                for (int i = 0; i < annotation.Count; ++i)
                {
                    bool found = annotation.TryGetNavigationLinkAt(i, out navLink);
                    ExceptionUtilities.Assert(found, "Navigation links should be ordered sequentially for writing");
                    this.WriteNavigationLink(writer, navLink);
                }
            }

            writer.WriteEnd();
        }

        private void WriteNavigationLink(ODataWriter writer, ODataNestedResourceInfo link)
        {
            writer.WriteStart(link);
            var expanded = link.GetAnnotation<ODataNavigationLinkExpandedItemObjectModelAnnotation>();
            if (expanded != null)
            {
                var feed = expanded.ExpandedItem as ODataResourceSet;
                if (feed != null)
                {
                    this.WriteFeed(writer, feed);
                }
                else
                {
                    ODataResource entry = expanded.ExpandedItem as ODataResource;
                    if (entry != null || expanded.ExpandedItem == null)
                    {
                        this.WriteEntry(writer, entry);
                    }
                    else
                    {
                        ExceptionUtilities.Assert(expanded.ExpandedItem is ODataEntityReferenceLink, "Content of a nav. link can only be a feed, entry or entity reference link.");
                        writer.WriteEntityReferenceLink((ODataEntityReferenceLink)expanded.ExpandedItem);
                    }
                }
            }

            writer.WriteEnd();
        }
    }
}
