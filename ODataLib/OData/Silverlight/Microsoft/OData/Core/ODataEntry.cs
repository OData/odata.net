//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Evaluation;
    #endregion Namespaces

    /// <summary>
    /// Represents a single entity.
    /// </summary>
    [DebuggerDisplay("Id: {Id} TypeName: {TypeName}")]
    public sealed class ODataEntry : ODataItem
    {
        /// <summary>the metadata builder for this OData entry.</summary>
        private ODataEntityMetadataBuilder metadataBuilder;

        /// <summary>The entry ETag, as provided by the user or seen on the wire (never computed).</summary>
        private string etag;

        /// <summary>true if an etag was provided by the user or seen on the wire, false otherwise.</summary>
        private bool hasNonComputedETag;

        /// <summary>The Entry ID, as provided by the user or seen on the wire (never computed).</summary>
        private Uri id;

        /// <summary>true if an id was provided by the user or seen on the wire, false otherwise.</summary>
        private bool hasNonComputedId;

        /// <summary>Link used to edit the entry, as provided by the user or seen on the wire (never computed).</summary>
        private Uri editLink;

        /// <summary>true if an edit link was provided by the user or seen on the wire, false otherwise.</summary>
        private bool hasNonComputedEditLink;

        /// <summary>A link that can be used to read the entry, as provided by the user or seen on the wire (never computed).</summary>
        private Uri readLink;

        /// <summary>true if a read link was provided by the user or seen on the wire, false otherwise.</summary>
        private bool hasNonComputedReadLink;

        /// <summary>The default media resource of the media link entry, as provided by the user or seen on the wire (never computed).</summary>
        private ODataStreamReferenceValue mediaResource;

        /// <summary>The entry properties provided by the user or seen on the wire (never computed).</summary>
        private IEnumerable<ODataProperty> properties;
        
        /// <summary>The entry actions provided by the user or seen on the wire (never computed).</summary>
        private List<ODataAction> actions = new List<ODataAction>();

        /// <summary>The entry functions provided by the user or seen on the wire (never computed).</summary>
        private List<ODataFunction> functions = new List<ODataFunction>();

        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataWriter"/> for this <see cref="ODataEntry"/>.
        /// </summary>
        private ODataFeedAndEntrySerializationInfo serializationInfo;

        /// <summary>Gets or sets the entry ETag.</summary>
        /// <returns>The entry ETag.</returns>
        public string ETag
        {
            get
            {
                return this.MetadataBuilder.GetETag();
            }

            set
            {
                this.etag = value;
                this.hasNonComputedETag = true;
            }
        }

        /// <summary>Gets or sets the Entry identifier.</summary>
        /// <returns>The Entry identifier.</returns>
        public Uri Id
        {
            get
            {
                return this.MetadataBuilder.GetId();
            }

            set
            {
                this.id = value;
                this.hasNonComputedId = true;
            }
        }

        /// <summary>Gets or sets the link used to edit the entry.</summary>
        /// <returns>The link used to edit the entry.</returns>
        public Uri EditLink
        {
            get
            {
                return this.MetadataBuilder.GetEditLink();
            }

            set
            {
                this.editLink = value;
                this.hasNonComputedEditLink = true;
            }
        }

        /// <summary>Gets or sets the value that shows if the entry is a transient entry or not</summary>
        /// <returns>true if the entry is a transient entity, false otherwise.</returns>
        public bool IsTransient
        {
            get;
            set;
        }

        /// <summary>Gets or sets a link that can be used to read the entry.</summary>
        /// <returns>The link that can be used to read the entry.</returns>
        public Uri ReadLink
        {
            get
            {
                return this.MetadataBuilder.GetReadLink();
            }

            set
            {
                this.readLink = value;
                this.hasNonComputedReadLink = true;
            }
        }

        /// <summary>Gets or sets the default media resource of the media link entry.</summary>
        /// <returns>The default media resource of the media link entry.</returns>
        public ODataStreamReferenceValue MediaResource
        {
            get { return this.MetadataBuilder.GetMediaResource(); }
            set { this.mediaResource = value; }
        }

        /// <summary>Gets the entity actions.</summary>
        /// <returns>The entity actions.</returns>
        public IEnumerable<ODataAction> Actions
        {
            get { return this.MetadataBuilder.GetActions(); }
        }

        /// <summary>Gets the entity functions.</summary>
        /// <returns>The entity functions.</returns>
        public IEnumerable<ODataFunction> Functions
        {
            get { return this.MetadataBuilder.GetFunctions(); }
        }

        /// <summary>Gets or sets the entry properties.</summary>
        /// <returns>The entry properties.</returns>
        /// <remarks>
        /// Non-property content goes to annotations.
        /// </remarks>
        public IEnumerable<ODataProperty> Properties
        {
            get { return this.MetadataBuilder.GetProperties(this.properties); }
            set { this.properties = value; }
        }

        /// <summary>Gets or sets the type name of the entry.</summary>
        /// <returns>The type name of the entry.</returns>
        public string TypeName
        {
            get;
            set;
        }

        /// <summary>
        /// Collection of custom instance annotations.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "We want to allow the same instance annotation collection instance to be shared across ODataLib OM instances.")]
        public ICollection<ODataInstanceAnnotation> InstanceAnnotations
        {
            get { return this.GetInstanceAnnotations(); }
            set { this.SetInstanceAnnotations(value); }
        }

        /// <summary>
        /// The metadata builder for this OData entry.
        /// </summary>
        internal ODataEntityMetadataBuilder MetadataBuilder
        {
            get
            {
                if (this.metadataBuilder == null)
                {
                    this.metadataBuilder = new NoOpEntityMetadataBuilder(this);
                }

                return this.metadataBuilder;
            }

            set
            {
                Debug.Assert(value != null, "MetadataBuilder != null");
                this.metadataBuilder = value;
            }
        }

        /// <summary>
        /// Returns the entry's Id property that has been set directly, and was not computed using the metadata builder.
        /// </summary>
        internal Uri NonComputedId
        {
            get
            {
                return this.id;
            }
        }

        /// <summary>
        /// true if an id was provided by the user or seen on the wire, false otherwise.
        /// </summary>
        internal bool HasNonComputedId
        {
            get
            {
                return this.hasNonComputedId;
            }
        }

        /// <summary>
        /// Returns the entry's EditLink property that has been set directly, and was not computed using the metadata builder.
        /// </summary>
        internal Uri NonComputedEditLink
        {
            get
            {
                return this.editLink;
            }
        }

        /// <summary>
        /// true if an edit link was provided by the user or seen on the wire, false otherwise.
        /// </summary>
        internal bool HasNonComputedEditLink
        {
            get
            {
                return this.hasNonComputedEditLink;
            }
        }

        /// <summary>
        /// Returns the entry's ReadLink property that has been set directly, and was not computed using the metadata builder.
        /// </summary>
        internal Uri NonComputedReadLink
        {
            get
            {
                return this.readLink;
            }
        }

        /// <summary>
        /// true if a read link was provided by the user or seen on the wire, false otherwise.
        /// </summary>
        internal bool HasNonComputedReadLink
        {
            get
            {
                return this.hasNonComputedReadLink;
            }
        }

        /// <summary>
        /// Returns the entry's ETag property that has been set directly, and was not computed using the metadata builder.
        /// </summary>
        internal string NonComputedETag
        {
            get
            {
                return this.etag;
            }
        }

        /// <summary>
        /// true if an etag was provided by the user or seen on the wire, false otherwise.
        /// </summary>
        internal bool HasNonComputedETag
        {
            get
            {
                return this.hasNonComputedETag;
            }
        }

        /// <summary>Returns the default media resource of the media link entry that has been set directly and was not computed using the metadata builder.</summary>
        internal ODataStreamReferenceValue NonComputedMediaResource
        {
            get
            {
                return this.mediaResource;
            }
        }

        /// <summary>Returns the entity properties that has been set directly and was not computed using the metadata builder.</summary>
        internal IEnumerable<ODataProperty> NonComputedProperties
        {
            get
            {
                return this.properties;
            }
        }

        /// <summary>Returns the entity actions that has been set directly and was not computed using the metadata builder.</summary>
        internal IEnumerable<ODataAction> NonComputedActions
        {
            get
            {
                return this.actions == null ? null : new ReadOnlyEnumerable<ODataAction>(this.actions);
            }
        }

        /// <summary>Returns the entity functions that has been set directly and was not computed using the metadata builder.</summary>
        internal IEnumerable<ODataFunction> NonComputedFunctions
        {
            get
            {
                return this.functions == null ? null : new ReadOnlyEnumerable<ODataFunction>(this.functions);
            }
        }


        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataWriter"/> for this <see cref="ODataEntry"/>.
        /// </summary>
        internal ODataFeedAndEntrySerializationInfo SerializationInfo
        {
            get
            {
                return this.serializationInfo;
            }

            set
            {
                this.serializationInfo = ODataFeedAndEntrySerializationInfo.Validate(value);
            }
        }

        /// <summary>
        /// Add action to entry.
        /// </summary>
        /// <param name="action">The action to add.</param>
        public void AddAction(ODataAction action)
        {
            ExceptionUtils.CheckArgumentNotNull(action, "action");
            if (!this.actions.Contains(action))
            {
                this.actions.Add(action);
            }
        }

        /// <summary>
        /// Add function to entry.
        /// </summary>
        /// <param name="function">The function to add.</param>
        public void AddFunction(ODataFunction function)
        {
            ExceptionUtils.CheckArgumentNotNull(function, "function");
            if (!this.functions.Contains(function))
            {
                this.functions.Add(function);
            }
        }
    }
}
