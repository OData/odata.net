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
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.OData;
    using System.Data.OData.Atom;
    using System.Data.Services.Common;
    using System.Diagnostics;
    using System.Linq;
    #endregion Namespaces.

    /// <summary>Use this class to represent a type (primitive, complex or entity).</summary>
    [DebuggerDisplay("{Name}: {InstanceType}, {ResourceTypeKind}")]
#if INTERNAL_DROP
    internal class ResourceType : ODataAnnotatable
#else
    public class ResourceType : ODataAnnotatable
#endif
    {
        #region Fields.
        /// <summary>
        /// Empty list of properties.
        /// </summary>
        internal static readonly ReadOnlyCollection<ResourceProperty> EmptyProperties = new ReadOnlyCollection<ResourceProperty>(new ResourceProperty[0]);

        /// <summary>
        /// ResourceTypeKind for the type that this structure represents.
        /// </summary>
        private readonly ResourceTypeKind resourceTypeKind;

        /// <summary>
        /// Reference to clr type that this resource represents.
        /// </summary>
        private readonly Type type;

        /// <summary>
        /// Reference to base resource type.
        /// </summary>
        private readonly ResourceType baseType;

        /// <summary>
        /// Name of the resource type.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Full name of the resource.
        /// </summary>
        private readonly string fullName;

        /// <summary>
        /// Namespace for this type.
        /// </summary>
        private readonly string namespaceName;

        /// <summary>
        /// Whether this type is abstract.
        /// </summary>
        private readonly bool abstractType;

        /// <summary>
        /// Whether the resource type has open properties.
        /// </summary>
        private bool isOpenType;

        /// <summary>
        /// Whether the corresponding instance type actually represents this node's CLR type.
        /// </summary>
        private bool canReflectOnInstanceType;

        /// <summary>
        /// List of properties declared in this type (includes properties only defined in this type, not in the base type).
        /// </summary>
        private IList<ResourceProperty> propertiesDeclaredOnThisType;

        /// <summary>
        /// List of all properties for this type (includes properties defined in the base type also).
        /// </summary>
        private ReadOnlyCollection<ResourceProperty> allProperties;

        /// <summary>
        /// List of key properties for this type.
        /// </summary>
        private ReadOnlyCollection<ResourceProperty> keyProperties;

        /// <summary>
        /// List of etag properties for this type.
        /// </summary>
        private ReadOnlyCollection<ResourceProperty> etagProperties;

        /// <summary>
        /// Is true, if the type is set to readonly.
        /// </summary>
        private bool isReadOnly;

        /// <summary>
        /// True if the resource type includes a default stream.
        /// </summary>
        private bool isMediaLinkEntry;

        /// <summary>
        /// True if the virtual load properties is already called, otherwise false.
        /// </summary>
        private bool isLoadPropertiesMethodCalled;

        /// <summary>
        /// True if the EPM info was initialized for this type.
        /// </summary>
        private bool epmInfoInitialized;
        #endregion Fields.

        #region Constructors.
        /// <summary>
        /// Constructs a new instance of Astoria type using the specified clr type.
        /// </summary>
        /// <param name="instanceType">CLR type that represents the flow format inside the Astoria runtime.</param>
        /// <param name="resourceTypeKind">Kind of the resource type.</param>
        /// <param name="baseType">Base type of the resource type.</param>
        /// <param name="namespaceName">Namespace name of the given resource type.</param>
        /// <param name="name">Name of the given resource type.</param>
        /// <param name="isAbstract">Whether the resource type is an abstract type or not.</param>
        public ResourceType(
            Type instanceType,
            ResourceTypeKind resourceTypeKind,
            ResourceType baseType,
            string namespaceName,
            string name,
            bool isAbstract)
            : this(instanceType, baseType, namespaceName, name, isAbstract)
        {
            ExceptionUtils.CheckArgumentNotNull(instanceType, "instanceType");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(name, "name");
            CheckResourceTypeKind(resourceTypeKind, "resourceTypeKind");
            if (resourceTypeKind == ResourceTypeKind.Primitive || resourceTypeKind == ResourceTypeKind.MultiValue)
            {
                throw new ArgumentException(Strings.ResourceType_InvalidValueForResourceTypeKind, "resourceTypeKind");
            }

            if (baseType != null && baseType.ResourceTypeKind != resourceTypeKind)
            {
                throw new ArgumentException(
                    Strings.ResourceType_InvalidResourceTypeKindInheritance(resourceTypeKind.ToString(), baseType.ResourceTypeKind.ToString()),
                    "resourceTypeKind");
            }

            if (instanceType.IsValueType)
            {
                throw new ArgumentException(Strings.ResourceType_TypeCannotBeValueType, "instanceType");
            }

            this.resourceTypeKind = resourceTypeKind;
        }

        /// <summary>
        /// Constructs a new instance of Resource type for the either given clr primitive type or multiValue type.
        /// This constructor must be called only for primitive or multiValue types.
        /// </summary>
        /// <param name="type">The instance type of the resource type.</param>
        /// <param name="resourceTypeKind">Kind of the resource type.</param>
        /// <param name="namespaceName">Namespace of the type.</param>
        /// <param name="name">Name of the type.</param>
        internal ResourceType(Type type, ResourceTypeKind resourceTypeKind, string namespaceName, string name)
            : this(type, null, namespaceName, name, false)
        {
            DebugUtils.CheckNoExternalCallers();
#if DEBUG
            Debug.Assert(
                resourceTypeKind == ResourceTypeKind.Primitive || resourceTypeKind == ResourceTypeKind.MultiValue,
                "Only primitive or multiValue resource types can be created by this constructor.");
            Debug.Assert(
                resourceTypeKind != ResourceTypeKind.Primitive || PrimitiveTypeUtils.IsPrimitiveType(type),
                "Primitive resource types must have a primitive instance type.");
#endif

            this.resourceTypeKind = resourceTypeKind;
            this.isReadOnly = true;
        }

        /// <summary>
        /// Constructs a new instance of Astoria type using the specified clr type.
        /// </summary>
        /// <param name="type">CLR type from which metadata needs to be pulled.</param>
        /// <param name="baseType">Base type of the resource type.</param>
        /// <param name="namespaceName">Namespace name of the given resource type.</param>
        /// <param name="name">Name of the given resource type.</param>
        /// <param name="isAbstract">Whether the resource type is an abstract type or not.</param>
        private ResourceType(
            Type type,
            ResourceType baseType,
            string namespaceName,
            string name,
            bool isAbstract)
        {
            ExceptionUtils.CheckArgumentNotNull(type, "type");
            ExceptionUtils.CheckArgumentNotNull(name, "name");

            this.name = name;
            this.namespaceName = namespaceName ?? string.Empty;

            // This is to optimize the string property name in PlainXmlSerializer.WriteStartElementWithType function.
            // Checking here is a fixed overhead, and the gain is every time we serialize a string property.
            if (name == "String" && Object.ReferenceEquals(namespaceName, EdmConstants.EdmNamespace))
            {
                this.fullName = EdmConstants.EdmStringTypeName;
            }
            else
            {
                this.fullName = string.IsNullOrEmpty(namespaceName) ? name : namespaceName + "." + name;
            }

            this.type = type;
            this.abstractType = isAbstract;
            this.canReflectOnInstanceType = true;

            if (baseType != null)
            {
                this.baseType = baseType;
            }
        }
        #endregion Constructors.

        #region Properties.
        /// <summary>
        /// True if the resource type includes a default stream.
        /// </summary>
        public bool IsMediaLinkEntry
        {
            [DebuggerStepThrough]
            get
            {
                return this.isMediaLinkEntry;
            }

            set
            {
                this.ThrowIfSealed();
                if (this.resourceTypeKind != ResourceTypeKind.EntityType && value == true)
                {
                    throw new InvalidOperationException(Strings.ResourceType_HasStreamAttributeOnlyAppliesToEntityType(this.name));
                }

                this.isMediaLinkEntry = value;
            }
        }

        /// <summary>
        /// Reference to clr type that this resource represents.
        /// </summary>
        public Type InstanceType
        {
            [DebuggerStepThrough]
            get { return this.type; }
        }

        /// <summary>
        /// Reference to base resource type, if any.
        /// </summary>
        public ResourceType BaseType
        {
            [DebuggerStepThrough]
            get { return this.baseType; }
        }

        /// <summary>
        /// ResourceTypeKind of this type.
        /// </summary>
        public ResourceTypeKind ResourceTypeKind
        {
            [DebuggerStepThrough]
            get { return this.resourceTypeKind; }
        }
        
        /// <summary>
        /// Returns the list of properties for this type.
        /// </summary>
        public ReadOnlyCollection<ResourceProperty> Properties
        {
            get
            {
                return this.InitializeProperties();
            }
        }

        /// <summary>
        /// List of properties declared on this type.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0014:DoNotHandleProhibitedExceptionsRule", Justification = "always rethrows the exception")]
        public ReadOnlyCollection<ResourceProperty> PropertiesDeclaredOnThisType
        {
            get
            {
                ReadOnlyCollection<ResourceProperty> readOnlyProperties = this.propertiesDeclaredOnThisType as ReadOnlyCollection<ResourceProperty>;
                if (readOnlyProperties == null)
                {
                    // This method will call the virtual method, if that's not been called yet and add the list of properties
                    // returned by the virtual method to the properties collection.
                    this.GetPropertiesDeclaredOnThisType();
                    readOnlyProperties = new ReadOnlyCollection<ResourceProperty>(this.propertiesDeclaredOnThisType ?? ResourceType.EmptyProperties);

                    if (!this.isReadOnly)
                    {
                        return readOnlyProperties;
                    }

                    // First try and validate the type. If that succeeds, then cache the results. otherwise we need to revert the results.
                    IList<ResourceProperty> propertyCollection = this.propertiesDeclaredOnThisType;
                    this.propertiesDeclaredOnThisType = readOnlyProperties;

                    try
                    {
                        this.ValidateType();
                    }
                    catch (Exception)
                    {
                        this.propertiesDeclaredOnThisType = propertyCollection;
                        throw;
                    }
                }

                Debug.Assert(this.isReadOnly, "PropetiesDeclaredInThisType - at this point, the resource type must be readonly");
                return readOnlyProperties;
            }
        }

        /// <summary>
        /// Returns the list of key properties for this type, if this type is entity type.
        /// </summary>
        public ReadOnlyCollection<ResourceProperty> KeyProperties
        {
            get
            {
                if (this.keyProperties == null)
                {
                    ResourceType rootType = this;
                    while (rootType.BaseType != null)
                    {
                        rootType = rootType.BaseType;
                    }

                    ReadOnlyCollection<ResourceProperty> readOnlyKeyProperties;
                    if (rootType.Properties == null)
                    {
                        readOnlyKeyProperties = ResourceType.EmptyProperties;
                    }
                    else
                    {
                        List<ResourceProperty> key = rootType.Properties.Where(p => p.IsOfKind(ResourcePropertyKind.Key)).ToList();
                        key.Sort(ResourceType.ResourcePropertyComparison);
                        readOnlyKeyProperties = new ReadOnlyCollection<ResourceProperty>(key);
                    }

                    if (!this.isReadOnly)
                    {
                        return readOnlyKeyProperties;
                    }

                    this.keyProperties = readOnlyKeyProperties;
                }

                Debug.Assert(this.isReadOnly, "KeyProperties - at this point, the resource type must be readonly");
                Debug.Assert(
                    (this.ResourceTypeKind != ResourceTypeKind.EntityType && this.keyProperties.Count == 0) ||
                    (this.ResourceTypeKind == ResourceTypeKind.EntityType && this.keyProperties.Count > 0),
                    "Entity type must have key properties and non-entity types cannot have key properties");

                return this.keyProperties;
            }
        }

        /// <summary>
        /// Returns the list of etag properties for this type.
        /// </summary>
        public ReadOnlyCollection<ResourceProperty> ETagProperties
        {
            get
            {
                if (this.etagProperties == null)
                {
                    ReadOnlyCollection<ResourceProperty> etag = new ReadOnlyCollection<ResourceProperty>(this.Properties.Where(p => p.IsOfKind(ResourcePropertyKind.ETag)).ToList());
                    if (!this.isReadOnly)
                    {
                        return etag;
                    }

                    this.etagProperties = etag;
                }

                Debug.Assert(this.isReadOnly, "ETagProperties - at this point, the resource type must be readonly");
                return this.etagProperties;
            }
        }

        /// <summary>
        /// Gets the name of the resource.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the fullname of the resource.
        /// </summary>
        public string FullName
        {
            get { return this.fullName; }
        }

        /// <summary>
        /// Returns the namespace of this type.
        /// </summary>
        public string Namespace
        {
            get { return this.namespaceName; }
        }

        /// <summary>
        /// Indicates whether this is an abstract type.
        /// </summary>
        public bool IsAbstract
        {
            get { return this.abstractType; }
        }

        /// <summary>
        /// Indicates whether the resource type has open properties.
        /// </summary>
        public bool IsOpenType
        {
            [DebuggerStepThrough]
            get
            {
                return this.isOpenType;
            }

            set
            {
                this.ThrowIfSealed();

                // Complex types can not be marked as open.
                if (this.resourceTypeKind == ResourceTypeKind.ComplexType && value == true)
                {
                    throw new InvalidOperationException(Strings.ResourceType_ComplexTypeCannotBeOpen(this.FullName));
                }

                this.isOpenType = value;
            }
        }

        /// <summary>
        /// Whether the corresponding instance type actually represents this node's CLR type.
        /// </summary>
        public bool CanReflectOnInstanceType
        {
            [DebuggerStepThrough]
            get
            {
                return this.canReflectOnInstanceType;
            }

            set
            {
                this.ThrowIfSealed();
                this.canReflectOnInstanceType = value;
            }
        }

        /// <summary>
        /// PlaceHolder to hold custom state information about resource type.
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
        /// Returns true, if this resource type has been set to read only. Otherwise returns false.
        /// </summary>
        public bool IsReadOnly
        {
            get { return this.isReadOnly; }
        }

        /// <summary>
        /// List of all named streams on this type (includes named streams defined on the base types)
        /// </summary>
        internal IEnumerable<ResourceProperty> NamedStreams
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();

                return this.Properties.Where(p => p.IsOfKind(ResourcePropertyKind.Stream));
            }
        }
        #endregion Properties.

        #region Methods.
        /// <summary>
        /// Get a ResourceType representing a primitive type given a .NET System.Type object.
        /// </summary>
        /// <param name="type">.NET type to get the primitive type from.</param>
        /// <returns>A ResourceType object representing the primitive type or null if not primitive.</returns>
        public static ResourceType GetPrimitiveResourceType(Type type)
        {
            ExceptionUtils.CheckArgumentNotNull(type, "type");

            foreach (ResourceType resourceType in PrimitiveTypeUtils.PrimitiveTypes)
            {
                if (resourceType.InstanceType == type)
                {
                    return resourceType;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a <see cref="MultiValueResourceType"/> representing a multiValue of the specified <paramref name="itemType"/> items.
        /// </summary>
        /// <param name="itemType">The <see cref="ResourceType"/> of a single item in the multiValue.</param>
        /// <returns>A <see cref="MultiValueResourceType"/> object representing a multiValue of the specified <paramref name="itemType"/> items.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "MultiValue is a Name")]
        public static MultiValueResourceType GetMultiValueResourceType(ResourceType itemType)
        {
            ExceptionUtils.CheckArgumentNotNull(itemType, "itemType");
            return new MultiValueResourceType(itemType);
        }

        /// <summary>
        /// Adds the given property to this ResourceType instance.
        /// </summary>
        /// <param name="property">resource property to be added.</param>
        public void AddProperty(ResourceProperty property)
        {
            ExceptionUtils.CheckArgumentNotNull(property, "property");
            Debug.Assert(!string.IsNullOrEmpty(property.Name), "!string.IsNullOrEmpty(property.Name)");

            // only check whether the property with the same name exists in this type.
            // we will look in base types properties when the type is sealed.
            this.ThrowIfSealed();

            this.AddPropertyImplementation(property);
        }

        /// <summary>
        /// Adds an <see cref="EntityPropertyMappingAttribute"/> for the resource type.
        /// </summary>
        /// <param name="attribute">Given <see cref="EntityPropertyMappingAttribute"/>.</param>
        public void AddEntityPropertyMappingAttribute(EntityPropertyMappingAttribute attribute)
        {
            ExceptionUtils.CheckArgumentNotNull(attribute, "attribute");

            // EntityPropertyMapping attribute can not be added to readonly resource types.
            this.ThrowIfSealed();

            if (this.ResourceTypeKind != ResourceTypeKind.EntityType)
            {
                throw new InvalidOperationException(Strings.ResourceType_EpmOnlyAllowedOnEntityTypes(this.Name));
            }

            // Initialize the EPM annotation for this type
            EpmResourceTypeAnnotation epm = this.Epm();
            if (epm == null)
            {
                epm = new EpmResourceTypeAnnotation();
                this.SetAnnotation(epm);
            }

            // And add the attribute to it
            epm.OwnEpmAttributes.Add(attribute);
        }

        /// <summary>
        /// Make the resource type readonly from now on. This means that no more changes can be made to the resource type anymore.
        /// </summary>
        public void SetReadOnly()
        {
#if DEBUG
            IList<ResourceProperty> currentPropertyCollection = this.propertiesDeclaredOnThisType;
#endif
            // If this is a multivalue resource type, set its ItemType to ReadOnly. This has to be done first
            // as MultiValueResourceType instances are readonly to begin with, but their ItemTypes are not.
            if (this.ResourceTypeKind == Providers.ResourceTypeKind.MultiValue)
            {
                MultiValueResourceType multiValueType = this as MultiValueResourceType;
                Debug.Assert(
                    multiValueType != null,
                    "ResourceTypeKind is MultiValue but the instance is not of MultiValueResourceType");
                multiValueType.ItemType.SetReadOnly();
            }
            
            // if its already sealed, its a no-op
            if (this.isReadOnly)
            {
                return;
            }

            // We need to set readonly at the start to avoid any circular loops that may result due to navigation properties.
            // If there are any exceptions, we need to set readonly to false.
            this.isReadOnly = true;

            // There can be properties with the same name in the base class also (using the new construct)
            // if the base type is not null, then we need to make sure that there is no property with the same name.
            // Otherwise, we are only populating property declared for this type and clr gaurantees that they are unique
            if (this.BaseType != null)
            {
                this.BaseType.SetReadOnly();

                // Mark current type as OpenType if base is an OpenType
                if (this.BaseType.IsOpenType && this.ResourceTypeKind != ResourceTypeKind.ComplexType)
                {
                    this.isOpenType = true;
                }

                // Mark the current type as being a Media Link Entry if the base type is a Media Link Entry.
                if (this.BaseType.IsMediaLinkEntry) 
                {
                    this.isMediaLinkEntry = true;
                }

                // Make sure current type is not a CLR type if base is not a CLR type.
                if (!this.BaseType.CanReflectOnInstanceType)
                {
                    this.canReflectOnInstanceType = false;
                }
            }

            // set all the properties to readonly
            if (this.propertiesDeclaredOnThisType != null)
            {
                foreach (ResourceProperty p in this.propertiesDeclaredOnThisType)
                {
                    p.SetReadOnly();
                }
            }
#if DEBUG
            // We cannot change the properties collection method. Basically, we should not be calling Properties or PropertiesDeclaredOnThisType properties
            // since they call the virtual LoadPropertiesDeclaredOnThisType and we want to postpone that virtual call until we actually need to do something
            // more useful with the properties
            Debug.Assert(
                Object.ReferenceEquals(this.propertiesDeclaredOnThisType, currentPropertyCollection), 
                "We should not have modified the properties collection instance");
#endif
        }

        /// <summary>Tries to find the property for the specified name.</summary>
        /// <param name="propertyName">Name of property to resolve.</param>
        /// <returns>Resolved property; possibly null.</returns>
        internal ResourceProperty TryResolvePropertyName(string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            // In case of empty property name this will return null, which means propery is not found
            // Devnote(pqian, 11/01/2010):
            // For most part of the system, a named stream is not part of the "properties" collection
            // Because a streaming property is handled with special routine, we need to exclude all streams
            return this.Properties.FirstOrDefault(p => p.Name == propertyName && !p.IsOfKind(ResourcePropertyKind.Stream));
        }

        /// <summary>
        /// Tries to find the named stream for the specified name.
        /// </summary>
        /// <param name="streamName">Name of stream to resolve.</param>
        /// <returns>Resolved named stream; possibly null.</returns>
        internal ResourceProperty TryResolveNamedStream(string streamName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(streamName), "!string.IsNullOrEmpty(streamName)");

            // In case of empty stream name this will return null, which means stream is not found
            return this.NamedStreams.FirstOrDefault(ns => ns.Name.Equals(streamName, StringComparison.Ordinal));
        }

        /// <summary>
        /// Checks if the given type is assignable to this type. In other words, if this type
        /// is a supertype of the given type or not.
        /// </summary>
        /// <param name="subType">Resource type to check.</param>
        /// <returns>true, if the given type is assignable to this type. Otherwise returns false.</returns>
        internal bool IsAssignableFrom(ResourceType subType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(subType != null, "subType != null");

            while (subType != null)
            {
                if (subType == this)
                {
                    return true;
                }

                subType = subType.BaseType;
            }

            return false;
        }

        /// <summary>
        /// By initializing the Epm for the resource type, ensures that the information is available.
        /// </summary>
        internal void EnsureEpmAvailability()
        {
            DebugUtils.CheckNoExternalCallers();
            this.InitializeProperties();
        }

        /// <summary>
        /// Return the list of properties declared by this resource type. This method gives a chance to lazy load the properties
        /// of a resource type, instead of loading them upfront. This property will be called once and only once, whenever
        /// ResourceType.Properties or ResourceType.PropertiesDeclaredOnThisType property is accessed.
        /// </summary>
        /// <returns>The list of properties declared on this type.</returns>
        protected virtual IEnumerable<ResourceProperty> LoadPropertiesDeclaredOnThisType()
        {
            return new ResourceProperty[0];
        }

        /// <summary>
        /// Compares two resource property instances, sorting them so keys are first,
        /// and are alphabetically ordered in case-insensitive ordinal order.
        /// </summary>
        /// <param name="a">First property to compare.</param>
        /// <param name="b">Second property to compare.</param>
        /// <returns>
        /// Less than zero if a sorts before b; zero if equal; greater than zero if a sorts
        /// after b.
        /// </returns>
        private static int ResourcePropertyComparison(ResourceProperty a, ResourceProperty b)
        {
            return StringComparer.OrdinalIgnoreCase.Compare(a.Name, b.Name);
        }

        /// <summary>
        /// Check whether the given value for <see cref="ResourceTypeKind"/> is valid. If not, throw argument exception.
        /// </summary>
        /// <param name="kind">Value for ResourceTypeKind.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="ArgumentException">If the value is not valid.</exception>
        private static void CheckResourceTypeKind(ResourceTypeKind kind, string parameterName)
        {
            if (kind < ResourceTypeKind.EntityType ||
                kind > ResourceTypeKind.MultiValue)
            {
                throw new ArgumentException(Strings.General_InvalidEnumValue(kind.GetType().Name), parameterName);
            }
        }

        /// <summary>
        /// Initializes all properties for the resource type, to be used by Properties getter.
        /// </summary>
        /// <returns>Collection of properties exposed by this resource type.</returns>
        private ReadOnlyCollection<ResourceProperty> InitializeProperties()
        {
            if (this.allProperties == null)
            {
                ReadOnlyCollection<ResourceProperty> readOnlyAllProps;
                List<ResourceProperty> allProps = new List<ResourceProperty>();
                if (this.BaseType != null)
                {
                    allProps.AddRange(this.BaseType.Properties);
                }

                allProps.AddRange(this.PropertiesDeclaredOnThisType);
                readOnlyAllProps = new ReadOnlyCollection<ResourceProperty>(allProps);

                if (!this.isReadOnly)
                {
                    // this type can still change, thus don't cache, just return
                    return readOnlyAllProps;
                }

                // this type is static now, cache all props and return
                this.allProperties = readOnlyAllProps;
            }

            Debug.Assert(this.isReadOnly, "Propeties - at this point, the resource type must be readonly");
            return this.allProperties;
        }

        /// <summary>
        /// Checks if the resource type is sealed. If not, it throws an InvalidOperationException.
        /// </summary>
        private void ThrowIfSealed()
        {
            if (this.isReadOnly)
            {
                throw new InvalidOperationException(Strings.ResourceType_Sealed(this.FullName));
            }
        }

        /// <summary>
        /// Validate the given <paramref name="property"/> and adds it to the list of properties for this type
        /// </summary>
        /// <param name="property">property which needs to be added.</param>
        private void AddPropertyImplementation(ResourceProperty property)
        {
            if (this.propertiesDeclaredOnThisType == null)
            {
                this.propertiesDeclaredOnThisType = new List<ResourceProperty>();
            }

            foreach (ResourceProperty resourceProperty in this.propertiesDeclaredOnThisType)
            {
                if (resourceProperty.Name == property.Name)
                {
                    throw new InvalidOperationException(Strings.ResourceType_PropertyWithSameNameAlreadyExists(resourceProperty.Name, this.FullName));
                }
            }

            if (property.IsOfKind(ResourcePropertyKind.Stream))
            {
                // NamedStream Property
                // Can only add named streams to entity types.
                if (this.ResourceTypeKind != Providers.ResourceTypeKind.EntityType)
                {
                    throw new InvalidOperationException(Strings.ResourceType_NamedStreamsOnlyApplyToEntityType(this.FullName));
                }

                // NamedStream cannot be used as key or etag (you cannot create a property with a mixed flag that contains stream)
                Debug.Assert(!property.IsOfKind(ResourcePropertyKind.Key) && !property.IsOfKind(ResourcePropertyKind.ETag), "NamedStream property kind must be used alone");
                Debug.Assert(!property.CanReflectOnInstanceTypeProperty, "NamedStream properties must not be able to reflect");
            }
            else
            {
                if (property.IsOfKind(ResourcePropertyKind.Key))
                {
                    if (this.baseType != null)
                    {
                        throw new InvalidOperationException(Strings.ResourceType_NoKeysInDerivedTypes);
                    }

                    if (this.ResourceTypeKind != ResourceTypeKind.EntityType)
                    {
                        throw new InvalidOperationException(Strings.ResourceType_KeyPropertiesOnlyOnEntityTypes);
                    }

                    Debug.Assert(property.ResourceType.ResourceTypeKind == ResourceTypeKind.Primitive, "This check must have been done in ResourceProperty.ValidatePropertyParameters method");
                    Debug.Assert(!property.IsOfKind(ResourcePropertyKind.ETag), "This check must have been done in ResourceProperty.ValidatePropertyParameters method");
                    Debug.Assert(property.IsOfKind(ResourcePropertyKind.Primitive), "This check must have been done in ResourceProperty.ValidatePropertyParameters method");
                }

                if (property.IsOfKind(ResourcePropertyKind.ETag))
                {
                    if (this.ResourceTypeKind != ResourceTypeKind.EntityType)
                    {
                        throw new InvalidOperationException(Strings.ResourceType_ETagPropertiesOnlyOnEntityTypes);
                    }

                    Debug.Assert(property.ResourceType.ResourceTypeKind == ResourceTypeKind.Primitive, "This check must have been done in ResourceProperty.ValidatePropertyParameters method");
                    Debug.Assert(property.IsOfKind(ResourcePropertyKind.Primitive), "This check must have been done in ResourceProperty.ValidatePropertyParameters method");
                    Debug.Assert(!property.IsOfKind(ResourcePropertyKind.Key), "This check must have been done in ResourceProperty.ValidatePropertyParameters method");
                }

                Debug.Assert(property.ResourceType != GetPrimitiveResourceType(typeof(System.IO.Stream)), "Non NamedStream resource using Stream type");
            }

            this.propertiesDeclaredOnThisType.Add(property);
        }

        /// <summary>
        /// Calls the virtual LoadPropertiesDeclaredOnThisType method, if its not already called and then
        /// adds the properties returned by the method to the list of properties for this type.
        /// </summary>
        private void GetPropertiesDeclaredOnThisType()
        {
            // We just call the virtual LoadPropertiesDeclaredOnThisType method only once. If it hasn't been called yet,
            // then call the method and update the state to reflect that.
            if (!this.isLoadPropertiesMethodCalled)
            {
                foreach (ResourceProperty p in this.LoadPropertiesDeclaredOnThisType())
                {
                    this.AddPropertyImplementation(p);

                    // if this type is already set to readonly, make sure that new properties returned by the virtual method
                    // are also set to readonly
                    if (this.IsReadOnly)
                    {
                        p.SetReadOnly();
                    }
                }

                this.isLoadPropertiesMethodCalled = true;
            }
        }

        /// <summary>
        /// This method is called only when the Properties property is called and the type is already set to read-only.
        /// This method validates all the properties w.r.t to the base type and calls SetReadOnly on all the properties.
        /// </summary>
        private void ValidateType()
        {
            Debug.Assert(this.isLoadPropertiesMethodCalled && this.IsReadOnly, "This method must be invoked only if LoadPropertiesDeclaredOnThisType has been called and the type is set to ReadOnly");

            if (this.BaseType != null)
            {
                // make sure that there are no properties with the same name. Properties with duplicate name within the type
                // is already checked in AddProperty method
                foreach (ResourceProperty rp in this.BaseType.Properties)
                {
                    if (this.propertiesDeclaredOnThisType.Where(p => p.Name == rp.Name).FirstOrDefault() != null)
                    {
                        throw new InvalidOperationException(Strings.ResourceType_PropertyWithSameNameAlreadyExists(rp.Name, this.FullName));
                    }
                }
            }
            else if (this.ResourceTypeKind == ResourceTypeKind.EntityType)
            {
                if (this.propertiesDeclaredOnThisType.Where(p => p.IsOfKind(ResourcePropertyKind.Key)).FirstOrDefault() == null)
                {
                    throw new InvalidOperationException(Strings.ResourceType_MissingKeyPropertiesForEntity(this.FullName));
                }
            }

            // set all the properties to readonly
            foreach (ResourceProperty p in this.propertiesDeclaredOnThisType)
            {
                p.SetReadOnly();

                // Note that we cache the propertyinfo objects for each CLR properties in the ResourceType class
                // rather than the ResourceProperty class because the same ResourceProperty instance can be added
                // to multiple ResourceType instances.
                if (p.CanReflectOnInstanceTypeProperty)
                {
                    // TODO: WCF DS will need to do this close to this place. Probably we will need an internal hook here
                    //   for this reason.
                    // this.GetPropertyInfoDecaredOnThisType(p);
                }
            }

            // Resolve EpmInfos now that everything in the type hierarchy is readonly
            try
            {
                if (!this.epmInfoInitialized)
                {
                    EpmResourceTypeAnnotation.BuildEpm(this);
                    this.MarkEpmInfoInitialized();
                }
            }
            catch
            {
                // If an exception was thrown from this.BuildDynamicEpmInfo(this) method
                // EpmSourceTree and EpmTargetTree may be only half constructed and need to be reset.
                EpmResourceTypeAnnotation epm = this.Epm();
                if (epm != null && !this.epmInfoInitialized)
                {
                    epm.Reset();
                }

                throw;
            }
        }

        /// <summary>
        /// Marks the EpmInfo as initialized and verifies that it's valid.
        /// </summary>
        private void MarkEpmInfoInitialized()
        {
            this.epmInfoInitialized = true;

            EpmResourceTypeAnnotation epm = this.Epm();
            if (epm != null)
            {
                epm.EpmSourceTree.Validate(this);
                epm.EpmTargetTree.Validate();
            }
        }
        #endregion Methods.
    }
}
