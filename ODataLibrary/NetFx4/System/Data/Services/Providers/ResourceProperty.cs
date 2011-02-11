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

namespace System.Data.Services.Providers
{
    #region Namespaces.
    using System;
    using System.Data.OData;
    using System.Diagnostics;
    #endregion Namespaces.

    /// <summary>
    /// Use this class to describe a property on a resource.
    /// </summary>
    [DebuggerDisplay("{kind}: {name}")]
#if INTERNAL_DROP
    internal class ResourceProperty : ODataAnnotatable
#else
    public class ResourceProperty : ODataAnnotatable
#endif
    {
        #region Private fields.

        /// <summary>
        /// The name of this property.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The kind of resource Type that this property refers to.
        /// For e.g. for collection properties, this would return the resource type,
        /// and not the collection type that this property refers to.
        /// </summary>
        private readonly ResourceType propertyResourceType;

        /// <summary>
        /// The kind of property this is in relation to the resource.
        /// </summary>
        private ResourcePropertyKind kind;

        /// <summary>
        /// Is true, if this property is a actual clr property on the property type. In this case,
        /// astoria service will do reflection over the property type and get/set value for this property.
        /// False means that astoria service needs to go through the IDataServiceQueryProvider contract to get/set value for this provider.
        /// </summary>
        private bool canReflectOnInstanceTypeProperty;

        /// <summary>
        /// Is true, if the resource property is set to readonly i.e. fully initialized and validated. No more changes can be made,
        /// once the resource property is set to readonly.
        /// </summary>
        private bool isReadOnly;

        /// <summary>
        /// MIME type for the property, if it's a primitive value.
        /// </summary>
        private string mimeType;

        #endregion Private fields.

        /// <summary>
        /// Initializes a new ResourceProperty instance for an open property.
        /// </summary>
        /// <param name="name">Property name for the property.</param>
        /// <param name="kind">Property kind.</param>
        /// <param name="propertyResourceType">The type of the resource that this property refers to</param>
        public ResourceProperty(
                string name,
                ResourcePropertyKind kind,
                ResourceType propertyResourceType)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(name, "name");
            ExceptionUtils.CheckArgumentNotNull(propertyResourceType, "propertyResourceType");

            ValidatePropertyParameters(kind, propertyResourceType);

            this.kind = kind;
            this.name = name;
            this.propertyResourceType = propertyResourceType;
            this.canReflectOnInstanceTypeProperty = kind.HasFlag(ResourcePropertyKind.Stream) ? false : true;
        }

        #region Properties

        /// <summary>
        /// Indicates whether this property can be accessed through reflection on the declaring resource instance type.
        /// </summary>
        /// <remarks>A 'true' value here indicates service will use reflection to get the property info on the declaring ResourceType.InstanceType.
        /// 'false' means that service will go through IDataServiceQueryProvider interface to get/set this property's value.</remarks>
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

        /// <summary>
        /// The resource type that this property refers to.
        /// For resource set reference properties, 
        /// this will return the element of the resource set, and not the 
        /// resource set itself.
        /// </summary>
        public ResourceType ResourceType
        {
            [DebuggerStepThrough]
            get { return this.propertyResourceType; }
        }

        /// <summary>
        /// The property name.
        /// </summary>
        public string Name
        {
            [DebuggerStepThrough]
            get { return this.name; }
        }

        /// <summary>
        /// MIME type for the property, if it's a primitive value; null if none specified.
        /// </summary>
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

                if (!HttpUtils.IsValidMediaTypeName(value))
                {
                    throw new InvalidOperationException(Strings.ResourceProperty_MimeTypeNotValid(value, this.Name));
                }

                this.mimeType = value;
            }
        }

        /// <summary>
        /// The kind of property this is in relation to the resource.
        /// </summary>
        public ResourcePropertyKind Kind
        {
            [DebuggerStepThrough]
            get
            {
                return this.kind;
            }
        }

        /// <summary>
        /// PlaceHolder to hold custom state information about resource property.
        /// </summary>
        public object CustomState
        {
            get 
            {
                return this.GetCustomState();
            }

            set
            {
                this.SetCustomState(value);
            }
        }

        /// <summary>
        /// Returns true, if this resource property has been set to read only. Otherwise returns false.
        /// </summary>
        public bool IsReadOnly
        {
            get { return this.isReadOnly; }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Sets the resource property to readonly. Once this method is called, no more changes can be made to resource property.
        /// </summary>
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
        /// Return true if this property is of the given kind.
        /// </summary>
        /// <param name="checkKind">Flag which needs to be checked on the current property kind.</param>
        /// <returns>true if the current property is of the given kind.</returns>
        internal bool IsOfKind(ResourcePropertyKind checkKind)
        {
            DebugUtils.CheckNoExternalCallers();

            return ResourceProperty.IsOfKind(this.kind, checkKind);
        }

        /// <summary>
        /// Return true if the given property kind is of the given kind.
        /// </summary>
        /// <param name="propertyKind">Kind of the property.</param>
        /// <param name="kind">Flag which needs to be checked on property kind.</param>
        /// <returns>true if the kind flag is set on the given property kind.</returns>
        private static bool IsOfKind(ResourcePropertyKind propertyKind, ResourcePropertyKind kind)
        {
            return ((propertyKind & kind) == kind);
        }

        /// <summary>
        /// Validates that the given property kind is valid.
        /// </summary>
        /// <param name="kind">Property kind to check.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        private static void CheckResourcePropertyKind(ResourcePropertyKind kind, string parameterName)
        {
            if (kind != ResourcePropertyKind.ResourceReference &&
                kind != ResourcePropertyKind.ResourceSetReference &&
                kind != ResourcePropertyKind.ComplexType &&
                kind != ResourcePropertyKind.Primitive &&
                kind != ResourcePropertyKind.MultiValue &&
                kind != ResourcePropertyKind.Stream &&
                kind != (ResourcePropertyKind.Primitive | ResourcePropertyKind.Key) &&
                kind != (ResourcePropertyKind.Primitive | ResourcePropertyKind.ETag))
            {
                throw new ArgumentException(Strings.General_InvalidEnumValue(kind.GetType().Name), parameterName);
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

            if (IsOfKind(kind, ResourcePropertyKind.MultiValue))
            {
                if (propertyResourceType.ResourceTypeKind != ResourceTypeKind.MultiValue)
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
                if (propertyResourceType != ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)))
                {
                    throw new ArgumentException(Strings.ResourceProperty_PropertyKindAndResourceTypeKindMismatch("kind", "propertyResourceType"));
                }
            }
            else if (propertyResourceType == ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)))
            {
                // all other property kinds:
                throw new ArgumentException(Strings.ResourceProperty_PropertyKindAndResourceTypeKindMismatch("kind", "propertyResourceType"));
            }

            if (IsOfKind(kind, ResourcePropertyKind.Key) && Nullable.GetUnderlyingType(propertyResourceType.InstanceType) != null)
            {
                throw new ArgumentException(Strings.ResourceProperty_KeyPropertiesCannotBeNullable);
            }
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
