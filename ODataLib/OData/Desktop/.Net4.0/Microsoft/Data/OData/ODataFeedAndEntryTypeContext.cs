//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData
{
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Evaluation;
    using Microsoft.Data.OData.Metadata;

    /// <summary>
    /// The context object to answer basic questions regarding the type of the entry or feed.
    /// </summary>
    internal class ODataFeedAndEntryTypeContext : IODataFeedAndEntryTypeContext
    {
        /// <summary>
        /// Default Url convention.
        /// </summary>
        private static readonly UrlConvention DefaultUrlConvention = UrlConvention.CreateWithExplicitValue(/*generateKeyAsSegment*/ false);

        /// <summary>
        /// If true, throw if any of the set or type name cannot be determined; if false, return null when any of the set or type name cannot determined.
        /// </summary>
        private readonly bool throwIfMissingTypeInfo;

        /// <summary>
        /// Constructs an instance of <see cref="ODataFeedAndEntryTypeContext"/>.
        /// </summary>
        /// <param name="throwIfMissingTypeInfo">If true, throw if any of the set or type name cannot be determined; if false, return null when any of the set or type name cannot determined.</param>
        private ODataFeedAndEntryTypeContext(bool throwIfMissingTypeInfo)
        {
            this.throwIfMissingTypeInfo = throwIfMissingTypeInfo;
        }

        /// <summary>
        /// The entity set name of the feed or entry.
        /// </summary>
        public virtual string EntitySetName
        {
            get { return this.ValidateAndReturn(default(string)); }
        }

        /// <summary>
        /// The element type name of the entity set of the feed or entry.
        /// </summary>
        public virtual string EntitySetElementTypeName
        {
            get { return this.ValidateAndReturn(default(string)); }
        }

        /// <summary>
        /// The expected entity type name of the entry.
        /// For example, in the request URI 'http://example.com/Service.svc/People/Namespace.VIP_Person', the expected entity type is Namespace.VIP_Person.
        /// (The entity set element type name in this example may be Person, and the actual entity type of a particular entity might be a type more derived than VIP_Person)
        /// </summary>
        public virtual string ExpectedEntityTypeName
        {
            get { return null; }
        }

        /// <summary>
        /// true if the entry is an MLE, false otherwise.
        /// </summary>
        public virtual bool IsMediaLinkEntry
        {
            get { return false; }
        }

        /// <summary>
        /// The Url convention to use for the entity set.
        /// </summary>
        public virtual UrlConvention UrlConvention
        {
            get { return DefaultUrlConvention; }
        }

        /// <summary>
        /// Creates an instance of <see cref="ODataFeedAndEntryTypeContext"/>.
        /// </summary>
        /// <param name="serializationInfo">The serialization info from the feed or entry instance.</param>
        /// <param name="entitySet">The entity set of the feed or entry.</param>
        /// <param name="entitySetElementType">The element type of the entity set.</param>
        /// <param name="expectedEntityType">The expected entity type of the feed or entry.</param>
        /// <param name="model">The Edm model instance to use.</param>
        /// <param name="throwIfMissingTypeInfo">If true, throw if any of the set or type name cannot be determined; if false, return null when any of the set or type name cannot determined.</param>
        /// <returns>A new instance of <see cref="ODataFeedAndEntryTypeContext"/>.</returns>
        internal static ODataFeedAndEntryTypeContext Create(ODataFeedAndEntrySerializationInfo serializationInfo, IEdmEntitySet entitySet, IEdmEntityType entitySetElementType, IEdmEntityType expectedEntityType, IEdmModel model, bool throwIfMissingTypeInfo)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");
            if (serializationInfo != null)
            {
                return new ODataFeedAndEntryTypeContextWithoutModel(serializationInfo);
            }
            
            if (entitySet != null && model.IsUserModel())
            {
                Debug.Assert(entitySetElementType != null, "entitySetElementType != null");
                Debug.Assert(expectedEntityType != null, "expectedEntityType != null");
                return new ODataFeedAndEntryTypeContextWithModel(entitySet, entitySetElementType, expectedEntityType, model);
            }

            return new ODataFeedAndEntryTypeContext(throwIfMissingTypeInfo);
        }

        /// <summary>
        /// Validate and return the given value.
        /// </summary>
        /// <typeparam name="T">The type of the value to validate.</typeparam>
        /// <param name="value">The value to validate.</param>
        /// <returns>The return value.</returns>
        private T ValidateAndReturn<T>(T value) where T : class
        {
            if (this.throwIfMissingTypeInfo && value == null)
            {
                throw new ODataException(OData.Strings.ODataFeedAndEntryTypeContext_MetadataOrSerializationInfoMissing);
            }

            return value;
        }

        /// <summary>
        /// The context object to answer basic questions regarding the type of the entry or feed based on the serialization info.
        /// </summary>
        private sealed class ODataFeedAndEntryTypeContextWithoutModel : ODataFeedAndEntryTypeContext
        {
            /// <summary>
            /// The serialization info of the entry for writing without model.
            /// </summary>
            private readonly ODataFeedAndEntrySerializationInfo serializationInfo;

            /// <summary>
            /// Constructs an instance of <see cref="ODataFeedAndEntryTypeContext"/>.
            /// </summary>
            /// <param name="serializationInfo">The serialization info from the feed or entry instance.</param>
            internal ODataFeedAndEntryTypeContextWithoutModel(ODataFeedAndEntrySerializationInfo serializationInfo)
                : base(/*throwIfMissingTypeInfo*/false)
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(serializationInfo != null, "serializationInfo != null");
                this.serializationInfo = serializationInfo;
            }

            /// <summary>
            /// The entity set name of the feed or entry.
            /// </summary>
            public override string EntitySetName
            {
                get { return this.serializationInfo.EntitySetName; }
            }

            /// <summary>
            /// The element type name of the entity set of the feed or entry.
            /// </summary>
            public override string EntitySetElementTypeName
            {
                get { return this.serializationInfo.EntitySetElementTypeName; }
            }

            /// <summary>
            /// The expected entity type name of the entry.
            /// For example, in the request URI 'http://example.com/Service.svc/People/Namespace.VIP_Person', the expected entity type is Namespace.VIP_Person.
            /// (The entity set element type name in this example may be Person, and the actual entity type of a particular entity might be a type more derived than VIP_Person)
            /// </summary>
            public override string ExpectedEntityTypeName
            {
                get { return this.serializationInfo.ExpectedTypeName; }
            }

            /// <summary>
            /// true if the entry is an MLE, false otherwise.
            /// </summary>
            public override bool IsMediaLinkEntry
            {
                get
                {
                    // When writing without model, user should always set the ODataEntry.MediaResource property if the entry is a media link entry.
                    // This is consistent with the requirement for the with model scenario.
                    // Returning false here so the metadata builder won't create a new instance of the ODataStreamReferenceValue.
                    return false;
                }
            }

            /// <summary>
            /// The Url convention to use for the entity set.
            /// </summary>
            public override UrlConvention UrlConvention
            {
                get { return DefaultUrlConvention; }
            }
        }
        
        /// <summary>
        /// The context object to answer basic questions regarding the type of the entry or feed based on the metadata.
        /// </summary>
        private sealed class ODataFeedAndEntryTypeContextWithModel : ODataFeedAndEntryTypeContext
        {
            /// <summary>
            /// The Edm model instance to use.
            /// </summary>
            private readonly IEdmModel model;

            /// <summary>
            /// The entity set of the feed or entry.
            /// </summary>
            private readonly IEdmEntitySet entitySet;

            /// <summary>
            /// The element type of the entity set of the feed or entry.
            /// </summary>
            private readonly IEdmEntityType entitySetElementType;

            /// <summary>
            /// The expected entity type of the feed or entry.
            /// For example, in the request URI 'http://example.com/Service.svc/People/Namespace.VIP_Person', the expected entity type is Namespace.VIP_Person.
            /// (The entity set element type name in this example may be Person, and the actual entity type of a particular entity might be a type more derived than VIP_Person)
            /// </summary>
            private readonly IEdmEntityType expectedEntityType;

            /// <summary>
            /// The entity set name of the feed or entry.
            /// </summary>
            private readonly SimpleLazy<string> lazyEntitySetName;

            /// <summary>
            /// true if the entry is an media link entry or if the feed contains media link entries, false otherwise.
            /// </summary>
            private readonly SimpleLazy<bool> lazyIsMediaLinkEntry;

            /// <summary>
            /// The url convention to use for the entity set.
            /// </summary>
            private readonly SimpleLazy<UrlConvention> lazyUrlConvention;

            /// <summary>
            /// Constructs an instance of <see cref="ODataFeedAndEntryTypeContext"/>.
            /// </summary>
            /// <param name="entitySet">The entity set of the feed or entry.</param>
            /// <param name="entitySetElementType">The element type of the entity set.</param>
            /// <param name="expectedEntityType">The expected entity type of the feed or entry.</param>
            /// <param name="model">The Edm model instance to use.</param>
            internal ODataFeedAndEntryTypeContextWithModel(IEdmEntitySet entitySet, IEdmEntityType entitySetElementType, IEdmEntityType expectedEntityType, IEdmModel model)
                : base(/*throwIfMissingTypeInfo*/false)
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(model != null, "model != null");
                Debug.Assert(entitySet != null, "entitySet != null");
                Debug.Assert(entitySetElementType != null, "entitySetElementType != null");
                Debug.Assert(expectedEntityType != null, "expectedEntityType != null");

                this.entitySet = entitySet;
                this.entitySetElementType = entitySetElementType;
                this.expectedEntityType = expectedEntityType;
                this.model = model;

                this.lazyEntitySetName = new SimpleLazy<string>(() => this.model.IsDefaultEntityContainer(this.entitySet.Container) ? this.entitySet.Name : this.entitySet.Container.FullName() + "." + this.entitySet.Name);
                this.lazyIsMediaLinkEntry = new SimpleLazy<bool>(() => this.model.HasDefaultStream(this.expectedEntityType));
                this.lazyUrlConvention = new SimpleLazy<UrlConvention>(() => UrlConvention.ForEntityContainer(this.model, this.entitySet.Container));
            }

            /// <summary>
            /// The entity set name of the feed or entry.
            /// </summary>
            public override string EntitySetName
            {
                get { return this.lazyEntitySetName.Value; }
            }

            /// <summary>
            /// The element type name of the entity set of the feed or entry.
            /// </summary>
            public override string EntitySetElementTypeName
            {
                get { return this.entitySetElementType.FullName(); }
            }

            /// <summary>
            /// The expected entity type name of the entry.
            /// For example, in the request URI 'http://example.com/Service.svc/People/Namespace.VIP_Person', the expected entity type is Namespace.VIP_Person.
            /// (The entity set element type name in this example may be Person, and the actual entity type of a particular entity might be a type more derived than VIP_Person)
            /// </summary>
            public override string ExpectedEntityTypeName
            {
                get { return this.expectedEntityType.FullName(); }
            }

            /// <summary>
            /// true if the entry is an MLE, false otherwise.
            /// </summary>
            public override bool IsMediaLinkEntry
            {
                get { return this.lazyIsMediaLinkEntry.Value; }
            }

            /// <summary>
            /// The Url convention to use for the entity set.
            /// </summary>
            public override UrlConvention UrlConvention
            {
                get { return this.lazyUrlConvention.Value; }
            }
        }
    }
}
