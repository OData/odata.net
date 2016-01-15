//---------------------------------------------------------------------
// <copyright file="ModelBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Utils.ODataLibTest
{
    using System;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Test.OData.Utils.Common;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.Test.OData.Utils.ODataLibOM;

    /// <summary>
    /// A helper class to dynamically build models.
    /// </summary>
    public static class ModelBuilder
    {
        /// <summary>Default namespace for the test model.</summary>
        private const string ModelNamespace = "TestModel";

        /// <summary>Default name for container.</summary>
        private const string DefaultContainerName = "DefaultContainer";

        /// <summary>An internal counter for the number of generated types.</summary>
        private static uint generatedTypeCount = 0;

        /// <summary>Returns an identifier that is unique across all uses of the model builder.</summary>
        /// <returns>The next unique identifier to use.</returns>
        public static uint NextUniqueId()
        {
            return generatedTypeCount++;
        }

        /// <summary>
        /// Creates a new entity container with the specified name.
        /// </summary>
        /// <param name="model">The <see cref="EntityModelSchema"/> to create the entity container in.</param>
        /// <param name="containerName">The local name (without namespace) for the entity container to create.</param>
        /// <param name="namespaceName">The namespce name for the entity container to create.</param>
        /// <returns>The newly created entity container instance.</returns>
        public static EdmEntityContainer EntityContainer(this EdmModel model, string containerName, string namespaceName = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(containerName, "containerName");

            var entityContainer = new EdmEntityContainer(namespaceName ?? ModelNamespace, containerName);
            model.AddElement(entityContainer);
            return entityContainer;
        }

        /// <summary>
        /// Creates a new entity type with the specified name.
        /// </summary>
        /// <param name="model">The <see cref="EntityModelSchema"/> to create the entity type in.</param>
        /// <param name="localName">The local name (without namespace) for the entity type to create.</param>
        /// <param name="namespaceName">The (optional) namespace name for the type to create.</param>
        /// <param name="baseType">The (optional) base type of the entity.</param>
        /// <param name="isAbstract">The (optional) indication that the type is abstract.</param>
        /// <param name="isOpen">The (optional) indication that the type is open.</param>
        /// <returns>The newly created entity type instance.</returns>
        public static EdmEntityType EntityType(this EdmModel model, string localName, string namespaceName = null, IEdmEntityType baseType = null, bool isAbstract = false, bool isOpen = false)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(localName, "localName");

            namespaceName = namespaceName ?? ModelNamespace;
            EdmEntityType entityType = new EdmEntityType(namespaceName, localName, baseType, isAbstract, isOpen);

            model.AddElement(entityType);
            return entityType;
        }

        /// <summary>
        /// Creates a new enum type with the specified name.
        /// </summary>
        /// <param name="model">The <see cref="EntityModelSchema"/> to create the enum type in.</param>
        /// <param name="localName">The local name (without namespace) for the enum type to create.</param>
        /// <param name="namespaceName">The (optional) namespace name for the type to create.</param>
        /// <param name="underlyingType">The (optional) underlying type of the enum.</param>
        /// <param name="isFlags">If the enum type is flag (optional).</param>
        /// <returns>The newly created enum type instance.</returns>
        public static EdmEnumType EnumType(this EdmModel model, string localName, string namespaceName = null, EdmPrimitiveTypeKind underlyingType = EdmPrimitiveTypeKind.Int32, bool isFlags = false)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(localName, "localName");

            namespaceName = namespaceName ?? ModelNamespace;
            EdmEnumType enumType = new EdmEnumType(namespaceName, localName, underlyingType, isFlags);

            model.AddElement(enumType);
            return enumType;
        }

        /// <summary>
        /// Creates a new function import with the specified name.
        /// </summary>
        /// <param name="container">The <see cref="EntityContainer"/> to create the function import in.</param>
        /// <param name="localName">The name for the function import to create.</param>
        /// <returns>The newly created function import instance.</returns>
        public static EdmFunctionImport FunctionImport(this EdmEntityContainer container, IEdmFunction function)
        {
            ExceptionUtilities.CheckArgumentNotNull(container, "container");
            ExceptionUtilities.CheckArgumentNotNull(function, "function");

            return container.AddFunctionImport(function.Name, function);
        }

        /// <summary>
        /// Creates a new action import with the specified name.
        /// </summary>
        /// <param name="container">The <see cref="EntityContainer"/> to create the action import in.</param>
        /// <param name="localName">The name for the action import to create.</param>
        /// <returns>The newly created action import instance.</returns>
        public static EdmActionImport ActionImport(this EdmEntityContainer container, IEdmAction action)
        {
            ExceptionUtilities.CheckArgumentNotNull(container, "container");

            return container.AddActionImport(action);
        }

        /// <summary>
        /// Adds a (primitive, complex or collection) property to the <paramref name="entityType"/>. 
        /// Returns the modified entity type for composability.
        /// </summary>
        /// <param name="entityType">The <see cref="EntityType"/> to add the new property to.</param>
        /// <param name="propertyName">The name of the property to add.</param>
        /// <param name="type">The data type of the property.</param>
        /// <param name="concurrencyMode">A flag indicating whether the property is an ETag property (default = false).</param>
        /// <returns>The <paramref name="entityType"/> instance after adding the property to it.</returns>
        public static EdmEntityType KeyProperty(this EdmEntityType entityType, string propertyName, IEdmTypeReference type, EdmConcurrencyMode concurrencyMode = EdmConcurrencyMode.None)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            var property = entityType.AddStructuralProperty(propertyName, type, string.Empty, concurrencyMode);
            entityType.AddKeys(property);

            return entityType;
        }


        /// <summary>
        /// <summary>
        /// Adds a new property to the <paramref name="entityType"/>.
        /// </summary>
        /// <param name="entityType">The entity type to add the property to.</param>
        /// <param name="propertyName">The name of the property to add.</param>
        /// <param name="propertyValue">The value of the property; expected to have a <see cref="EntityModelTypeAnnotation"/> to determine the type of the property.</param>
        /// <remarks>The method uses the <see cref="EntityModelTypeAnnotation"/> to create a property of the correct type.</remarks>
        public static EdmEntityType Property(this EdmEntityType entityType, EdmModel model, string propertyName, ODataValue propertyValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");
            ExceptionUtilities.CheckArgumentNotNull(propertyValue, "propertyValue");

            var primitiveValue = propertyValue as ODataPrimitiveValue;
            if (primitiveValue != null)
            {
                entityType.AddStructuralProperty(propertyName, MetadataUtils.GetPrimitiveTypeReference(primitiveValue.Value.GetType()));
            }

            var complexValue = propertyValue as ODataComplexValue;
            if (complexValue != null)
            {
                entityType.AddStructuralProperty(propertyName, model.FindDeclaredType(complexValue.TypeName).ToTypeReference());
            }

            var collectionValue = propertyValue as ODataCollectionValue;
            if (collectionValue != null)
            {

                entityType.AddStructuralProperty(propertyName, model.FindDeclaredType(collectionValue.TypeName).ToTypeReference());
            }

            var nullValue = propertyValue as ODataNullValue;
            if (nullValue != null)
            {
                //Since we can't know what type a null value should have been we will just use a string property
                entityType.AddStructuralProperty(propertyName, MetadataUtils.GetPrimitiveTypeReference(typeof(string)));
            }

            var streamValue = propertyValue as ODataStreamReferenceValue;
            if (streamValue != null)
            {
                entityType.AddStructuralProperty(propertyName, EdmPrimitiveTypeKind.Stream);
            }

            return entityType;
        }

        /// <summary>
        /// Adds a navigation property to the <paramref name="entityType"/>. This method creates an association type
        /// in order to add the navigation property.
        /// Returns the modified entity type for composability.
        /// </summary>
        /// <param name="entityType">The <see cref="EntityType"/> to add the navigation property to.</param>
        /// <param name="propertyName">The name of the property to add.</param>
        /// <param name="otherEndType">The type of the other end of the navigation property.</param>
        /// <returns>The <paramref name="entityType"/> instance after adding the navigation property to it.</returns>
        public static EdmEntityType NavigationProperty(this EdmEntityType entityType, string propertyName, EdmEntityType otherEndType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");

            // Create a navigation property representing one side of an association.
            // The partner representing the other side exists only inside this property and is not added to the target entity type,
            // so it should not cause any name collisions.
            EdmNavigationProperty navProperty = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                propertyName,
                otherEndType.ToTypeReference(),
                /*dependentProperties*/ null,
                /*principalProperties*/ null,
                /*containsTarget*/ false,
                EdmOnDeleteAction.None,
                "Partner",
                entityType.ToTypeReference(true),
                /*partnerDependentProperties*/ null,
                /*partnerPrincipalProperties*/ null,
                /*partnerContainsTarget*/ false,
                EdmOnDeleteAction.None);

            entityType.AddProperty(navProperty);
            return entityType;

        }

        /// <summary>
        /// Adds a stream reference property to the <paramref name="entityType"/>.
        /// Returns the modified entity type for composability.
        /// </summary>
        /// <param name="entityType">The <see cref="EntityType"/> to add the navigation property to.</param>
        /// <param name="propertyName">The name of the property to add.</param>
        /// <returns>The <paramref name="entityType"/> instance after adding the stream reference property to it.</returns>
        public static EdmEntityType StreamProperty(this EdmEntityType entityType, string propertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");

            // add the named stream property to the generated entity type
            entityType.AddStructuralProperty(propertyName, EdmPrimitiveTypeKind.Stream);
            return entityType;
        }

        /// <summary>
        /// Finds property on structural type by its name.
        /// </summary>
        /// <param name="structuralType">The structural type to search for the property.</param>
        /// <param name="propertyName">The name of the property to look for.</param>
        /// <returns>The member property found, or null if no such property exists.</returns>
        /// <remarks>The method will fail if there's more than one matching property.</remarks>
        public static IEdmProperty GetProperty(this EdmStructuredType structuralType, string propertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(structuralType, "structuralType");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");

            return structuralType.FindProperty(propertyName);
        }

        /// <summary>
        /// Adds a (primitive, complex or collection) property to the <paramref name="structuralType"/>. 
        /// Returns the modified structural type for composability.
        /// </summary>
        /// <typeparam name="T">The type of the structural type to add the property to.</typeparam>
        /// <param name="structuralType">The structural type to add the new property to.</param>
        /// <param name="propertyName">The name of the property to add.</param>
        /// <param name="type">The data type of the property.</param>
        /// <param name="isETagProperty">true if the property is an ETag property; otherwise false (default).</param>
        /// <returns>The <paramref name="structuralType"/> instance after adding the property to it.</returns>
        public static T Property<T>(this T structuralType, string propertyName, IEdmTypeReference type, EdmConcurrencyMode isETagProperty = EdmConcurrencyMode.None)
            where T : EdmStructuredType
        {
            ExceptionUtilities.CheckArgumentNotNull(structuralType, "structuralType");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            structuralType.AddStructuralProperty(propertyName, type, string.Empty, isETagProperty);
            return structuralType;
        }

        /// <summary>
        /// Adds a complex property to the <paramref name="structuralType"/>. 
        /// Returns the modified structural type for composability.
        /// </summary>
        /// <typeparam name="T">The type of the structural type to add the property to.</typeparam>
        /// <param name="structuralType">The structural type to add the new property to.</param>
        /// <param name="propertyName">The name of the property to add.</param>
        /// <param name="type">The data type of the property.</param>
        /// <param name="isNullable">true if the property should be nullable; otherwise false. The default is false.</param>
        /// <returns>The <paramref name="structuralType"/> instance after adding the property to it.</returns>
        public static T Property<T>(this T structuralType, string propertyName, EdmPrimitiveTypeKind type, bool isNullable = false)
            where T : EdmStructuredType
        {
            ExceptionUtilities.CheckArgumentNotNull(structuralType, "structuralType");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            structuralType.AddStructuralProperty(propertyName, type, isNullable);
            return structuralType;
        }

        /// <summary>
        /// Creates a new complex type with the specified name.
        /// </summary>
        /// <param name="model">The <see cref="EntityModelSchema"/> to create the complex type in.</param>
        /// <param name="localName">The local name (without namespace) for the complex type to create.</param>
        /// <param name="namespaceName">The (optional) namespace name for the type to create.</param>
        /// <returns>The newly created complex type instance.</returns>
        public static EdmComplexType ComplexType(this EdmModel model, string localName, string namespaceName = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(localName, "localName");

            namespaceName = namespaceName ?? ModelNamespace;
            EdmComplexType complexType = new EdmComplexType(namespaceName, localName);
            model.AddElement(complexType);
            return complexType;
        }

        /// <summary>
        /// Adds an entity set to the <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The model to add the entity set to.</param>
        /// <param name="name">The name of the entity set to add.</param>
        /// <param name="entityType">The entity type for the entity set.</param>
        /// <returns>The newly created entity set.</returns>
        public static EdmEntitySet EntitySet(this EdmModel model, string name, EdmEntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(name, "name");
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");

            var container = model.EntityContainer as EdmEntityContainer;
            if (container == null)
            {
                container = new EdmEntityContainer("ModelNamespace", "DefaultContainer");
                model.AddElement(container);
            }

            return container.AddEntitySet(name, entityType);
        }

        /// <summary>
        /// Finds a complex type by its name.
        /// </summary>
        /// <param name="name">The name or full name to look for.</param>
        /// <returns>The entity type found or null if none exists.</returns>
        public static EdmComplexType GetComplexType(this EdmModel model, string name)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(name, "name");

            return model.FindDeclaredType(name) as EdmComplexType;
        }

        /// <summary>
        /// Finds an entity type by its name.
        /// </summary>
        /// <param name="name">The name or full name to look for.</param>
        /// <returns>The entity type found or null if none exists.</returns>
        public static EdmEntityType GetEntityType(this EdmModel model, string name)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(name, "name");

            return model.FindDeclaredType(name) as EdmEntityType;
        }

        /// <summary>
        /// Finds an entity set by its name.
        /// </summary>
        /// <param name="name">The name to look for.</param>
        /// <returns>The entity set found or null if none exists.</returns>
        public static IEdmEntitySet GetEntitySet(this EdmModel model, string name)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(name, "name");

            IEdmEntityContainer container = model.EntityContainer;
            return container.FindEntitySet(name);
        }

        /// <summary>
        /// Finds a operation import by its name.
        /// </summary>
        /// <param name="name">The name to look for.</param>
        /// <returns>The operation import found or null if none exists.</returns>
        public static IEdmOperationImport GetFunctionImport(this EdmModel model, string name)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(name, "name");

            IEdmEntityContainer container = model.EntityContainer;
            return container.FindOperationImports(name).SingleOrDefault();
        }

        /// <summary>
        /// Gets the <see cref="EntityType"/> of the specified entry or feed payload element.
        /// </summary>
        /// <param name="payloadElement">The payload element to get the entity type for.</param>
        /// <param name="model">The model to find the entity type in.</param>
        /// <returns>The <see cref="EntityType"/> of the <paramref name="payloadElement"/>.</returns>
        public static IEdmEntityType GetPayloadElementEntityType(ODataAnnotatable payloadElement, EdmModel model)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckArgumentNotNull(model, "model");

            ExceptionUtilities.Assert(
                payloadElement is ODataEntry || payloadElement is ODataFeed,
                "Can only determine entity type for entry or feed payloads.");

            ODataFeed feed = payloadElement as ODataFeed;
            if (feed != null)
            {
                // A feed doesn't know it's type. If it doesn't have any entries we can't determine the type.
                var feedentry = feed.GetAnnotation<ODataFeedEntriesObjectModelAnnotation>().FirstOrDefault();
                if (feedentry != null)
                {
                    return model.FindDeclaredType(feedentry.TypeName) as IEdmEntityType;
                }

                return null;
            }

            ODataEntry entry = payloadElement as ODataEntry;
            if (entry != null)
            {
                return model.FindDeclaredType(entry.TypeName) as IEdmEntityType;
            }

            return null;
        }

        /// <summary>
        /// Gets the data type of a property value specified in the property instance payload element.
        /// </summary>
        /// <param name="propertyInstance">The property instance payload element to inspect.</param>
        /// <returns>The data type of the property value (can be used to define the metadata for this property).</returns>
        public static IEdmTypeReference GetPayloadElementPropertyValueType(IEdmStructuralProperty propertyInstance)
        {
            ExceptionUtilities.CheckArgumentNotNull(propertyInstance, "propertyInstance");
            return propertyInstance.Type;
        }

        /// <summary>
        /// Runs a set of fixup operations on the EdmModel:
        ///   * ensures the existence and consistency of the default container
        /// </summary>
        /// <param name="model">The Model to run the fixup operations on.</param>
        /// <returns>The <paramref name="model"/> after having executed all fixup operations.</returns>
        public static EdmModel Fixup(this EdmModel model)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");

            // Apply default fixups
            model.AddDefaultContainerFixup();

            return model;
        }

        /// <summary>
        /// Add default container into the model
        ///     * add top level EntitySet
        ///     * add NavigationPropertyBinding
        /// </summary>
        /// <param name="model">The Model to run the fixup operations on.</param>
        public static void AddDefaultContainerFixup(this EdmModel model)
        {
            AddDefaultContainerFixup(model, ModelNamespace);
        }

        /// <summary>
        /// Add default container into the model
        ///     * add top level EntitySet
        ///     * add NavigationPropertyBinding
        /// </summary>
        /// <param name="model">The Model to run the fixup operations on.</param>
        /// <param name="namespaceName">The namespace to create the default container under.</param>
        public static void AddDefaultContainerFixup(this EdmModel model, string namespaceName)
        {
            EdmEntityContainer container = null;
            if (model.EntityContainer == null)
            {
                container = new EdmEntityContainer(namespaceName, DefaultContainerName + "_sub");
                model.AddElement(container);
            }
            else
            {
                container = model.EntityContainer as EdmEntityContainer;
            }

            ExceptionUtilities.CheckArgumentNotNull(container, "DefaultContainer");

            // create EntitySet
            foreach (EdmEntityType entityType in model.SchemaElements.OfType<IEdmEntityType>().Where(e => e.BaseType == null && container.EntitySets().All(set => set.EntityType() != e)))
            {
                container.AddEntitySet(entityType.Name, entityType);
            }

            Func<IEdmEntityType, IEdmEntitySet> findEntitySet =
                (entityType) =>
                {
                    var searchType = entityType;
                    while (searchType != null)
                    {
                        var entitySet = container.EntitySets().FirstOrDefault(s => s.EntityType() == searchType);
                        if (entitySet != null)
                        {
                            return entitySet;
                        }

                        searchType = searchType.BaseEntityType();
                    }

                    ExceptionUtilities.Assert(false, "Failed to find an entity set for type " + entityType.FullName());
                    return null;
                };

            // create NavigationPropertyBinding
            foreach (EdmNavigationProperty property in model.SchemaElements.OfType<IEdmEntityType>().SelectMany(entityType => entityType.DeclaredNavigationProperties()))
            {
                var sourceEntitySet = findEntitySet(property.DeclaringEntityType) as EdmEntitySet;
                ExceptionUtilities.CheckArgumentNotNull(sourceEntitySet, "SourceEntitySet");

                var targetEntityType = (property.Type.Definition.TypeKind == EdmTypeKind.Collection)
                                            ? ((property.Type.Definition as EdmCollectionType).ElementType.Definition as EdmEntityType)
                                            : (property.Type.Definition as EdmEntityType);
                ExceptionUtilities.CheckArgumentNotNull(targetEntityType, "TargetEntityType");
                var targetEntitySet = findEntitySet(targetEntityType);
                ExceptionUtilities.CheckArgumentNotNull(targetEntitySet, "TargetEntitySet");

                if (sourceEntitySet.NavigationPropertyBindings.All(target => target.NavigationProperty != property))
                {
                    sourceEntitySet.AddNavigationTarget(property, targetEntitySet);
                }
            }
        }
    }
}
