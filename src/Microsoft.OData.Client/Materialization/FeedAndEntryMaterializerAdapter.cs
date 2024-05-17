//---------------------------------------------------------------------
// <copyright file="FeedAndEntryMaterializerAdapter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Materialization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData;
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

        /// <summary>An enumerator of <see cref="ODataResource"/> values.</summary>
        private IEnumerator<ODataResource> feedEntries;

        /// <summary>The current feed.</summary>
        private ODataResourceSet currentFeed;

        /// <summary>The current feed.</summary>
        private ODataDeltaResourceSet currentDeltaFeed;

        /// <summary>The current entry.</summary>
        private ODataResource currentEntry;

        /// <summary>The stack of read <see cref="IMaterializerState"/> items for ODataDeltaResourceSet.</summary>
        private readonly Stack<IMaterializerState> deltaMaterializerStateItems;

        /// <summary>The stack of read <see cref="IMaterializerState"/> items for ODataResource.</summary>
        private readonly Stack<IMaterializerState> entryMaterializerStateItems;

        /// <summary>
        /// The materializer context.
        /// </summary>
        private readonly IODataMaterializerContext materializerContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedAndEntryMaterializerAdapter"/> class.
        /// </summary>
        /// <param name="messageReader">The messageReader that is used to get the format of the reader.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="model">The model.</param>
        /// <param name="mergeOption">The mergeOption.</param>
        /// <param name="materializerContext">The materializer context.</param>
        internal FeedAndEntryMaterializerAdapter(ODataMessageReader messageReader, ODataReaderWrapper reader, ClientEdmModel model, MergeOption mergeOption, IODataMaterializerContext materializerContext, bool isDeltaPayload = false)
            : this(ODataUtils.GetReadFormat(messageReader), reader, model, mergeOption, materializerContext, isDeltaPayload)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedAndEntryMaterializerAdapter"/> class. Used for tests so no ODataMessageReader is required
        /// </summary>
        /// <param name="odataFormat">The format of the reader.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="model">The model.</param>
        /// <param name="mergeOption">The mergeOption.</param>
        /// <param name="materializerContext">The materializer context.</param>
        internal FeedAndEntryMaterializerAdapter(ODataFormat odataFormat, ODataReaderWrapper reader, ClientEdmModel model, MergeOption mergeOption, IODataMaterializerContext materializerContext, bool isDeltaPayload = false)
        {
            this.readODataFormat = odataFormat;
            this.clientEdmModel = model;
            this.mergeOption = mergeOption;
            this.reader = reader;
            this.currentEntry = null;
            this.currentFeed = null;
            this.feedEntries = null;
            this.currentDeltaFeed = null;
            this.materializerContext = materializerContext;

            if (isDeltaPayload)
            {
                // If we have a delta payload, we need the stack data structure to keep track of the read items.
                this.deltaMaterializerStateItems = new Stack<IMaterializerState>();
            }
            else
            {
                this.entryMaterializerStateItems = new Stack<IMaterializerState>();
            }
        }

        /// <summary>
        /// Gets the current feed.
        /// </summary>
        public ODataResourceSet CurrentFeed
        {
            get { return this.currentFeed; }
        }

        /// <summary>
        /// Gets the current delta resource set feed.
        /// </summary>
        public ODataDeltaResourceSet CurrentDeltaFeed
        {
            get { return this.currentDeltaFeed; }
        }

        /// <summary>
        /// Gets the current entry.
        /// </summary>
        public ODataResource CurrentEntry
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
                this.feedEntries = MaterializerFeed.GetFeed(this.currentFeed, this.materializerContext).Entries.GetEnumerator();
            }

            if (this.currentFeed != null && this.currentFeed.Count.HasValue)
            {
                return this.currentFeed.Count.Value;
            }

            throw new InvalidOperationException(DSClient.Strings.MaterializeFromObject_CountNotPresent);
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
                        ODataResourceSet feed;
                        MaterializerDeltaFeed deltaEntry;
                        MaterializerEntry entryAndState;
                        if (this.TryReadFeedOrEntry(true, out feed, out deltaEntry, out entryAndState))
                        {
                            this.currentEntry = entryAndState != null ? entryAndState.Entry : null;
                            this.currentFeed = feed;
                            this.currentDeltaFeed = deltaEntry?.DeltaFeed;

                            if (this.currentFeed != null)
                            {
                                this.feedEntries = MaterializerFeed.GetFeed(this.currentFeed, this.materializerContext).Entries.GetEnumerator();

                                // Try to read the first entry.
                                if (!this.feedEntries.MoveNext())
                                {
                                    this.feedEntries = null;
                                    this.currentEntry = null;
                                    return false;
                                }
                                else
                                {
                                    this.currentEntry = this.feedEntries.Current;
                                }
                            }

                            return true;
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                case ODataReaderState.ResourceSetEnd: 
                case ODataReaderState.DeltaResourceSetEnd:
                case ODataReaderState.ResourceEnd:
                    if (this.TryRead() || this.reader.State != ODataReaderState.Completed)
                    {
                        throw DSClient.Error.InternalError(InternalError.UnexpectedReadState);
                    }

                    this.currentEntry = null;
                    this.currentDeltaFeed = null;
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
        /// <param name="deltaFeed">The delta feed.</param>
        /// <param name="entry">The entry.</param>
        /// <returns>true if a value was read, otherwise false</returns>
        private bool TryReadFeedOrEntry(bool lazy, out ODataResourceSet feed, out MaterializerDeltaFeed deltaFeed, out MaterializerEntry entry)
        {
            entry = null;
            feed = null;
            deltaFeed = null;

            if (this.TryStartReadFeedOrEntry())
            {
                if (this.reader.State == ODataReaderState.ResourceStart)
                {
                    entry = this.ReadEntryCore();
                }
                else if (this.reader.State == ODataReaderState.DeltaResourceSetStart)
                {
                    deltaFeed = this.ReadDeltaFeedCore();
                }
                else
                {
                    feed = this.ReadFeedCore(lazy);
                }
            }

            Debug.Assert(feed == null || entry == null || deltaFeed == null, "feed == null || entry == null || deltaFeed == null");
            return feed != null || entry != null || deltaFeed != null;
        }

        /// <summary>
        /// Tries to read the start of a feed or entry.
        /// </summary>
        /// <returns>true if a value was read, otherwise false</returns>
        private bool TryStartReadFeedOrEntry()
        {
            return this.TryRead() && (this.reader.State == ODataReaderState.ResourceSetStart || this.reader.State == ODataReaderState.ResourceStart || this.reader.State == ODataReaderState.DeltaResourceSetStart);
        }

        /// <summary>
        /// Tries to read a feed.
        /// </summary>
        /// <param name="lazy">if set to <c>true</c> [lazy].</param>
        /// <param name="feed">The feed.</param>
        /// <returns>true if a value was read, otherwise false</returns>
        private bool TryReadFeed(bool lazy, out ODataResourceSet feed)
        {
            if (this.TryStartReadFeedOrEntry())
            {
                this.ExpectState(ODataReaderState.ResourceSetStart);
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
        private ODataResourceSet ReadFeedCore(bool lazy)
        {
            this.ExpectState(ODataReaderState.ResourceSetStart);

            ODataResourceSet result = (ODataResourceSet)this.reader.Item;

            IEnumerable<ODataResource> lazyEntries = this.LazyReadEntries();

            if (lazy)
            {
                MaterializerFeed.CreateFeed(result, lazyEntries, this.materializerContext);
            }
            else
            {
                MaterializerFeed.CreateFeed(result, new List<ODataResource>(lazyEntries), this.materializerContext);
            }

            return result;
        }

        /// <summary>
        /// Reads an <see cref="ODataDeltaResourceSet"/>.
        /// </summary>
        /// <returns>The <see cref="MaterializerDeltaFeed"/> of the read <see cref="ODataDeltaResourceSet"/>.</returns>
        private MaterializerDeltaFeed ReadDeltaFeedCore()
        {
            this.ExpectState(ODataReaderState.DeltaResourceSetStart);

            ODataDeltaResourceSet result = (ODataDeltaResourceSet)this.reader.Item;
            MaterializerDeltaFeed feed = null;

            Debug.Assert(this.deltaMaterializerStateItems != null, "this.deltaMaterializerStateItems should not be null");

            feed = MaterializerDeltaFeed.CreateDeltaFeed(
                    result,
                    new List<IMaterializerState>(),
                    this.materializerContext);

            this.deltaMaterializerStateItems.Push(feed);

            do
            {
                this.AssertRead();

                switch (this.reader.State)
                {
                    case ODataReaderState.ResourceStart:
                        MaterializerEntry entry = this.ReadEntryCore();
                        feed.AddEntry(entry);
                        break;
                    case ODataReaderState.DeletedResourceStart:
                        MaterializerDeletedEntry deletedMaterializerEntry = this.ReadDeletedResource();
                        feed.AddEntry(deletedMaterializerEntry);
                        break;
                    case ODataReaderState.DeletedResourceEnd:
                    case ODataReaderState.ResourceEnd:
                    case ODataReaderState.DeltaResourceSetEnd:
                        this.deltaMaterializerStateItems?.Pop();
                        break;
                    default:
                        throw DSClient.Error.InternalError(InternalError.UnexpectedReadState);
                }
            }
            while (this.reader.State != ODataReaderState.DeltaResourceSetEnd);

            return feed;
        }

        /// <summary>
        /// Adds an <see cref="IMaterializerState"/> entry to a parent entry.
        /// </summary>
        /// <param name="materializerState">The <see cref="IMaterializerState"/> entry being added to the parent item.</param>
        private void AddDeltaResourceToParent(IMaterializerState materializerState)
        {
            IMaterializerState topItem = this.deltaMaterializerStateItems.Peek();

            if (topItem is MaterializerDeltaFeed delta)
            {
                delta.AddEntry(materializerState);
            }
            else if (topItem is MaterializerEntry entry)
            {
                entry.AddNestedItem(materializerState);
            }
        }

        /// <summary>
        /// Adds an <see cref="IMaterializerState"/> entry to a parent entry.
        /// </summary>
        /// <param name="materializerState">The <see cref="IMaterializerState"/> entry being added to the parent item.</param>
        private void AddResourceToParent(IMaterializerState materializerState)
        {
            IMaterializerState topItem = this.entryMaterializerStateItems.Peek();

            if (topItem is MaterializerEntry entry)
            {
                entry.AddNestedItem(materializerState);
            }
        }

        /// <summary>
        /// Lazily reads entries.
        /// </summary>
        /// <returns>An enumerable that will lazily read entries when enumerated.</returns>
        private IEnumerable<ODataResource> LazyReadEntries()
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
                this.ExpectState(ODataReaderState.ResourceStart);
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
            this.ExpectState(ODataReaderState.ResourceStart);

            ODataResource result = (ODataResource)this.reader.Item;

            MaterializerEntry entry;
            List<ODataNestedResourceInfo> navigationLinks = new List<ODataNestedResourceInfo>();

            if (result != null)
            {
                entry = MaterializerEntry.CreateEntry(
                    result,
                    this.readODataFormat,
                    this.mergeOption != MergeOption.NoTracking,
                    this.clientEdmModel,
                    this.materializerContext);

                // A resource cannot be a top-level item in a bulk-update. 
                // The resource has to be contained in a delta resource set. 
                if (this.deltaMaterializerStateItems?.Count > 0)
                {
                    this.deltaMaterializerStateItems.Push(entry);
                }

                if (this.entryMaterializerStateItems != null)
                {
                    this.entryMaterializerStateItems.Push(entry);
                }

                do
                {
                    this.AssertRead();

                    switch (this.reader.State)
                    {
                        case ODataReaderState.NestedResourceInfoStart:
                            // Cache the list of navigation links here but don't add them to the entry because all of the key properties may not be available yet.
                            navigationLinks.Add(this.ReadNestedResourceInfo());
                            
                            break;
                        case ODataReaderState.ResourceEnd:
                            if (this.deltaMaterializerStateItems?.Count > 0)
                            {
                                this.deltaMaterializerStateItems.Pop();
                            }

                            if (this.entryMaterializerStateItems?.Count > 0)
                            {
                                this.entryMaterializerStateItems.Pop();
                            }

                            break;
                        default:
                            throw DSClient.Error.InternalError(InternalError.UnexpectedReadState);
                    }
                }
                while (this.reader.State != ODataReaderState.ResourceEnd);

                if (!entry.Entry.IsTransient)
                {
                    entry.UpdateEntityDescriptor();
                }
            }
            else
            {
                entry = MaterializerEntry.CreateEmpty();
                this.ReadAndExpectState(ODataReaderState.ResourceEnd);
            }

            // Add the navigation links here now that all of the property values have been read and are available to build the links.
            foreach (ODataNestedResourceInfo navigationLink in navigationLinks)
            {
                entry.AddNestedResourceInfo(navigationLink);
            }

            return entry;
        }

        /// <summary>
        /// Reads a navigation link.
        /// </summary>
        /// <returns>A navigation link.</returns>
        private ODataNestedResourceInfo ReadNestedResourceInfo()
        {
            Debug.Assert(this.reader.State == ODataReaderState.NestedResourceInfoStart, "this.reader.State == ODataReaderState.NestedResourceInfoStart");

            ODataNestedResourceInfo link = (ODataNestedResourceInfo)this.reader.Item;

            MaterializerNestedEntry nestedEntry;

            if (this.TryReadFeedOrEntry(false, out ODataResourceSet feed, out MaterializerDeltaFeed deltaFeed, out MaterializerEntry entry))
            {
                if (feed != null)
                {
                    nestedEntry = MaterializerNestedEntry.CreateNestedEntry(
                        link,
                        new List<IMaterializerState>(),
                        this.materializerContext);

                    nestedEntry.Feed = feed;

                    MaterializerFeed materializerFeed = MaterializerFeed.GetFeed(feed, this.materializerContext);

                    foreach (ODataResource resource in materializerFeed.Entries)
                    {
                        MaterializerEntry materializerEntry = MaterializerEntry.GetEntry(resource, this.materializerContext);

                        materializerFeed.AddItem(materializerEntry);
                    }

                    nestedEntry.AddNestedItem(materializerFeed);
                    AddResourceToParent(nestedEntry);
                }
                else if (deltaFeed != null)
                {
                    nestedEntry = MaterializerNestedEntry.CreateNestedEntry(
                        link, 
                        new List<IMaterializerState>(), 
                        this.materializerContext);

                    nestedEntry.AddNestedItem(deltaFeed);

                    Debug.Assert(this.deltaMaterializerStateItems != null, "this.deltaMaterializerStateItems should not be null");

                    if (this.deltaMaterializerStateItems.Count > 0)
                    {
                        AddDeltaResourceToParent(nestedEntry);
                    }                     
                }
                else
                {
                    Debug.Assert(entry != null, "entry != null");

                    nestedEntry = MaterializerNestedEntry.CreateNestedEntry(
                        link,
                        new List<IMaterializerState>(),
                        this.materializerContext);

                    nestedEntry.Entry = entry;

                    nestedEntry.AddNestedItem(entry);
                    AddResourceToParent(nestedEntry);
                }

                this.ReadAndExpectState(ODataReaderState.NestedResourceInfoEnd);
            }

            this.ExpectState(ODataReaderState.NestedResourceInfoEnd);

            return link;
        }

        /// <summary>
        /// Reads an <see cref="ODataDeletedResource"/>
        /// </summary>
        /// <returns>An <see cref="MaterializerDeletedEntry"/> of the read <see cref="ODataDeletedResource"/>.</returns>
        private MaterializerDeletedEntry ReadDeletedResource()
        {
            this.ExpectState(ODataReaderState.DeletedResourceStart);

            ODataDeletedResource result = (ODataDeletedResource)this.reader.Item;
            MaterializerDeletedEntry entry;

            Debug.Assert(this.deltaMaterializerStateItems != null, "this.deltaMaterializerStateItems should not be null");

            entry = MaterializerDeletedEntry.CreateDeletedEntry(
                    result,
                    this.materializerContext);

            this.deltaMaterializerStateItems.Push(entry);

            do
            {
                this.AssertRead();

                switch (this.reader.State)
                {
                    case ODataReaderState.DeletedResourceEnd:
                        if (this.deltaMaterializerStateItems.Count > 0)
                        {
                            this.deltaMaterializerStateItems.Pop();
                        }
                        break;
                    default:
                        throw DSClient.Error.InternalError(InternalError.UnexpectedReadState);
                }
            }
            while (this.reader.State != ODataReaderState.DeletedResourceEnd);

            return entry;
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
