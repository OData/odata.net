//---------------------------------------------------------------------
// <copyright file="EntityDescriptorData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Product;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents data for the EntityDescriptor (from Microsoft.OData.Client namespace).
    /// As a general rule, all state here should also be present on the equivalent class.
    /// However, visibility should be based on whether that state has an effect on the product behavior.
    /// </summary>
    public sealed class EntityDescriptorData : DescriptorData
    {
        private object entity;
        private List<LinkInfoData> linkInfos;
        private List<OperationDescriptorData> operationDescriptors;
        private List<StreamDescriptorData> streamDescriptors;
        private StreamDescriptorData defaultStreamDescriptor;
        private DataServiceContextData contextData;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityDescriptorData"/> class.
        /// </summary>
        /// <param name="contextData">The context data which contains this entity descriptor data</param>
        /// <param name="entity">The entity.</param>
        internal EntityDescriptorData(DataServiceContextData contextData, object entity)
            : this(contextData)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");

            this.entity = entity;
            this.EntityClrType = entity.GetType();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityDescriptorData"/> class.
        /// </summary>
        /// <param name="contextData">The context data which contains this entity descriptor data</param>
        /// <param name="identity">The entity identity.</param>
        /// <param name="entityClrType">Type of the entity.</param>
        internal EntityDescriptorData(DataServiceContextData contextData, Uri identity, Type entityClrType)
            : this(contextData)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityClrType, "entityClrType");

            this.EntityClrType = entityClrType;
            this.Identity = identity;
        }

        /// <summary>
        /// Initializes a new instance of the EntityDescriptorData class
        /// </summary>
        /// <param name="contextData">The context data which contains this entity descriptor data</param>
        private EntityDescriptorData(DataServiceContextData contextData)
        {
            ExceptionUtilities.CheckArgumentNotNull(contextData, "contextData");
            this.contextData = contextData;
            this.linkInfos = new List<LinkInfoData>();
            this.operationDescriptors = new List<OperationDescriptorData>();
            this.streamDescriptors = new List<StreamDescriptorData>();
        }

        /// <summary>
        /// Gets or sets the name of the entity set.
        /// </summary>
        /// <value>The name of the entity set.</value>
        public string EntitySetName { get; set; }

        /// <summary>
        /// Gets or sets the server type name
        /// </summary>
        public string ServerTypeName { get; set; }

        /// <summary>
        /// Gets the CLR type of the entity.
        /// </summary>
        public Type EntityClrType { get; private set; }

        /// <summary>
        /// Gets or sets the entity's identity.
        /// </summary>
        /// <value>The identity.</value>
        public Uri Identity { get; set; }
        
        /// <summary>
        /// Gets or sets the entity instance.
        /// </summary>
        /// <value>The entity.</value>
        public object Entity
        {
            get
            {
                return this.entity;
            }

            set
            {
                if (this.entity != null && this.entity != value)
                {
                    throw new TaupoInvalidOperationException(
                        string.Format(CultureInfo.InvariantCulture, "Cannot re-purpose existing entity descriptor data for different entity. Descriptor = {0}.", this));
                }

                if (value != null && this.EntityClrType != value.GetType())
                {
                    throw new TaupoInvalidOperationException(
                        string.Format(CultureInfo.InvariantCulture, "Cannot re-purpose existing entity descriptor data for an entity of different CLR type '{0}'. Descriptor = {1}.", value.GetType(), this));
                }

                this.entity = value;
            }
        }

        /// <summary>
        /// Gets or sets the stream ETag
        /// </summary>
        public string StreamETag
        {
            get
            {
                if (this.defaultStreamDescriptor == null)
                {
                    return null;
                }

                return this.defaultStreamDescriptor.ETag;
            }

            set
            {
                this.InitializeDefaultStreamDescriptor();
                this.defaultStreamDescriptor.ETag = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the read stream Uri
        /// </summary>
        public Uri ReadStreamUri
        {
            get
            {
                if (this.defaultStreamDescriptor == null)
                {
                    return null;
                }

                return this.defaultStreamDescriptor.SelfLink;
            }

            set
            {
                this.InitializeDefaultStreamDescriptor();
                this.defaultStreamDescriptor.SelfLink = value;
            }
        }

        /// <summary>
        /// Gets or sets the edit stream Uri
        /// </summary>
        public Uri EditStreamUri
        {
            get
            {
                if (this.defaultStreamDescriptor == null)
                {
                    return null;
                }

                return this.defaultStreamDescriptor.EditLink;
            }

            set
            {
                this.InitializeDefaultStreamDescriptor();
                this.defaultStreamDescriptor.EditLink = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this descriptor refers to an MLE
        /// </summary>
        public bool IsMediaLinkEntry
        {
            get { return this.defaultStreamDescriptor != null; }
        }

        /// <summary>
        /// Gets or sets the ETag.
        /// </summary>
        /// <value>The Etag for the entity.</value>
        public string ETag { get; set; }

        /// <summary>
        /// Gets or sets the self link
        /// </summary>
        public Uri SelfLink { get; set; }

        /// <summary>
        /// Gets or sets the edit link
        /// </summary>
        public Uri EditLink { get; set; }

        /// <summary>
        /// Gets or sets the parent for insert.
        /// </summary>
        /// <value>The parent for insert.</value>
        public object ParentForInsert { get; set; }

        /// <summary>
        /// Gets or sets the parent property for insert.
        /// </summary>
        /// <value>The parent property for insert.</value>
        public string ParentPropertyForInsert { get; set; }

        /// <summary>
        /// Gets the list of link info's
        /// </summary>
        public ReadOnlyCollection<LinkInfoData> LinkInfos
        {
            get
            {
                return this.linkInfos.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the list of operation descriptors
        /// </summary>
        public ReadOnlyCollection<OperationDescriptorData> OperationDescriptors
        {
            get
            {
                return this.operationDescriptors.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the list of stream descriptors.
        /// </summary>
        public ReadOnlyCollection<StreamDescriptorData> StreamDescriptors
        {
            get
            {
                return this.streamDescriptors.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets or sets the state of the default stream
        /// </summary>
        public EntityStates DefaultStreamState
        {
            get
            {
                ExceptionUtilities.CheckObjectNotNull(this.defaultStreamDescriptor, "Cannot get stream state if entity is not an MLE");
                return this.defaultStreamDescriptor.State;
            }

            set
            {
                this.InitializeDefaultStreamDescriptor();
                this.defaultStreamDescriptor.State = value;
            }
        }

        /// <summary>
        /// Gets the default stream descriptor if this is an MLE. Otherwise throws.
        /// </summary>
        public StreamDescriptorData DefaultStreamDescriptor
        {
            get
            {
                ExceptionUtilities.CheckObjectNotNull(this.defaultStreamDescriptor, "Cannot get default stream descriptor if entity is not an MLE");
                return this.defaultStreamDescriptor;
            }
        }

        /// <summary>
        /// Gets or sets the link to insert the entity into when adding it
        /// </summary>
        public Uri InsertLink { get; set; }
                     
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{{ State = {0}, Identity = {1}, EntityType = {2} }}",
                this.State,
                this.Identity,
                this.Entity != null ? this.Entity.GetType().Name : this.EntityClrType.Name);
        }

        /// <summary>
        /// Clones the current entity descriptor data
        /// </summary>
        /// <returns>A clone of the current entity descriptor data</returns>
        /// <param name="clonedContextData">The cloned context data which will contain the cloned entity descriptor data</param>
        public EntityDescriptorData Clone(DataServiceContextData clonedContextData)
        {
            var clone = new EntityDescriptorData(clonedContextData);
            clone.ChangeOrder = this.ChangeOrder;
            clone.entity = this.entity;
            clone.EntityClrType = this.EntityClrType;
            clone.EntitySetName = this.EntitySetName;
            clone.ETag = this.ETag;
            clone.Identity = this.Identity;
            clone.linkInfos.AddRange(this.linkInfos.Select(l => l.Clone()));
            clone.operationDescriptors.AddRange(this.operationDescriptors.Select(o => o.Clone()));
            clone.streamDescriptors.AddRange(this.streamDescriptors.Select(n => n.Clone(clone)));
            clone.ParentForInsert = this.ParentForInsert;
            clone.ParentPropertyForInsert = this.ParentPropertyForInsert;
            clone.State = this.State;

            if (this.EditLink != null)
            {
                clone.EditLink = new Uri(this.EditLink.OriginalString, UriKind.RelativeOrAbsolute);
            }

            if (this.SelfLink != null)
            {
                clone.SelfLink = new Uri(this.SelfLink.OriginalString, UriKind.RelativeOrAbsolute);
            }

            if (this.InsertLink != null)
            {
                clone.InsertLink = new Uri(this.InsertLink.OriginalString, UriKind.RelativeOrAbsolute);
            }

            if (this.IsMediaLinkEntry)
            {
                clone.defaultStreamDescriptor = this.defaultStreamDescriptor.Clone(clone);
            }

            return clone;
        }

        /// <summary>
        /// Creates a new stream descriptor data and adds it to this entity descriptor data
        /// </summary>
        /// <param name="state">The state of the stream descriptor data</param>
        /// <param name="changeOrder">The change order for the stream descriptor data</param>
        /// <param name="name">The name of the stream</param>
        /// <returns>The stream descriptor data</returns>
        public StreamDescriptorData CreateStreamDescriptorData(EntityStates state, long changeOrder, string name)
        {
            ExceptionUtilities.CheckArgumentNotNull(name, "name");
            ExceptionUtilities.Assert(!this.streamDescriptors.Any(s => s.Name == name), "Stream descriptor data with name '{0}' already exists on entity descriptor data: {1}", name, this);

            var streamData = new StreamDescriptorData(this, name);
            this.contextData.ChangeStateAndChangeOrder(streamData, state, changeOrder);
            
            this.streamDescriptors.Add(streamData);

            return streamData;
        }

        /// <summary>
        /// Creates a new link info data and adds it to this entity descriptor data
        /// </summary>
        /// <param name="name">The name of the link</param>
        /// <returns>The link info data</returns>
        public LinkInfoData CreateLinkInfoData(string name)
        {
            ExceptionUtilities.CheckArgumentNotNull(name, "name");
            ExceptionUtilities.Assert(!this.linkInfos.Any(l => l.Name == name), "Link info data with name '{0}' already exists on entity descriptor data: {1}", name, this);

            var linkInfoData = new LinkInfoData(name);
            
            this.linkInfos.Add(linkInfoData);

            return linkInfoData;
        }

        /// <summary>
        /// Creates a new operation descriptor data and adds it to this entity descriptor data
        /// </summary>
        /// <param name="metadata">The metadata of the action.</param>
        /// <param name="target">The target of the action.</param>
        /// <param name="title">The title of the action.</param>
        /// <param name="isAction">Whether it's action or function.</param>
        /// <returns>The operation descriptor data</returns>
        public OperationDescriptorData CreateOperationDescriptorData(Uri metadata, Uri target, string title, bool isAction)
        {
            var operationDescriptorData = new OperationDescriptorData(metadata, target, title, isAction);

            this.operationDescriptors.Add(operationDescriptorData);

            return operationDescriptorData;
        }

        /// <summary>
        /// Removes the stream descriptor data with the given name
        /// </summary>
        /// <param name="name">The name of the descriptor to remove</param>
        public void RemoveStreamDescriptorData(string name)
        {
            var stream = this.streamDescriptors.SingleOrDefault(s => s.Name == name);
            ExceptionUtilities.CheckObjectNotNull(stream, "Could not find stream descriptor data with name '{0}' on entity data: {1}", name, this);
            this.streamDescriptors.Remove(stream);
        }

        /// <summary>
        /// Removes the link info data with the given name
        /// </summary>
        /// <param name="name">The name of the link to remove</param>
        public void RemoveLinkInfoData(string name)
        {
            var linkInfo = this.linkInfos.SingleOrDefault(l => l.Name == name);
            ExceptionUtilities.CheckObjectNotNull(linkInfo, "Could not find link info data with name '{0}' on entity data: {1}", name, this);
            this.linkInfos.Remove(linkInfo);
        }

        internal void RemoveStreamDescriptorDatas()
        {
            this.streamDescriptors.Clear();
        }

        internal void RemoveLinkInfoDatas()
        {
            this.linkInfos.Clear();
        }

        internal void RemoveOperationDescriptorData()
        {
            this.operationDescriptors.Clear();
        }
        
        private void InitializeDefaultStreamDescriptor()
        {
            if (this.defaultStreamDescriptor == null)
            {
                this.defaultStreamDescriptor = new StreamDescriptorData(this);
                this.defaultStreamDescriptor.State = EntityStates.Unchanged;
            }
        }
    }
}
