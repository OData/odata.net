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

namespace System.Data.Services.Client
{
    #region Namespaces.

    using System;
    using System.Collections.Generic;
    using System.Data.Services.Client.Metadata;
    using System.Data.Services.Client.Providers;
    using System.Data.Services.Common;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Annotations;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.Edm.Library.Annotations;
    using c = System.Data.Services.Client;

    #endregion Namespaces.

    /// <summary>
    /// EdmModel describing the client metadata
    /// </summary>
    internal sealed class ClientEdmModel : EdmElement, IEdmModel
    {
        /// <summary>A cache that maps a client Clr type to it corresponding Edm type.</summary>
        private readonly Dictionary<Type, EdmTypeCacheValue> clrToEdmTypeCache = new Dictionary<Type, EdmTypeCacheValue>(EqualityComparer<Type>.Default);

        /// <summary>A cache that maps a client type name to the corresponding client type annotation.</summary>
        private readonly Dictionary<string, ClientTypeAnnotation> typeNameToClientTypeAnnotationCache =
            new Dictionary<string, ClientTypeAnnotation>(StringComparer.Ordinal);

        /// <summary>The annotations manager.</summary>
        private readonly EdmDirectValueAnnotationsManager directValueAnnotationsManager = new EdmDirectValueAnnotationsManager();

        /// <summary>The max protocol version this Edm model is created for.</summary>
        private readonly DataServiceProtocolVersion maxProtocolVersion;

        /// <summary>Referenced core model.</summary>
        private readonly IEnumerable<IEdmModel> coreModel = new IEdmModel[] { EdmCoreModel.Instance };

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maxProtocolVersion">The protocol version this Edm model is created for.</param>
        internal ClientEdmModel(DataServiceProtocolVersion maxProtocolVersion)
        {
            this.maxProtocolVersion = maxProtocolVersion;
        }

        /// <summary>
        /// Returns all the vocabulary annotations defined in the model.
        /// </summary>
        public IEnumerable<IEdmVocabularyAnnotation> VocabularyAnnotations
        {
            get { return Enumerable.Empty<IEdmVocabularyAnnotation>(); }
        }

        /// <summary>
        /// Returns all the referenced models.
        /// </summary>
        public IEnumerable<IEdmModel> ReferencedModels
        {
            get { return this.coreModel; }
        }

        /// <summary>
        /// Returns all the schema elements.
        /// </summary>
        public IEnumerable<IEdmSchemaElement> SchemaElements
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Returns the model's annotations manager.
        /// </summary>
        public IEdmDirectValueAnnotationsManager DirectValueAnnotationsManager
        {
            get
            {
                return this.directValueAnnotationsManager;
            }
        }

        /// <summary>
        /// Gets the max protocol version of the model.
        /// </summary>
        internal DataServiceProtocolVersion MaxProtocolVersion
        {
            get { return this.maxProtocolVersion; }
        }

        /// <summary>
        /// Return the entity container with the given name.
        /// </summary>
        /// <param name="name">Name of the entity container.</param>
        /// <returns>EntityContainer with the given name.</returns>
        public IEdmEntityContainer FindDeclaredEntityContainer(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches for a schema element with the given name in this model and returns null if no such schema element exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the schema element being found.</param>
        /// <returns>The requested schema element, or null if no such schema element exists.</returns>
        public IEdmSchemaType FindDeclaredType(string qualifiedName)
        {
            ClientTypeAnnotation clientTypeAnnotation = null;
            if (this.typeNameToClientTypeAnnotationCache.TryGetValue(qualifiedName, out clientTypeAnnotation))
            {
                return (IEdmSchemaType)clientTypeAnnotation.EdmType;
            }

            return null;
        }

        /// <summary>
        /// Searches for functions with the given name in this model and returns null if no such function exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the function being found.</param>
        /// <returns>A set functions sharing the specified qualified name, or an empty enumerable if no such function exists.</returns>
        public IEnumerable<IEdmFunction> FindDeclaredFunctions(string qualifiedName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches for a value term with the given name in this model and returns null if no such value term exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the value term being found.</param>
        /// <returns>The requested value term, or null if no such value term exists.</returns>
        public IEdmValueTerm FindDeclaredValueTerm(string qualifiedName)
        {
            return null;
        }

        /// <summary>
        /// Finds a list of types that derive directly from the supplied type.
        /// </summary>
        /// <param name="type">The base type that derived types are being searched for.</param>
        /// <returns>A list of types that derive directly from the type.</returns>
        public IEnumerable<IEdmStructuredType> FindDirectlyDerivedTypes(IEdmStructuredType type)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches for vocabulary annotations specified by this model or a referenced model for a given element.
        /// </summary>
        /// <param name="element">The annotated element.</param>
        /// <returns>The vocabulary annotations for the element.</returns>
        public IEnumerable<IEdmVocabularyAnnotation> FindDeclaredVocabularyAnnotations(IEdmVocabularyAnnotatable element)
        {
            return Enumerable.Empty<IEdmVocabularyAnnotation>();
        }

        /// <summary>
        /// Get or create a client EDM type instance.
        /// </summary>
        /// <param name="type">type to wrap</param>
        /// <returns>client type</returns>
        internal IEdmType GetOrCreateEdmType(Type type)
        {
            EdmTypeCacheValue cachedEdmValue = this.GetOrCreateEdmTypeInternal(type);
            IEdmType edmType = cachedEdmValue.EdmType;
            if (edmType.TypeKind == EdmTypeKind.Complex || edmType.TypeKind == EdmTypeKind.Entity)
            {
                bool? hasProperties = cachedEdmValue.HasProperties;
                if (!hasProperties.HasValue)
                {
                    hasProperties = ClientTypeUtil.GetPropertiesOnType(type, /*declaredOnly*/false).Any();

                    lock (this.clrToEdmTypeCache)
                    {
                        EdmTypeCacheValue existing = this.clrToEdmTypeCache[type];
                        existing.HasProperties = hasProperties;
                    }
                }

                if (hasProperties == false)
                {
                    throw c.Error.InvalidOperation(c.Strings.ClientType_NoSettableFields(type.ToString()));
                }
            }

            return edmType;
        }

        /// <summary>
        /// Get the client type annotation for the given name.
        /// </summary>
        /// <param name="edmTypeName">Name of the type.</param>
        /// <returns>An instance of ClientTypeAnnotation for the type with the given name.</returns>
        internal ClientTypeAnnotation GetClientTypeAnnotation(string edmTypeName)
        {
            Debug.Assert(WebUtil.GetCollectionItemWireTypeName(edmTypeName) == null, "This method must not be called for collections");
            IEdmType result = this.clrToEdmTypeCache.Values.First(e => e.EdmType.FullName() == edmTypeName).EdmType;
            Debug.Assert(result != null, "result != null");
            return this.GetClientTypeAnnotation(result);
        }

        /// <summary>Returns <paramref name="type"/> and its base types, in the order of most base type first and <paramref name="type"/> last.</summary>
        /// <param name="type">Type instance in question.</param>
        /// <param name="keyProperties">Returns the list of key properties if <paramref name="type"/> is an entity type; null otherwise.</param>
        /// <param name="hasProperties">true if <paramref name="type"/> has any (declared or inherited) properties; otherwise false.</param>
        /// <returns>Returns <paramref name="type"/> and its base types, in the order of most base type first and <paramref name="type"/> last.</returns>
        private static Type[] GetTypeHierarchy(Type type, out PropertyInfo[] keyProperties, out bool hasProperties)
        {
            Debug.Assert(type != null, "type != null");

            keyProperties = ClientTypeUtil.GetKeyPropertiesOnType(type, out hasProperties);

            List<Type> hierarchy = new List<Type>();
            if (keyProperties != null)
            {
                // type is an entity. Return all types between keyPropertyDeclaredType and type inclusive.
                Type baseEntityType;
                if (keyProperties.Length > 0)
                {
                    baseEntityType = keyProperties[0].DeclaringType;
                }
                else
                {
                    // Find the type where the DataServiceEntityAttribute is declared on.
                    baseEntityType = type;
                    Debug.Assert(type.GetCustomAttributes(true).OfType<DataServiceEntityAttribute>().Any(), "type.GetCustomAttributes(true).OfType<DataServiceEntityAttribute>().Any()");
                    while (!baseEntityType.GetCustomAttributes(false).OfType<DataServiceEntityAttribute>().Any() && c.PlatformHelper.GetBaseType(baseEntityType) != null)
                    {
                        baseEntityType = c.PlatformHelper.GetBaseType(baseEntityType);
                    }

                    Debug.Assert(baseEntityType != null, "keyPropertyDeclaringType != null");
                }

                do
                {
                    hierarchy.Insert(0, type);
                }
                while (type != baseEntityType && (type = c.PlatformHelper.GetBaseType(type)) != null);
            }
            else
            {
                // type is a complex type. Return all types on the hierarchy where there are properties defined.
                do
                {
                    hierarchy.Insert(0, type);
                }
                while ((type = c.PlatformHelper.GetBaseType(type)) != null && ClientTypeUtil.GetPropertiesOnType(type, false /*declaredOnly*/).Any());
            }

            return hierarchy.ToArray();
        }

        /// <summary>
        /// Find properties with dynamic MIME type related properties and 
        /// set the references from each ClientProperty to its related MIME type property
        /// </summary>
        /// <param name="edmStructuredType">Client edm type instance to wire up the mime type properties.</param>
        private void SetMimeTypeForProperties(IEdmStructuredType edmStructuredType)
        {
            MimeTypePropertyAttribute attribute = (MimeTypePropertyAttribute)this.GetClientTypeAnnotation(edmStructuredType).ElementType.GetCustomAttributes(typeof(MimeTypePropertyAttribute), true).SingleOrDefault();
            if (null != attribute)
            {
                IEdmProperty dataProperty = edmStructuredType.Properties().SingleOrDefault(p => p.Name == attribute.DataPropertyName);
                if (dataProperty == null)
                {
                    throw c.Error.InvalidOperation(c.Strings.ClientType_MissingMimeTypeDataProperty(this.GetClientTypeAnnotation(edmStructuredType).ElementTypeName, attribute.DataPropertyName));
                }

                IEdmProperty mimeTypeProperty = edmStructuredType.Properties().SingleOrDefault(p => p.Name == attribute.MimeTypePropertyName);
                if (mimeTypeProperty == null)
                {
                    throw c.Error.InvalidOperation(c.Strings.ClientType_MissingMimeTypeProperty(this.GetClientTypeAnnotation(edmStructuredType).ElementTypeName, attribute.MimeTypePropertyName));
                }

                this.GetClientPropertyAnnotation(dataProperty).MimeTypeProperty = this.GetClientPropertyAnnotation(mimeTypeProperty);
            }
        }

        /// <summary>
        /// Get or create a client EDM type instance.
        /// </summary>
        /// <param name="type">type to wrap</param>
        /// <returns>client type</returns>
        private EdmTypeCacheValue GetOrCreateEdmTypeInternal(Type type)
        {
            Debug.Assert(type != null, "type != null");

            EdmTypeCacheValue cachedEdmType;
            lock (this.clrToEdmTypeCache)
            {
                this.clrToEdmTypeCache.TryGetValue(type, out cachedEdmType);
            }

            if (cachedEdmType == null)
            {
                if (PrimitiveType.IsKnownNullableType(type))
                {
                    cachedEdmType = this.GetOrCreateEdmTypeInternal(null /*baseType*/, type, ClientTypeUtil.EmptyPropertyInfoArray, false /*isEntity*/, false /*hasProperties*/);
                }
                else
                {
                    PropertyInfo[] keyProperties;
                    bool hasProperties;
                    Type[] hierarchy = ClientEdmModel.GetTypeHierarchy(type, out keyProperties, out hasProperties);

                    Debug.Assert(keyProperties == null || keyProperties.Length == 0 || keyProperties.All(p => p.DeclaringType == keyProperties[0].DeclaringType), "All key properties must be declared on the same type.");

                    bool isEntity = keyProperties != null;
                    keyProperties = keyProperties ?? ClientTypeUtil.EmptyPropertyInfoArray;
                    foreach (Type t in hierarchy)
                    {
                        // Pass in the full list of key properties for the most base type to be added there.  We only allow key properties to be
                        // declared on the same type.
                        IEdmStructuredType edmBaseType = cachedEdmType == null ? null : cachedEdmType.EdmType as IEdmStructuredType;
                        cachedEdmType = this.GetOrCreateEdmTypeInternal(edmBaseType, t, keyProperties, isEntity, t == type ? hasProperties : (bool?)null);

                        // Pass in an empty PropertyInfo array on subsequent derived types.
                        keyProperties = ClientTypeUtil.EmptyPropertyInfoArray;
                    }
                }
            }

            Debug.Assert(cachedEdmType != null, "cachedEdmType != null");
            return cachedEdmType;
        }

        /// <summary>
        /// Get or create a client EDM type instance.
        /// </summary>
        /// <param name="edmBaseType">The base type of this structured type.</param>
        /// <param name="type">type to wrap</param>
        /// <param name="keyProperties">List of key properties to add to <paramref name="type"/> if the type is an entity type; null otherwise.</param>
        /// <param name="isEntity">true if <paramref name="type"/> is an entity type; false otherwise.</param>
        /// <param name="hasProperties">true if the <paramref name="type"/> is known to have properties; false if <paramref name="type"/> is known to have no properties; null if nothing is known about the properties.</param>
        /// <returns>client type</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502", Justification = "cyclomatic complexity")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:MethodCoupledWithTooManyTypesFromDifferentNamespaces", Justification = "should refactor the method in the future.")]
        private EdmTypeCacheValue GetOrCreateEdmTypeInternal(IEdmStructuredType edmBaseType, Type type, PropertyInfo[] keyProperties, bool isEntity, bool? hasProperties)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(keyProperties != null, "keyProperties != null");

            EdmTypeCacheValue cachedEdmType;
            lock (this.clrToEdmTypeCache)
            {
                this.clrToEdmTypeCache.TryGetValue(type, out cachedEdmType);
            }

            if (cachedEdmType == null)
            {
                Type collectionType;
                if (PrimitiveType.IsKnownNullableType(type))
                {
                    PrimitiveType primitiveType;
                    PrimitiveType.TryGetPrimitiveType(type, out primitiveType);
                    Debug.Assert(primitiveType != null, "primitiveType != null");
                    cachedEdmType = new EdmTypeCacheValue(primitiveType.CreateEdmPrimitiveType(), hasProperties);
                }
                else if ((collectionType = ClientTypeUtil.GetImplementationType(type, typeof(ICollection<>))) != null && ClientTypeUtil.GetImplementationType(type, typeof(IDictionary<,>)) == null)
                {
                    // Collection Type
                    Type elementType = collectionType.GetGenericArguments()[0];
                    IEdmType itemType = this.GetOrCreateEdmTypeInternal(elementType).EdmType;

                    // Note that while collection of a collection is not allowed, we cannot throw here since it's a breaking change.
                    // We will throw during SaveChanges() and we have unit test validating the error case.
                    Debug.Assert(
                        itemType.TypeKind == EdmTypeKind.Entity || itemType.TypeKind == EdmTypeKind.Complex || itemType.TypeKind == EdmTypeKind.Primitive || itemType.TypeKind == EdmTypeKind.Collection,
                        "itemType.TypeKind == EdmTypeKind.Entity || itemType.TypeKind == EdmTypeKind.Complex || itemType.TypeKind == EdmTypeKind.Primitive || itemType.TypeKind == EdmTypeKind.Collection");

                    cachedEdmType = new EdmTypeCacheValue(new EdmCollectionType(itemType.ToEdmTypeReference(ClientTypeUtil.CanAssignNull(elementType))), hasProperties);
                }
                else
                {
                    if (isEntity)
                    {
                        Action<EdmEntityTypeWithDelayLoadedProperties> delayLoadEntityProperties = (entityType) =>
                        {
                            // Create properties without modifying the entityType.
                            // This will leave entityType intact in case of an exception during loading.
                            List<IEdmProperty> loadedProperties = new List<IEdmProperty>();
                            List<IEdmStructuralProperty> loadedKeyProperties = new List<IEdmStructuralProperty>();
                            foreach (PropertyInfo property in ClientTypeUtil.GetPropertiesOnType(type, /*declaredOnly*/edmBaseType != null).OrderBy(p => p.Name))
                            {
                                // A better approach for getting to the value of a property without triggering lazy construction
                                // could be desired, but I don't control the auto proxy generation stuff, so for now...
                                // we just favor convwention over configuration and expect to find a field with the same name
                                // as the property, but prefixed with __ or _.
                                FieldInfo backingField = type.GetField("__" + property.Name, BindingFlags.NonPublic | BindingFlags.Instance);
                                if (backingField == null)
                                    backingField = type.GetField("_" + property.Name, BindingFlags.NonPublic | BindingFlags.Instance);
                                IEdmProperty edmProperty = this.CreateEdmProperty((EdmStructuredType)entityType, property, backingField);
                                loadedProperties.Add(edmProperty);

                                if (edmBaseType == null && keyProperties.Any(k => k.DeclaringType == type && k.Name == property.Name))
                                {
                                    Debug.Assert(edmProperty.PropertyKind == EdmPropertyKind.Structural, "edmProperty.PropertyKind == EdmPropertyKind.Structural");
                                    Debug.Assert(edmProperty.Type.TypeKind() == EdmTypeKind.Primitive, "edmProperty.Type.TypeKind() == EdmTypeKind.Primitive");
                                    loadedKeyProperties.Add((IEdmStructuralProperty)edmProperty);
                                }
                            }

                            // Now add properties to the entityType.
                            foreach (IEdmProperty property in loadedProperties)
                            {
                                entityType.AddProperty(property);
                            }

                            entityType.AddKeys(loadedKeyProperties);
                        };

                        // Creating an entity type
                        Debug.Assert(edmBaseType == null || edmBaseType.TypeKind == EdmTypeKind.Entity, "baseType == null || baseType.TypeKind == EdmTypeKind.Entity");
                        cachedEdmType = new EdmTypeCacheValue(
                            new EdmEntityTypeWithDelayLoadedProperties(CommonUtil.GetModelTypeNamespace(type), CommonUtil.GetModelTypeName(type), (IEdmEntityType)edmBaseType, c.PlatformHelper.IsAbstract(type), false /*isOpen*/, delayLoadEntityProperties),
                            hasProperties);
                    }
                    else
                    {
                        Action<EdmComplexTypeWithDelayLoadedProperties> delayLoadComplexProperties = (complexType) =>
                        {
                            // Create properties without modifying the complexType.
                            // This will leave complexType intact in case of an exception during loading.
                            List<IEdmProperty> loadedProperties = new List<IEdmProperty>();
                            foreach (PropertyInfo property in ClientTypeUtil.GetPropertiesOnType(type, /*declaredOnly*/edmBaseType != null).OrderBy(p => p.Name))
                            {
                                // A better approach for getting to the value of a property without triggering lazy construction
                                // could be desired, but I don't control the auto proxy generation stuff, so for now...
                                // we just favor convwention over configuration and expect to find a field with the same name
                                // as the property, but prefixed with __ or _.
                                FieldInfo backingField = type.GetField("__" + property.Name, BindingFlags.NonPublic | BindingFlags.Instance);
                                if (backingField == null)
                                    backingField = type.GetField("_" + property.Name, BindingFlags.NonPublic | BindingFlags.Instance);
                                IEdmProperty edmProperty = this.CreateEdmProperty(complexType, property, backingField);
                                loadedProperties.Add(edmProperty);
                            }

                            // Now add properties to the complexType.
                            foreach (IEdmProperty property in loadedProperties)
                            {
                                complexType.AddProperty(property);
                            }
                        };

                        // Creating a complex type
                        Debug.Assert(edmBaseType == null || edmBaseType.TypeKind == EdmTypeKind.Complex, "baseType == null || baseType.TypeKind == EdmTypeKind.Complex");
                        cachedEdmType = new EdmTypeCacheValue(
                            new EdmComplexTypeWithDelayLoadedProperties(CommonUtil.GetModelTypeNamespace(type), CommonUtil.GetModelTypeName(type), (IEdmComplexType)edmBaseType, c.PlatformHelper.IsAbstract(type), delayLoadComplexProperties),
                            hasProperties);
                    }
                }

                Debug.Assert(cachedEdmType != null, "cachedEdmType != null");
                
                IEdmType edmType = cachedEdmType.EdmType;
                ClientTypeAnnotation clientTypeAnnotation = this.GetOrCreateClientTypeAnnotation(edmType, type);
                this.SetClientTypeAnnotation(edmType, clientTypeAnnotation);

                if (edmType.TypeKind == EdmTypeKind.Entity || edmType.TypeKind == EdmTypeKind.Complex)
                {
                    IEdmStructuredType edmStructuredType = edmType as IEdmStructuredType;
                    Debug.Assert(edmStructuredType != null, "edmStructuredType != null");
                    this.SetMimeTypeForProperties(edmStructuredType);
                }

                // Need to cache the type before loading the properties so we don't stack overflow because
                // loading the property can trigger calls to GetOrCreateEdmType on the same type.
                lock (this.clrToEdmTypeCache)
                {
                    EdmTypeCacheValue existing;
                    if (this.clrToEdmTypeCache.TryGetValue(type, out existing))
                    {
                        cachedEdmType = existing;
                    }
                    else
                    {
                        this.clrToEdmTypeCache.Add(type, cachedEdmType);
                    }
                }
            }

            return cachedEdmType;
        }

        /// <summary>
        /// Creates an Edm property.
        /// </summary>
        /// <param name="declaringType">Type declaring this property.</param>
        /// <param name="propertyInfo">PropertyInfo instance for this property.</param>
        /// <returns>Returns a new instance of Edm property.</returns>
        private IEdmProperty CreateEdmProperty(IEdmStructuredType declaringType, PropertyInfo propertyInfo, FieldInfo backingField)
        {
            IEdmType propertyEdmType = this.GetOrCreateEdmTypeInternal(propertyInfo.PropertyType).EdmType;
            Debug.Assert(
                propertyEdmType.TypeKind == EdmTypeKind.Entity ||
                propertyEdmType.TypeKind == EdmTypeKind.Complex ||
                propertyEdmType.TypeKind == EdmTypeKind.Primitive ||
                propertyEdmType.TypeKind == EdmTypeKind.Collection,
                "Property kind should be Entity, Complex, Primitive or Collection.");

            IEdmProperty edmProperty;
            bool isPropertyNullable = ClientTypeUtil.CanAssignNull(propertyInfo.PropertyType);
            if (propertyEdmType.TypeKind == EdmTypeKind.Entity || (propertyEdmType.TypeKind == EdmTypeKind.Collection && ((IEdmCollectionType)propertyEdmType).ElementType.TypeKind() == EdmTypeKind.Entity))
            {
                IEdmEntityType declaringEntityType = declaringType as IEdmEntityType;
                if (declaringEntityType == null)
                {
                    throw c.Error.InvalidOperation(c.Strings.ClientTypeCache_NonEntityTypeCannotContainEntityProperties(propertyInfo.Name, propertyInfo.DeclaringType.ToString()));
                }

                // Create a navigation property representing one side of an association.
                // The partner representing the other side exists only inside this property and is not added to the target entity type,
                // so it should not cause any name collisions.
                edmProperty = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                    propertyInfo.Name,
                    propertyEdmType.ToEdmTypeReference(isPropertyNullable),
                    /*dependentProperties*/ null,
                    /*containsTarget*/ false,
                    EdmOnDeleteAction.None,
                    "Partner",
                    declaringEntityType.ToEdmTypeReference(true),
                    /*partnerDependentProperties*/ null,
                    /*partnerContainsTarget*/ false,
                    EdmOnDeleteAction.None);
            }
            else
            {
                edmProperty = new EdmStructuralProperty(declaringType, propertyInfo.Name, propertyEdmType.ToEdmTypeReference(isPropertyNullable));
            }

            edmProperty.SetClientPropertyAnnotation(new ClientPropertyAnnotation(edmProperty, propertyInfo, backingField, this));
            return edmProperty;
        }

        /// <summary>
        /// Gets or creates client type annotation.
        /// </summary>
        /// <param name="edmType">The EdmType to use for creating client type annotation</param>
        /// <param name="type">The Clr type to create client type annotation for.</param>
        /// <returns>Client type annotation</returns>
        private ClientTypeAnnotation GetOrCreateClientTypeAnnotation(IEdmType edmType, Type type)
        {
            ClientTypeAnnotation clientTypeAnnotation;
            string qualifiedName = type.ToString();

            if (edmType.TypeKind == EdmTypeKind.Complex || edmType.TypeKind == EdmTypeKind.Entity)
            {
                lock (this.typeNameToClientTypeAnnotationCache)
                {
                    if (this.typeNameToClientTypeAnnotationCache.TryGetValue(qualifiedName, out clientTypeAnnotation) && clientTypeAnnotation.ElementType != type)
                    {
                        qualifiedName = type.AssemblyQualifiedName;
                        if (this.typeNameToClientTypeAnnotationCache.TryGetValue(qualifiedName, out clientTypeAnnotation) && clientTypeAnnotation.ElementType != type)
                        {
                            throw c.Error.InvalidOperation(Strings.ClientType_MultipleTypesWithSameName(qualifiedName));
                        }
                    }

                    if (clientTypeAnnotation == null)
                    {
                        clientTypeAnnotation = new ClientTypeAnnotation(edmType, type, qualifiedName, this);
                        this.typeNameToClientTypeAnnotationCache.Add(qualifiedName, clientTypeAnnotation);
                    }
                    else
                    {
                        Debug.Assert(clientTypeAnnotation.ElementType == type, "existing clientTypeAnnotation.ElementType == type");
                    }
                }
            }
            else
            {
                clientTypeAnnotation = new ClientTypeAnnotation(edmType, type, qualifiedName, this);
            }

            return clientTypeAnnotation;
        }

        /// <summary>
        /// Cache value for the type cache.
        /// </summary>
        private sealed class EdmTypeCacheValue
        {
            /// <summary>The cached EDM type.</summary>
            private readonly IEdmType edmType;

            /// <summary>true if the Clr type this EDM type is based on has settable properties; otherwise false.</summary>
            private bool? hasProperties;

            /// <summary>
            /// Creates a new instance of the EDM type cache value.
            /// </summary>
            /// <param name="edmType">The cached EDM type.</param>
            /// <param name="hasProperties">true if the Clr type this EDM type is based on has settable properties; otherwise false.</param>
            public EdmTypeCacheValue(IEdmType edmType, bool? hasProperties)
            {
                this.edmType = edmType;
                this.hasProperties = hasProperties;
            }

            /// <summary>
            /// The cached EDM type.
            /// </summary>
            public IEdmType EdmType
            {
                get
                {
                    return this.edmType;
                }
            }

            /// <summary>
            /// true if the Clr type this EDM type is based on has settable properties; otherwise false.
            /// </summary>
            public bool? HasProperties
            {
                get
                {
                    return this.hasProperties;
                }

                set
                {
                    this.hasProperties = value;
                }
            }
        }
    }
}
