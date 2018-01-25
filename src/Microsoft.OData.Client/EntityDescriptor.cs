//---------------------------------------------------------------------
// <copyright file="EntityDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;

    /// <summary>
    /// represents the cached entity
    /// </summary>
    [DebuggerDisplay("State = {state}, Uri = {editLink}, Element = {entity.GetType().ToString()}")]
    public sealed class EntityDescriptor : Descriptor
    {
        #region Fields
        /// <summary>uri to identitfy the entity</summary>
        /// <remarks>&lt;atom:id&gt;identity&lt;/id&gt;</remarks>
        private Uri identity;

        /// <summary>entity</summary>
        private object entity;

        /// <summary>tracks information about the default stream, if any.</summary>
        private StreamDescriptor defaultStreamDescriptor;

        /// <summary>uri of the resource set to add the entity to during save</summary>
        private Uri addToUri;

        /// <summary>uri to query the entity</summary>
        /// <remarks>&lt;atom:link rel="self" href="queryLink" /&gt;</remarks>
        private Uri selfLink;

        /// <summary>uri to edit the entity. In case of deep add, this can also refer to the navigation property name.</summary>
        /// <remarks>&lt;atom:link rel="edit" href="editLink" /&gt;</remarks>
        private Uri editLink;

        /// <summary>
        /// Contains the LinkInfo (navigation and relationship links) for navigation properties
        /// </summary>
        private Dictionary<string, LinkInfo> relatedEntityLinks;

        /// <summary>
        /// entity descriptor instance which contains metadata from responses which haven't been fully processed/materialized yet.
        /// This is used only in non-batch SaveChanges scenario.
        /// </summary>
        private EntityDescriptor transientEntityDescriptor;

        /// <summary>List of named streams for this entity</summary>
        private Dictionary<string, StreamDescriptor> streamDescriptors;

        /// <summary>List of service operation descriptors for this entity.</summary>
        private List<OperationDescriptor> operationDescriptors;

        #endregion

        /// <summary>
        /// Create a new instance of Entity descriptor.
        /// </summary>
        /// <param name="model">The client model</param>
        internal EntityDescriptor(ClientEdmModel model)
            : base(EntityStates.Unchanged)
        {
            this.Model = model;
            this.PropertiesToSerialize = new HashSet<string>(StringComparer.Ordinal);
        }

        #region Properties

        /// <summary>Gets the URI that is the identity value of the entity.</summary>
        /// <returns>The <see cref="P:Microsoft.OData.Client.EntityDescriptor.Identity" /> property corresponds to the identity element of the entry that represents the entity in the Atom response.</returns>
        public Uri Identity
        {
            get
            {
                return this.identity;
            }

            internal set
            {
                this.identity = value;
                this.ParentForInsert = null;
                this.ParentPropertyForInsert = null;
                this.ParentForUpdate = null;
                this.ParentPropertyForUpdate = null;
                this.addToUri = null;

                this.identity = value;
            }
        }

        /// <summary>Gets the URI that is used to return the entity resource.</summary>
        /// <returns>A URI that returns the entity.</returns>
        public Uri SelfLink
        {
            get
            {
                Debug.Assert(this.selfLink == null || this.selfLink.IsAbsoluteUri, "this.selfLink == null || this.selfLink.IsAbsoluteUri");
                return this.selfLink;
            }

            internal set
            {
                this.selfLink = value;
            }
        }

        /// <summary>Gets the URI that modifies the entity.</summary>
        /// <returns>The edit link URI for the entity resource.</returns>
        public Uri EditLink
        {
            get
            {
                Debug.Assert(this.editLink == null || this.editLink.IsAbsoluteUri, "this.editLink == null || this.editLink.IsAbsoluteUri");
                return this.editLink;
            }

            internal set
            {
                this.editLink = value;
            }
        }

        /// <summary>Gets the URI that accesses the binary property data of the entity.</summary>
        /// <returns>A URI that accesses the binary property as a stream.</returns>
        /// <remarks>
        /// If the entity for the box is an MLE this property stores the content source URI of the MLE.
        /// That is, it stores the read URI for the associated MR.
        /// Setting it to non-null marks the entity as MLE.
        /// </remarks>
        public Uri ReadStreamUri
        {
            get
            {
                return this.defaultStreamDescriptor != null ? this.defaultStreamDescriptor.SelfLink : null;
            }

            internal set
            {
                if (value != null)
                {
                    this.CreateDefaultStreamDescriptor().SelfLink = value;
                }
            }
        }

        /// <summary>Gets the URI that modifies the binary property data of the entity.</summary>
        /// <returns>The <see cref="P:Microsoft.OData.Client.EntityDescriptor.EditStreamUri" /> property contains the edit-media link URI for the Media Resource that is associated with the entity, which is a Media Link Entry.</returns>
        /// <remarks>
        /// If the entity for the box is an MLE this property stores the edit-media link URI.
        /// That is, it stores the URI to send PUTs for the associated MR.
        /// Setting it to non-null marks the entity as MLE.
        /// </remarks>
        public Uri EditStreamUri
        {
            get
            {
                return this.defaultStreamDescriptor != null ? this.defaultStreamDescriptor.EditLink : null;
            }

            internal set
            {
                if (value != null)
                {
                    this.CreateDefaultStreamDescriptor().EditLink = value;
                }
            }
        }

        /// <summary>Gets the entity that contains the update data.</summary>
        /// <returns>An object that contains update data.</returns>
        public object Entity
        {
            get
            {
                return this.entity;
            }

            internal set
            {
                Debug.Assert(this.entity == null, "The entity instance must be set only once");
                this.entity = value;

                if (value != null)
                {
                    IEdmType edmType = this.Model.GetOrCreateEdmType(value.GetType());
                    ClientTypeAnnotation clientTypeAnnotation = this.Model.GetClientTypeAnnotation(edmType);

                    this.EdmValue = new ClientEdmStructuredValue(value, this.Model, clientTypeAnnotation);

                    if (clientTypeAnnotation.IsMediaLinkEntry)
                    {
                        this.CreateDefaultStreamDescriptor();
                    }
                }
            }
        }

        /// <summary>Gets an eTag value that indicates the state of data targeted for update since the last call to <see cref="M:Microsoft.OData.Client.DataServiceContext.SaveChanges" />.</summary>
        /// <returns>The state of data.</returns>
        public string ETag { get; set; }

        /// <summary>Gets the eTag for the media resource associated with an entity that is a media link entry.</summary>
        /// <returns>A string value that indicates the state of data.</returns>
        public string StreamETag
        {
            get
            {
                return this.defaultStreamDescriptor != null ? this.defaultStreamDescriptor.ETag : null;
            }

            internal set
            {
                this.CreateDefaultStreamDescriptor().ETag = value;
            }
        }

        /// <summary>Gets the parent entity that is related to the entity.</summary>
        /// <returns>An object that is the parent entity in the relationship link.</returns>
        /// <remarks>This is only set for entities added through AddRelateObject call</remarks>
        public EntityDescriptor ParentForInsert { get; internal set; }

        /// <summary>Gets the name of the property of the entity that is a navigation property and links to the parent entity.</summary>
        /// <returns>The name of the parent property.</returns>
        public string ParentPropertyForInsert { get; internal set; }

        /// <summary>Gets the parent entity that is related to the entity.</summary>
        /// <returns>An object that is the parent entity in the relationship link.</returns>
        /// <remarks>This is only set for entities updated through UpdateRelateObject call</remarks>
        public EntityDescriptor ParentForUpdate { get; internal set; }

        /// <summary>Gets the name of the property of the entity that is a navigation property and links to the parent entity.</summary>
        /// <returns>The name of the parent property.</returns>
        public string ParentPropertyForUpdate { get; internal set; }

        /// <summary>Gets the name of the type in the data source to which the entity is mapped.</summary>
        /// <returns>A string that is the name of the data type.</returns>
        public String ServerTypeName { get; internal set; }

        /// <summary>Returns a collection of links that are the relationships in which the entity participates.</summary>
        /// <returns>A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of <see cref="T:Microsoft.OData.Client.LinkInfo" /> objects that represents links in which the entity participates.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "LinkInfoCollection is cumbersome and Links isn't expressive enough")]
        public ReadOnlyCollection<LinkInfo> LinkInfos
        {
            get
            {
                return this.relatedEntityLinks == null ? new ReadOnlyCollection<LinkInfo>(new List<LinkInfo>(0)) : new ReadOnlyCollection<LinkInfo>(this.relatedEntityLinks.Values.ToList());
            }
        }

        /// <summary>Returns a collection of named binary data streams that belong to the entity.</summary>
        /// <returns>A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of <see cref="T:Microsoft.OData.Client.StreamDescriptor" /> objects that are the named binary data streams that belong to the entity.</returns>
        public ReadOnlyCollection<StreamDescriptor> StreamDescriptors
        {
            get
            {
                return this.streamDescriptors == null ? new ReadOnlyCollection<StreamDescriptor>(new List<StreamDescriptor>(0)) : new ReadOnlyCollection<StreamDescriptor>(this.streamDescriptors.Values.ToList());
            }
        }

        /// <summary>Gets a collection of operation descriptors associated with the entity.</summary>
        /// <returns>A collection of operation descriptors associated with the entity.</returns>
        public ReadOnlyCollection<OperationDescriptor> OperationDescriptors
        {
            get
            {
                return this.operationDescriptors == null ? new ReadOnlyCollection<OperationDescriptor>(new List<OperationDescriptor>()) : new ReadOnlyCollection<OperationDescriptor>(this.operationDescriptors);
            }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets the client model.
        /// </summary>
        internal ClientEdmModel Model { get; private set; }

        /// <summary>Parent entity</summary>
        internal object ParentEntity
        {
            get
            {
                return this.ParentEntityDescriptor != null ? this.ParentEntityDescriptor.entity : null;
            }
        }

        /// <summary>Parent entity descriptor</summary>
        internal EntityDescriptor ParentEntityDescriptor
        {
            get { return this.ParentForInsert ?? this.ParentForUpdate; }
        }

        /// <summary>Gets the name of the property of the entity that is a navigation property and links to the parent entity. </summary>
        internal string ParentProperty
        {
            get { return !string.IsNullOrEmpty(this.ParentPropertyForInsert) ? this.ParentPropertyForInsert : (!string.IsNullOrEmpty(this.ParentPropertyForUpdate) ? this.ParentPropertyForUpdate : null); }
        }

        /// <summary>this is a entity</summary>
        internal override DescriptorKind DescriptorKind
        {
            get { return DescriptorKind.Entity; }
        }

        /// <summary>
        /// Returns true if the resource was inserted via its parent. E.g. POST customer(0)/Orders
        /// </summary>
        internal bool IsDeepInsert
        {
            get { return this.ParentForInsert != null; }
        }

        /// <summary>
        /// The stream which contains the new content for the MR associated with this MLE.
        /// This stream is used during SaveChanges to POST/PUT the MR.
        /// Setting it to non-null marks the entity as MLE.
        /// </summary>
        internal DataServiceSaveStream SaveStream
        {
            get
            {
                return this.defaultStreamDescriptor != null ? this.defaultStreamDescriptor.SaveStream : null;
            }

            set
            {
                Debug.Assert(value != null, "SaveStream should never be set to null");
                this.CreateDefaultStreamDescriptor().SaveStream = value;
            }
        }

        /// <summary>
        /// Describes whether the SaveStream is for Insert or Update.
        /// The value NoStream is for both non-MLEs and MLEs with unmodified stream.
        /// </summary>
        internal EntityStates StreamState
        {
            get
            {
                return this.defaultStreamDescriptor != null ? this.defaultStreamDescriptor.State : EntityStates.Unchanged;
            }

            set
            {
                Debug.Assert(this.defaultStreamDescriptor != null, "this.defaultStreamDescriptor != null");
                this.defaultStreamDescriptor.State = value;
            }
        }

        /// <summary>
        /// Returns true if we know that the entity is MLE. Note that this does not include the information
        /// from the entity type. So if the entity was attributed with HasStream for example
        /// this boolean might not be aware of it.
        /// </summary>
        internal bool IsMediaLinkEntry
        {
            get { return this.defaultStreamDescriptor != null; }
        }

        /// <summary>
        /// Returns true if the entry has been modified (and thus should participate in SaveChanges).
        /// </summary>
        internal override bool IsModified
        {
            get
            {
                if (base.IsModified)
                {
                    return true;
                }
                else
                {
                    // If the entity is not modified but it does have a save stream associated with it
                    // it means that the MR for the MLE should be updated and thus we need to consider
                    // the entity as modified (so that it shows up during SaveChanges)
                    return this.defaultStreamDescriptor != null && this.defaultStreamDescriptor.SaveStream != null;
                }
            }
        }

        /// <summary>
        /// entity descriptor instance containing metadata from responses, which hasn't been fully processed yet.
        /// </summary>
        internal EntityDescriptor TransientEntityDescriptor
        {
            get
            {
                return this.transientEntityDescriptor;
            }

            set
            {
                Debug.Assert(value != null, "should never try to set null transient entity descriptors");
                Debug.Assert(value.Entity == null, "for transient entity descriptors, entity must be null.");

                if (this.transientEntityDescriptor == null)
                {
                    this.transientEntityDescriptor = value;
                }
                else
                {
                    // we should merge the data always, as we do in the query case. There might be servers, who might send partial data back.
                    AtomMaterializerLog.MergeEntityDescriptorInfo(this.transientEntityDescriptor, value, true /*mergeInfo*/, MergeOption.OverwriteChanges);
                }

                // During save changes call, BaseSaveResult.ChangeEntries contains the list of descriptors which are changed.
                // Since named streams changes show up in this list as StreamDescriptor, we need to update the StreamDescriptor
                // instance to have their individual transient StreamDescriptor.
                if (value.streamDescriptors != null && this.streamDescriptors != null)
                {
                    foreach (StreamDescriptor transientStreamInfo in value.streamDescriptors.Values)
                    {
                        StreamDescriptor existingStreamInfo;
                        if (this.streamDescriptors.TryGetValue(transientStreamInfo.Name, out existingStreamInfo))
                        {
                            existingStreamInfo.TransientNamedStreamInfo = transientStreamInfo;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the stream descriptor for the default stream associated with this entity.
        /// If this entity is not an MLE, then returns null;
        /// </summary>
        internal StreamDescriptor DefaultStreamDescriptor
        {
            get { return this.defaultStreamDescriptor; }
        }

        /// <summary>
        /// Gets the edm structured value associated with this entity.
        /// </summary>
        internal IEdmStructuredValue EdmValue { get; private set; }

        /// <summary>
        /// The entity set name provided in either AttachTo or AddObject.
        /// </summary>
        internal string EntitySetName { get; set; }

        /// <summary>
        ///  The hash set contains names of changed properties in this entity.
        /// </summary>
        internal HashSet<string> PropertiesToSerialize { get; set; }

        #endregion

        #region Internal Methods

        /// <summary>
        /// returns the most recent identity of the entity
        /// </summary>
        /// <returns>the identity of the entity, as returned in the latest response.</returns>
        internal Uri GetLatestIdentity()
        {
            if (this.TransientEntityDescriptor != null && this.TransientEntityDescriptor.Identity != null)
            {
                return this.TransientEntityDescriptor.Identity;
            }

            return this.Identity;
        }

        /// <summary>return the most recent edit link for the entity</summary>
        /// <returns>the uri to edit the entity associated with the entity descriptor.</returns>
        internal Uri GetLatestEditLink()
        {
            if (this.TransientEntityDescriptor != null && this.TransientEntityDescriptor.EditLink != null)
            {
                return this.TransientEntityDescriptor.EditLink;
            }

            return this.EditLink;
        }

        /// <summary>return the most recent edit link for the MR associated with the entity</summary>
        /// <returns>the uri to edit the MR associated with the entity descriptor.</returns>
        internal Uri GetLatestEditStreamUri()
        {
            if (this.TransientEntityDescriptor != null && this.TransientEntityDescriptor.EditStreamUri != null)
            {
                return this.TransientEntityDescriptor.EditStreamUri;
            }

            return this.EditStreamUri;
        }

        /// <summary>return the most recent etag for the entity</summary>
        /// <returns>etag for the entity associated with the entity descriptor.</returns>
        internal string GetLatestETag()
        {
            if (this.TransientEntityDescriptor != null && !String.IsNullOrEmpty(this.TransientEntityDescriptor.ETag))
            {
                return this.TransientEntityDescriptor.ETag;
            }

            return this.ETag;
        }

        /// <summary>return the most return etag for the MR associated with the entity</summary>
        /// <returns>etag for the MR associated with the entity descriptor.</returns>
        internal string GetLatestStreamETag()
        {
            if (this.TransientEntityDescriptor != null && !String.IsNullOrEmpty(this.TransientEntityDescriptor.StreamETag))
            {
                return this.TransientEntityDescriptor.StreamETag;
            }

            return this.StreamETag;
        }

        /// <summary>return the most recent type name of the entity as returned in the response payload.</summary>
        /// <returns>the type name of the entity as returned in the response payload.</returns>
        internal string GetLatestServerTypeName()
        {
            if (this.TransientEntityDescriptor != null && !String.IsNullOrEmpty(this.TransientEntityDescriptor.ServerTypeName))
            {
                return this.TransientEntityDescriptor.ServerTypeName;
            }

            return this.ServerTypeName;
        }

        /// <summary>uri to edit the entity</summary>
        /// <param name="baseUriResolver">retrieves the baseUri to use for a given entity set.</param>
        /// <param name="queryLink">whether to return the query link or edit link</param>
        /// <returns>absolute uri which can be used to edit the entity</returns>
        internal Uri GetResourceUri(UriResolver baseUriResolver, bool queryLink)
        {
            // If the entity was inserted using the AddRelatedObject API
            if (this.ParentEntityDescriptor != null)
            {
                // This is the batch scenario, where the entity might not have been saved yet, and there is another operation
                // (for e.g. PUT $1/links/BestFriend or something). Hence we need to generate a Uri with the changeorder number.
                if (this.ParentEntityDescriptor.Identity == null)
                {
                    Uri relativeReferenceUri = UriUtil.CreateUri("$" + this.ParentEntityDescriptor.ChangeOrder.ToString(CultureInfo.InvariantCulture), UriKind.Relative);
                    Uri absoluteReferenceUri = baseUriResolver.GetOrCreateAbsoluteUri(relativeReferenceUri);
                    Uri requestUri = UriUtil.CreateUri(this.ParentProperty, UriKind.Relative);
                    return UriUtil.CreateUri(absoluteReferenceUri, requestUri);
                }
                else
                {
                    Debug.Assert(this.ParentEntityDescriptor.ParentEntityDescriptor == null, "This code assumes that parentChild relationships will only ever be one level deep");
                    Debug.Assert(this.ParentProperty != null, "ParentProperty != null");
                    LinkInfo linkInfo;
                    if (this.ParentEntityDescriptor.TryGetLinkInfo(this.ParentProperty, out linkInfo))
                    {
                        if (linkInfo.NavigationLink != null)
                        {
                            return linkInfo.NavigationLink;
                        }
                    }

                    return UriUtil.CreateUri(this.ParentEntityDescriptor.GetLink(queryLink), this.GetLink(queryLink));
                }
            }
            else
            {
                return this.GetLink(queryLink);
            }
        }

        /// <summary>is the entity the same as the source or target entity</summary>
        /// <param name="related">related end</param>
        /// <returns>true if same as source or target entity</returns>
        internal bool IsRelatedEntity(LinkDescriptor related)
        {
            return ((this.entity == related.Source) || (this.entity == related.Target));
        }

        /// <summary>
        /// Return the related end for this resource. One should call this method, only if the resource is inserted via deep resource.
        /// </summary>
        /// <returns>returns the related end via which the resource was inserted.</returns>
        internal LinkDescriptor GetRelatedEnd()
        {
            Debug.Assert(this.IsDeepInsert, "For related end, this must be a deep insert");
            Debug.Assert(this.Identity == null, "If the identity is set, it means that the edit link no longer has the property name");

            return new LinkDescriptor(this.ParentForInsert.entity, this.ParentPropertyForInsert, this.entity, this.Model);
        }

        /// <summary>
        /// clears all the changes - like closes the save stream, clears the transient entity descriptor.
        /// This method is called when the client is done with sending all the pending requests.
        /// </summary>
        internal override void ClearChanges()
        {
            this.transientEntityDescriptor = null;
            this.CloseSaveStream();
        }

        /// <summary>
        /// Closes the save stream if there's any and sets it to null
        /// </summary>
        internal void CloseSaveStream()
        {
            if (this.defaultStreamDescriptor != null)
            {
                this.defaultStreamDescriptor.CloseSaveStream();
            }
        }

        /// <summary>
        /// Add the given navigation link to the entity descriptor
        /// </summary>
        /// <param name="propertyName">name of the navigation property via which this entity is related to the other end.</param>
        /// <param name="navigationUri">uri that can be used to navigate from this entity to the other end.</param>
        internal void AddNestedResourceInfo(string propertyName, Uri navigationUri)
        {
            LinkInfo linkInfo = this.GetLinkInfo(propertyName);

            // There are scenarios where we need to overwrite an existing link (when someone tries to refresh the object)
            linkInfo.NavigationLink = navigationUri;
        }

        /// <summary>
        /// Add the given association link to the entity descriptor
        /// </summary>
        /// <param name="propertyName">name of the navigation property via which this entity is related to the other end.</param>
        /// <param name="associationUri">uri that can be used to navigate associations for this property.</param>
        internal void AddAssociationLink(string propertyName, Uri associationUri)
        {
            LinkInfo linkInfo = this.GetLinkInfo(propertyName);

            // There are scenarios where we need to overwrite an existing link (when someone tries to refresh the object)
            linkInfo.AssociationLink = associationUri;
        }

        /// <summary>
        /// Merges the given linkInfo to the entity descriptor,
        /// overwrites existing links with new ones (coming from the payload)
        /// </summary>
        /// <param name="linkInfo">linkInfo</param>
        internal void MergeLinkInfo(LinkInfo linkInfo)
        {
            if (this.relatedEntityLinks == null)
            {
                this.relatedEntityLinks = new Dictionary<string, LinkInfo>(StringComparer.Ordinal);
            }

            LinkInfo existingLinkInfo = null;
            if (!this.relatedEntityLinks.TryGetValue(linkInfo.Name, out existingLinkInfo))
            {
                this.relatedEntityLinks[linkInfo.Name] = linkInfo;
            }
            else
            {
                // overwrite existing links with new ones (coming from the payload).
                if (linkInfo.AssociationLink != null)
                {
                    existingLinkInfo.AssociationLink = linkInfo.AssociationLink;
                }

                if (linkInfo.NavigationLink != null)
                {
                    existingLinkInfo.NavigationLink = linkInfo.NavigationLink;
                }
            }
        }

        /// <summary>
        /// Try and get the navigation link. If the navigation link is not specified, then its used the self link of the entity and
        /// appends the property name.
        /// </summary>
        /// <param name="baseUriResolver">retrieves the appropriate baseUri for a given entitySet.</param>
        /// <param name="property">ClientProperty instance representing the navigation property.</param>
        /// <returns>returns the uri for the given link. If the link is not present, its uses the self link of the current entity and appends the navigation property name.</returns>
        internal Uri GetNestedResourceInfo(UriResolver baseUriResolver, ClientPropertyAnnotation property)
        {
            LinkInfo linkInfo = null;
            Uri uri = null;
            if (this.TryGetLinkInfo(property.PropertyName, out linkInfo))
            {
                uri = linkInfo.NavigationLink;
            }

            if (uri == null)
            {
                Uri relativeUri = UriUtil.CreateUri(property.PropertyName, UriKind.Relative);
                uri = UriUtil.CreateUri(this.GetResourceUri(baseUriResolver, true /*queryLink*/), relativeUri);
            }

            return uri;
        }

        /// <summary>
        /// Returns the LinkInfo for the given navigation property.
        /// </summary>
        /// <param name="propertyName">name of the navigation property </param>
        /// <param name="linkInfo"> LinkInfo for the navigation propery</param>
        /// <returns>true if LinkInfo is found for the navigation property, false if not found</returns>
        internal bool TryGetLinkInfo(string propertyName, out LinkInfo linkInfo)
        {
            Util.CheckArgumentNullAndEmpty(propertyName, "propertyName");
            Debug.Assert(propertyName.IndexOf('/') == -1, "propertyName.IndexOf('/') == -1");

            linkInfo = null;
            if (this.TransientEntityDescriptor != null && this.TransientEntityDescriptor.TryGetLinkInfo(propertyName, out linkInfo))
            {
                return true;
            }
            else if (this.relatedEntityLinks != null)
            {
                return this.relatedEntityLinks.TryGetValue(propertyName, out linkInfo);
            }

            return false;
        }

        /// <summary>
        /// Check if there is a stream with this name. If yes, returns the information about that stream, otherwise add a streams with the given name.
        /// </summary>
        /// <param name="name">name of the stream.</param>
        /// <returns>an existing or new namedstreaminfo instance with the given name.</returns>
        internal StreamDescriptor AddStreamInfoIfNotPresent(string name)
        {
            StreamDescriptor namedStreamInfo;
            if (this.streamDescriptors == null)
            {
                this.streamDescriptors = new Dictionary<string, StreamDescriptor>(StringComparer.Ordinal);
            }

            if (!this.streamDescriptors.TryGetValue(name, out namedStreamInfo))
            {
                namedStreamInfo = new StreamDescriptor(name, this);
                this.streamDescriptors.Add(name, namedStreamInfo);
            }

            return namedStreamInfo;
        }

        /// <summary>
        /// Adds an operation descriptor to the list of operation descriptors.
        /// </summary>
        /// <param name="operationDescriptor">the operation descriptor to add.</param>
        internal void AddOperationDescriptor(OperationDescriptor operationDescriptor)
        {
            Debug.Assert(operationDescriptor != null, "operationDescriptor != null");

            if (this.operationDescriptors == null)
            {
                this.operationDescriptors = new List<OperationDescriptor>();
            }

            // The protocol allows multiple descriptors with the same rel, so we don't check for duplicate entries here.
            this.operationDescriptors.Add(operationDescriptor);
        }

        /// <summary>
        /// Clears all operator descriptors
        /// </summary>
        internal void ClearOperationDescriptors()
        {
            if (this.operationDescriptors != null)
            {
                this.operationDescriptors.Clear();
            }
        }

        /// <summary>
        /// Appends OperationDescriptors to the existing list of OperationDescriptors
        /// </summary>
        /// <param name="descriptors">List containing OperationDescriptors to add for this entityDescriptor</param>
        internal void AppendOperationalDescriptors(IEnumerable<OperationDescriptor> descriptors)
        {
            if (this.operationDescriptors == null)
            {
                this.operationDescriptors = new List<OperationDescriptor>();
            }

            this.operationDescriptors.AddRange(descriptors);
        }

        /// <summary>
        /// Gets the stream info with the given name.
        /// </summary>
        /// <param name="name">name of the stream.</param>
        /// <param name="namedStreamInfo">information about the stream with the given name.</param>
        /// <returns>true if there is a stream with the given name, otherwise returns false.</returns>
        internal bool TryGetNamedStreamInfo(string name, out StreamDescriptor namedStreamInfo)
        {
            namedStreamInfo = null;

            if (this.streamDescriptors != null)
            {
                return this.streamDescriptors.TryGetValue(name, out namedStreamInfo);
            }

            return false;
        }

        /// <summary>
        /// Merges the given named stream info object.
        /// If the stream descriptor is already present, then this method merges the info from the given stream descriptor
        /// into the existing one, otherwise justs add this given stream descriptor to the list of stream descriptors for
        /// this entity.
        /// </summary>
        /// <param name="materializedStreamDescriptor">namedStreamInfo instance containing information about the stream.</param>
        internal void MergeStreamDescriptor(StreamDescriptor materializedStreamDescriptor)
        {
            if (this.streamDescriptors == null)
            {
                this.streamDescriptors = new Dictionary<string, StreamDescriptor>(StringComparer.Ordinal);
            }

            StreamDescriptor existingStreamDescriptor = null;
            if (!this.streamDescriptors.TryGetValue(materializedStreamDescriptor.Name, out existingStreamDescriptor))
            {
                this.streamDescriptors[materializedStreamDescriptor.Name] = materializedStreamDescriptor;
                materializedStreamDescriptor.EntityDescriptor = this;
            }
            else
            {
                StreamDescriptor.MergeStreamDescriptor(existingStreamDescriptor, materializedStreamDescriptor);
                Debug.Assert(ReferenceEquals(existingStreamDescriptor.EntityDescriptor, this), "All stream descriptors that are already tracked by the entity must point to the same entity descriptor instance");
            }
        }

        /// <summary>
        /// Sets up the descriptor's parent descriptor and parent property. Only valid if the descriptor is in the Added state.
        /// If the property ParentForUpdate and ParentPropertyForUpdate of descriptor has already been set, this method will also set value for these two properties to null.
        /// </summary>
        /// <param name="parentDescriptor">The parent descriptor.</param>
        /// <param name="propertyForInsert">The property for insert.</param>
        internal void SetParentForInsert(EntityDescriptor parentDescriptor, string propertyForInsert)
        {
            Debug.Assert(parentDescriptor != null, "parentDescriptor != null");
            Debug.Assert(!String.IsNullOrEmpty(propertyForInsert), "!string.IsNullOrEmpty(propertyForInsert)");
            Debug.Assert(this.State == EntityStates.Added, "State == EntityStates.Added");

            this.ParentForInsert = parentDescriptor;
            this.ParentPropertyForInsert = propertyForInsert;
            this.ParentForUpdate = null;
            this.ParentPropertyForUpdate = null;
        }

        /// <summary>
        /// Sets up the descriptor's parent descriptor and parent property for update. Only valid if the descriptor is in the Modified state.
        /// If the property ParentForInsert and ParentPropertyForInsert of descriptor has already been set, this method will also set value for these two properties to null.
        /// </summary>
        /// <param name="parentDescriptor">The parent descriptor.</param>
        /// <param name="propertyForUpdate">The property for update.</param>
        internal void SetParentForUpdate(EntityDescriptor parentDescriptor, string propertyForUpdate)
        {
            Debug.Assert(parentDescriptor != null, "parentDescriptor != null");
            Debug.Assert(!String.IsNullOrEmpty(propertyForUpdate), "!string.IsNullOrEmpty(propertyForUpdate)");
            Debug.Assert(this.State == EntityStates.Modified, "State == EntityStates.Modified");

            this.ParentForUpdate = parentDescriptor;
            this.ParentPropertyForUpdate = propertyForUpdate;
            this.ParentForInsert = null;
            this.ParentPropertyForInsert = null;
        }

        /// <summary>
        /// Sets the entity set URI to use for inserting the entity tracked by this descriptor. Only valid if the descriptor is in the added state.
        /// </summary>
        /// <param name="entitySetInsertUri">The entity set insert URI.</param>
        internal void SetEntitySetUriForInsert(Uri entitySetInsertUri)
        {
            Debug.Assert(entitySetInsertUri != null, "entitySetInsertUri != null");
            Debug.Assert(this.State == EntityStates.Added, "State == EntityStates.Added");

            this.addToUri = entitySetInsertUri;
        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        /// Returns LinkInfo for the given property, if it does not exists than a new one is created.
        /// </summary>
        /// <param name="propertyName">name of the navigation property</param>
        /// <returns>LinkInfo for propertyName</returns>
        private LinkInfo GetLinkInfo(String propertyName)
        {
            if (this.relatedEntityLinks == null)
            {
                this.relatedEntityLinks = new Dictionary<string, LinkInfo>(StringComparer.Ordinal);
            }

            LinkInfo linkInfo = null;
            if (!this.relatedEntityLinks.TryGetValue(propertyName, out linkInfo))
            {
                linkInfo = new LinkInfo(propertyName);
                this.relatedEntityLinks[propertyName] = linkInfo;
            }

            return linkInfo;
        }

        /// <summary>
        /// In V1, we used to not support self links. Hence we used to use edit links as self links.
        /// IN V2, we are adding support for self links. But if there are not specified, we need to
        /// fall back on the edit link.
        /// </summary>
        /// <param name="queryLink">whether to get query link or the edit link.</param>
        /// <returns>the query or the edit link, as specified in the <paramref name="queryLink"/> parameter.</returns>
        private Uri GetLink(bool queryLink)
        {
            // If asked for a self link and self-link is present, return self link
            Uri link;
            if (queryLink && this.SelfLink != null)
            {
                return this.SelfLink;
            }

            // otherwise return edit link if present.
            if ((link = this.GetLatestEditLink()) != null)
            {
                return link;
            }

            if (this.State != EntityStates.Added)
            {
                throw new ArgumentNullException(Strings.EntityDescriptor_MissingSelfEditLink(this.identity));
            }
            else
            {
                Debug.Assert(this.TransientEntityDescriptor == null, "The transient entity container must be null, when the entity is in added state");

                // If the entity is in added state, and either the parent property or the addToUri must be non-null
                Debug.Assert(this.addToUri != null || !String.IsNullOrEmpty(this.ParentPropertyForInsert), "For entities in added state, parentProperty or addToUri must be specified");
                if (this.addToUri != null)
                {
                    return this.addToUri;
                }
                else
                {
                    return UriUtil.CreateUri(this.ParentPropertyForInsert, UriKind.Relative);
                }
            }
        }

        /// <summary>
        /// Creates a default stream descriptor, if there is none yet, and returns it.
        /// If there is one already present, then returns the current instance.
        /// </summary>
        /// <returns>stream descriptor representing the default stream.</returns>
        private StreamDescriptor CreateDefaultStreamDescriptor()
        {
            if (this.defaultStreamDescriptor == null)
            {
                this.defaultStreamDescriptor = new StreamDescriptor(this);
            }

            return this.defaultStreamDescriptor;
        }

        #endregion Private Methods
    }
}
