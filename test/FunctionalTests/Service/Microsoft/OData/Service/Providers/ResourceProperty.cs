//---------------------------------------------------------------------
// <copyright file="ResourceProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>Use this class to describe a property on a resource.</summary>
    [DebuggerDisplay("{kind}: {name}")]
    public class ResourceProperty
    {
        #region Private fields

        /// <summary>The name of this property.</summary>
        private readonly string name;

        /// <summary>The kind of resource Type that this property refers to.
        /// For e.g. for collection properties, this would return the resource type,
        /// and not the collection type that this property refers to.</summary>
        private readonly ResourceType propertyResourceType;

        /// <summary>The kind of property this is in relation to the resource.</summary>
        private ResourcePropertyKind kind;

        /// <summary> Is true, if this property is a actual clr property on the property type. In this case,
        /// astoria service will do reflection over the property type and get/set value for this property.
        /// False means that astoria service needs to go through the IDataServiceQueryProvider contract to get/set value for this provider.</summary>
        private bool canReflectOnInstanceTypeProperty;

        /// <summary>Is true, if the resource property is set to readonly i.e. fully initialized and validated. No more changes can be made,
        /// once the resource property is set to readonly.</summary>
        private bool isReadOnly;

        /// <summary>MIME type for the property, if it's a primitive value.</summary>
        private string mimeType;

        /// <summary>list of custom annotations that needs to be flowed via $metadata endpoint.</summary>
        private Dictionary<string, object> customAnnotations;

        #endregion Private fields

        /// <summary>Initializes a new <see cref="T:Microsoft.OData.Service.Providers.ResourceProperty" /> for an open property.</summary>
        /// <param name="name">Property name for the property as string.</param>
        /// <param name="kind">
        ///   <see cref="T:Microsoft.OData.Service.Providers.ResourcePropertyKind" />.</param>
        /// <param name="propertyResourceType">The <see cref="T:Microsoft.OData.Service.Providers.ResourceType" /> of the resource to which the property refers.</param>
        public ResourceProperty(
                string name,
                ResourcePropertyKind kind,
                ResourceType propertyResourceType)
        {
            WebUtil.CheckStringArgumentNullOrEmpty(name, "name");
            WebUtil.CheckArgumentNull(propertyResourceType, "propertyResourceType");

            ValidatePropertyParameters(kind, propertyResourceType);

            this.kind = kind;
            this.name = name;
            this.propertyResourceType = propertyResourceType;
            this.canReflectOnInstanceTypeProperty = kind.HasFlag(ResourcePropertyKind.Stream) ? false : true;
        }

        #region Properties

        /// <summary>Gets a value that indicates whether this property can be accessed through reflection on the declaring resource instance type.</summary>
        /// <returns>true when the property can be accessed through reflection; otherwise false.</returns>
        /// <remarks>A 'true' value here typically indicates astoria service will use reflection to get the property info on the declaring ResourceType.InstanceType.
        /// 'false' means that astoria service will go through IDataServiceQueryProvider interface to get/set this property's value.</remarks>
        public bool CanReflectOnInstanceTypeProperty
        {
            get
            {
                return this.canReflectOnInstanceTypeProperty;
            }

            set
            {
                this.ThrowIfSealed();
                
                if (this.IsOfKind(ResourcePropertyKind.Stream))
                {
                    // NamedStream properties: this is read only
                    throw new InvalidOperationException(Strings.ResourceProperty_NamedStreamCannotReflect);
                }

                this.canReflectOnInstanceTypeProperty = value;
            }
        }

        /// <summary>Gets the type of the resource property.</summary>
        /// <returns>The <see cref="T:Microsoft.OData.Service.Providers.ResourceType" /> of the resource property.</returns>
        /// <remarks>For collection, this will return the element of the collection, and not the collection.</remarks>
        public ResourceType ResourceType
        {
            [DebuggerStepThrough]
            get { return this.propertyResourceType; }
        }

        /// <summary>Gets the name of the resource property.</summary>
        /// <returns>The name of the resource property as string.</returns>
        public string Name
        {
            [DebuggerStepThrough]
            get { return this.name; }
        }

        /// <summary>Gets or sets MIME type for the property.</summary>
        /// <returns>String value that indicates MIME type.</returns>
        public string MimeType
        {
            [DebuggerStepThrough]
            get
            {
                return this.mimeType;
            }

            set
            {
                this.ThrowIfSealed();

                if (String.IsNullOrEmpty(value))
                {
                    throw new InvalidOperationException(Strings.ResourceProperty_MimeTypeAttributeEmpty(this.Name));
                }

                if (!this.IsOfKind(ResourcePropertyKind.Primitive))
                {
                    throw new InvalidOperationException(Strings.ResourceProperty_MimeTypeAttributeOnNonPrimitive(this.Name, this.Kind));
                }

                if (!WebUtil.IsValidMimeType(value))
                {
                    throw new InvalidOperationException(Strings.ResourceProperty_MimeTypeNotValid(value, this.Name));
                }

                this.mimeType = value;
            }
        }

        /// <summary>Gets the kind of the resource property with regard to the resource.</summary>
        /// <returns>A <see cref="T:Microsoft.OData.Service.Providers.ResourcePropertyKind" /> value.</returns>
        public ResourcePropertyKind Kind
        {
            [DebuggerStepThrough]
            get
            {
                return this.kind;
            }

            [DebuggerStepThrough]
            internal set
            {
                Debug.Assert(!this.isReadOnly, "Kind - the resource property cannot be readonly");
                this.kind = value;
            }
        }

        /// <summary>Gets or sets custom state information about a resource property that is defined by the developer.</summary>
        /// <returns>State information.</returns>
        public object CustomState
        {
            get;
            set;
        }

        /// <summary>Gets a Boolean value that indicates whether the property is read-only.</summary>
        /// <returns>True if the property is read-only.</returns>
        public bool IsReadOnly
        {
            get { return this.isReadOnly; }
        }

        /// <summary>The kind of type this property has in relation to the data service.</summary>
        internal ResourceTypeKind TypeKind
        {
            get
            {
                return this.ResourceType.ResourceTypeKind;
            }
        }

        /// <summary>The type of the property.</summary>
        internal Type Type
        {
            get
            {
                return this.Kind == ResourcePropertyKind.ResourceSetReference 
                    ? typeof(System.Collections.Generic.IEnumerable<>).MakeGenericType(this.propertyResourceType.InstanceType) 
                    : this.propertyResourceType.InstanceType;
            }
        }

        /// <summary>
        /// Returns the list of custom annotations defined on this property.
        /// </summary>
        internal IEnumerable<KeyValuePair<string, object>> CustomAnnotations
        {
            get
            {
                if (this.customAnnotations == null)
                {
                    return WebUtil.EmptyKeyValuePairStringObject;
                }

                return this.customAnnotations;
            }
        }

        #endregion Properties

        #region Methods
        /// <summary>Sets the resource property to read-only.</summary>
        /// <remarks>Once this method is called, no more changes can be made to resource property.</remarks>
        public void SetReadOnly()
        {
            // If its already set to readonly, do no-op
            if (this.isReadOnly)
            {
                return;
            }

            this.ResourceType.SetReadOnly();
            this.isReadOnly = true;
        }

        /// <summary>
        /// Add the given annotation to the list of annotations that needs to flowed via the $metadata endpoint
        /// </summary>
        /// <param name="annotationNamespace">NamespaceName to which the custom annotation belongs to.</param>
        /// <param name="annotationName">Name of the annotation.</param>
        /// <param name="annotationValue">Value of the annotation.</param>
        internal void AddCustomAnnotation(string annotationNamespace, string annotationName, object annotationValue)
        {
            WebUtil.ValidateAndAddAnnotation(ref this.customAnnotations, annotationNamespace, annotationName, annotationValue);
        }

        /// <summary>
        /// return true if this property is of the given kind
        /// </summary>
        /// <param name="checkKind">flag which needs to be checked on the current property kind</param>
        /// <returns>true if the current property is of the given kind</returns>
        internal bool IsOfKind(ResourcePropertyKind checkKind)
        {
            return ResourceProperty.IsOfKind(this.kind, checkKind);
        }

        /// <summary>
        /// return true if the given property kind is of the given kind
        /// </summary>
        /// <param name="propertyKind">kind of the property</param>
        /// <param name="kind">flag which needs to be checked on property kind</param>
        /// <returns>true if the kind flag is set on the given property kind</returns>
        private static bool IsOfKind(ResourcePropertyKind propertyKind, ResourcePropertyKind kind)
        {
            return ((propertyKind & kind) == kind);
        }

        /// <summary>
        /// Validates that the given property kind is valid
        /// </summary>
        /// <param name="kind">property kind to check</param>
        /// <param name="parameterName">name of the parameter</param>
        private static void CheckResourcePropertyKind(ResourcePropertyKind kind, string parameterName)
        {
            if (kind != ResourcePropertyKind.ResourceReference &&
                kind != ResourcePropertyKind.ResourceSetReference &&
                kind != ResourcePropertyKind.ComplexType &&
                kind != ResourcePropertyKind.Primitive &&
                kind != ResourcePropertyKind.Collection &&
                kind != ResourcePropertyKind.Stream &&
                kind != (ResourcePropertyKind.Primitive | ResourcePropertyKind.Key) &&
                kind != (ResourcePropertyKind.Primitive | ResourcePropertyKind.ETag))
            {
                throw new ArgumentException(Strings.InvalidEnumValue(kind.GetType().Name), parameterName);
            }
        }

        /// <summary>
        /// Validate the parameters of the resource property constructor.
        /// </summary>
        /// <param name="kind">kind of the resource property.</param>
        /// <param name="propertyResourceType">resource type that this property refers to.</param>
        private static void ValidatePropertyParameters(ResourcePropertyKind kind, ResourceType propertyResourceType)
        {
            CheckResourcePropertyKind(kind, "kind");

            if (IsOfKind(kind, ResourcePropertyKind.ResourceReference) || IsOfKind(kind, ResourcePropertyKind.ResourceSetReference))
            {
                if (propertyResourceType.ResourceTypeKind != ResourceTypeKind.EntityType)
                {
                    throw new ArgumentException(Strings.ResourceProperty_PropertyKindAndResourceTypeKindMismatch("kind", "propertyResourceType"));
                }
            }

            if (IsOfKind(kind, ResourcePropertyKind.Primitive))
            {
                if (propertyResourceType.ResourceTypeKind != ResourceTypeKind.Primitive)
                {
                    throw new ArgumentException(Strings.ResourceProperty_PropertyKindAndResourceTypeKindMismatch("kind", "propertyResourceType"));
                }
            }

            if (IsOfKind(kind, ResourcePropertyKind.ComplexType))
            {
                if (propertyResourceType.ResourceTypeKind != ResourceTypeKind.ComplexType)
                {
                    throw new ArgumentException(Strings.ResourceProperty_PropertyKindAndResourceTypeKindMismatch("kind", "propertyResourceType"));
                }
            }

            if (IsOfKind(kind, ResourcePropertyKind.Collection))
            {
                if (propertyResourceType.ResourceTypeKind != ResourceTypeKind.Collection)
                {
                    throw new ArgumentException(Strings.ResourceProperty_PropertyKindAndResourceTypeKindMismatch("kind", "propertyResourceType"));
                }
            }

            if (IsOfKind(kind, ResourcePropertyKind.Stream))
            {
                if (kind != ResourcePropertyKind.Stream)
                {
                    throw new ArgumentException(Strings.ResourceProperty_NamedStreamKindMustBeUsedAlone);
                }

                // Stream property should be declared on the Primitive Type Edm.Stream
                if (propertyResourceType != PrimitiveResourceTypeMap.TypeMap.GetPrimitive(typeof(System.IO.Stream)))
                {
                    throw new ArgumentException(Strings.ResourceProperty_PropertyKindAndResourceTypeKindMismatch("kind", "propertyResourceType"));
                }
            }
            else if (propertyResourceType == PrimitiveResourceTypeMap.TypeMap.GetPrimitive(typeof(System.IO.Stream)))
            {
                // all other property kinds:
                throw new ArgumentException(Strings.ResourceProperty_PropertyKindAndResourceTypeKindMismatch("kind", "propertyResourceType"));
            }

            if (IsOfKind(kind, ResourcePropertyKind.Key) && Nullable.GetUnderlyingType(propertyResourceType.InstanceType) != null)
            {
                throw new ArgumentException(Strings.ResourceProperty_KeyPropertiesCannotBeNullable);
            }

            Debug.Assert(propertyResourceType.ResourceTypeKind != ResourceTypeKind.EntityCollection, "EntityCollectionResourceType is not a supported type for ResourceProperty.");
        }

        /// <summary>
        /// Checks if the resource type is sealed. If not, it throws an InvalidOperationException.
        /// </summary>
        private void ThrowIfSealed()
        {
            if (this.isReadOnly)
            {
                throw new InvalidOperationException(Strings.ResourceProperty_Sealed(this.Name));
            }
        }
        #endregion Methods
    }
}
