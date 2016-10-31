//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Client.Materialization;
    using System.Diagnostics;
    using Microsoft.Data.OData;

    /// <summary>
    /// Class that is responsible for configuration of actions that are invoked from a response
    /// </summary>
    public class DataServiceClientResponsePipelineConfiguration
    {
        /// <summary> Actions to be run when reading start entry called </summary>
        private readonly List<Action<ReadingEntryArgs>> readingStartEntryActions;

        /// <summary> Actions to be run when reading end entry called </summary>
        private readonly List<Action<ReadingEntryArgs>> readingEndEntryActions;

        /// <summary> Actions to be run when reading start feed called </summary>
        private readonly List<Action<ReadingFeedArgs>> readingStartFeedActions;

        /// <summary> Actions to be run when reading end feed called </summary>
        private readonly List<Action<ReadingFeedArgs>> readingEndFeedActions;

        /// <summary> Actions to be run when reading start link called </summary>
        private readonly List<Action<ReadingNavigationLinkArgs>> readingStartNavigationLinkActions;

        /// <summary> Actions to be run when reading end link called </summary>
        private readonly List<Action<ReadingNavigationLinkArgs>> readingEndNavigationLinkActions;

        /// <summary> Actions to be run after an entry has been materialized </summary>
        private readonly List<Action<MaterializedEntityArgs>> materializedEntityActions;

        /// <summary> The message reader setting configurations. </summary>
        private readonly List<Action<MessageReaderSettingsArgs>> messageReaderSettingsConfigurationActions;

        /// <summary> The sender. </summary>
        private readonly object sender;

        /// <summary>
        /// Creates a Data service client response pipeline class
        /// </summary>
        /// <param name="sender"> The sender for the Reading Atom event.</param>
        internal DataServiceClientResponsePipelineConfiguration(object sender)
        {
            Debug.Assert(sender != null, "sender!= null");

            this.sender = sender;
            this.readingEndEntryActions = new List<Action<ReadingEntryArgs>>();
            this.readingEndFeedActions = new List<Action<ReadingFeedArgs>>();
            this.readingEndNavigationLinkActions = new List<Action<ReadingNavigationLinkArgs>>();

            this.readingStartEntryActions = new List<Action<ReadingEntryArgs>>();
            this.readingStartFeedActions = new List<Action<ReadingFeedArgs>>();
            this.readingStartNavigationLinkActions = new List<Action<ReadingNavigationLinkArgs>>();

            this.materializedEntityActions = new List<Action<MaterializedEntityArgs>>();

            this.messageReaderSettingsConfigurationActions = new List<Action<MessageReaderSettingsArgs>>();
        }

        /// <summary>
        /// Internal event instance used by the public ReadingEntity event.
        /// </summary>
        internal event EventHandler<ReadingWritingEntityEventArgs> ReadingAtomEntity;

        /// <summary>
        /// Gets a value indicating whether this instance has handlers.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has handlers; otherwise, <c>false</c>.
        /// </value>
        internal bool HasConfigurations
        {
            get
            {
                return this.readingStartEntryActions.Count > 0 ||
                    this.readingEndEntryActions.Count > 0 ||
                    this.readingStartFeedActions.Count > 0 ||
                    this.readingEndFeedActions.Count > 0 ||
                    this.readingStartNavigationLinkActions.Count > 0 ||
                    this.readingEndNavigationLinkActions.Count > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has atom reading entity handlers.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has atom reading entity handlers; otherwise, <c>false</c>.
        /// </value>
        internal bool HasAtomReadingEntityHandlers
        {
            get { return this.ReadingAtomEntity != null; }
        }

        /// <summary>
        /// Gets whether there is a reading entity handler
        /// </summary>
        internal bool HasReadingEntityHandlers
        {
            get
            {
                if (this.ReadingAtomEntity != null || this.materializedEntityActions.Count > 0)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Called when [reader settings created].
        /// </summary>
        /// <param name="messageReaderSettingsAction">The reader message settings configuration.</param>
        /// <returns>The response pipeline configuration.</returns>
        public DataServiceClientResponsePipelineConfiguration OnMessageReaderSettingsCreated(Action<MessageReaderSettingsArgs> messageReaderSettingsAction)
        {
            WebUtil.CheckArgumentNull(messageReaderSettingsAction, "messageReaderSettingsAction");

            this.messageReaderSettingsConfigurationActions.Add(messageReaderSettingsAction);
            return this;
        }

        /// <summary>
        /// Called when [read start entry].
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The response pipeline configuration.</returns>
        public DataServiceClientResponsePipelineConfiguration OnEntryStarted(Action<ReadingEntryArgs> action)
        {
            WebUtil.CheckArgumentNull(action, "action");

            this.readingStartEntryActions.Add(action);
            return this;
        }

        /// <summary>
        /// Called when [read end entry].
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The response pipeline configuration.</returns>
        public DataServiceClientResponsePipelineConfiguration OnEntryEnded(Action<ReadingEntryArgs> action)
        {
            WebUtil.CheckArgumentNull(action, "action");

            this.readingEndEntryActions.Add(action);
            return this;
        }

        /// <summary>
        /// Called when [read start feed].
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The response pipeline configuration.</returns>
        public DataServiceClientResponsePipelineConfiguration OnFeedStarted(Action<ReadingFeedArgs> action)
        {
            WebUtil.CheckArgumentNull(action, "action");

            this.readingStartFeedActions.Add(action);
            return this;
        }

        /// <summary>
        /// Called when [read end feed].
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The response pipeline configuration.</returns>
        public DataServiceClientResponsePipelineConfiguration OnFeedEnded(Action<ReadingFeedArgs> action)
        {
            WebUtil.CheckArgumentNull(action, "action");

            this.readingEndFeedActions.Add(action);
            return this;
        }

        /// <summary>
        /// Called when [read start navigation link].
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The response pipeline configuration.</returns>
        public DataServiceClientResponsePipelineConfiguration OnNavigationLinkStarted(Action<ReadingNavigationLinkArgs> action)
        {
            WebUtil.CheckArgumentNull(action, "action");

            this.readingStartNavigationLinkActions.Add(action);
            return this;
        }

        /// <summary>
        /// Called when [read end navigation link].
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The response pipeline configuration.</returns>
        public DataServiceClientResponsePipelineConfiguration OnNavigationLinkEnded(Action<ReadingNavigationLinkArgs> action)
        {
            WebUtil.CheckArgumentNull(action, "action");

            this.readingEndNavigationLinkActions.Add(action);
            return this;
        }

        /// <summary>
        /// Called when [entity materialized].
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The response pipeline configuration.</returns>
        public DataServiceClientResponsePipelineConfiguration OnEntityMaterialized(Action<MaterializedEntityArgs> action)
        {
            WebUtil.CheckArgumentNull(action, "action");

            this.materializedEntityActions.Add(action);
            return this;
        }

        /// <summary>
        /// Executes actions that configure reader settings.
        /// </summary>
        /// <param name="readerSettings">The reader settings.</param>
        internal void ExecuteReaderSettingsConfiguration(ODataMessageReaderSettingsBase readerSettings)
        {
            Debug.Assert(readerSettings != null, "readerSettings != null");

            if (this.messageReaderSettingsConfigurationActions.Count > 0)
            {
                MessageReaderSettingsArgs args = new MessageReaderSettingsArgs(new DataServiceClientMessageReaderSettingsShim(readerSettings));
                foreach (Action<MessageReaderSettingsArgs> readerSettingsConfigurationAction in this.messageReaderSettingsConfigurationActions)
                {
                    readerSettingsConfigurationAction(args);
                }
            }
        }

        /// <summary>
        /// Executes the on entry end actions.
        /// </summary>
        /// <param name="entry">The entry.</param>
        internal void ExecuteOnEntryEndActions(ODataEntry entry)
        {
            // Be noticed that the entry could be null in some case, like expand.
            if (this.readingEndEntryActions.Count > 0)
            {
                ReadingEntryArgs args = new ReadingEntryArgs(entry);
                foreach (Action<ReadingEntryArgs> entryAction in this.readingEndEntryActions)
                {
                    entryAction(args);
                }
            }
        }

        /// <summary>
        /// Executes the on entry start actions.
        /// </summary>
        /// <param name="entry">The entry.</param>
        internal void ExecuteOnEntryStartActions(ODataEntry entry)
        {
            // Be noticed that the entry could be null in some case, like expand.
            if (this.readingStartEntryActions.Count > 0)
            {
                ReadingEntryArgs args = new ReadingEntryArgs(entry);
                foreach (Action<ReadingEntryArgs> entryAction in this.readingStartEntryActions)
                {
                    entryAction(args);
                }
            }
        }

        /// <summary>
        /// Executes the on feed end actions.
        /// </summary>
        /// <param name="feed">The feed.</param>
        internal void ExecuteOnFeedEndActions(ODataFeed feed)
        {
            Debug.Assert(feed != null, "entry != null");

            if (this.readingEndFeedActions.Count > 0)
            {
                ReadingFeedArgs args = new ReadingFeedArgs(feed);
                foreach (Action<ReadingFeedArgs> feedAction in this.readingEndFeedActions)
                {
                    feedAction(args);
                }
            }
        }

        /// <summary>
        /// Executes the on feed start actions.
        /// </summary>
        /// <param name="feed">The feed.</param>
        internal void ExecuteOnFeedStartActions(ODataFeed feed)
        {
            Debug.Assert(feed != null, "feed != null");
            if (this.readingStartFeedActions.Count > 0)
            {
                ReadingFeedArgs args = new ReadingFeedArgs(feed);
                foreach (Action<ReadingFeedArgs> feedAction in this.readingStartFeedActions)
                {
                    feedAction(args);
                }
            }
        }

        /// <summary>
        /// Executes the on navigation end actions.
        /// </summary>
        /// <param name="link">The link.</param>
        internal void ExecuteOnNavigationEndActions(ODataNavigationLink link)
        {
            Debug.Assert(link != null, "link != null");
            if (this.readingEndNavigationLinkActions.Count > 0)
            {
                ReadingNavigationLinkArgs args = new ReadingNavigationLinkArgs(link);
                foreach (Action<ReadingNavigationLinkArgs> navAction in this.readingEndNavigationLinkActions)
                {
                    navAction(args);
                }
            }
        }

        /// <summary>
        /// Executes the on navigation start actions.
        /// </summary>
        /// <param name="link">The link.</param>
        internal void ExecuteOnNavigationStartActions(ODataNavigationLink link)
        {
            Debug.Assert(link != null, "link != null");

            if (this.readingStartNavigationLinkActions.Count > 0)
            {
                ReadingNavigationLinkArgs args = new ReadingNavigationLinkArgs(link);
                foreach (Action<ReadingNavigationLinkArgs> navAction in this.readingStartNavigationLinkActions)
                {
                    navAction(args);
                }
            }
        }

        /// <summary>
        /// Fires after the entry was materialized
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="entity">The entity.</param>
        internal void ExecuteEntityMaterializedActions(ODataEntry entry, object entity)
        {
            Debug.Assert(entry != null, "entry != null");
            Debug.Assert(entity != null, "entity != entity");

            if (this.materializedEntityActions.Count > 0)
            {
                MaterializedEntityArgs args = new MaterializedEntityArgs(entry, entity);
                foreach (Action<MaterializedEntityArgs> materializedEntryArgsAction in this.materializedEntityActions)
                {
                    materializedEntryArgsAction(args);
                }
            }
        }

        /// <summary>
        /// Fires the reading atom entity event.
        /// </summary>
        /// <param name="materializerEntry">The materializer entry.</param>
        internal void FireReadingAtomEntityEvent(MaterializerEntry materializerEntry)
        {
            Debug.Assert(materializerEntry != null, "materializerEntry != null");

            if (this.ReadingAtomEntity != null)
            {
                if (materializerEntry.Format == ODataFormat.Atom)
                {
                    ReadingEntityInfo readingEntityInfo = materializerEntry.Entry.GetAnnotation<ReadingEntityInfo>();
                    Debug.Assert(readingEntityInfo != null, "readingEntityInfo != null");
                    ReadingWritingEntityEventArgs args = new ReadingWritingEntityEventArgs(materializerEntry.ResolvedObject, readingEntityInfo.EntryPayload, readingEntityInfo.BaseUri);
                    this.ReadingAtomEntity(this.sender, args);
                }
#if DEBUG
                else
                {
                    // For Json formats, there must not be any readingEntityInfo annotation on the entry.
                    ReadingEntityInfo readingEntityInfo = materializerEntry.Entry.GetAnnotation<ReadingEntityInfo>();
                    Debug.Assert(readingEntityInfo == null, "readingEntityInfo == null");
                }
#endif
            }
        }

        /// <summary>
        /// Fires the end entry events.
        /// </summary>
        /// <param name="entry">The entry.</param>
        internal void FireEndEntryEvents(MaterializerEntry entry)
        {
            Debug.Assert(entry != null, "entry!=null");

            if (this.HasReadingEntityHandlers)
            {
                this.ExecuteEntityMaterializedActions(entry.Entry, entry.ResolvedObject);

                this.FireReadingAtomEntityEvent(entry);
            }
        }
    }
}
