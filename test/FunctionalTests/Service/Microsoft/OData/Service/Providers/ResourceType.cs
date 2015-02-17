//---------------------------------------------------------------------
// <copyright file="ResourceType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;

    #endregion Namespaces

    /// <summary>Use this class to represent a DataService type (primitive, complex or entity).</summary>
    [DebuggerDisplay("{Name}: {InstanceType}, {ResourceTypeKind}")]
    public class ResourceType
    {
        #region Fields

        /// <summary> empty list of properties </summary>
        internal static readonly ReadOnlyCollection<ResourceProperty> EmptyProperties = new ReadOnlyCollection<ResourceProperty>(new ResourceProperty[0]);

        /// <summary>Primitive string resource type.</summary>
        internal static readonly ResourceType PrimitiveStringResourceType = PrimitiveResourceTypeMap.TypeMap.GetPrimitive(typeof(string));

        /// <summary> ResourceTypeKind for the type that this structure represents </summary>
        private readonly ResourceTypeKind resourceTypeKind;

        /// <summary> Reference to clr type that this resource represents </summary>
        private readonly Type type;

        /// <summary> Reference to base resource type </summary>
        private readonly ResourceType baseType;

        /// <summary> name of the resource.</summary>
        private readonly string name;

        /// <summary> full name of the resource.</summary>
        private readonly string fullName;

        /// <summary> Namespace for this type.</summary>
        private readonly string namespaceName;

        /// <summary>Whether this type is abstract.</summary>
        private readonly bool abstractType;

        /// <summary>lock object.</summary>
        private readonly object lockPropertiesLoad = new object();

        /// <summary>Whether the resource type has open properties.</summary>
        private bool isOpenType;

        /// <summary>Whether the corresponding instance type actually represents this node's CLR type.</summary>
        private bool canReflectOnInstanceType;

        /// <summary> List of properties declared in this type (includes properties only defined in this type, not in the base type) </summary>
        private IList<ResourceProperty> propertiesDeclaredOnThisType;

        /// <summary> List of all properties for this type (includes properties defined in the base type also) </summary>
        private ReadOnlyCollection<ResourceProperty> allProperties;

        /// <summary> list of key properties for this type</summary>
        private ReadOnlyCollection<ResourceProperty> keyProperties;

        /// <summary> list of etag properties for this type.</summary>
        private ReadOnlyCollection<ResourceProperty> etagProperties;

        /// <summary>If ResourceProperty.CanReflectOnInstanceTypeProperty is true, we cache the PropertyInfo object.</summary>
        private Dictionary<ResourceProperty, ResourcePropertyInfo> propertyInfosDeclaredOnThisType = new Dictionary<ResourceProperty, ResourcePropertyInfo>(ReferenceEqualityComparer<ResourceProperty>.Instance);

        /// <summary>is true, if the type is set to readonly.</summary>
        private bool isReadOnly;

        /// <summary>True if the resource type includes a default stream </summary>
        private bool isMediaLinkEntry;

        /// <summary>True if the virtual load properties is already called, otherwise false.</summary>
        private bool isLoadPropertiesMethodCalled;

#if DEBUG
        /// <summary>tracker to detect recursion during lazy loading.</summary>
        private bool propertiesDeclaredOnThisTypeLoadingHitPreviously = false;

        /// <summary>tracker to detect recursion during lazy loading.</summary>
        private bool propertiesLoadingHitPreviously = false;
#endif

        /// <summary>list of custom annotations that needs to be flowed via $metadata endpoint.</summary>
        private Dictionary<string, object> customAnnotations;

        /// <summary>Version of the resource type.</summary>
        private Version metadataVersion;

        /// <summary>Schema version of the resource type.</summary>
        private MetadataEdmSchemaVersion schemaVersion = (MetadataEdmSchemaVersion)(-1);

        #endregion Fields

        #region Constructors

        /// <summary>Creates an instance of a data service <see cref="T:Microsoft.OData.Service.Providers.ResourceType" />.</summary>
        /// <param name="instanceType">CLR type that represents the format inside the WCF Data Services?runtime.</param>
        /// <param name="resourceTypeKind">
        ///   <see cref="T:Microsoft.OData.Service.Providers.ResourceTypeKind" /> of the resource type.</param>
        /// <param name="baseType">Base type of the resource type as string.</param>
        /// <param name="namespaceName">Namespace name of the resource type as string.</param>
        /// <param name="name">Name of the given resource type as string.</param>
        /// <param name="isAbstract">Boolean value that indicates whether the resource type is an abstract type.</param>
        public ResourceType(
                    Type instanceType,
                    ResourceTypeKind resourceTypeKind,
                    ResourceType baseType,
                    string namespaceName,
                    string name,
                    bool isAbstract)
            : this(instanceType, baseType, namespaceName, name, isAbstract)
        {
            WebUtil.CheckArgumentNull(instanceType, "instanceType");
            WebUtil.CheckStringArgumentNullOrEmpty(name, "name");
            WebUtil.CheckResourceTypeKind(resourceTypeKind, "resourceTypeKind");
            if (resourceTypeKind == ResourceTypeKind.Primitive || resourceTypeKind == ResourceTypeKind.Collection || resourceTypeKind == Providers.ResourceTypeKind.EntityCollection)
            {
                throw new ArgumentException(Strings.ResourceType_InvalidValueForResourceTypeKind("resourceTypeKind"), "resourceTypeKind");
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
        /// Constructs a new instance of Resource type for the either given clr primitive type or collection type.
        /// This constructor must be called only for primitive or collection types.
        /// </summary>
        /// <param name="type">The instance type of the resource type.</param>
        /// <param name="resourceTypeKind"> kind of the resource type</param>
        /// <param name="namespaceName">namespace of the type.</param>
        /// <param name="name">name of the type.</param>
        internal ResourceType(Type type, ResourceTypeKind resourceTypeKind, string namespaceName, string name)
            : this(type, null, namespaceName, name, false)
        {
            Debug.Assert(
                resourceTypeKind == ResourceTypeKind.Primitive || resourceTypeKind == ResourceTypeKind.Collection || resourceTypeKind == Providers.ResourceTypeKind.EntityCollection,
                "Only primitive, collection or entity collection resource types can be created by this constructor.");
            this.resourceTypeKind = resourceTypeKind;
            this.isReadOnly = true;

            // Initialize versions - since those are the only mutable fields (lazy init makes them mutable) and primitive types are singletons
            // so we need to make sure there are no race conditions.
            switch (resourceTypeKind)
            {
                case ResourceTypeKind.Primitive:
                    this.InitializeMetadataAndSchemaVersionForPrimitiveType();
                    break;
                case ResourceTypeKind.Collection:
                    this.InitializeMetadataAndSchemaVersionForCollectionType();
                    break;
                case ResourceTypeKind.EntityCollection:
                    this.InitializeMetadataAndSchemaVersionForEntityCollectionType();
                    break;
            }
        }

        /// <summary>
        /// Constructs a new instance of Astoria type using the specified clr type
        /// </summary>
        /// <param name="type">clr type from which metadata needs to be pulled </param>
        /// <param name="baseType">base type of the resource type</param>
        /// <param name="namespaceName">Namespace name of the given resource type.</param>
        /// <param name="name">name of the given resource type.</param>
        /// <param name="isAbstract">whether the resource type is an abstract type or not.</param>
        private ResourceType(
                    Type type,
                    ResourceType baseType,
                    string namespaceName,
                    string name,
                    bool isAbstract)
        {
            WebUtil.CheckArgumentNull(type, "type");
            WebUtil.CheckArgumentNull(name, "name");

            this.name = name;
            this.namespaceName = namespaceName ?? string.Empty;

            // This is to optimize the string property name in NonEntitySerializer.WriteStartElementWithType function.
            // Checking here is a fixed overhead, and the gain is every time we serialize a string property.
            if (name == "String" && Object.ReferenceEquals(namespaceName, XmlConstants.EdmNamespace))
            {
                this.fullName = XmlConstants.EdmStringTypeName;
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

        #endregion Constructors

        #region Properties

        /// <summary>Gets or sets a Boolean value that is true if the resource type includes a default stream.</summary>
        /// <returns>true if the resource type includes a default stream; otherwise, false.</returns>
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
                if (this.resourceTypeKind != ResourceTypeKind.EntityType && value)
                {
                    throw new InvalidOperationException(Strings.ReflectionProvider_HasStreamAttributeOnlyAppliesToEntityType(this.name));
                }

                this.isMediaLinkEntry = value;
            }
        }

        /// <summary>Reference to the CLR type that this resource represents.</summary>
        /// <returns>The instance type as a <see cref="T:Microsoft.OData.Service.Providers.ResourceType" />.</returns>
        public Type InstanceType
        {
            [DebuggerStepThrough]
            get { return this.type; }
        }

        /// <summary>Gets a reference to base resource type, if any.</summary>
        /// <returns>The <see cref="T:Microsoft.OData.Service.Providers.ResourceType" /> of the base type.</returns>
        public ResourceType BaseType
        {
            [DebuggerStepThrough]
            get { return this.baseType; }
        }

        /// <summary>Gets the <see cref="T:Microsoft.OData.Service.Providers.ResourceTypeKind" /> for the type.</summary>
        /// <returns>The <see cref="T:Microsoft.OData.Service.Providers.ResourceTypeKind" /> for the type.</returns>
        public ResourceTypeKind ResourceTypeKind
        {
            [DebuggerStepThrough]
            get { return this.resourceTypeKind; }
        }

        /// <summary>Gets a list of properties declared on this type that includes only properties defined on the type, not in the base type.</summary>
        /// <returns>The <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of <see cref="T:Microsoft.OData.Service.Providers.ResourceProperty" />.</returns>
        public ReadOnlyCollection<ResourceProperty> Properties
        {
            get
            {
                return this.InitializeProperties();
            }
        }

        /// <summary>Gets the list of properties declared on this type.</summary>
        /// <returns>The <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of <see cref="T:Microsoft.OData.Service.Providers.ResourceProperty" />.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0014:DoNotHandleProhibitedExceptionsRule", Justification = "always rethrows the exception")]
        public ReadOnlyCollection<ResourceProperty> PropertiesDeclaredOnThisType
        {
            get
            {
                ReadOnlyCollection<ResourceProperty> readOnlyProperties =
                    this.propertiesDeclaredOnThisType as ReadOnlyCollection<ResourceProperty>;
                if (readOnlyProperties == null)
                {
                    if (!this.isReadOnly)
                    {
                        return this.CreateReadOnlyDeclaredPropertiesCollection();
                    }

                    lock (this.lockPropertiesLoad)
                    {
                        // check and see if another thread already did the work
                        readOnlyProperties = this.propertiesDeclaredOnThisType as ReadOnlyCollection<ResourceProperty>;
                        if (readOnlyProperties == null)
                        {
#if DEBUG
                            Debug.Assert(!this.propertiesDeclaredOnThisTypeLoadingHitPreviously, "We should not call this property recursively as it will mess up the lazy loading, attempts to make it work tend to break multithed scenarios, just don't do it.");
                            this.propertiesDeclaredOnThisTypeLoadingHitPreviously = true;
#endif
                            readOnlyProperties = this.CreateReadOnlyDeclaredPropertiesCollection();
                            this.ValidateType(readOnlyProperties);
                            this.propertiesDeclaredOnThisType = readOnlyProperties;
                        }
                    }
                }

                Debug.Assert(this.isReadOnly, "PropetiesDeclaredInThisType - at this point, the resource type must be readonly");
                return readOnlyProperties;
            }
        }

        /// <summary>Gets a list of key properties for this type</summary>
        /// <returns>The <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of <see cref="T:Microsoft.OData.Service.Providers.ResourceProperty" />.</returns>
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

        /// <summary>Gets the list of properties for this type.</summary>
        /// <returns>The <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of <see cref="T:Microsoft.OData.Service.Providers.ResourceType" />.</returns>
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

        /// <summary>Gets the name of the resource type.</summary>
        /// <returns>The name of the resource type.</returns>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>Gets the full name of the resource.</summary>
        /// <returns>The full name of the resource type as string.</returns>
        public string FullName
        {
            get { return this.fullName; }
        }

        /// <summary>Gets the namespace of the resource type.</summary>
        /// <returns>The namespace of the resource type.</returns>
        public string Namespace
        {
            get { return this.namespaceName; }
        }


        /// <summary>Gets the short qulified name of the resource type.</summary>
        /// <returns>The short qualified name of the resource type.</returns>
        public string ShortQualifiedName
        {
            get
            {
                if (this.Namespace != null && this.namespaceName.Equals("Edm"))
                {
                    return this.name;
                }

                return this.FullName;
            }
        }

        /// <summary>Gets a Boolean value that indicates whether this is an abstract type.</summary>
        /// <returns>true if <see cref="T:Microsoft.OData.Service.Providers.ResourceType" /> is abstract; otherwise, false.</returns>
        public bool IsAbstract
        {
            get { return this.abstractType; }
        }

        /// <summary>Gets whether the resource type has open properties.</summary>
        /// <returns>true if the resource type has open properties defined; otherwise, false.</returns>
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

                this.isOpenType = value;
            }
        }

        /// <summary>Gets whether the corresponding instance type represents the CLR type of this entity.</summary>
        /// <returns>True if the instance type represents a CLR type; otherwise false.</returns>
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

        /// <summary>Gets or sets a placeholder to hold custom state information about a resource type that is defined by the developer.</summary>
        /// <returns>The custom state information defined by the developer.</returns>
        public object CustomState
        {
            get;
            set;
        }

        /// <summary>Gets a Boolean value that is true if this resource type has been set to read-only.</summary>
        /// <returns>true if this resource type has been set to read-only; otherwise, false.</returns>
        public bool IsReadOnly
        {
            get { return this.isReadOnly; }
        }

        /// <summary> List of all named streams on this type (includes named streams defined on the base types) </summary>
        internal IEnumerable<ResourceProperty> NamedStreams
        {
            get
            {
                return this.Properties.Where(p => p.IsOfKind(ResourcePropertyKind.Stream));
            }
        }

        /// <summary> List of named streams declared on this type (does not includes named streams defined on the base types) </summary>
        internal IEnumerable<ResourceProperty> NamedStreamsDeclaredOnThisType
        {
            get
            {
                return this.PropertiesDeclaredOnThisType.Where(p => p.IsOfKind(ResourcePropertyKind.Stream));
            }
        }

        /// <summary>Indicates whether this type or one of its base types has a named stream.</summary>
        internal bool HasNamedStreams
        {
            get
            {
                Debug.Assert(this.isReadOnly, "this.isReadOnly");
                return this.Properties.Any(p => p.IsOfKind(ResourcePropertyKind.Stream));
            }
        }

        /// <summary>returns true if there is a named stream property declared on this type.</summary>
        internal bool HasNamedStreamsDeclaredOnThisType
        {
            get
            {
                Debug.Assert(this.isReadOnly, "this.isReadOnly");
                return this.PropertiesDeclaredOnThisType.Any(p => p.IsOfKind(ResourcePropertyKind.Stream));
            }
        }

        /// <summary>
        /// Returns the list of custom annotations defined on this set.
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

        /// <summary>
        /// Returns the version of the resource type based on the metadata.
        /// </summary>
        internal Version MetadataVersion
        {
            get
            {
                Debug.Assert(this.IsReadOnly, "this.IsReadOnly - this method must be called only after resource type is set to ReadOnly");
                if (this.metadataVersion == null)
                {
                    Debug.Assert(
                        this.resourceTypeKind == ResourceTypeKind.ComplexType || this.resourceTypeKind == ResourceTypeKind.EntityType,
                        "Primitive or Collection types should initialize their versions in the constructor and not lazily in the property getter.");
                    this.InitializeMetadataAndSchemaVersionForComplexOrEntityType();
                }

                return this.metadataVersion;
            }
        }

        /// <summary>
        /// Schema version of the resource type.
        /// </summary>
        internal MetadataEdmSchemaVersion SchemaVersion
        {
            get
            {
                if (this.schemaVersion == (MetadataEdmSchemaVersion)(-1))
                {
                    Debug.Assert(
                        this.resourceTypeKind == ResourceTypeKind.ComplexType || this.resourceTypeKind == ResourceTypeKind.EntityType,
                        "Primitive or Collection types should initialize their versions in the constructor and not lazily in the property getter.");
                    this.InitializeMetadataAndSchemaVersionForComplexOrEntityType();
                }

                return this.schemaVersion;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>Gets a resource type that represent a primitive type when given a <see cref="T:System.Type" /> object.</summary>
        /// <returns>The resource type.</returns>
        /// <param name="type">The <see cref="T:System.Type" /> type from which to get the primitive type.</param>
        public static ResourceType GetPrimitiveResourceType(Type type)
        {
            return PrimitiveResourceTypeMap.TypeMap.GetPrimitive(type);
        }

        /// <summary>Gets a <see cref="T:Microsoft.OData.Service.Providers.CollectionResourceType" /> representing a collection of the specified <paramref name="itemType" /> items.</summary>
        /// <returns>A <see cref="T:Microsoft.OData.Service.Providers.CollectionResourceType" /> representing a collection of the specified <paramref name="itemType" /> items.</returns>
        /// <param name="itemType">The type of item in the collection.</param>
        public static CollectionResourceType GetCollectionResourceType(ResourceType itemType)
        {
            WebUtil.CheckArgumentNull(itemType, "itemType");
            return new CollectionResourceType(itemType);
        }

        /// <summary>Gets a <see cref="T:Microsoft.OData.Service.Providers.EntityCollectionResourceType" /> representing a collection of the specified <paramref name="itemType" /> items.</summary>
        /// <returns>A <see cref="T:Microsoft.OData.Service.Providers.MultiValueResourceType" /> object representing a collection of the specified <paramref name="itemType" /> items.</returns>
        /// <param name="itemType">The <see cref="T:Microsoft.OData.Service.Providers.ResourceType" /> of a single item in the collection.</param>
        public static EntityCollectionResourceType GetEntityCollectionResourceType(ResourceType itemType)
        {
            WebUtil.CheckArgumentNull(itemType, "itemType");
            return new EntityCollectionResourceType(itemType);
        }

        /// <summary>Adds the property supplied by the <paramref name="property" /> parameter to the type.</summary>
        /// <param name="property">The <see cref="T:Microsoft.OData.Service.Providers.ResourceProperty" /> property to be added.</param>
        public void AddProperty(ResourceProperty property)
        {
            WebUtil.CheckArgumentNull(property, "property");
            Debug.Assert(!string.IsNullOrEmpty(property.Name), "!string.IsNullOrEmpty(property.Name)");

            // only check whether the property with the same name exists in this type.
            // we will look in base types properties when the type is sealed.
            this.ThrowIfSealed();

            this.AddPropertyInternal(property);
        }

        /// <summary>Sets the resource type to read-only.</summary>
        public void SetReadOnly()
        {
#if DEBUG
            IList<ResourceProperty> currentPropertyCollection = this.propertiesDeclaredOnThisType;
#endif
            // If this is a collection resource type, set its ItemType to ReadOnly. This has to be done first
            // as CollectionResourceType instances are readonly to begin with, but their ItemTypes are not.
            if (this.ResourceTypeKind == Providers.ResourceTypeKind.Collection)
            {
                CollectionResourceType collectionType = this as CollectionResourceType;
                Debug.Assert(
                    collectionType != null,
                    "ResourceTypeKind is Collection but the instance is not of CollectionResourceType");
                collectionType.ItemType.SetReadOnly();
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
            Debug.Assert(Object.ReferenceEquals(this.propertiesDeclaredOnThisType, currentPropertyCollection), "We should not have modified the properties collection instance");
#endif
        }

        /// <summary>
        /// Compares whether the given 2 resource types references are equal.
        /// </summary>
        /// <param name="resourceType1">First resource type instance.</param>
        /// <param name="resourceType2">Second resource type instance.</param>
        /// <returns>true if the both the parameters refer to the same instance, otherwise returns false.</returns>
        internal static bool CompareReferences(ResourceType resourceType1, ResourceType resourceType2)
        {
            if (resourceType1 == resourceType2)
            {
                return true;
            }

            Debug.Assert(resourceType1.FullName != resourceType2.FullName, "if the reference do not match, then the full names must be different");
            return false;
        }

        /// <summary>
        /// Add the given annotation to the list of annotations that needs to be flowed via the $metadata endpoint
        /// </summary>
        /// <param name="annotationNamespace">NamespaceName to which the custom annotation belongs to.</param>
        /// <param name="annotationName">Name of the annotation.</param>
        /// <param name="annotationValue">Value of the annotation.</param>
        internal void AddCustomAnnotation(string annotationNamespace, string annotationName, object annotationValue)
        {
            Debug.Assert(
                this.resourceTypeKind == ResourceTypeKind.EntityType || this.resourceTypeKind == ResourceTypeKind.ComplexType,
                "Custom annotations are only allowed on entity or complex types.");

            WebUtil.ValidateAndAddAnnotation(ref this.customAnnotations, annotationNamespace, annotationName, annotationValue);
        }

        /// <summary>
        /// Changes the key property to non key property and removes it from the key properties list
        /// </summary>
        internal void RemoveKeyProperties()
        {
            Debug.Assert(!this.isReadOnly, "The resource type cannot be sealed - RemoveKeyProperties");
            ReadOnlyCollection<ResourceProperty> key = this.KeyProperties;

            Debug.Assert(key.Count == 1, "Key Properties count must be zero");
            Debug.Assert(this.BaseType == null, "BaseType must be null");
            Debug.Assert(key[0].IsOfKind(ResourcePropertyKind.Key), "must be key property");

            ResourceProperty property = key[0];
            property.Kind = property.Kind ^ ResourcePropertyKind.Key;
        }

        /// <summary>Tries to find the property for the specified name.</summary>
        /// <param name="propertyName">Name of property to resolve.</param>
        /// <returns>Resolved property; possibly null.</returns>
        /// <remarks>This will search ALL properties declared on the type.</remarks>
        internal ResourceProperty TryResolvePropertyName(string propertyName)
        {
            return this.TryResolvePropertyName(propertyName, 0);
        }

        /// <summary>Tries to find the property for the specified name, excluding the specific kinds of property.</summary>
        /// <param name="propertyName">Name of property to resolve.</param>
        /// <param name="exceptKind">The property kind to filter out.</param>
        /// <remarks>NamedStream is a special kind of property that should be excluded when querying properties declared on a type. The exception is when the scenario specifically asks for ALL properties.</remarks>
        /// <returns>Resolved property; possibly null.</returns>
        internal ResourceProperty TryResolvePropertyName(string propertyName, ResourcePropertyKind exceptKind)
        {
            // In case of empty property name this will return null, which means propery is not found
            return this.Properties.FirstOrDefault(p => p.Name == propertyName && (p.Kind & exceptKind) == 0);
        }

        /// <summary>Tries to find the property declared on this type for the specified name.</summary>
        /// <param name="propertyName">Name of property to resolve.</param>
        /// <returns>Resolved property; possibly null.</returns>
        internal ResourceProperty TryResolvePropertiesDeclaredOnThisTypeByName(string propertyName)
        {
            return this.TryResolvePropertiesDeclaredOnThisTypeByName(propertyName, 0);
        }

        /// <summary>Tries to find the property declared on this type for the specified name.</summary>
        /// <param name="propertyName">Name of property to resolve.</param>
        /// <param name="exceptKind">The property kind to filter out.</param>
        /// <returns>Resolved property; possibly null.</returns>
        internal ResourceProperty TryResolvePropertiesDeclaredOnThisTypeByName(string propertyName, ResourcePropertyKind exceptKind)
        {
            // In case of empty property name this will return null, which means propery is not found
            return this.PropertiesDeclaredOnThisType.FirstOrDefault(p => p.Name == propertyName && (p.Kind & exceptKind) == 0);
        }

        /// <summary>
        /// Determines the minimum version that can be used for this specific type.
        /// 
        /// Note: if you don't know the specific type only the set, you will need to find the maximum of this
        ///       for all types in the hierarchy for the set type see ResourceSetWrapper.MinimumPayloadVersion
        /// </summary>
        /// <param name="service">The data service instance</param>
        /// <param name="resourceSet">The set that the type belongs to.</param>
        /// <returns>The minimum version that can be used for a payload of this specific type.</returns>
        internal Version GetMinimumResponseVersion(IDataService service, ResourceSetWrapper resourceSet)
        {
            Debug.Assert(resourceSet.ResourceType.IsAssignableFrom(this), "The passed in resourceSet does not include this type");
            Version minimumVersion = VersionUtil.Version4Dot0;

            // If target set contains collection properties we need v4.0
            // If target type contains named streams, we need v4.0
            minimumVersion = VersionUtil.RaiseVersion(minimumVersion, this.GetMinimumProtocolVersion());

            // If the resource type is an open type, then we do not know the metadata of the open property and hence cannot
            // predict the response version. Hence we need to bump the version to the maximum, 
            // and if we encounter during serialization, anything greater than the computed response version, we will throw instream error.
            if (this.IsOpenType)
            {
                Version maxProtocolVersion = service.Configuration.DataServiceBehavior.MaxProtocolVersion.ToVersion();
                Version requestMaxVersion = service.OperationContext.RequestMessage.RequestMaxVersion;
                Version responseVersion = (requestMaxVersion < maxProtocolVersion) ? requestMaxVersion : maxProtocolVersion;
                minimumVersion = VersionUtil.RaiseVersion(minimumVersion, responseVersion);
            }

            return minimumVersion;
        }

        /// <summary>
        /// For protocol version, we just need to check for the features that the type uses, and return the minimum
        /// protocol version for the type.
        /// </summary>
        /// <returns>minimum protocol version that is required for this resource type.</returns>
        internal Version GetMinimumProtocolVersion()
        {
            Version minimumVersion = VersionUtil.Version4Dot0;

            // If target set contains collection properties we need v4.0
            // If target type contains named streams, we need v4.0
            return VersionUtil.RaiseVersion(minimumVersion, this.MetadataVersion);
        }

        /// <summary>
        /// Checks if the given type is assignable to this type. In other words, if this type
        /// is a subtype of the given type or not.
        /// </summary>
        /// <param name="subType">resource type to check.</param>
        /// <returns>true, if the given type is assignable to this type. Otherwise returns false.</returns>
        internal bool IsAssignableFrom(ResourceType subType)
        {
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
        /// Gets the property info for the resource property
        /// </summary>
        /// <param name="resourceProperty">Resource property instance to get the property info</param>
        /// <returns>Returns the propertyinfo object for the specified resource property.</returns>
        /// <remarks>The method searchies this type as well as all its base types for the property.</remarks>
        internal PropertyInfo GetPropertyInfo(ResourceProperty resourceProperty)
        {
            return this.GetResourcePropertyInfo(resourceProperty).PropertyInfo;
        }

        /// <summary>
        /// Get the value of the given property
        /// </summary>
        /// <param name="resourceProperty">Resource property instance to get the property info</param>
        /// <param name="target">Instance of the declaring type.</param>
        /// <returns>the value of the property from the target.</returns>
        internal object GetPropertyValue(ResourceProperty resourceProperty, object target)
        {
            return this.GetResourcePropertyInfo(resourceProperty).PropertyGetter(target);
        }

        /// <summary>
        /// Gets an enumeration containing this type and all its base types.
        /// </summary>
        /// <returns>This type and all its base types.</returns>
        internal IEnumerable<ResourceType> BaseTypesAndSelf()
        {
            ResourceType resourceType = this;
            for (; resourceType != null; resourceType = resourceType.BaseType)
            {
                yield return resourceType;
            }
        }

        /// <summary>Returns a list of properties declared by this resource type. </summary>
        /// <returns>The list of properties declared on this type.</returns>
        /// <remarks>
        /// This method gives a chance to lazy load the properties of a resource type, instead of loading them upfront. 
        /// This property will be called once and only once, whenever ResourceType.Properties or ResourceType.PropertiesDeclaredOnThisType property is accessed.
        /// </remarks>
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
        /// Create the ReadOnly collection of Properties declared explicitly on this type
        /// </summary>
        /// <returns>A readonly collection of properties declared explicitly on this type.</returns>
        private ReadOnlyCollection<ResourceProperty> CreateReadOnlyDeclaredPropertiesCollection()
        {
            // This method will call the virtual method, if that's not been called yet and add the list of properties
            // returned by the virtual method to the properties collection.
            this.GetPropertiesDeclaredOnThisType();
            ReadOnlyCollection<ResourceProperty> readOnlyProperties = new ReadOnlyCollection<ResourceProperty>(this.propertiesDeclaredOnThisType ??
                                                                                             ResourceType.EmptyProperties);
            return readOnlyProperties;
        }

        /// <summary>
        /// Initializes all properties for the resource type, to be used by Properties getter.
        /// </summary>
        /// <returns>Collection of properties exposed by this resource type.</returns>
        private ReadOnlyCollection<ResourceProperty> InitializeProperties()
        {
            if (this.allProperties == null)
            {
                if (!this.isReadOnly)
                {
                    // this type can still change, thus don't cache, just return
                    return this.CreateReadOnlyPropertiesCollection();
                }

                lock (this.lockPropertiesLoad)
                {
                    if (this.allProperties == null)
                    {
#if DEBUG
                        Debug.Assert(!this.propertiesLoadingHitPreviously, "We should not call this property recursively as it will mess up the lazy loading, attempts to make it work tend to break multithed scenarios, just don't do it.");
                        this.propertiesLoadingHitPreviously = true;
#endif
                        // this type is static now, cache all props and return
                        this.allProperties = this.CreateReadOnlyPropertiesCollection();
                    }
                }
            }

            Debug.Assert(this.isReadOnly, "Propeties - at this point, the resource type must be readonly");
            return this.allProperties;
        }

        /// <summary>
        /// Creates a ReadOnly colleciton of all the properties visible on this type.
        /// </summary>
        /// <returns>ReadOnly colleciton of all the properties visible on this type.</returns>
        private ReadOnlyCollection<ResourceProperty> CreateReadOnlyPropertiesCollection()
        {
            List<ResourceProperty> allProps = new List<ResourceProperty>();
            if (this.BaseType != null)
            {
                allProps.AddRange(this.BaseType.Properties);
            }

            allProps.AddRange(this.PropertiesDeclaredOnThisType);
            return new ReadOnlyCollection<ResourceProperty>(allProps);
        }

        /// <summary>
        /// Gets the property info for the resource property
        /// </summary>
        /// <param name="resourceProperty">Resource property instance to get the property info</param>
        /// <returns>Returns the propertyinfo object for the specified resource property.</returns>
        /// <remarks>The method searchies this type as well as all its base types for the property.</remarks>
        private ResourcePropertyInfo GetResourcePropertyInfo(ResourceProperty resourceProperty)
        {
            Debug.Assert(resourceProperty != null, "resourceProperty != null");
            Debug.Assert(resourceProperty.CanReflectOnInstanceTypeProperty, "resourceProperty.CanReflectOnInstanceTypeProperty");

            ResourcePropertyInfo propertyInfo = null;
            ResourceType resourceType = this;
            while (propertyInfo == null && resourceType != null)
            {
                propertyInfo = resourceType.GetPropertyInfoDecaredOnThisType(resourceProperty);
                resourceType = resourceType.BaseType;
            }

            Debug.Assert(propertyInfo != null, "propertyInfo != null");
            return propertyInfo;
        }

        /// <summary>
        /// Gets the property info for the resource property declared on this type
        /// </summary>
        /// <param name="resourceProperty">Resource property instance to get the property info</param>
        /// <returns>Returns the propertyinfo object for the specified resource property.</returns>
        private ResourcePropertyInfo GetPropertyInfoDecaredOnThisType(ResourceProperty resourceProperty)
        {
            Debug.Assert(resourceProperty != null, "resourceProperty != null");
            Debug.Assert(resourceProperty.CanReflectOnInstanceTypeProperty, "resourceProperty.CanReflectOnInstanceTypeProperty");

            if (this.propertyInfosDeclaredOnThisType == null)
            {
                this.propertyInfosDeclaredOnThisType = new Dictionary<ResourceProperty, ResourcePropertyInfo>(ReferenceEqualityComparer<ResourceProperty>.Instance);
            }

            ResourcePropertyInfo resourcePropertyInfo;
            const BindingFlags BindingFlags = WebUtil.PublicInstanceBindingFlags;

            // TryGetValue is not thread-safe, MSDN suggests that we use a ReaderWriterLock to synchronize access to this 
            // dictionary http://msdn.microsoft.com/en-us/library/system.threading.readerwriterlock.aspx
            // Instead, we're going to lock on the second TryGetValue call as this will ensure that once
            // the resourcetype has its properties initialized, there are no locks for modifications to this property.
            if (!this.propertyInfosDeclaredOnThisType.TryGetValue(resourceProperty, out resourcePropertyInfo))
            {
                lock (this.propertiesDeclaredOnThisType)
                {
                    if (!this.propertyInfosDeclaredOnThisType.TryGetValue(resourceProperty, out resourcePropertyInfo))
                    {
                        PropertyInfo propertyInfo = this.InstanceType.GetProperty(resourceProperty.Name, BindingFlags);
                        if (propertyInfo == null)
                        {
                            throw new DataServiceException(500, Strings.BadProvider_UnableToGetPropertyInfo(this.FullName, resourceProperty.Name));
                        }

                        resourcePropertyInfo = this.AddPropertyToResourceTypePropertyInfoCache(resourceProperty, propertyInfo);
                    }
                }
            }

            Debug.Assert(resourcePropertyInfo != null, "propertyInfo != null");
            return resourcePropertyInfo;
        }

        /// <summary>
        /// Adds A resourcepropertyInfo instance to the internal propertyInfosDeclaredOnThisType dictionary.
        /// </summary>
        /// <param name="resourceProperty">The resource property to add to the dictionary</param>
        /// <param name="propertyInfo">The propertyInfo handle to the instance property represented by the <paramref name="resourceProperty"/> </param>
        /// <returns>A resourcepropertyInfo instance that allows access to the instance property</returns>
        private ResourcePropertyInfo AddPropertyToResourceTypePropertyInfoCache(ResourceProperty resourceProperty, PropertyInfo propertyInfo)
        {
            ResourcePropertyInfo resourcePropertyInfo = new ResourcePropertyInfo(propertyInfo);
            var propertiesDictionary = new Dictionary<ResourceProperty, ResourcePropertyInfo>(this.propertyInfosDeclaredOnThisType, ReferenceEqualityComparer<ResourceProperty>.Instance);
            propertiesDictionary.Add(resourceProperty, resourcePropertyInfo);
            Interlocked.Exchange(ref this.propertyInfosDeclaredOnThisType, propertiesDictionary);
            return resourcePropertyInfo;
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
        private void AddPropertyInternal(ResourceProperty property)
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

                    if (typeof(Microsoft.Spatial.ISpatial).IsAssignableFrom(property.ResourceType.type))
                    {
                        throw new InvalidOperationException(Strings.ResourceType_SpatialKeyOrETag(property.Name, this.name));
                    }

                    Debug.Assert(property.TypeKind == ResourceTypeKind.Primitive, "This check must have been done in ResourceProperty.ValidatePropertyParameters method");
                    Debug.Assert(!property.IsOfKind(ResourcePropertyKind.ETag), "This check must have been done in ResourceProperty.ValidatePropertyParameters method");
                    Debug.Assert(property.IsOfKind(ResourcePropertyKind.Primitive), "This check must have been done in ResourceProperty.ValidatePropertyParameters method");
                }

                if (property.IsOfKind(ResourcePropertyKind.ETag))
                {
                    if (this.ResourceTypeKind != ResourceTypeKind.EntityType)
                    {
                        throw new InvalidOperationException(Strings.ResourceType_ETagPropertiesOnlyOnEntityTypes);
                    }

                    if (typeof(Microsoft.Spatial.ISpatial).IsAssignableFrom(property.ResourceType.type))
                    {
                        throw new InvalidOperationException(Strings.ResourceType_SpatialKeyOrETag(property.Name, this.name));
                    }

                    Debug.Assert(property.TypeKind == ResourceTypeKind.Primitive, "This check must have been done in ResourceProperty.ValidatePropertyParameters method");
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
                    this.AddPropertyInternal(p);

                    // if this type is already set to readonly, make sure that new properties returned by the virtual method
                    // are also set to readonly
                    if (this.IsReadOnly)
                    {
                        p.SetReadOnly();
                    }
                }

                Debug.Assert(!this.isLoadPropertiesMethodCalled, "The LoadPropertiesDeclaredOnThisType() was called more than once, because of multi-threading");
                this.isLoadPropertiesMethodCalled = true;
            }
        }

        /// <summary>
        /// This method is called only when the Properties property is called and the type is already set to read-only.
        /// This method validates all the properties w.r.t to the base type and calls SetReadOnly on all the properties.
        /// </summary>
        /// <param name="declaredProperties">The declaredProperties of the current type.</param>
        private void ValidateType(ReadOnlyCollection<ResourceProperty> declaredProperties)
        {
            Debug.Assert(this.isLoadPropertiesMethodCalled && this.IsReadOnly, "This method must be invoked only if LoadPropertiesDeclaredOnThisType has been called and the type is set to ReadOnly");

            if (this.BaseType != null)
            {
                // make sure that there are no properties with the same name. Properties with duplicate name within the type
                // is already checked in AddProperty method
                foreach (ResourceProperty rp in this.BaseType.Properties)
                {
                    if (declaredProperties.Where(p => p.Name == rp.Name).FirstOrDefault() != null)
                    {
                        throw new InvalidOperationException(Strings.ResourceType_PropertyWithSameNameAlreadyExists(rp.Name, this.FullName));
                    }
                }
            }
            else if (this.ResourceTypeKind == ResourceTypeKind.EntityType)
            {
                if (declaredProperties.Where(p => p.IsOfKind(ResourcePropertyKind.Key)).FirstOrDefault() == null)
                {
                    throw new InvalidOperationException(Strings.ResourceType_MissingKeyPropertiesForEntity(this.FullName));
                }
            }

            // set all the properties to readonly
            foreach (ResourceProperty p in declaredProperties)
            {
                p.SetReadOnly();

                // Note that we cache the propertyinfo objects for each CLR properties in the ResourceType class
                // rather than the ResourceProperty class because the same ResourceProperty instance can be added
                // to multiple ResourceType instances.
                if (p.CanReflectOnInstanceTypeProperty)
                {
                    this.GetPropertyInfoDecaredOnThisType(p);
                }
            }
        }

        /// <summary>
        /// Initialize metadata and schema version for this primitive resource type.
        /// </summary>
        private void InitializeMetadataAndSchemaVersionForPrimitiveType()
        {
            Debug.Assert(this.resourceTypeKind == ResourceTypeKind.Primitive, "This method only works on primitive types.");
            Debug.Assert(this.metadataVersion == null, "this.metadataVersion == null");
            Debug.Assert(this.schemaVersion == (MetadataEdmSchemaVersion)(-1), "this.schemaVersion == (MetadataEdmSchemaVersion)(-1)");

            this.metadataVersion = VersionUtil.Version4Dot0;
            this.schemaVersion = MetadataEdmSchemaVersion.Version4Dot0;
        }

        /// <summary>
        /// Initialize metadata and schema version for this collection resource type.
        /// </summary>
        private void InitializeMetadataAndSchemaVersionForCollectionType()
        {
            Debug.Assert(this.resourceTypeKind == ResourceTypeKind.Collection, "This method only works on collection types.");
            Debug.Assert(this.metadataVersion == null, "this.metadataVersion == null");
            Debug.Assert(this.schemaVersion == (MetadataEdmSchemaVersion)(-1), "this.schemaVersion == (MetadataEdmSchemaVersion)(-1)");

            // Bump up the metadata version to 4.0 and schema version to 4.0
            this.metadataVersion = VersionUtil.Version4Dot0;
            this.schemaVersion = MetadataEdmSchemaVersion.Version4Dot0;
        }

        /// <summary>
        /// Initialize metadata and schema version for this entity collection resource type.
        /// </summary>
        private void InitializeMetadataAndSchemaVersionForEntityCollectionType()
        {
            Debug.Assert(this.resourceTypeKind == ResourceTypeKind.EntityCollection, "This method only works on entity collection types.");
            Debug.Assert(this.metadataVersion == null, "this.metadataVersion == null");
            Debug.Assert(this.schemaVersion == (MetadataEdmSchemaVersion)(-1), "this.schemaVersion == (MetadataEdmSchemaVersion)(-1)");

            this.metadataVersion = VersionUtil.Version4Dot0;
            this.schemaVersion = MetadataEdmSchemaVersion.Version4Dot0;
        }

        /// <summary>
        /// Initialize metadata and schema version for this resource type.
        /// </summary>
        private void InitializeMetadataAndSchemaVersionForComplexOrEntityType()
        {
            Debug.Assert(this.metadataVersion == null, "this.metadataVersion == null");
            Debug.Assert(this.schemaVersion == (MetadataEdmSchemaVersion)(-1), "this.schemaVersion == (MetadataEdmSchemaVersion)(-1)");
            Debug.Assert(
                this.resourceTypeKind == ResourceTypeKind.ComplexType || this.resourceTypeKind == ResourceTypeKind.EntityType,
                "This method only works on complex or entity types.");

            Version resourceTypeMetadataVersion = VersionUtil.Version4Dot0;
            MetadataEdmSchemaVersion resourceTypeSchemaVersion = MetadataEdmSchemaVersion.Version4Dot0;

            if (this.baseType != null)
            {
                resourceTypeMetadataVersion = this.baseType.MetadataVersion;
                resourceTypeSchemaVersion = this.baseType.SchemaVersion;
            }

            MetadataEdmSchemaVersion propertySchemaVersion;
            resourceTypeMetadataVersion = VersionUtil.RaiseVersion(
                resourceTypeMetadataVersion,
                this.ComputeMetadataAndSchemaVersionForPropertyCollection(this.PropertiesDeclaredOnThisType, null, out propertySchemaVersion));
            resourceTypeSchemaVersion = VersionUtil.RaiseVersion(resourceTypeSchemaVersion, propertySchemaVersion);

            this.metadataVersion = resourceTypeMetadataVersion;
            this.schemaVersion = resourceTypeSchemaVersion;
        }

        /// <summary>
        /// Computes metadata and schema version for the given property collection.
        /// </summary>
        /// <param name="propertyCollection">List of resource properties whose metadata version needs to be calculated.</param>
        /// <param name="visitedComplexTypes">List of complex types visited.</param>
        /// <param name="propertySchemaVersion">Returns the schema version of the resource property collection.</param>
        /// <returns>The metadata version of the resource property collection.</returns>
        private Version ComputeMetadataAndSchemaVersionForPropertyCollection(IEnumerable<ResourceProperty> propertyCollection, HashSet<ResourceType> visitedComplexTypes, out MetadataEdmSchemaVersion propertySchemaVersion)
        {
            Version propertyMetadataVersion = VersionUtil.Version4Dot0;
            propertySchemaVersion = MetadataEdmSchemaVersion.Version4Dot0;

            foreach (ResourceProperty property in propertyCollection)
            {
                if (property.IsOfKind(ResourcePropertyKind.ComplexType))
                {
                    if (visitedComplexTypes == null)
                    {
                        visitedComplexTypes = new HashSet<ResourceType>(ReferenceEqualityComparer<ResourceType>.Instance);
                    }
                    else if (visitedComplexTypes.Contains(property.ResourceType))
                    {
                        continue;
                    }

                    visitedComplexTypes.Add(property.ResourceType);

                    // If the property is complex type property, raise the version to the version of complex type
                    // To avoid endless loops in complex types, instead of calling Version property on complex type
                    // we call this method which recursively walks through the tree and gets the max version for the
                    // tree. Since the version is cached at the entity level, for every entity, we compute this once.
                    // But this is not cached for complex type until the Version property on complex type is called
                    // which happens when the request uri target is complex type.
                    MetadataEdmSchemaVersion complexTypeSchemaVersion;
                    propertyMetadataVersion = VersionUtil.RaiseVersion(
                        propertyMetadataVersion,
                        this.ComputeMetadataAndSchemaVersionForPropertyCollection(property.ResourceType.PropertiesDeclaredOnThisType, visitedComplexTypes, out complexTypeSchemaVersion));
                    propertySchemaVersion = VersionUtil.RaiseVersion(propertySchemaVersion, complexTypeSchemaVersion);
                }
                else if (property.IsOfKind(ResourcePropertyKind.Primitive) ||
                         property.IsOfKind(ResourcePropertyKind.Collection) ||
                         property.IsOfKind(ResourcePropertyKind.Stream))
                {
                    propertyMetadataVersion = VersionUtil.RaiseVersion(propertyMetadataVersion, property.ResourceType.MetadataVersion);
                    propertySchemaVersion = VersionUtil.RaiseVersion(propertySchemaVersion, property.ResourceType.SchemaVersion);
                }
            }

            return propertyMetadataVersion;
        }

        #endregion Methods

        /// <summary>
        /// Private class to cache ResourceProperty info
        /// </summary>
        private class ResourcePropertyInfo
        {
            /// <summary>
            /// Constructs a new instance of ResourceProperty info.
            /// </summary>
            /// <param name="propertyInfo">PropertyInfo instance.</param>
            internal ResourcePropertyInfo(PropertyInfo propertyInfo)
            {
                ParameterExpression instance = Expression.Parameter(typeof(Object), "instance");

                this.PropertyInfo = propertyInfo;
                this.PropertyGetter = (Func<object, object>)Expression.Lambda(
                    Expression.Convert(
                        Expression.Call(
                            Expression.Convert(instance, propertyInfo.DeclaringType),
                            propertyInfo.GetGetMethod()),
                        typeof(Object)),
                        instance).Compile();
            }

            /// <summary>
            /// PropertyInfo for the given property.
            /// </summary>
            internal PropertyInfo PropertyInfo
            {
                get;
                private set;
            }

            /// <summary>
            /// Compiled expression to get the property value.
            /// </summary>
            internal Func<object, object> PropertyGetter
            {
                get;
                private set;
            }
        }
    }
}
