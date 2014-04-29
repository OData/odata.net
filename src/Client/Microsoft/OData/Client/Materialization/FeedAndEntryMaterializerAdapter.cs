//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Client.Materialization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Core;
    using Microsoft.OData.Client;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// Class for reading top level feeds or entries and adapting it for the materializer
    /// </summary>
    internal class FeedAndEntryMaterializerAdapter
    {
        /// <summary>The odata format being read.</summary>
        private readonly ODataFormat readODataFormat;

        /// <summary>The reader.</summary>
        private readonly ODataReaderWrapper reader;

        /// <summary>The Client Edm Model used to determine type information.</summary>
        private readonly ClientEdmModel clientEdmModel;

        /// <summary>MergeOption information to determine how to merge descriptors.</summary>
        private readonly MergeOption mergeOption;

        /// <summary>An enumerator of <see cref="ODataEntry"/> values.</summary>
        private IEnumerator<ODataEntry> feedEntries;

        /// <summary>The current feed.</summary>
        private ODataFeed currentFeed;

        /// <summary>The current entry.</summary>
        private ODataEntry currentEntry;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedAndEntryMaterializerAdapter"/> class.
        /// </summary>
        /// <param name="messageReader">The messageReader that is used to get the format of the reader.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="model">The model.</param>
        /// <param name="mergeOption">The mergeOption.</param>
        internal FeedAndEntryMaterializerAdapter(ODataMessageReader messageReader, ODataReaderWrapper reader, ClientEdmModel model, MergeOption mergeOption)
            : this(ODataUtils.GetReadFormat(messageReader), reader, model, mergeOption)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedAndEntryMaterializerAdapter"/> class. Used for tests so no ODataMessageReader is required
        /// </summary>
        /// <param name="odataFormat">The format of the reader.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="model">The model.</param>
        /// <param name="mergeOption">The mergeOption.</param>
        internal FeedAndEntryMaterializerAdapter(ODataFormat odataFormat, ODataReaderWrapper reader, ClientEdmModel model, MergeOption mergeOption)
        {
            this.readODataFormat = odataFormat;
            this.clientEdmModel = model;
            this.mergeOption = mergeOption;
            this.reader = reader;
            this.currentEntry = null;
            this.currentFeed = null;
            this.feedEntries = null;
        }

        /// <summary>
        /// Gets the current feed.
        /// </summary>
        public ODataFeed CurrentFeed
        {
            get { return this.currentFeed; }
        }

        /// <summary>
        /// Gets the current entry.
        /// </summary>
        public ODataEntry CurrentEntry
        {
            get { return this.currentEntry; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is end of stream.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is end of stream; otherwise, <c>false</c>.
        /// </value>
        public bool IsEndOfStream
        {
            get { return this.reader.State == ODataReaderState.Completed; }
        }

        /// <summary>
        /// The count tag's value, if requested
        /// </summary>
        /// <param name="readIfNoFeed">Should read pull if no feed exists.</param>
        /// <returns>The count value returned from the server</returns>
        public long GetCountValue(bool readIfNoFeed)
        {
            if (this.currentFeed == null && this.currentEntry == null && readIfNoFeed && this.TryReadFeed(true, out this.currentFeed))
            {
                this.feedEntries = MaterializerFeed.GetFeed(this.currentFeed).Entries.GetEnumerator();
            }

            if (this.currentFeed != null && this.currentFeed.Count.HasValue)
            {
                return this.currentFeed.Count.Value;
            }

            throw new InvalidOperationException(DSClient.Strings.MaterializeFromAtom_CountNotPresent);
        }

        /// <summary>
        /// Read a feed or entry, with the expected type.
        /// </summary>
        /// <returns>true if a value was read, otherwise false</returns>
        public bool Read()
        {
            if (this.feedEntries != null)
            {
                // ISSUE: this might throw - refactor?
                if (this.feedEntries.MoveNext())
                {
                    this.currentEntry = this.feedEntries.Current;
                    return true;
                }
                else
                {
                    this.feedEntries = null;
                    this.currentEntry = null;
                }
            }

            switch (this.reader.State)
            {
                case ODataReaderState.Completed:
                    this.currentEntry = null;
                    return false;
                case ODataReaderState.Start:
                    {
                        ODataFeed feed;
                        MaterializerEntry entryAndState;
                        if (this.TryReadFeedOrEntry(true, out feed, out entryAndState))
                        {
                            this.currentEntry = entryAndState != null ? entryAndState.Entry : null;
                            this.currentFeed = feed;
                            if (this.currentFeed != null)
                            {
                                this.feedEntries = MaterializerFeed.GetFeed(this.currentFeed).Entries.GetEnumerator();
                            }

                            return true;
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }

                case ODataReaderState.FeedEnd:
                case ODataReaderState.EntryEnd:
                    if (this.TryRead() || this.reader.State != ODataReaderState.Completed)
                    {
                        throw DSClient.Error.InternalError(InternalError.UnexpectedReadState);
                    }

                    this.currentEntry = null;
                    return false;
                default:
                    throw DSClient.Error.InternalError(InternalError.UnexpectedReadState);
            }
        }

        /// <summary>
        /// Disposes the reader
        /// </summary>
        public void Dispose()
        {
            if (this.feedEntries != null)
            {
                this.feedEntries.Dispose();
                this.feedEntries = null;
            }
        }

        /// <summary>
        /// Tries to read a feed or entry.
        /// </summary>
        /// <param name="lazy">if set to <c>true</c> [lazy].</param>
        /// <param name="feed">The feed.</param>
        /// <param name="entry">The entry.</param>
        /// <returns>true if a value was read, otherwise false</returns>
        private bool TryReadFeedOrEntry(bool lazy, out ODataFeed feed, out MaterializerEntry entry)
        {
            if (this.TryStartReadFeedOrEntry())
            {
                if (this.reader.State == ODataReaderState.EntryStart)
                {
                    entry = this.ReadEntryCore();
                    feed = null;
                }
                else
                {
                    entry = null;
                    feed = this.ReadFeedCore(lazy);
                }
            }
            else
            {
                feed = null;
                entry = null;
            }

            Debug.Assert(feed == null || entry == null, "feed == null || entry == null");
            return feed != null || entry != null;
        }

        /// <summary>
        /// Tries to read the start of a feed or entry.
        /// </summary>
        /// <returns>true if a value was read, otherwise false</returns>
        private bool TryStartReadFeedOrEntry()
        {
            return this.TryRead() && (this.reader.State == ODataReaderState.FeedStart || this.reader.State == ODataReaderState.EntryStart);
        }

        /// <summary>
        /// Tries to read a feed.
        /// </summary>
        /// <param name="lazy">if set to <c>true</c> [lazy].</param>
        /// <param name="feed">The feed.</param>
        /// <returns>true if a value was read, otherwise false</returns>
        private bool TryReadFeed(bool lazy, out ODataFeed feed)
        {
            if (this.TryStartReadFeedOrEntry())
            {
                this.ExpectState(ODataReaderState.FeedStart);
                feed = this.ReadFeedCore(lazy);
            }
            else
            {
                feed = null;
            }

            return feed != null;
        }

        /// <summary>
        /// Reads the remainder of a feed.
        /// </summary>
        /// <param name="lazy">if set to <c>true</c> [lazy].</param>
        /// <returns>A feed.</returns>
        private ODataFeed ReadFeedCore(bool lazy)
        {
            this.ExpectState(ODataReaderState.FeedStart);

            ODataFeed result = (ODataFeed)this.reader.Item;

            IEnumerable<ODataEntry> lazyEntries = this.LazyReadEntries();

            if (lazy)
            {
                MaterializerFeed.CreateFeed(result, lazyEntries);
            }
            else
            {
                MaterializerFeed.CreateFeed(result, new List<ODataEntry>(lazyEntries));
            }

            return result;
        }

        /// <summary>
        /// Lazily reads entries.
        /// </summary>
        /// <returns>An enumerable that will lazily read entries when enumerated.</returns>
        private IEnumerable<ODataEntry> LazyReadEntries()
        {
            MaterializerEntry entryAndState;
            while (this.TryReadEntry(out entryAndState))
            {
                yield return entryAndState.Entry;
            }
        }

        /// <summary>
        /// Tries to read an entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>true if a value was read, otherwise false</returns>
        private bool TryReadEntry(out MaterializerEntry entry)
        {
            if (this.TryStartReadFeedOrEntry())
            {
                this.ExpectState(ODataReaderState.EntryStart);
                entry = this.ReadEntryCore();
                return true;
            }
            else
            {
                entry = default(MaterializerEntry);
                return false;
            }
        }

        /// <summary>
        /// Reads the remainder of an entry.
        /// </summary>
        /// <returns>An entry.</returns>
        private MaterializerEntry ReadEntryCore()
        {
            this.ExpectState(ODataReaderState.EntryStart);

            ODataEntry result = (ODataEntry)this.reader.Item;

            MaterializerEntry entry;
            List<ODataNavigationLink> navigationLinks = new List<ODataNavigationLink>();
            if (result != null)
            {
                entry = MaterializerEntry.CreateEntry(
                    result,
                    this.readODataFormat,
                    this.mergeOption != MergeOption.NoTracking,
                    this.clientEdmModel);

                do
                {
                    this.AssertRead();

                    switch (this.reader.State)
                    {
                        case ODataReaderState.NavigationLinkStart:
                            // Cache the list of navigation links here but don't add them to the entry because all of the key properties may not be available yet.
                            navigationLinks.Add(this.ReadNavigationLink());
                            break;
                        case ODataReaderState.EntryEnd:
                            break;
                        default:
                            throw DSClient.Error.InternalError(InternalError.UnexpectedReadState);
                    }
                }
                while (this.reader.State != ODataReaderState.EntryEnd);

                if (!entry.Entry.IsTransient)
                {
                    entry.UpdateEntityDescriptor();
                }
            }
            else
            {
                entry = MaterializerEntry.CreateEmpty();
                this.ReadAndExpectState(ODataReaderState.EntryEnd);
            }

            // Add the navigation links here now that all of the property values have been read and are available to build the links.
            foreach (ODataNavigationLink navigationLink in navigationLinks)
            {
                entry.AddNavigationLink(navigationLink);
            }

            return entry;
        }

        /// <summary>
        /// Reads a navigation link.
        /// </summary>
        /// <returns>A navigation link.</returns>
        private ODataNavigationLink ReadNavigationLink()
        {
            Debug.Assert(this.reader.State == ODataReaderState.NavigationLinkStart, "this.reader.State == ODataReaderState.NavigationLinkStart");

            ODataNavigationLink link = (ODataNavigationLink)this.reader.Item;

            MaterializerEntry entry;
            ODataFeed feed;
            if (this.TryReadFeedOrEntry(false, out feed, out entry))
            {
                if (feed != null)
                {
                    MaterializerNavigationLink.CreateLink(link, feed);
                }
                else
                {
                    Debug.Assert(entry != null, "entry != null");
                    MaterializerNavigationLink.CreateLink(link, entry);
                }

                this.ReadAndExpectState(ODataReaderState.NavigationLinkEnd);
            }

            this.ExpectState(ODataReaderState.NavigationLinkEnd);

            return link;
        }

        /// <summary>
        /// Tries to read from the ODataReader.
        /// </summary>
        /// <returns>True if a value is read, otherwise false</returns>
        private bool TryRead()
        {
            try
            {
                return this.reader.Read();
            }
            catch (ODataErrorException e)
            {
                throw new DataServiceClientException(DSClient.Strings.Deserialize_ServerException(e.Error.Message), e);
            }
            catch (ODataException o)
            {
                throw new InvalidOperationException(o.Message, o);
            }
        }

        /// <summary>
        /// Reads from the reader and asserts the reader is in the expected state.
        /// </summary>
        /// <param name="expectedState">The expected state.</param>
        private void ReadAndExpectState(ODataReaderState expectedState)
        {
            this.AssertRead();

            this.ExpectState(expectedState);
        }

        /// <summary>
        /// Asserts that an item could be read.
        /// </summary>
        private void AssertRead()
        {
            if (!this.TryRead())
            {
                throw DSClient.Error.InternalError(InternalError.UnexpectedReadState);
            }
        }

        /// <summary>
        /// Asserts the reader is in the expected state.
        /// </summary>
        /// <param name="expectedState">The expected state.</param>
        private void ExpectState(ODataReaderState expectedState)
        {
            if (this.reader.State != expectedState)
            {
                throw DSClient.Error.InternalError(InternalError.UnexpectedReadState);
            }
        }
    }
}
