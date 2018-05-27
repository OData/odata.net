//---------------------------------------------------------------------
// <copyright file="ObjectModelWriteReadStreamer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.Streaming
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;

    /// <summary>
    /// Class for streaming OData payloads from ODataWriter to ODataReader.
    /// </summary>
    public class ObjectModelWriteReadStreamer
    {
        /// <summary>
        /// List of states we should go through when reading.
        /// </summary>
        private List<ODataReaderState> expectedStates;

        /// <summary>
        /// The stack of items that have been collected during reading.
        /// </summary>
        private Stack<ODataItem> readItems;

        /// <summary>
        /// Public constructor
        /// </summary>
        public ObjectModelWriteReadStreamer()
        {
            this.expectedStates = new List<ODataReaderState>();
            this.readItems = new Stack<ODataItem>();
        }

        /// <summary>
        /// Write payload kind to message.
        /// </summary>
        /// <param name="messageWriter">Message writer to write payload to.</param>
        /// <param name="payloadKind">The kind of payload we are writing.</param>
        /// <param name="payload">The payload to write.</param>
        /// <param name="functionImport">Function import whose parameters are to be written when the payload kind is Parameters.</param>
        /// <returns>The object read after writing.</returns>
        public ODataItem WriteMessage(ODataMessageWriterTestWrapper messageWriter, ODataMessageReaderTestWrapper messageReader, ODataPayloadKind payloadKind, object payload, IEdmOperationImport functionImport = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(messageWriter, "messageWriter");
            ExceptionUtilities.CheckArgumentNotNull(messageReader, "messageReader");

            switch (payloadKind)
            {
                case ODataPayloadKind.ResourceSet:
                    this.WriteTopLevelFeed(messageWriter, messageReader, (ODataResourceSet)payload);
                    break;
                case ODataPayloadKind.Resource:
                    this.WriteTopLevelEntry(messageWriter, messageReader, (ODataResource)payload);
                    break;
                default:
                    ExceptionUtilities.Assert(false, "The payload kind '{0}' is not yet supported by ObjectModelWriteReadStreamer.", payloadKind);
                    break;
            }

            return readItems.SingleOrDefault();
        }

        private void WriteTopLevelFeed(ODataMessageWriterTestWrapper messageWriter, ODataMessageReaderTestWrapper messageReader, ODataResourceSet feed)
        {
            var feedWriter = messageWriter.CreateODataResourceSetWriter();
            Lazy<ODataReader> lazyReader = new Lazy<ODataReader>(() => messageReader.CreateODataResourceSetReader());
            this.WriteFeed(feedWriter, lazyReader, feed);
        }

        private void WriteTopLevelEntry(ODataMessageWriterTestWrapper messageWriter, ODataMessageReaderTestWrapper messageReader, ODataResource entry)
        {
            ODataWriter entryWriter = messageWriter.CreateODataResourceWriter();
            Lazy<ODataReader> lazyEntryReader = new Lazy<ODataReader>(() => messageReader.CreateODataResourceReader());
            this.WriteEntry(entryWriter, lazyEntryReader, entry);
        }

        private void WriteFeed(ODataWriter writer, Lazy<ODataReader> lazyReader, ODataResourceSet feed)
        {
            this.WriteStart(writer, feed);
            var annotation = feed.GetAnnotation<ODataFeedEntriesObjectModelAnnotation>();
            if (annotation != null)
            {
                int count = annotation.Count;
                for (int i = 0; i < count; ++i)
                {
                    this.WriteEntry(writer, lazyReader, annotation[i]);
                }
            }

            this.WriteEnd(writer, ODataReaderState.ResourceSetEnd);
            this.Read(lazyReader);
        }
        
        private void WriteEntry(ODataWriter writer, Lazy<ODataReader> lazyReader, ODataResource entry)
        {
            this.WriteStart(writer, entry);
            var annotation = entry.GetAnnotation<ODataEntryNavigationLinksObjectModelAnnotation>();
            ODataNestedResourceInfo navLink = null;
            if (annotation != null)
            {
                for (int i = 0; i < annotation.Count; ++i)
                {
                    bool found = annotation.TryGetNavigationLinkAt(i, out navLink);
                    ExceptionUtilities.Assert(found, "Navigation links should be ordered sequentially for writing");
                    this.WriteNavigationLink(writer, lazyReader, navLink);
                }
            }
            
            this.WriteEnd(writer, ODataReaderState.ResourceEnd);
            this.Read(lazyReader);
        }

        private void WriteNavigationLink(ODataWriter writer, Lazy<ODataReader> lazyReader, ODataNestedResourceInfo link)
        {
            this.WriteStart(writer, link);
            var expanded = link.GetAnnotation<ODataNavigationLinkExpandedItemObjectModelAnnotation>();
            if (expanded != null)
            {
                var feed = expanded.ExpandedItem as ODataResourceSet;
                if (feed != null)
                {
                    this.WriteFeed(writer, lazyReader, feed);
                }
                else
                {
                    ODataResource entry = expanded.ExpandedItem as ODataResource;
                    if (entry != null || expanded.ExpandedItem == null)
                    {
                        this.WriteEntry(writer, lazyReader, entry);
                    }
                    else
                    {
                        ExceptionUtilities.Assert(expanded.ExpandedItem is ODataEntityReferenceLink, "Content of a nav. link can only be a feed, entry or entity reference link.");
                        writer.WriteEntityReferenceLink((ODataEntityReferenceLink)expanded.ExpandedItem);
                    }
                }
            }

            this.WriteEnd(writer, ODataReaderState.NestedResourceInfoEnd);
            this.Read(lazyReader);
        }

        private void WriteEnd(ODataWriter writer, ODataReaderState expectedState)
        {
            this.expectedStates.Add(expectedState);
            writer.WriteEnd();
            writer.Flush();
        }

        private void Read(Lazy<ODataReader> lazyReader)
        {
            foreach (var state in this.expectedStates)
            {
                lazyReader.Value.Read();
                ExceptionUtilities.Assert(lazyReader.Value.State == state, "Expected %1, Found %2", state, lazyReader.Value.State);
                switch (state)
                {
                    case ODataReaderState.ResourceSetStart:
                        if (readItems.Count > 0)
                        {
                            ODataNestedResourceInfo navLink = (ODataNestedResourceInfo)readItems.Peek();
                            var annotation = navLink.GetAnnotation<ODataNavigationLinkExpandedItemObjectModelAnnotation>();
                            if (annotation == null)
                            {
                                annotation = new ODataNavigationLinkExpandedItemObjectModelAnnotation();
                                navLink.SetAnnotation(annotation);
                            }

                            annotation.ExpandedItem = lazyReader.Value.Item;
                        }

                        readItems.Push(lazyReader.Value.Item);
                        break;
                    case ODataReaderState.ResourceStart:
                        var currentEntry = (ODataResource)lazyReader.Value.Item;
                        if (readItems.Count > 0)
                        {
                            ODataResourceSet feed = readItems.Peek() as ODataResourceSet;
                            if (feed != null)
                            {
                                var annotation = feed.GetAnnotation<ODataFeedEntriesObjectModelAnnotation>();
                                if (annotation == null)
                                {
                                    annotation = new ODataFeedEntriesObjectModelAnnotation();
                                    feed.SetAnnotation(annotation);
                                }
                                annotation.Add((ODataResource)lazyReader.Value.Item);
                            }
                            else
                            {
                                ODataNestedResourceInfo navLink = (ODataNestedResourceInfo)readItems.Peek();
                                var annotation = navLink.GetAnnotation<ODataNavigationLinkExpandedItemObjectModelAnnotation>();
                                if (annotation == null)
                                {
                                    annotation = new ODataNavigationLinkExpandedItemObjectModelAnnotation();
                                    navLink.SetAnnotation(annotation);
                                }

                                annotation.ExpandedItem = currentEntry;
                            }
                        }

                        readItems.Push(currentEntry);
                        break;
                    case ODataReaderState.NestedResourceInfoStart:
                        ODataResource entry = (ODataResource)readItems.Peek();
                        var navLinksAnnotation = entry.GetAnnotation<ODataEntryNavigationLinksObjectModelAnnotation>();
                        if (navLinksAnnotation == null)
                        {
                            navLinksAnnotation = new ODataEntryNavigationLinksObjectModelAnnotation();
                            entry.SetAnnotation(navLinksAnnotation);
                        }

                        navLinksAnnotation.Add((ODataNestedResourceInfo)lazyReader.Value.Item, entry.Properties.Count() + navLinksAnnotation.Count);
                        readItems.Push(lazyReader.Value.Item);
                        break;

                    case ODataReaderState.ResourceEnd:
                    case ODataReaderState.ResourceSetEnd:
                    case ODataReaderState.NestedResourceInfoEnd:
                        if (readItems.Count() > 1)
                            readItems.Pop();
                        break;
                }
            }

            this.expectedStates.Clear();
        }

        private void WriteStart(ODataWriter writer, ODataItem item)
        {
            var feed = item as ODataResourceSet;
            if (feed != null)
            {
                this.expectedStates.Add(ODataReaderState.ResourceSetStart);
                writer.WriteStart(feed);
                return;
            }

            var entry = item as ODataResource;
            if (entry != null)
            {
                this.expectedStates.Add(ODataReaderState.ResourceStart);
                writer.WriteStart(entry);
                return;
            }

            var navLink = item as ODataNestedResourceInfo;
            if (navLink != null)
            {
                this.expectedStates.Add(ODataReaderState.NestedResourceInfoStart);
                writer.WriteStart(navLink);
            }
        }
    }
}
