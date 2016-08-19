//---------------------------------------------------------------------
// <copyright file="ODataResourceTypeContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;

    /// <summary>
    /// The context object to answer basic questions regarding the type of the resource or resource set.
    /// </summary>
    internal class ODataResourceTypeContext : IODataResourceTypeContext
    {
        /// <summary>
        /// The expected resource type of the resource set or resource.
        /// </summary>
        protected IEdmStructuredType expectedResourceType;

        /// <summary>
        /// The expected resource type name of the resource set or resource.
        /// </summary>
        protected string expectedResourceTypeName;

        /// <summary>
        /// If true, throw if any of the set or type name cannot be determined; if false, return null when any of the set or type name cannot determined.
        /// </summary>
        private readonly bool throwIfMissingTypeInfo;

        /// <summary>
        /// Constructs an instance of <see cref="ODataResourceTypeContext"/>.
        /// </summary>
        /// <param name="throwIfMissingTypeInfo">If true, throw if any of the set or type name cannot be determined; if false, return null when any of the set or type name cannot determined.</param>
        private ODataResourceTypeContext(bool throwIfMissingTypeInfo)
        {
            this.throwIfMissingTypeInfo = throwIfMissingTypeInfo;
        }

        /// <summary>
        /// Constructs an instance of <see cref="ODataResourceTypeContext"/>.
        /// </summary>
        /// <param name="expectedResourceType">The expected resource type of resource set or resource.</param>
        /// <param name="throwIfMissingTypeInfo">If true, throw if any of the set or type name cannot be determined; if false, return null when any of the set or type name cannot determined.</param>
        private ODataResourceTypeContext(IEdmStructuredType expectedResourceType, bool throwIfMissingTypeInfo)
        {
            this.expectedResourceType = expectedResourceType;
            this.throwIfMissingTypeInfo = throwIfMissingTypeInfo;
        }

        /// <summary>
        /// The navigation source name of the resource set or resource.
        /// </summary>
        public virtual string NavigationSourceName
        {
            get { return this.ValidateAndReturn(default(string)); }
        }

        /// <summary>
        /// The entity type name of the navigation source of the resource set or resource.
        /// </summary>
        public virtual string NavigationSourceEntityTypeName
        {
            get { return this.ValidateAndReturn(default(string)); }
        }

        /// <summary>
        /// The full type name of the navigation source of the resource set or resource.
        /// </summary>
        public virtual string NavigationSourceFullTypeName
        {
            get { return this.ValidateAndReturn(default(string)); }
        }

        /// <summary>
        /// The kind of the navigation source of the resource set or resource.
        /// </summary>
        public virtual EdmNavigationSourceKind NavigationSourceKind
        {
            get { return EdmNavigationSourceKind.None; }
        }

        /// <summary>
        /// The expected entity type name of the resource.
        /// For example, in the request URI 'http://example.com/Service.svc/People/Namespace.VIP_Person', the expected entity type is Namespace.VIP_Person.
        /// (The entity set element type name in this example may be Person, and the actual entity type of a particular entity might be a type more derived than VIP_Person)
        /// </summary>
        public virtual string ExpectedResourceTypeName
        {
            get
            {
                if (this.expectedResourceTypeName == null)
                {
                    this.expectedResourceTypeName = this.expectedResourceType == null ? null : this.expectedResourceType.FullTypeName();
                }

                return this.expectedResourceTypeName;
            }
        }

        /// <summary>
        /// The expected resource type.
        /// For example, in the request URI 'http://example.com/Service.svc/People/Namespace.VIP_Person', the expected resource type is Namespace.VIP_Person.
        /// (The entity set element type name in this example may be Person, and the actual entity type of a particular entity might be a type more derived than VIP_Person)
        /// </summary>
        public virtual IEdmStructuredType ExpectedResourceType
        {
            get { return this.expectedResourceType; }
        }

        /// <summary>
        /// The flag we use to identify if the current resource is from a collection type or not.
        /// </summary>
        public virtual bool IsFromCollection
        {
            get { return false; }
        }

        /// <summary>
        /// true if the resource is an MLE, false otherwise.
        /// </summary>
        public virtual bool IsMediaLinkEntry
        {
            get { return false; }
        }

        /// <summary>
        /// Creates an instance of <see cref="ODataResourceTypeContext"/>.
        /// </summary>
        /// <param name="serializationInfo">The serialization info from the resource set or resource instance.</param>
        /// <param name="navigationSource">The navigation source of the resource set or resource.</param>
        /// <param name="navigationSourceEntityType">The entity type of the navigation source.</param>
        /// <param name="expectedResourceType">The expected structured type of the resource set or resource.</param>
        /// <param name="throwIfMissingTypeInfo">If true, throw if any of the set or type name cannot be determined; if false, return null when any of the set or type name cannot determined.</param>
        /// <returns>A new instance of <see cref="ODataResourceTypeContext"/>.</returns>
        internal static ODataResourceTypeContext Create(ODataResourceSerializationInfo serializationInfo, IEdmNavigationSource navigationSource, IEdmEntityType navigationSourceEntityType, IEdmStructuredType expectedResourceType, bool throwIfMissingTypeInfo)
        {
            if (serializationInfo != null)
            {
                return new ODataResourceTypeContextWithoutModel(serializationInfo);
            }

            // We are creating an ODataResourceTypeContext for a complex item with navigation source is null.
            if (expectedResourceType != null && expectedResourceType.IsODataComplexTypeKind())
            {
                return new ODataResourceTypeContextWithModel(null, null, expectedResourceType);
            }

            // We are creating an ODataResourceTypeContext for an item in Navigation Source(e.g. an entity set).
            if (navigationSource != null && expectedResourceType != null)
            {
                return new ODataResourceTypeContextWithModel(navigationSource, navigationSourceEntityType, expectedResourceType);
            }

            return new ODataResourceTypeContext(expectedResourceType, throwIfMissingTypeInfo);
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
                throw new ODataException(Strings.ODataResourceTypeContext_MetadataOrSerializationInfoMissing);
            }

            return value;
        }

        /// <summary>
        /// The context object to answer basic questions regarding the type of the resource or resource set based on the serialization info.
        /// </summary>
        internal sealed class ODataResourceTypeContextWithoutModel : ODataResourceTypeContext
        {
            /// <summary>
            /// The serialization info of the resource for writing without model.
            /// </summary>
            private readonly ODataResourceSerializationInfo serializationInfo;

            /// <summary>
            /// Constructs an instance of <see cref="ODataResourceTypeContext"/>.
            /// </summary>
            /// <param name="serializationInfo">The serialization info from the resource set or resource instance.</param>
            internal ODataResourceTypeContextWithoutModel(ODataResourceSerializationInfo serializationInfo)
                : base(/*throwIfMissingTypeInfo*/false)
            {
                Debug.Assert(serializationInfo != null, "serializationInfo != null");
                this.serializationInfo = serializationInfo;
            }

            /// <summary>
            /// The navigation source name of the resource set or resource.
            /// </summary>
            public override string NavigationSourceName
            {
                get { return this.serializationInfo.NavigationSourceName; }
            }

            /// <summary>
            /// The entity type name of the navigation source of the resource set or resource.
            /// </summary>
            public override string NavigationSourceEntityTypeName
            {
                get { return this.serializationInfo.NavigationSourceEntityTypeName; }
            }

            /// <summary>
            /// The full type name of the navigation source of the resource set or resource.
            /// </summary>
            public override string NavigationSourceFullTypeName
            {
                get
                {
                    if (this.IsFromCollection)
                    {
                        return EdmLibraryExtensions.GetCollectionTypeName(
                            this.serializationInfo.NavigationSourceEntityTypeName);
                    }
                    else
                    {
                        return this.serializationInfo.NavigationSourceEntityTypeName;
                    }
                }
            }

            /// <summary>
            /// The kind of the navigation source of the resource set or resource.
            /// </summary>
            public override EdmNavigationSourceKind NavigationSourceKind
            {
                get { return this.serializationInfo.NavigationSourceKind; }
            }

            /// <summary>
            /// The expected entity type name of the resource.
            /// For example, in the request URI 'http://example.com/Service.svc/People/Namespace.VIP_Person', the expected entity type is Namespace.VIP_Person.
            /// (The entity set element type name in this example may be Person, and the actual entity type of a particular entity might be a type more derived than VIP_Person)
            /// </summary>
            public override string ExpectedResourceTypeName
            {
                get { return this.serializationInfo.ExpectedTypeName; }
            }

            /// <summary>
            /// The expected resource type.
            /// For example, in the request URI 'http://example.com/Service.svc/People/Namespace.VIP_Person', the expected resource type is Namespace.VIP_Person.
            /// (The entity set element type name in this example may be Person, and the actual entity type of a particular entity might be a type more derived than VIP_Person)
            /// </summary>
            public override IEdmStructuredType ExpectedResourceType
            {
                get { return null; }
            }

            /// <summary>
            /// true if the resource is an MLE, false otherwise.
            /// </summary>
            public override bool IsMediaLinkEntry
            {
                get
                {
                    // When writing without model, user should always set the ODataResource.MediaResource property if the resource is a media link resource.
                    // This is consistent with the requirement for the with model scenario.
                    // Returning false here so the metadata builder won't create a new instance of the ODataStreamReferenceValue.
                    return false;
                }
            }

            /// <summary>
            /// The flag we use to identify if the current resource is from a collection type or not.
            /// </summary>
            public override bool IsFromCollection
            {
                get { return this.serializationInfo.IsFromCollection; }
            }
        }

        /// <summary>
        /// The context object to answer basic questions regarding the type of the resource or resource set based on the metadata.
        /// </summary>
        internal sealed class ODataResourceTypeContextWithModel : ODataResourceTypeContext
        {
            /// <summary>
            /// The navigation source of the entity set or entity.
            /// </summary>
            private readonly IEdmNavigationSource navigationSource;

            /// <summary>
            /// The entity type of the navigation source of the entity set or entity.
            /// </summary>
            private readonly IEdmEntityType navigationSourceEntityType;

            /// <summary>
            /// The navigation source name of the resource set or resource.
            /// </summary>
            private readonly string navigationSourceName;

            /// <summary>
            /// true if the resource is an media link resource or if the resource set contains media link entries, false otherwise.
            /// </summary>
            private readonly bool isMediaLinkEntry;

            /// <summary>
            /// The flag we use to identify if the current resource is from a navigation property with collection type or not.
            /// </summary>
            private readonly bool isFromCollection = false;

            /// <summary>
            /// The full type name of the navigation source of the entity set or entity.
            /// </summary>
            private string navigationSourceFullTypeName;

            /// <summary>
            /// The entity type name of the navigation source of the entity set or entity.
            /// </summary>
            private string navigationSourceEntityTypeName;

            /// <summary>
            /// Constructs an instance of <see cref="ODataResourceTypeContext"/>.
            /// </summary>
            /// <param name="navigationSource">The navigation source of the resource set or resource.</param>
            /// <param name="navigationSourceEntityType">The entity type of the navigation source.</param>
            /// <param name="expectedResourceType">The expected resource type of the resource set or resource.</param>
            internal ODataResourceTypeContextWithModel(IEdmNavigationSource navigationSource, IEdmEntityType navigationSourceEntityType, IEdmStructuredType expectedResourceType)
                : base(expectedResourceType, /*throwIfMissingTypeInfo*/false)
            {
                Debug.Assert(expectedResourceType != null, "expectedResourceType != null");
                Debug.Assert(navigationSource != null
                             && navigationSourceEntityType != null
                             || expectedResourceType.IsODataComplexTypeKind(),
                    "navigationSource != null && navigationSourceEntityType != null || expectedResourceType.IsODataComplexTypeKind()");

                this.navigationSource = navigationSource;
                this.navigationSourceEntityType = navigationSourceEntityType;

                IEdmContainedEntitySet containedEntitySet = navigationSource as IEdmContainedEntitySet;
                if (containedEntitySet != null)
                {
                    if (containedEntitySet.NavigationProperty.Type.TypeKind() == EdmTypeKind.Collection)
                    {
                        this.isFromCollection = true;
                    }
                }

                IEdmUnknownEntitySet unknownEntitySet = navigationSource as IEdmUnknownEntitySet;
                if (unknownEntitySet != null)
                {
                    if (unknownEntitySet.Type.TypeKind == EdmTypeKind.Collection)
                    {
                        this.isFromCollection = true;
                    }
                }

                this.navigationSourceName = this.navigationSource == null ? null : this.navigationSource.Name;
                var entityType = this.expectedResourceType as IEdmEntityType;
                this.isMediaLinkEntry = entityType == null ? false : entityType.HasStream;
            }

            /// <summary>
            /// The navigation source name of the resource set or resource.
            /// </summary>
            public override string NavigationSourceName
            {
                get { return this.navigationSourceName; }
            }

            /// <summary>
            /// The entity type name of the navigation source of the resource set or resource.
            /// </summary>
            public override string NavigationSourceEntityTypeName
            {
                get
                {
                    if (navigationSourceEntityType != null)
                    {
                        this.navigationSourceEntityTypeName = navigationSourceEntityType.FullName();
                    }

                    return this.navigationSourceEntityTypeName;
                }
            }

            /// <summary>
            /// The full type name of the navigation source of the resource set or resource.
            /// </summary>
            public override string NavigationSourceFullTypeName
            {
                get
                {
                    if (this.navigationSource != null)
                    {
                        this.navigationSourceFullTypeName = this.navigationSource.Type.FullTypeName();
                    }

                    return this.navigationSourceFullTypeName;
                }
            }

            /// <summary>
            /// The kind of the navigation source of the resource set or resource.
            /// </summary>
            public override EdmNavigationSourceKind NavigationSourceKind
            {
                get { return this.navigationSource.NavigationSourceKind(); }
            }

            /// <summary>
            /// The expected resource type name of the resource.
            /// For example, in the request URI 'http://example.com/Service.svc/People/Namespace.VIP_Person', the expected resource type is Namespace.VIP_Person.
            /// (The entity set element type name in this example may be Person, and the actual resource type of a particular resource might be a type more derived than VIP_Person)
            /// </summary>
            public override string ExpectedResourceTypeName
            {
                get { return this.expectedResourceType.FullTypeName(); }
            }

            /// <summary>
            /// The expected resource type.
            /// For example, in the request URI 'http://example.com/Service.svc/People/Namespace.VIP_Person', the expected resource type is Namespace.VIP_Person.
            /// (The entity set element type name in this example may be Person, and the actual entity type of a particular entity might be a type more derived than VIP_Person)
            /// </summary>
            public override IEdmStructuredType ExpectedResourceType
            {
                get { return this.expectedResourceType; }
            }

            /// <summary>
            /// true if the resource is an MLE, false otherwise.
            /// </summary>
            public override bool IsMediaLinkEntry
            {
                get { return this.isMediaLinkEntry; }
            }

            /// <summary>
            /// The flag we use to identify if the current resource is from a collection type or not.
            /// </summary>
            public override bool IsFromCollection
            {
                get { return this.isFromCollection; }
            }

            /// <summary>
            /// The entity type of the navigation source of the resource set or resource.
            /// </summary>
            internal IEdmEntityType NavigationSourceEntityType
            {
                get { return this.navigationSourceEntityType; }
            }
        }
    }
}
