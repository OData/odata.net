//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    using System.Diagnostics;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Evaluation;

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
        /// The navigation source name of the feed or entry.
        /// </summary>
        public virtual string NavigationSourceName
        {
            get { return this.ValidateAndReturn(default(string)); }
        }

        /// <summary>
        /// The entity type name of the navigation source of the feed or entry.
        /// </summary>
        public virtual string NavigationSourceEntityTypeName
        {
            get { return this.ValidateAndReturn(default(string)); }
        }

        /// <summary>
        /// The kind of the navigation source of the feed or entry.
        /// </summary>
        public virtual EdmNavigationSourceKind NavigationSourceKind
        {
            get { return EdmNavigationSourceKind.None; }
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
        /// The type kind of the entity type of the feed or entry.
        /// </summary>
        public virtual bool IsFromCollection
        {
            get { return false; }
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
        /// <param name="navigationSource">The navigation source of the feed or entry.</param>
        /// <param name="navigationSourceEntityType">The entity type of the navigation source.</param>
        /// <param name="expectedEntityType">The expected entity type of the feed or entry.</param>
        /// <param name="model">The Edm model instance to use.</param>
        /// <param name="throwIfMissingTypeInfo">If true, throw if any of the set or type name cannot be determined; if false, return null when any of the set or type name cannot determined.</param>
        /// <returns>A new instance of <see cref="ODataFeedAndEntryTypeContext"/>.</returns>
        internal static ODataFeedAndEntryTypeContext Create(ODataFeedAndEntrySerializationInfo serializationInfo, IEdmNavigationSource navigationSource, IEdmEntityType navigationSourceEntityType, IEdmEntityType expectedEntityType, IEdmModel model, bool throwIfMissingTypeInfo)
        {
            Debug.Assert(model != null, "model != null");
            if (serializationInfo != null)
            {
                return new ODataFeedAndEntryTypeContextWithoutModel(serializationInfo);
            }

            if (navigationSource != null && model.IsUserModel())
            {
                Debug.Assert(navigationSourceEntityType != null, "navigationSourceEntityType != null");
                Debug.Assert(expectedEntityType != null, "expectedEntityType != null");
                return new ODataFeedAndEntryTypeContextWithModel(navigationSource, navigationSourceEntityType, expectedEntityType, model);
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
                throw new ODataException(OData.Core.Strings.ODataFeedAndEntryTypeContext_MetadataOrSerializationInfoMissing);
            }

            return value;
        }

        /// <summary>
        /// The context object to answer basic questions regarding the type of the entry or feed based on the serialization info.
        /// </summary>
        internal sealed class ODataFeedAndEntryTypeContextWithoutModel : ODataFeedAndEntryTypeContext
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
                Debug.Assert(serializationInfo != null, "serializationInfo != null");
                this.serializationInfo = serializationInfo;
            }

            /// <summary>
            /// The navigation source name of the feed or entry.
            /// </summary>
            public override string NavigationSourceName
            {
                get { return this.serializationInfo.NavigationSourceName; }
            }

            /// <summary>
            /// The entity type name of the navigation source of the feed or entry.
            /// </summary>
            public override string NavigationSourceEntityTypeName
            {
                get { return this.serializationInfo.NavigationSourceEntityTypeName; }
            }

            /// <summary>
            /// The kind of the navigation source of the feed or entry.
            /// </summary>
            public override EdmNavigationSourceKind NavigationSourceKind
            {
                get { return this.serializationInfo.NavigationSourceKind; }
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

            /// <summary>
            /// The type kind of the entity type of the feed or entry.
            /// </summary>
            public override bool IsFromCollection
            {
                get { return this.serializationInfo.IsFromCollection; }
            }
        }

        /// <summary>
        /// The context object to answer basic questions regarding the type of the entry or feed based on the metadata.
        /// </summary>
        internal sealed class ODataFeedAndEntryTypeContextWithModel : ODataFeedAndEntryTypeContext
        {
            /// <summary>
            /// The Edm model instance to use.
            /// </summary>
            private readonly IEdmModel model;

            /// <summary>
            /// The navigation source of the feed or entry.
            /// </summary>
            private readonly IEdmNavigationSource navigationSource;

            /// <summary>
            /// The entity type of the navigation source of the feed or entry.
            /// </summary>
            private readonly IEdmEntityType navigationSourceEntityType;

            /// <summary>
            /// The expected entity type of the feed or entry.
            /// For example, in the request URI 'http://example.com/Service.svc/People/Namespace.VIP_Person', the expected entity type is Namespace.VIP_Person.
            /// (The entity set element type name in this example may be Person, and the actual entity type of a particular entity might be a type more derived than VIP_Person)
            /// </summary>
            private readonly IEdmEntityType expectedEntityType;

            /// <summary>
            /// The navigation source name of the feed or entry.
            /// </summary>
            private readonly string navigationSourceName;

            /// <summary>
            /// true if the entry is an media link entry or if the feed contains media link entries, false otherwise.
            /// </summary>
            private readonly bool isMediaLinkEntry;

            /// <summary>
            /// The url convention to use for the entity set.
            /// </summary>
            private readonly SimpleLazy<UrlConvention> lazyUrlConvention;

            /// <summary>
            /// The flag we use to identify if the current entry is from a navigation property with collection type or not.
            /// </summary>
            private readonly bool isFromCollection = false;

            /// <summary>
            /// Constructs an instance of <see cref="ODataFeedAndEntryTypeContext"/>.
            /// </summary>
            /// <param name="navigationSource">The navigation source of the feed or entry.</param>
            /// <param name="navigationSourceEntityType">The entity type of the navigation source.</param>
            /// <param name="expectedEntityType">The expected entity type of the feed or entry.</param>
            /// <param name="model">The Edm model instance to use.</param>
            internal ODataFeedAndEntryTypeContextWithModel(IEdmNavigationSource navigationSource, IEdmEntityType navigationSourceEntityType, IEdmEntityType expectedEntityType, IEdmModel model)
                : base(/*throwIfMissingTypeInfo*/false)
            {
                Debug.Assert(model != null, "model != null");
                Debug.Assert(navigationSource != null, "navigationSource != null");
                Debug.Assert(navigationSourceEntityType != null, "navigationSourceEntityType != null");
                Debug.Assert(expectedEntityType != null, "expectedEntityType != null");

                this.navigationSource = navigationSource;
                this.navigationSourceEntityType = navigationSourceEntityType;
                this.expectedEntityType = expectedEntityType;
                this.model = model;

                IEdmContainedEntitySet containedEntitySet = navigationSource as IEdmContainedEntitySet;
                if (containedEntitySet != null)
                {
                    if (containedEntitySet.NavigationProperty.Type.TypeKind() == EdmTypeKind.Collection)
                    {
                        this.isFromCollection = true;
                    }
                }

                this.navigationSourceName = this.navigationSource.Name;
                this.isMediaLinkEntry = this.expectedEntityType.HasStream;
                this.lazyUrlConvention = new SimpleLazy<UrlConvention>(() => UrlConvention.ForModel(this.model));
            }

            /// <summary>
            /// The navigation source name of the feed or entry.
            /// </summary>
            public override string NavigationSourceName
            {
                get { return this.navigationSourceName; }
            }

            /// <summary>
            /// The entity type name of the navigation source of the feed or entry.
            /// </summary>
            public override string NavigationSourceEntityTypeName
            {
                get { return this.navigationSourceEntityType.FullName(); }
            }

            /// <summary>
            /// The kind of the navigation source of the feed or entry.
            /// </summary>
            public override EdmNavigationSourceKind NavigationSourceKind
            {
                get { return this.navigationSource.NavigationSourceKind(); }
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
                get { return this.isMediaLinkEntry; }
            }

            /// <summary>
            /// The Url convention to use for the entity set.
            /// </summary>
            public override UrlConvention UrlConvention
            {
                get { return this.lazyUrlConvention.Value; }
            }

            /// <summary>
            /// The type kind of the entity type of the feed or entry.
            /// </summary>
            public override bool IsFromCollection
            {
                get { return this.isFromCollection; }
            }

            /// <summary>
            /// The entity type of the navigation source of the feed or entry.
            /// </summary>
            internal IEdmEntityType NavigationSourceEntityType
            {
                get { return this.navigationSourceEntityType; }
            }
        }
    }
}
