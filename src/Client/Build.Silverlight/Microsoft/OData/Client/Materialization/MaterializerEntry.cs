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
    using Microsoft.OData.Client.Metadata;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// Materializer state for a given ODataEntry
    /// </summary>
    internal class MaterializerEntry
    {
        /// <summary>The entry.</summary>
        private readonly ODataEntry entry;

        /// <summary>entity descriptor object which keeps track of the entity state and other entity specific information.</summary>
        private readonly EntityDescriptor entityDescriptor;

        /// <summary>True if the context format is Atom or if the MergeOption is anything other than NoTracking.</summary>
        private readonly bool isAtomOrTracking;

        /// <summary>Entry flags.</summary>
        private EntryFlags flags;

        /// <summary>List of navigation links for this entry.</summary>
        private ICollection<ODataNavigationLink> navigationLinks = ODataMaterializer.EmptyLinks;

        /// <summary>
        /// Creates a new instance of MaterializerEntry.
        /// </summary>
        private MaterializerEntry()
        {
        }

        /// <summary>
        /// Creates a new instance of MaterializerEntry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="format">The format the entry was read in.</param>
        /// <param name="isTracking">True if the contents of the entry will be tracked in the context, otherwise False.</param>
        /// <param name="model">The client model.</param>
        private MaterializerEntry(ODataEntry entry, ODataFormat format, bool isTracking, ClientEdmModel model)
        {
            Debug.Assert(entry != null, "entry != null");

            this.entry = entry;
            this.Format = format;
            this.entityDescriptor = new EntityDescriptor(model);
#pragma warning disable 618
            this.isAtomOrTracking = isTracking || this.Format == ODataFormat.Atom;
#pragma warning restore 618
            string serverTypeName = this.Entry.TypeName;
            SerializationTypeNameAnnotation serializationTypeNameAnnotation = entry.GetAnnotation<SerializationTypeNameAnnotation>();
            if (serializationTypeNameAnnotation != null)
            {
                // If the annotation has a value use it. Otherwise, in JSON-Light, the types can be inferred from the
                // context URI even if they are not present on the wire, so just use the type name from the entry.
                if (serializationTypeNameAnnotation.TypeName != null || this.Format != ODataFormat.Json)
                {
                    serverTypeName = serializationTypeNameAnnotation.TypeName;
                }
            }

            this.entityDescriptor.ServerTypeName = serverTypeName;
        }

        /// <summary>
        /// Creates a new instance of MaterializerEntry using the given entity descriptor for LoadProperty.
        /// </summary>
        /// <param name="entityDescriptor">Entity descriptor.</param>
        /// <param name="format">OData Format.</param>
        /// <param name="isTracking">Whether this entity is being tracked.</param>
        /// <remarks>Use this constructor only for LoadProperty scenario.</remarks>
        private MaterializerEntry(EntityDescriptor entityDescriptor, ODataFormat format, bool isTracking)
        {
            this.entityDescriptor = entityDescriptor;
            this.Format = format;
#pragma warning disable 618
            this.isAtomOrTracking = isTracking || this.Format == ODataFormat.Atom;
#pragma warning restore 618
            this.SetFlagValue(EntryFlags.ShouldUpdateFromPayload | EntryFlags.EntityHasBeenResolved | EntryFlags.ForLoadProperty, true);
        }

        /// <summary>
        /// Masks used get/set the status of the entry
        /// </summary>
        [Flags]
        private enum EntryFlags
        {
            /// <summary>Bitmask for ShouldUpdateFromPayload flag.</summary>
            ShouldUpdateFromPayload = 0x01,

            /// <summary>Bitmask for CreatedByMaterializer flag.</summary>
            CreatedByMaterializer = 0x02,

            /// <summary>Bitmask for EntityHasBeenResolved flag.</summary>
            EntityHasBeenResolved = 0x04,

            /// <summary>Bitmask for MediaLinkEntry flag (value).</summary>
            EntityDescriptorUpdated = 0x08,

            /// <summary>Bitmask for LoadProperty scenario.</summary>
            ForLoadProperty = 0x10,
        }

        /// <summary>
        /// Gets the entry.
        /// </summary>
        public ODataEntry Entry
        {
            get { return this.entry; }
        }

        /// <summary>
        /// True if the context format is Atom or if the context's MergeOption is anything other than NoTracking.
        /// This is used to avoid building URI metadata information that is not needed outside of the context, such
        /// as odata.id and odata.editlink. Since this information is always available in the payload with Atom, for
        /// backward compatibility we continue using it as we always have, even for NoTracking cases.
        /// </summary>
        public bool IsAtomOrTracking
        {
            get { return this.isAtomOrTracking; }
        }

        /// <summary>
        /// Entry ID.
        /// </summary>
        public Uri Id
        {
            get
            {
                Debug.Assert(this.IsAtomOrTracking, "Id property should not be used when this.IsAtomOrTracking is false.");
                return this.entry.Id;
            }
        }

        /// <summary>
        /// Properties of the entry.
        /// </summary>
        /// <remarks>
        /// Non-property content goes to annotations.
        /// </remarks>
        public IEnumerable<ODataProperty> Properties
        {
            get { return this.entry != null ? this.entry.Properties : null; }
        }

        /// <summary>The entity descriptor.</summary>
        public EntityDescriptor EntityDescriptor
        {
            get { return this.entityDescriptor; }
        }

        /// <summary>Resolved object.</summary>
        public object ResolvedObject
        {
            get { return this.entityDescriptor != null ? this.entityDescriptor.Entity : null; }
            set { this.entityDescriptor.Entity = value; }
        }

        /// <summary>Actual type of the ResolvedObject.</summary>
        public ClientTypeAnnotation ActualType { get; set; }

        /// <summary>Whether values should be updated from payload.</summary>
        public bool ShouldUpdateFromPayload
        {
            get { return this.GetFlagValue(EntryFlags.ShouldUpdateFromPayload); }
            set { this.SetFlagValue(EntryFlags.ShouldUpdateFromPayload, value); }
        }

        /// <summary>Whether the entity has been resolved / created.</summary>
        public bool EntityHasBeenResolved
        {
            get { return this.GetFlagValue(EntryFlags.EntityHasBeenResolved); }
            set { this.SetFlagValue(EntryFlags.EntityHasBeenResolved, value); }
        }

        /// <summary>Whether the materializer has created the ResolvedObject instance.</summary>
        public bool CreatedByMaterializer
        {
            get { return this.GetFlagValue(EntryFlags.CreatedByMaterializer); }
            set { this.SetFlagValue(EntryFlags.CreatedByMaterializer, value); }
        }

        /// <summary>Is this entry created for LoadProperty.</summary>
        public bool ForLoadProperty
        {
            get { return this.GetFlagValue(EntryFlags.ForLoadProperty); }
        }

        /// <summary>The navigation links.</summary>
        public ICollection<ODataNavigationLink> NavigationLinks
        {
            get { return this.navigationLinks; }
        }

        /// <summary> Gets the format </summary>
        internal ODataFormat Format { get; private set; }

        /// <summary> Whether the entity descriptor has been updated.</summary>
        private bool EntityDescriptorUpdated
        {
            get { return this.GetFlagValue(EntryFlags.EntityDescriptorUpdated); }
            set { this.SetFlagValue(EntryFlags.EntityDescriptorUpdated, value); }
        }

        /// <summary>
        /// Creates an empty entry.
        /// </summary>
        /// <returns>An empty entry.</returns>
        public static MaterializerEntry CreateEmpty()
        {
            return new MaterializerEntry();
        }

        /// <summary>
        /// Creates the materializer entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="format">The format the entry was read in.</param>
        /// <param name="isTracking">True if the contents of the entry will be tracked in the context, otherwise False.</param>
        /// <param name="model">The client model.</param>
        /// <returns>A new materializer entry.</returns>
        public static MaterializerEntry CreateEntry(ODataEntry entry, ODataFormat format, bool isTracking, ClientEdmModel model)
        {
            Debug.Assert(entry.GetAnnotation<MaterializerEntry>() == null, "MaterializerEntry has already been created.");

            MaterializerEntry materializerEntry = new MaterializerEntry(entry, format, isTracking, model);
            entry.SetAnnotation<MaterializerEntry>(materializerEntry);

            return materializerEntry;
        }

        /// <summary>
        /// Creates the materializer entry for LoadProperty scenario.
        /// </summary>
        /// <param name="descriptor">The entity descriptor.</param>
        /// <param name="format">The format the entry was read in.</param>
        /// <param name="isTracking">True if the contents of the entry will be tracked in the context, otherwise False.</param>
        /// <returns>A new materializer entry.</returns>
        public static MaterializerEntry CreateEntryForLoadProperty(EntityDescriptor descriptor, ODataFormat format, bool isTracking)
        {
            return new MaterializerEntry(descriptor, format, isTracking);
        }

        /// <summary>
        /// Gets an entry for a given ODataEntry.
        /// </summary>
        /// <param name="entry">The ODataEntry.</param>
        /// <returns>The materializer entry</returns>
        public static MaterializerEntry GetEntry(ODataEntry entry)
        {
            return entry.GetAnnotation<MaterializerEntry>();
        }

        /// <summary>
        /// Adds a navigation link.
        /// </summary>
        /// <param name="link">The link.</param>
        public void AddNavigationLink(ODataNavigationLink link)
        {
            if (this.IsAtomOrTracking)
            {
                this.EntityDescriptor.AddNavigationLink(link.Name, link.Url);
                Uri associationLinkUrl = link.AssociationLinkUrl;
                if (associationLinkUrl != null)
                {
                    this.EntityDescriptor.AddAssociationLink(link.Name, associationLinkUrl);
                }
            }

            if (this.navigationLinks == ODataMaterializer.EmptyLinks)
            {
                this.navigationLinks = new List<ODataNavigationLink>();
            }

            this.navigationLinks.Add(link);
        }

        /// <summary>
        /// Updates the entity descriptor.
        /// </summary>
        public void UpdateEntityDescriptor()
        {
            if (!this.EntityDescriptorUpdated)
            {
                // Named stream properties are represented on the result type as a DataServiceStreamLink, which contains the
                // ReadLink and EditLink for the stream. We need to build this metadata information even with NoTracking,
                // because it is exposed on the result instances directly, not just in the context.
                foreach (ODataProperty property in this.Properties)
                {
                    ODataStreamReferenceValue streamValue = property.Value as ODataStreamReferenceValue;
                    if (streamValue != null)
                    {
                        StreamDescriptor streamInfo = this.EntityDescriptor.AddStreamInfoIfNotPresent(property.Name);

                        if (streamValue.ReadLink != null)
                        {
                            streamInfo.SelfLink = streamValue.ReadLink;
                        }

                        if (streamValue.EditLink != null)
                        {
                            streamInfo.EditLink = streamValue.EditLink;
                        }

                        streamInfo.ETag = streamValue.ETag;

                        streamInfo.ContentType = streamValue.ContentType;
                    }
                }

                if (this.IsAtomOrTracking)
                {
                    if (this.Id == null)
                    {
                        throw DSClient.Error.InvalidOperation(DSClient.Strings.Deserialize_MissingIdElement);
                    }

                    this.EntityDescriptor.Identity = this.entry.Id;
                    this.EntityDescriptor.EditLink = this.entry.EditLink;
                    this.EntityDescriptor.SelfLink = this.entry.ReadLink;
                    this.EntityDescriptor.ETag = this.entry.ETag;

                    if (this.entry.MediaResource != null)
                    {
                        if (this.entry.MediaResource.ReadLink != null)
                        {
                            this.EntityDescriptor.ReadStreamUri = this.entry.MediaResource.ReadLink;
                        }

                        if (this.entry.MediaResource.EditLink != null)
                        {
                            this.EntityDescriptor.EditStreamUri = this.entry.MediaResource.EditLink;
                        }

                        if (this.entry.MediaResource.ETag != null)
                        {
                            this.EntityDescriptor.StreamETag = this.entry.MediaResource.ETag;
                        }
                    }

                    if (this.entry.Functions != null)
                    {
                        foreach (ODataFunction function in this.entry.Functions)
                        {
                            this.EntityDescriptor.AddOperationDescriptor(new FunctionDescriptor { Title = function.Title, Metadata = function.Metadata, Target = function.Target });
                        }
                    }

                    if (this.entry.Actions != null)
                    {
                        foreach (ODataAction action in this.entry.Actions)
                        {
                            this.EntityDescriptor.AddOperationDescriptor(new ActionDescriptor { Title = action.Title, Metadata = action.Metadata, Target = action.Target });
                        }
                    }
                }

                this.EntityDescriptorUpdated = true;
            }
        }

        #region Private methods.

        /// <summary>Gets the value for a masked item.</summary>
        /// <param name="mask">Mask value.</param>
        /// <returns>true if the flag is set; false otherwise.</returns>
        private bool GetFlagValue(EntryFlags mask)
        {
            return (this.flags & mask) != 0;
        }

        /// <summary>Sets the value for a masked item.</summary>
        /// <param name="mask">Mask value.</param>
        /// <param name="value">Value to set</param>
        private void SetFlagValue(EntryFlags mask, bool value)
        {
            if (value)
            {
                this.flags |= mask;
            }
            else
            {
                this.flags &= (~mask);
            }
        }

        #endregion Private methods.
    }
}
