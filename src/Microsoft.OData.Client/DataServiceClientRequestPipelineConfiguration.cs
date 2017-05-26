//---------------------------------------------------------------------
// <copyright file="DataServiceClientRequestPipelineConfiguration.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData;
    using ClientStrings = Microsoft.OData.Client.Strings;

    /// <summary>
    /// Class that holds a variety of events for writing the payload from the OData to the wire
    /// </summary>
    public class DataServiceClientRequestPipelineConfiguration
    {
        /// <summary> Actions to execute before start entry called. </summary>
        private readonly List<Action<WritingEntryArgs>> writingStartResourceActions;

        /// <summary> Actions to execute before end entry called. </summary>
        private readonly List<Action<WritingEntryArgs>> writingEndResourceActions;

        /// <summary> Actions to execute before entity reference link written. </summary>
        private readonly List<Action<WritingEntityReferenceLinkArgs>> writeEntityReferenceLinkActions;

        /// <summary> Actions to execute after before start navigation link called. </summary>
        private readonly List<Action<WritingNestedResourceInfoArgs>> writingStartNestedResourceInfoActions;

        /// <summary> Actions to execute before end navigation link called. </summary>
        private readonly List<Action<WritingNestedResourceInfoArgs>> writingEndNestedResourceInfoActions;

        /// <summary> The message writer setting configurations. </summary>
        private readonly List<Action<MessageWriterSettingsArgs>> messageWriterSettingsConfigurationActions;

        /// <summary> The delegate that represents how a message is created.</summary>
        private Func<DataServiceClientRequestMessageArgs, DataServiceClientRequestMessage> onmessageCreating;

        /// <summary>
        /// Creates a request pipeline configuration class
        /// </summary>
        internal DataServiceClientRequestPipelineConfiguration()
        {
            this.writeEntityReferenceLinkActions = new List<Action<WritingEntityReferenceLinkArgs>>();
            this.writingEndResourceActions = new List<Action<WritingEntryArgs>>();
            this.writingEndNestedResourceInfoActions = new List<Action<WritingNestedResourceInfoArgs>>();
            this.writingStartResourceActions = new List<Action<WritingEntryArgs>>();
            this.writingStartNestedResourceInfoActions = new List<Action<WritingNestedResourceInfoArgs>>();
            this.messageWriterSettingsConfigurationActions = new List<Action<MessageWriterSettingsArgs>>();
        }

        /// <summary>
        /// Gets the request message to be used for sending the request. By providing a custom message, users
        /// can replace the transport layer.
        /// </summary>
        public Func<DataServiceClientRequestMessageArgs, DataServiceClientRequestMessage> OnMessageCreating
        {
            get
            {
                return this.onmessageCreating;
            }

            set
            {
                if (this.ContextUsingSendingRequest)
                {
                    throw new DataServiceClientException(ClientStrings.Context_SendingRequest_InvalidWhenUsingOnMessageCreating);
                }

                this.onmessageCreating = value;
            }
        }

        /// <summary>
        /// Determines if OnMessageCreating is being used or not.
        /// </summary>
        internal bool HasOnMessageCreating
        {
            get { return this.OnMessageCreating != null; }
        }

        /// <summary>
        /// Gets or sets the a value indicating whether the context is using the sending request event or not.
        /// </summary>
        internal bool ContextUsingSendingRequest { get; set; }

        /// <summary>
        /// Called when [message writer created].
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns>The request pipeline configuration.</returns>
        public DataServiceClientRequestPipelineConfiguration OnMessageWriterSettingsCreated(Action<MessageWriterSettingsArgs> args)
        {
            WebUtil.CheckArgumentNull(args, "args");

            this.messageWriterSettingsConfigurationActions.Add(args);
            return this;
        }

        /// <summary>
        /// Called when [entry starting].
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The request pipeline configuration.</returns>
        public DataServiceClientRequestPipelineConfiguration OnEntryStarting(Action<WritingEntryArgs> action)
        {
            WebUtil.CheckArgumentNull(action, "action");

            this.writingStartResourceActions.Add(action);
            return this;
        }

        /// <summary>
        /// Called when [entry ending].
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The request pipeline configuration.</returns>
        public DataServiceClientRequestPipelineConfiguration OnEntryEnding(Action<WritingEntryArgs> action)
        {
            WebUtil.CheckArgumentNull(action, "action");

            this.writingEndResourceActions.Add(action);
            return this;
        }

        /// <summary>
        /// Called when [entity reference link].
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The request pipeline configuration.</returns>
        public DataServiceClientRequestPipelineConfiguration OnEntityReferenceLink(Action<WritingEntityReferenceLinkArgs> action)
        {
            WebUtil.CheckArgumentNull(action, "action");

            this.writeEntityReferenceLinkActions.Add(action);
            return this;
        }

        /// <summary>
        /// Called when [navigation link starting].
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The request pipeline configuration.</returns>
        public DataServiceClientRequestPipelineConfiguration OnNestedResourceInfoStarting(Action<WritingNestedResourceInfoArgs> action)
        {
            WebUtil.CheckArgumentNull(action, "action");

            this.writingStartNestedResourceInfoActions.Add(action);
            return this;
        }

        /// <summary>
        /// Called when [navigation link end].
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The request pipeline configuration.</returns>
        public DataServiceClientRequestPipelineConfiguration OnNestedResourceInfoEnding(Action<WritingNestedResourceInfoArgs> action)
        {
            WebUtil.CheckArgumentNull(action, "action");

            this.writingEndNestedResourceInfoActions.Add(action);
            return this;
        }

        /// <summary>
        /// Called when [create message writer settings configurations].
        /// </summary>
        /// <param name="writerSettings">The writer settings.</param>
        internal void ExecuteWriterSettingsConfiguration(ODataMessageWriterSettings writerSettings)
        {
            Debug.Assert(writerSettings != null, "writerSettings != null");

            if (this.messageWriterSettingsConfigurationActions.Count > 0)
            {
                MessageWriterSettingsArgs args = new MessageWriterSettingsArgs(writerSettings);
                foreach (Action<MessageWriterSettingsArgs> configureWriterSettings in this.messageWriterSettingsConfigurationActions)
                {
                    configureWriterSettings(args);
                }
            }
        }

        /// <summary>
        /// Fires before entry end.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="entity">The entity.</param>
        internal void ExecuteOnEntryEndActions(ODataResource entry, object entity)
        {
            Debug.Assert(entry != null, "entry != null");
            Debug.Assert(entity != null, "entity != entity");

            if (this.writingEndResourceActions.Count > 0)
            {
                WritingEntryArgs args = new WritingEntryArgs(entry, entity);
                foreach (Action<WritingEntryArgs> entryArgsAction in this.writingEndResourceActions)
                {
                    entryArgsAction(args);
                }
            }
        }

        /// <summary>
        /// Fires before entry start.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="entity">The entity.</param>
        internal void ExecuteOnEntryStartActions(ODataResource entry, object entity)
        {
            Debug.Assert(entry != null, "entry != null");
            Debug.Assert(entity != null, "entity != entity");

            if (this.writingStartResourceActions.Count > 0)
            {
                WritingEntryArgs args = new WritingEntryArgs(entry, entity);
                foreach (Action<WritingEntryArgs> entryArgsAction in this.writingStartResourceActions)
                {
                    entryArgsAction(args);
                }
            }
        }

        /// <summary>
        /// Fires before navigation end.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        internal void ExecuteOnNestedResourceInfoEndActions(ODataNestedResourceInfo link, object source, object target)
        {
            Debug.Assert(link != null, "link != null");

            if (this.writingEndNestedResourceInfoActions.Count > 0)
            {
                WritingNestedResourceInfoArgs args = new WritingNestedResourceInfoArgs(link, source, target);
                foreach (Action<WritingNestedResourceInfoArgs> navArgsAction in this.writingEndNestedResourceInfoActions)
                {
                    navArgsAction(args);
                }
            }
        }

        /// <summary>
        /// Fires before navigation start.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        internal void ExecuteOnNestedResourceInfoStartActions(ODataNestedResourceInfo link, object source, object target)
        {
            Debug.Assert(link != null, "link != null");

            if (this.writingStartNestedResourceInfoActions.Count > 0)
            {
                WritingNestedResourceInfoArgs args = new WritingNestedResourceInfoArgs(link, source, target);
                foreach (Action<WritingNestedResourceInfoArgs> navArgsAction in this.writingStartNestedResourceInfoActions)
                {
                    navArgsAction(args);
                }
            }
        }

        /// <summary>
        /// Fires before writing the on entity reference link.
        /// </summary>
        /// <param name="entityReferenceLink">The entity reference link.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        internal void ExecuteEntityReferenceLinkActions(ODataEntityReferenceLink entityReferenceLink, object source, object target)
        {
            Debug.Assert(entityReferenceLink != null, "entityReferenceLink != null");

            if (this.writeEntityReferenceLinkActions.Count > 0)
            {
                WritingEntityReferenceLinkArgs args = new WritingEntityReferenceLinkArgs(entityReferenceLink, source, target);
                foreach (Action<WritingEntityReferenceLinkArgs> navArgsAction in this.writeEntityReferenceLinkActions)
                {
                    navArgsAction(args);
                }
            }
        }
    }
}
