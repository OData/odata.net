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
    using Microsoft.OData.Core;
    using ClientStrings = Microsoft.OData.Client.Strings;

    /// <summary>
    /// Class that holds a variety of events for writing the payload from the OData to the wire
    /// </summary>
    public class DataServiceClientRequestPipelineConfiguration
    {
        /// <summary> Actions to execute before start entry called. </summary>
        private readonly List<Action<WritingEntryArgs>> writingStartEntryActions;

        /// <summary> Actions to execute before end entry called. </summary>
        private readonly List<Action<WritingEntryArgs>> writingEndEntryActions;

        /// <summary> Actions to execute before entity reference link written. </summary>
        private readonly List<Action<WritingEntityReferenceLinkArgs>> writeEntityReferenceLinkActions;

        /// <summary> Actions to execute after before start navigation link called. </summary>
        private readonly List<Action<WritingNavigationLinkArgs>> writingStartNavigationLinkActions;

        /// <summary> Actions to execute before end navigation link called. </summary>
        private readonly List<Action<WritingNavigationLinkArgs>> writingEndNavigationLinkActions;

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
            this.writingEndEntryActions = new List<Action<WritingEntryArgs>>();
            this.writingEndNavigationLinkActions = new List<Action<WritingNavigationLinkArgs>>();
            this.writingStartEntryActions = new List<Action<WritingEntryArgs>>();
            this.writingStartNavigationLinkActions = new List<Action<WritingNavigationLinkArgs>>();
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

            this.writingStartEntryActions.Add(action);
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

            this.writingEndEntryActions.Add(action);
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
        public DataServiceClientRequestPipelineConfiguration OnNavigationLinkStarting(Action<WritingNavigationLinkArgs> action)
        {
            WebUtil.CheckArgumentNull(action, "action");

            this.writingStartNavigationLinkActions.Add(action);
            return this;
        }

        /// <summary>
        /// Called when [navigation link end].
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The request pipeline configuration.</returns>
        public DataServiceClientRequestPipelineConfiguration OnNavigationLinkEnding(Action<WritingNavigationLinkArgs> action)
        {
            WebUtil.CheckArgumentNull(action, "action");

            this.writingEndNavigationLinkActions.Add(action);
            return this;
        }

        /// <summary>
        /// Called when [create message writer settings configurations].
        /// </summary>
        /// <param name="writerSettings">The writer settings.</param>
        internal void ExecuteWriterSettingsConfiguration(ODataMessageWriterSettingsBase writerSettings)
        {
            Debug.Assert(writerSettings != null, "writerSettings != null");

            if (this.messageWriterSettingsConfigurationActions.Count > 0)
            {
                MessageWriterSettingsArgs args = new MessageWriterSettingsArgs(new DataServiceClientMessageWriterSettingsShim(writerSettings));
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
        internal void ExecuteOnEntryEndActions(ODataEntry entry, object entity)
        {
            Debug.Assert(entry != null, "entry != null");
            Debug.Assert(entity != null, "entity != entity");

            if (this.writingEndEntryActions.Count > 0)
            {
                WritingEntryArgs args = new WritingEntryArgs(entry, entity);
                foreach (Action<WritingEntryArgs> entryArgsAction in this.writingEndEntryActions)
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
        internal void ExecuteOnEntryStartActions(ODataEntry entry, object entity)
        {
            Debug.Assert(entry != null, "entry != null");
            Debug.Assert(entity != null, "entity != entity");

            if (this.writingStartEntryActions.Count > 0)
            {
                WritingEntryArgs args = new WritingEntryArgs(entry, entity);
                foreach (Action<WritingEntryArgs> entryArgsAction in this.writingStartEntryActions)
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
        internal void ExecuteOnNavigationLinkEndActions(ODataNavigationLink link, object source, object target)
        {
            Debug.Assert(link != null, "link != null");

            if (this.writingEndNavigationLinkActions.Count > 0)
            {
                WritingNavigationLinkArgs args = new WritingNavigationLinkArgs(link, source, target);
                foreach (Action<WritingNavigationLinkArgs> navArgsAction in this.writingEndNavigationLinkActions)
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
        internal void ExecuteOnNavigationLinkStartActions(ODataNavigationLink link, object source, object target)
        {
            Debug.Assert(link != null, "link != null");

            if (this.writingStartNavigationLinkActions.Count > 0)
            {
                WritingNavigationLinkArgs args = new WritingNavigationLinkArgs(link, source, target);
                foreach (Action<WritingNavigationLinkArgs> navArgsAction in this.writingStartNavigationLinkActions)
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
