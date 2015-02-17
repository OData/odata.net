//---------------------------------------------------------------------
// <copyright file="ObjectModelWriteReadStreamer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.Streaming
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core;
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
                case ODataPayloadKind.Feed:
                    this.WriteTopLevelFeed(messageWriter, messageReader, (ODataFeed)payload);
                    break;
                case ODataPayloadKind.Entry:
                    this.WriteTopLevelEntry(messageWriter, messageReader, (ODataEntry)payload);
                    break;
                default:
                    ExceptionUtilities.Assert(false, "The payload kind '{0}' is not yet supported by ObjectModelWriteReadStreamer.", payloadKind);
                    break;
            }

            return readItems.SingleOrDefault();
        }

        private void WriteTopLevelFeed(ODataMessageWriterTestWrapper messageWriter, ODataMessageReaderTestWrapper messageReader, ODataFeed feed)
        {
            var feedWriter = messageWriter.CreateODataFeedWriter();
            Lazy<ODataReader> lazyReader = new Lazy<ODataReader>(() => messageReader.CreateODataFeedReader());
            this.WriteFeed(feedWriter, lazyReader, feed);
        }

        private void WriteTopLevelEntry(ODataMessageWriterTestWrapper messageWriter, ODataMessageReaderTestWrapper messageReader, ODataEntry entry)
        {
            ODataWriter entryWriter = messageWriter.CreateODataEntryWriter();
            Lazy<ODataReader> lazyEntryReader = new Lazy<ODataReader>(() => messageReader.CreateODataEntryReader());
            this.WriteEntry(entryWriter, lazyEntryReader, entry);
        }

        private void WriteFeed(ODataWriter writer, Lazy<ODataReader> lazyReader, ODataFeed feed)
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

            this.WriteEnd(writer, ODataReaderState.FeedEnd);
            this.Read(lazyReader);
        }
        
        private void WriteEntry(ODataWriter writer, Lazy<ODataReader> lazyReader, ODataEntry entry)
        {
            this.WriteStart(writer, entry);
            var annotation = entry.GetAnnotation<ODataEntryNavigationLinksObjectModelAnnotation>();
            ODataNavigationLink navLink = null;
            if (annotation != null)
            {
                for (int i = 0; i < annotation.Count; ++i)
                {
                    bool found = annotation.TryGetNavigationLinkAt(i, out navLink);
                    ExceptionUtilities.Assert(found, "Navigation links should be ordered sequentially for writing");
                    this.WriteNavigationLink(writer, lazyReader, navLink);
                }
            }
            
            this.WriteEnd(writer, ODataReaderState.EntryEnd);
            this.Read(lazyReader);
        }

        private void WriteNavigationLink(ODataWriter writer, Lazy<ODataReader> lazyReader, ODataNavigationLink link)
        {
            this.WriteStart(writer, link);
            var expanded = link.GetAnnotation<ODataNavigationLinkExpandedItemObjectModelAnnotation>();
            if (expanded != null)
            {
                var feed = expanded.ExpandedItem as ODataFeed;
                if (feed != null)
                {
                    this.WriteFeed(writer, lazyReader, feed);
                }
                else
                {
                    ODataEntry entry = expanded.ExpandedItem as ODataEntry;
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

            this.WriteEnd(writer, ODataReaderState.NavigationLinkEnd);
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
                    case ODataReaderState.FeedStart:
                        if (readItems.Count > 0)
                        {
                            ODataNavigationLink navLink = (ODataNavigationLink)readItems.Peek();
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
                    case ODataReaderState.EntryStart:
                        var currentEntry = (ODataEntry)lazyReader.Value.Item;
                        if (readItems.Count > 0)
                        {
                            ODataFeed feed = readItems.Peek() as ODataFeed;
                            if (feed != null)
                            {
                                var annotation = feed.GetAnnotation<ODataFeedEntriesObjectModelAnnotation>();
                                if (annotation == null)
                                {
                                    annotation = new ODataFeedEntriesObjectModelAnnotation();
                                    feed.SetAnnotation(annotation);
                                }
                                annotation.Add((ODataEntry)lazyReader.Value.Item);
                            }
                            else
                            {
                                ODataNavigationLink navLink = (ODataNavigationLink)readItems.Peek();
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
                    case ODataReaderState.NavigationLinkStart:
                        ODataEntry entry = (ODataEntry)readItems.Peek();
                        var navLinksAnnotation = entry.GetAnnotation<ODataEntryNavigationLinksObjectModelAnnotation>();
                        if (navLinksAnnotation == null)
                        {
                            navLinksAnnotation = new ODataEntryNavigationLinksObjectModelAnnotation();
                            entry.SetAnnotation(navLinksAnnotation);
                        }

                        navLinksAnnotation.Add((ODataNavigationLink)lazyReader.Value.Item, entry.Properties.Count() + navLinksAnnotation.Count);
                        readItems.Push(lazyReader.Value.Item);
                        break;

                    case ODataReaderState.EntryEnd:
                    case ODataReaderState.FeedEnd:
                    case ODataReaderState.NavigationLinkEnd:
                        if (readItems.Count() > 1)
                            readItems.Pop();
                        break;
                }
            }

            this.expectedStates.Clear();
        }

        private void WriteStart(ODataWriter writer, ODataItem item)
        {
            var feed = item as ODataFeed;
            if (feed != null)
            {
                this.expectedStates.Add(ODataReaderState.FeedStart);
                writer.WriteStart(feed);
                return;
            }

            var entry = item as ODataEntry;
            if (entry != null)
            {
                this.expectedStates.Add(ODataReaderState.EntryStart);
                writer.WriteStart(entry);
                return;
            }

            var navLink = item as ODataNavigationLink;
            if (navLink != null)
            {
                this.expectedStates.Add(ODataReaderState.NavigationLinkStart);
                writer.WriteStart(navLink);
            }
        }
    }
}
