//---------------------------------------------------------------------
// <copyright file="DataServiceClientResponsePipelineConfiguration.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData;
    using Microsoft.OData.Client.Materialization;

    /// <summary>
    /// Class that is responsible for configuration of actions that are invoked from a response
    /// </summary>
    public class DataServiceClientResponsePipelineConfiguration
    {
        /// <summary> Actions to be run when reading start entry called </summary>
        private readonly List<Action<ReadingEntryArgs>> readingStartResourceActions;

        /// <summary> Actions to be run when reading end entry called </summary>
        private readonly List<Action<ReadingEntryArgs>> readingEndResourceActions;

        /// <summary> Actions to be run when reading start feed called </summary>
        private readonly List<Action<ReadingFeedArgs>> readingStartFeedActions;

        /// <summary> Actions to be run when reading end feed called </summary>
        private readonly List<Action<ReadingFeedArgs>> readingEndFeedActions;

        /// <summary> Actions to be run when reading start link called </summary>
        private readonly List<Action<ReadingNestedResourceInfoArgs>> readingStartNestedResourceInfoActions;

        /// <summary> Actions to be run when reading end link called </summary>
        private readonly List<Action<ReadingNestedResourceInfoArgs>> readingEndNestedResourceInfoActions;

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
            this.readingEndResourceActions = new List<Action<ReadingEntryArgs>>();
            this.readingEndFeedActions = new List<Action<ReadingFeedArgs>>();
            this.readingEndNestedResourceInfoActions = new List<Action<ReadingNestedResourceInfoArgs>>();

            this.readingStartResourceActions = new List<Action<ReadingEntryArgs>>();
            this.readingStartFeedActions = new List<Action<ReadingFeedArgs>>();
            this.readingStartNestedResourceInfoActions = new List<Action<ReadingNestedResourceInfoArgs>>();

            this.materializedEntityActions = new List<Action<MaterializedEntityArgs>>();

            this.messageReaderSettingsConfigurationActions = new List<Action<MessageReaderSettingsArgs>>();
        }

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
                return this.readingStartResourceActions.Count > 0 ||
                    this.readingEndResourceActions.Count > 0 ||
                    this.readingStartFeedActions.Count > 0 ||
                    this.readingEndFeedActions.Count > 0 ||
                    this.readingStartNestedResourceInfoActions.Count > 0 ||
                    this.readingEndNestedResourceInfoActions.Count > 0;
            }
        }

        /// <summary>
        /// Gets whether there is a reading entity handler
        /// </summary>
        internal bool HasReadingEntityHandlers
        {
            get
            {
                if (this.materializedEntityActions.Count > 0)
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

            this.readingStartResourceActions.Add(action);
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

            this.readingEndResourceActions.Add(action);
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
        public DataServiceClientResponsePipelineConfiguration OnNestedResourceInfoStarted(Action<ReadingNestedResourceInfoArgs> action)
        {
            WebUtil.CheckArgumentNull(action, "action");

            this.readingStartNestedResourceInfoActions.Add(action);
            return this;
        }

        /// <summary>
        /// Called when [read end navigation link].
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The response pipeline configuration.</returns>
        public DataServiceClientResponsePipelineConfiguration OnNestedResourceInfoEnded(Action<ReadingNestedResourceInfoArgs> action)
        {
            WebUtil.CheckArgumentNull(action, "action");

            this.readingEndNestedResourceInfoActions.Add(action);
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
        internal void ExecuteReaderSettingsConfiguration(ODataMessageReaderSettings readerSettings)
        {
            Debug.Assert(readerSettings != null, "readerSettings != null");

            if (this.messageReaderSettingsConfigurationActions.Count > 0)
            {
                MessageReaderSettingsArgs args = new MessageReaderSettingsArgs(readerSettings);
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
        internal void ExecuteOnEntryEndActions(ODataResource entry)
        {
            // Be noticed that the entry could be null in some case, like expand.
            if (this.readingEndResourceActions.Count > 0)
            {
                ReadingEntryArgs args = new ReadingEntryArgs(entry);
                foreach (Action<ReadingEntryArgs> entryAction in this.readingEndResourceActions)
                {
                    entryAction(args);
                }
            }
        }

        /// <summary>
        /// Executes the on entry start actions.
        /// </summary>
        /// <param name="entry">The entry.</param>
        internal void ExecuteOnEntryStartActions(ODataResource entry)
        {
            // Be noticed that the entry could be null in some case, like expand.
            if (this.readingStartResourceActions.Count > 0)
            {
                ReadingEntryArgs args = new ReadingEntryArgs(entry);
                foreach (Action<ReadingEntryArgs> entryAction in this.readingStartResourceActions)
                {
                    entryAction(args);
                }
            }
        }

        /// <summary>
        /// Executes the on feed end actions.
        /// </summary>
        /// <param name="feed">The feed.</param>
        internal void ExecuteOnFeedEndActions(ODataResourceSet feed)
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
        internal void ExecuteOnFeedStartActions(ODataResourceSet feed)
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
        internal void ExecuteOnNavigationEndActions(ODataNestedResourceInfo link)
        {
            Debug.Assert(link != null, "link != null");
            if (this.readingEndNestedResourceInfoActions.Count > 0)
            {
                ReadingNestedResourceInfoArgs args = new ReadingNestedResourceInfoArgs(link);
                foreach (Action<ReadingNestedResourceInfoArgs> navAction in this.readingEndNestedResourceInfoActions)
                {
                    navAction(args);
                }
            }
        }

        /// <summary>
        /// Executes the on navigation start actions.
        /// </summary>
        /// <param name="link">The link.</param>
        internal void ExecuteOnNavigationStartActions(ODataNestedResourceInfo link)
        {
            Debug.Assert(link != null, "link != null");

            if (this.readingStartNestedResourceInfoActions.Count > 0)
            {
                ReadingNestedResourceInfoArgs args = new ReadingNestedResourceInfoArgs(link);
                foreach (Action<ReadingNestedResourceInfoArgs> navAction in this.readingStartNestedResourceInfoActions)
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
        internal void ExecuteEntityMaterializedActions(ODataResource entry, object entity)
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
        /// Fires the end entry events.
        /// </summary>
        /// <param name="entry">The entry.</param>
        internal void FireEndEntryEvents(MaterializerEntry entry)
        {
            Debug.Assert(entry != null, "entry!=null");

            if (this.HasReadingEntityHandlers)
            {
                this.ExecuteEntityMaterializedActions(entry.Entry, entry.ResolvedObject);
            }
        }
    }
}
