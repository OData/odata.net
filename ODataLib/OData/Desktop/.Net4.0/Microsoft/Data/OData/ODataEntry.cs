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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Evaluation;
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
        private string id;

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
        private IEnumerable<ODataAction> actions;

        /// <summary>The entry functions provided by the user or seen on the wire (never computed).</summary>
        private IEnumerable<ODataFunction> functions;

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
        public string Id
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

        /// <summary>Gets or sets the association links.</summary>
        /// <returns>The association links.</returns>
        public IEnumerable<ODataAssociationLink> AssociationLinks
        {
            get;
            set;
        }

        /// <summary>Gets or sets the entity actions.</summary>
        /// <returns>The entity actions.</returns>
        public IEnumerable<ODataAction> Actions
        {
            get { return this.MetadataBuilder.GetActions(); }
            set { this.actions = value; }
        }

        /// <summary>Gets or sets the entity functions.</summary>
        /// <returns>The entity functions.</returns>
        public IEnumerable<ODataFunction> Functions
        {
            get { return this.MetadataBuilder.GetFunctions(); }
            set { this.functions = value; }
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
                DebugUtils.CheckNoExternalCallers();
                if (this.metadataBuilder == null)
                {
                    this.metadataBuilder = new NoOpEntityMetadataBuilder(this);
                }

                return this.metadataBuilder;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(value != null, "MetadataBuilder != null");
                this.metadataBuilder = value;
            }
        }

        /// <summary>
        /// Returns the entry's Id property that has been set directly, and was not computed using the metadata builder.
        /// </summary>
        internal string NonComputedId
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
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
                DebugUtils.CheckNoExternalCallers();
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
                DebugUtils.CheckNoExternalCallers();
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
                DebugUtils.CheckNoExternalCallers();
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
                DebugUtils.CheckNoExternalCallers();
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
                DebugUtils.CheckNoExternalCallers();
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
                DebugUtils.CheckNoExternalCallers();
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
                DebugUtils.CheckNoExternalCallers();
                return this.hasNonComputedETag;
            }
        }

        /// <summary>Returns the default media resource of the media link entry that has been set directly and was not computed using the metadata builder.</summary>
        internal ODataStreamReferenceValue NonComputedMediaResource
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.mediaResource;
            }
        }

        /// <summary>Returns the entity properties that has been set directly and was not computed using the metadata builder.</summary>
        internal IEnumerable<ODataProperty> NonComputedProperties
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.properties;
            }
        }

        /// <summary>Returns the entity actions that has been set directly and was not computed using the metadata builder.</summary>
        internal IEnumerable<ODataAction> NonComputedActions
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.actions;
            }
        }

        /// <summary>Returns the entity functions that has been set directly and was not computed using the metadata builder.</summary>
        internal IEnumerable<ODataFunction> NonComputedFunctions
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.functions;
            }
        }


        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataWriter"/> for this <see cref="ODataEntry"/>.
        /// </summary>
        internal ODataFeedAndEntrySerializationInfo SerializationInfo
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.serializationInfo;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.serializationInfo = ODataFeedAndEntrySerializationInfo.Validate(value);
            }
        }
    }
}
