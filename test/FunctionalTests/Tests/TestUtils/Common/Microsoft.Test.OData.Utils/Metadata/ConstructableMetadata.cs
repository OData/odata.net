//---------------------------------------------------------------------
// <copyright file="ConstructableMetadata.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Utils.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;

    /// <summary>Metadata definition for the DSP. This also implements the <see cref="IEdmModel"/>.</summary>
    public class ConstructableMetadata : IEdmEntityContainer, IEdmModel
    {
        /// <summary>The annotations manager.</summary>
        private readonly IEdmDirectValueAnnotationsManager directValueAnnotationsManager;

        /// <summary>List of entity sets. Dictionary where key is the name of the entity set and value is the entity set itself.</summary>
        /// <remarks>Note that we store this such that we can quickly lookup an entity set based on its name.</remarks>
        private readonly Dictionary<string, IEdmEntitySet> entitySets;

        /// <summary>List of singletons. Dictionary where key is the name of the singleton and value is the singleton itself.</summary>
        /// <remarks>Note that we store this such that we can quickly lookup an singleton based on its name.</remarks>
        private readonly Dictionary<string, IEdmSingleton> singletons;

        /// <summary>List of entity types. Dictionary where key is the full name of the entity type and value is the entity type itself.</summary>
        /// <remarks>Note that we store this such that we can quickly lookup an entity type based on its name.</remarks>
        private readonly Dictionary<string, IEdmEntityType> entityTypes;

        /// <summary>List of complex types. Dictionary where key is the full name of the complex type and value is the complex type itself.</summary>
        /// <remarks>Note that we store this such that we can quickly lookup a complex type based on its name.</remarks>
        private readonly Dictionary<string, IEdmComplexType> complexTypes;

        /// <summary>List of operation imports. Dictionary where key is the full name of the operation and value is the operation itself.</summary>
        /// <remarks>Note that we store this such that we can quickly lookup a operation based on its name.</remarks>
        private readonly Dictionary<string, List<IEdmOperation>> operations;

        /// <summary>List of operation imports. Dictionary where key is the full name of the operationImport and value is the operationImport itself.</summary>
        /// <remarks>Note that we store this such that we can quickly lookup a operationImport based on its name.</remarks>
        private readonly Dictionary<string, List<IEdmOperationImport>> operationImports;

        private readonly Dictionary<IEdmStructuredType, List<IEdmStructuredType>> derivedTypeMappings;

        /// <summary>Name of the container to report.</summary>
        private readonly string containerName;

        /// <summary>Namespace name.</summary>
        private readonly string namespaceName;

        /// <summary>Creates new empty metadata definition.</summary>
        /// <param name="containerName">Name of the container to report.</param>
        /// <param name="namespaceName">Namespace name.</param>
        public ConstructableMetadata(string containerName, string namespaceName)
        {
            this.directValueAnnotationsManager = new EdmDirectValueAnnotationsManager();
            this.entitySets = new Dictionary<string, IEdmEntitySet>();
            this.singletons = new Dictionary<string, IEdmSingleton>();
            this.entityTypes = new Dictionary<string, IEdmEntityType>();
            this.complexTypes = new Dictionary<string, IEdmComplexType>();
            this.operationImports = new Dictionary<string, List<IEdmOperationImport>>();
            this.operations = new Dictionary<string, List<IEdmOperation>>();
            this.derivedTypeMappings = new Dictionary<IEdmStructuredType, List<IEdmStructuredType>>();
            this.containerName = containerName;
            this.namespaceName = namespaceName;
        }

        /// <summary>Adds a new entity type (without any properties).</summary>
        /// <param name="name">The name of the type.</param>
        /// <param name="instanceType">The instance type or null if this should be untyped resource.</param>
        /// <param name="baseType">The base type.</param>
        /// <param name="isAbstract">If the type should be abstract.</param>
        /// <returns>The newly created entity type.</returns>
        public IEdmEntityType AddEntityType(string name, Type instanceType, IEdmEntityType baseType, bool isAbstract)
        {
            return this.AddEntityType(name, instanceType, baseType, isAbstract, this.namespaceName);
        }

        /// <summary>Adds a new entity type (without any properties).</summary>
        /// <param name="name">The name of the type.</param>
        /// <param name="instanceType">The instance type or null if this should be untyped resource.</param>
        /// <param name="baseType">The base type.</param>
        /// <param name="isAbstract">If the type should be abstract.</param>
        /// <param name="nameSpace">The namespace of the entity type</param>
        /// <returns>The newly created entity type.</returns>
        public IEdmEntityType AddEntityType(string name, Type instanceType, IEdmEntityType baseType, bool isAbstract, string nameSpace, bool isOpen = false)
        {
            EdmEntityType entityType = new EdmEntityType(
                this.namespaceName,
                name,
                baseType,
                isAbstract,
                isOpen);

            this.entityTypes.Add(entityType.FullName(), entityType);

            if (entityType.BaseType != null)
            {
                List<IEdmStructuredType> derivedTypes;
                if (!this.derivedTypeMappings.TryGetValue(entityType.BaseType, out derivedTypes))
                {
                    derivedTypes = new List<IEdmStructuredType>();
                    this.derivedTypeMappings[entityType.BaseType] = derivedTypes;
                }
            }

            return entityType;
        }

        /// <summary>Adds a new complex type (without any properties).</summary>
        /// <param name="name">The name of the type.</param>
        /// <param name="instanceType">The instance type or null if this should be untyped resource.</param>
        /// <param name="baseType">The base type.</param>
        /// <param name="isAbstract">If the type should be abstract.</param>
        /// <param name="nameSpace">The namespace of the complex type</param>
        /// <returns>The newly created complex type.</returns>
        public IEdmComplexType AddComplexType(string name, Type instanceType, IEdmComplexType baseType, bool isAbstract)
        {
            return this.AddComplexType(name, instanceType, baseType, isAbstract, this.namespaceName);
        }

        /// <summary>Adds a new complex type (without any properties).</summary>
        /// <param name="name">The name of the type.</param>
        /// <param name="instanceType">The instance type or null if this should be untyped resource.</param>
        /// <param name="baseType">The base type.</param>
        /// <param name="isAbstract">If the type should be abstract.</param>
        /// <returns>The newly created complex type.</returns>
        public IEdmComplexType AddComplexType(string name, Type instanceType, IEdmComplexType baseType, bool isAbstract, string nameSpace)
        {
            EdmComplexType complexType = new EdmComplexType(
                this.namespaceName,
                name,
                baseType,
                isAbstract);

            this.complexTypes.Add(complexType.FullName(), complexType);

            if (complexType.BaseType != null)
            {
                List<IEdmStructuredType> derivedTypes;
                if (!this.derivedTypeMappings.TryGetValue(complexType.BaseType, out derivedTypes))
                {
                    derivedTypes = new List<IEdmStructuredType>();
                    this.derivedTypeMappings[complexType.BaseType] = derivedTypes;
                }
            }

            return complexType;
        }

        /// <summary>Adds a key property to the specified <paramref name="structuredType"/>.</summary>
        /// <param name="structuredType">The entity type to add the property to.</param>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="propertyType">The CLR type of the property to add. This can be only a primitive type.</param>
        /// <param name="etag">true if the property should be part of the ETag</param>
        public void AddKeyProperty(IEdmStructuredType structuredType, string name, Type propertyType)
        {
            EdmEntityType entityType = structuredType as EdmEntityType;
            if (entityType == null)
            {
                throw new InvalidOperationException("Expected an EdmEntityType instance.");
            }

            IEdmStructuralProperty keyProperty = this.AddPrimitiveProperty(entityType, name, propertyType);

            AddKeyFragment(entityType, keyProperty);
        }

        /// <summary>Adds a named stream property to the specified <paramref name="structuredType"/>.</summary>
        /// <param name="structuredType">The type to add the property to.</param>
        /// <param name="streamName">The name of the stream to add.</param>
        /// <returns>The newly created property.</returns>
        public IEdmStructuralProperty AddNamedStreamProperty(IEdmStructuredType structuredType, string streamName)
        {
            IEdmPrimitiveTypeReference streamTypeReference = EdmCoreModel.Instance.GetStream(true);
            return AddStructuralProperty(structuredType, streamName, streamTypeReference);
        }

        /// <summary>Adds a key property to the specified <paramref name="structuredType"/>.</summary>
        /// <param name="structuredType">The type to add the property to.</param>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="propertyType">The CLR type of the property to add. This can be only a primitive type.</param>
        public IEdmStructuralProperty AddPrimitiveProperty(IEdmStructuredType structuredType, string name, Type propertyType)
        {
            PropertyInfo propertyInfo = null;
            if (propertyType == null)
            {
                //TODO: Fix this so GetInstanceType works
                //propertyInfo = structuredType.GetInstanceType(this).GetProperty(name);
                propertyType = propertyInfo != null ? propertyInfo.PropertyType : null;
            }

            IEdmPrimitiveTypeReference typeReference = MetadataUtils.GetPrimitiveTypeReference(propertyType);
            return AddStructuralProperty(structuredType, name, typeReference);
        }

        /// <summary>Adds a new structural property.</summary>
        /// <param name="structuredType">The type to add the property to.</param>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="propertyTypeReference">The type of the property to add.</param>
        /// <returns>The newly created and added property.</returns>
        private IEdmStructuralProperty AddStructuralProperty(
            IEdmStructuredType structuredType,
            string name,
            IEdmTypeReference propertyTypeReference)
        {
            EdmStructuralProperty property = new EdmStructuralProperty(
                structuredType,
                name,
                propertyTypeReference,
                /*defaultValue*/null);

            ((EdmStructuredType)structuredType).AddProperty(property);

            return property;
        }

        /// <summary>Adds a new navigation property.</summary>
        /// <param name="entityType">The entity type to add the property to.</param>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="deleteAction">The delete action of the nav property.</param>
        /// <param name="propertyTypeReference">The type of the property to add.</param>
        /// <param name="containsTarget">The contains target of the nav property</param>
        /// <returns>The newly created and added property.</returns>
        private IEdmNavigationProperty AddNavigationProperty(
            IEdmEntityType entityType,
            string name,
            EdmOnDeleteAction deleteAction,
            IEdmTypeReference propertyTypeReference,
            bool containsTarget)
        {
            // Create a navigation property representing one side of an association.
            // The partner representing the other side exists only inside this property and is not added to the target entity type,
            // so it should not cause any name collisions.
            EdmNavigationProperty navProperty = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                name,
                propertyTypeReference,
                /*dependentProperties*/ null,
                /*principalProperties*/ null,
                containsTarget,
                deleteAction,
                "Partner",
                entityType.ToTypeReference(true),
                /*partnerDependentProperties*/ null,
                /*partnerPrincipalProperties*/ null,
                /*partnerContainsTarget*/ false,
                EdmOnDeleteAction.None);

            ((EdmStructuredType)entityType).AddProperty(navProperty);

            return navProperty;
        }

        /// <summary>Adds a complex property to the specified <paramref name="structuredType"/>.</summary>
        /// <param name="structuredType">The type to add the property to.</param>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="complexType">Complex type to use for the property.</param>
        /// <param name="isCollection">Whether the property is a collection of complex type.</param>
        public void AddComplexProperty(IEdmStructuredType structuredType, string name, IEdmComplexType complexType, bool isCollection = false)
        {
            AddStructuralProperty(structuredType, name,
                isCollection ? ((EdmComplexTypeReference)complexType.ToTypeReference(true)).ToCollectionTypeReference() : complexType.ToTypeReference(true));
        }

        /// <summary>Adds a property of type multiValue of complex typed items.</summary>
        /// <param name="structuredType">The type to add the property to.</param>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="multiValueItemType">Complex or primitive type for items in the multiValue.</param>
        public void AddMultiValueProperty(IEdmStructuredType structuredType, string name, Type multiValueItemType)
        {
            IEdmTypeReference edmType = MetadataUtils.GetPrimitiveTypeReference(multiValueItemType);
            if (edmType.TypeKind() != EdmTypeKind.Complex && edmType.TypeKind() != EdmTypeKind.Primitive)
            {
                throw new ArgumentException("Only complex or primitive types are allowed as items for the multiValue in this method.");
            }

            AddMultiValueProperty(structuredType, name, MetadataUtils.GetPrimitiveTypeReference(multiValueItemType).ToCollectionTypeReference());
        }

        /// <summary>Adds a property of type multiValue of primitive typed items.</summary>
        /// <param name="structuredType">The type to add the property to.</param>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="multiValueItemPrimitiveType">Primitive type for items in the multiValue.</param>
        public void AddMultiValueProperty(IEdmStructuredType structuredType, string name, IEdmTypeReference multiValueItemType)
        {
            AddStructuralProperty(structuredType, name, multiValueItemType);
        }

        /// <summary>Adds a contained entity set resource reference property to the specified <paramref name="entityType"/>.</summary>
        /// <param name="entityType">The entity type to add the property to.</param>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="targetEntityType">The entity type the entity set reference property points to.</param>
        /// <remarks>This creates a property pointing to a single resource in the contained entity set.</remarks>
        public IEdmNavigationProperty AddContainedResourceReferenceProperty(IEdmEntityType entityType, string name, IEdmEntityType targetEntityType)
        {
            return AddReferenceProperty(entityType, name, null, targetEntityType, false, true);
        }

        /// <summary>Adds an contained entity set reference property to the specified <paramref name="entityType"/>.</summary>
        /// <param name="entityType">The entity type to add the property to.</param>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="targetEntityType">The entity type the entity set reference property points to.</param>
        /// <remarks>This creates a property pointing to multiple resources in the contained entity set.</remarks>
        public IEdmNavigationProperty AddContainedResourceSetReferenceProperty(IEdmEntityType entityType, string name, IEdmEntityType targetEntityType)
        {
            return AddReferenceProperty(entityType, name, null, targetEntityType, true, true);
        }

        /// <summary>Adds a resource reference property to the specified <paramref name="entityType"/>.</summary>
        /// <param name="entityType">The entity type to add the property to.</param>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="targetEntitySet">The entity set the resource reference property points to.</param>
        /// <param name="targetEntityType">The entity type the entity set reference property points to. 
        /// Can be null in which case the base entity type of the entity set is used.</param>
        /// <remarks>This creates a property pointing to a single resource in the target entity set.</remarks>
        public IEdmNavigationProperty AddResourceReferenceProperty(IEdmEntityType entityType, string name, IEdmEntitySet targetEntitySet, IEdmEntityType targetEntityType)
        {
            return AddReferenceProperty(entityType, name, targetEntitySet, targetEntityType, false, false);
        }

        /// <summary>Adds an entity set reference property to the specified <paramref name="entityType"/>.</summary>
        /// <param name="entityType">The entity type to add the property to.</param>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="targetEntitySet">The entity set the entity set reference property points to.</param>
        /// <param name="targetEntityType">The entity type the entity set reference property points to. 
        /// Can be null in which case the base entity type of the entity set is used.</param>
        /// <remarks>This creates a property pointing to multiple resources in the target entity set.</remarks>
        public IEdmNavigationProperty AddResourceSetReferenceProperty(IEdmEntityType entityType, string name, IEdmEntitySet targetEntitySet, IEdmEntityType targetEntityType)
        {
            return AddReferenceProperty(entityType, name, targetEntitySet, targetEntityType, true, false);
        }

        /// <summary>Helper method to add a reference property.</summary>
        /// <param name="entityType">The entity type to add the property to.</param>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="targetEntitySet">The entity set the resource reference property points to.</param>
        /// <param name="targetEntityType">The entity type the entity set reference property points to.</param>
        /// <param name="resourceSetReference">true if the property should be a entity set reference, false if it should be an entity reference.</param>
        private IEdmNavigationProperty AddReferenceProperty(IEdmEntityType entityType, string name, IEdmEntitySet targetEntitySet, IEdmEntityType targetEntityType, bool resourceSetReference, bool containsTarget)
        {
            targetEntityType = targetEntityType ?? targetEntitySet.EntityType();

            IEdmTypeReference navPropertyTypeReference = resourceSetReference
                ? new EdmCollectionType(targetEntityType.ToTypeReference(true)).ToTypeReference(true)
                : targetEntityType.ToTypeReference(true);

            IEdmNavigationProperty navigationProperty = AddNavigationProperty(
                entityType,
                name,
                EdmOnDeleteAction.None,
                navPropertyTypeReference,
                containsTarget);

            return navigationProperty;
        }

        /// <summary>Adds an entity set to the metadata definition.</summary>
        /// <param name="name">The name of the entity set to add.</param>
        /// <param name="entityType">The type of entities in the entity set.</param>
        /// <returns>The newly created entity set.</returns>
        public IEdmEntitySet AddEntitySet(string name, IEdmEntityType entityType)
        {
            if (entityType.TypeKind != EdmTypeKind.Entity)
            {
                throw new ArgumentException("The type specified as the base type of an entity set is not an entity type.");
            }

            EdmEntitySet entitySet = new EdmEntitySet(this, name, entityType);
            this.SetAnnotationValue<IEdmEntitySet>(entityType, entitySet);
            this.entitySets.Add(name, entitySet);
            return entitySet;
        }

        /// <summary>Adds a singleton to the metadata definition.</summary>
        /// <param name="name">The name of the singleton to add.</param>
        /// <param name="entityType">The type of entity in the singleton.</param>
        /// <returns>The newly created singleton.</returns>
        public IEdmSingleton AddSingleton(string name, IEdmEntityType entityType)
        {
            if (entityType.TypeKind != EdmTypeKind.Entity)
            {
                throw new ArgumentException("The type specified as the base type of a singleton is not an entity type.");
            }

            EdmSingleton singleton = new EdmSingleton(this, name, entityType);
            this.SetAnnotationValue<IEdmSingleton>(entityType, singleton);
            this.singletons.Add(name, singleton);
            return singleton;
        }

        /// <summary>
        /// Initializes a new <see cref="IEdmOperationImport"/> instance.
        /// </summary>
        /// <param name="name">name of the service operation.</param>
        /// <param name="function">Function imported in.</param>
        /// <param name="resultSet">EntitySet of the result expected from this operation.</param>
        public EdmFunctionImport AddFunctionAndFunctionImport(string name, IEdmTypeReference bindingType, IEdmTypeReference resultType, IEdmEntitySet resultSet, bool isBindable)
        {
            var edmFunction = new EdmFunction(this.Namespace, name, resultType, isBindable, null, false /*isComposable*/);
            if (isBindable)
            {
                edmFunction.AddParameter("bindingparameter", bindingType);
            }

            IEdmPathExpression entitySetExpression = null;
            if (resultSet != null)
            {
                entitySetExpression = new EdmPathExpression(resultSet.Name);
            }

            this.AddOperation(edmFunction);
            var functionImport = new EdmFunctionImport(this, name, edmFunction, entitySetExpression, false);
            this.AddOperationImport(name, functionImport);
            return functionImport;
        }

        /// <summary>
        /// Initializes a new <see cref="IEdmOperationImport"/> instance.
        /// </summary>
        /// <param name="name">name of the service operation.</param>
        /// <param name="function">Function imported in.</param>
        /// <param name="resultSet">EntitySet of the result expected from this operation.</param>
        public EdmFunctionImport AddFunctionImport(string name, IEdmFunction function, IEdmEntitySet resultSet, bool includeInServiceDocument)
        {
            var functionImport = new EdmFunctionImport(this, name, function, new EdmPathExpression(resultSet.Name), includeInServiceDocument);
            this.AddOperationImport(name, functionImport);
            return functionImport;
        }

        /// <summary>
        /// Adds the function.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="boundType">Type of the bound.</param>
        /// <param name="resultType">Type of the result.</param>
        /// <param name="isBound">if set to <c>true</c> [is bound].</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public EdmFunction AddFunction(string name, IEdmTypeReference boundType, IEdmTypeReference returnType, bool isBound, IEdmPathExpression entitySetPathExpression, bool isComposable)
        {
            var function = new EdmFunction(namespaceName, name, returnType, isBound, entitySetPathExpression, isComposable);
            if (isBound)
            {
                function.AddParameter("bindingparameter", boundType);
            }

            this.AddOperation(function);
            return function;
        }

        /// <summary>
        /// Adds the action.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="boundType">Type of the bound.</param>
        /// <param name="resultType">Type of the result.</param>
        /// <param name="isBound">if set to <c>true</c> [is bound].</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public EdmAction AddAction(string name, IEdmTypeReference boundType, IEdmTypeReference resultType, bool isBound, IEdmPathExpression entitySetPathExpression, params Tuple<string, IEdmTypeReference>[] parameters)
        {
            var action = new EdmAction(namespaceName, name, resultType, isBound, entitySetPathExpression);
            if (isBound)
            {
                action.AddParameter(new EdmOperationParameter(action, "bindingparameter", boundType));
            }

            this.AddOperation(action);
            return action;
        }

        /// <summary>
        /// Initializes a new <see cref="IEdmActionImport"/> instance.
        /// </summary>
        /// <param name="name">name of the service operation.</param>
        /// <param name="action">Action that the action import is importing into the container.</param>
        /// <param name="resultSet">EntitySet of the result expected from this operation.</param>
        /// <returns>An ActionImport</returns>
        public EdmActionImport AddActionImport(string name, IEdmAction action, IEdmEntitySet resultSet)
        {
            EdmActionImport actionImport = new EdmActionImport(this,
                name,
                action,
                resultSet == null ? null : new EdmPathExpression(resultSet.Name));

            this.AddOperationImport(name, actionImport);
            return actionImport;
        }

        #region IEdmEntityContainer Members
        public IEnumerable<IEdmEntityContainerElement> Elements
        {
            get
            {
                // NOTE: we don't expose association sets
                return this.entitySets.Values.Cast<IEdmEntityContainerElement>()
                    .Union(this.singletons.Values)
                    .Union(this.operationImports.Values.SelectMany(f => f));
            }
        }

        /// <summary>
        /// Searches for an entity set with the given name in this entity container and returns null if no such set exists.
        /// </summary>
        /// <param name="name">The name of the element being found.</param>
        /// <returns>The requested element, or null if the element does not exist.</returns>
        public IEdmEntitySet FindEntitySet(string name)
        {
            IEdmEntitySet entitySet;
            if (this.entitySets.TryGetValue(name, out entitySet))
            {
                return entitySet;
            }

            return null;
        }

        /// <summary>
        /// Searches for an singleton with the given name in this entity container and returns null if no such set exists.
        /// </summary>
        /// <param name="name">The name of the element being found.</param>
        /// <returns>The requested element, or null if the element does not exist.</returns>
        public IEdmSingleton FindSingleton(string name)
        {
            IEdmSingleton singleton;
            if (this.singletons.TryGetValue(name, out singleton))
            {
                return singleton;
            }

            return null;
        }

        /// <summary>
        /// Searches for operation imports with the given name in this entity container and returns null if no such operation import exists.
        /// </summary>
        /// <param name="operationName">The name of the operation import being found.</param>
        /// <returns>A group of the requested operation imports, or null if no such operation import exists.</returns>
        public IEnumerable<IEdmOperationImport> FindOperationImports(string operationName)
        {
            List<IEdmOperationImport> operationImport;
            if (this.operationImports.TryGetValue(operationName, out operationImport))
            {
                return operationImport;
            }

            return new IEdmOperationImport[0];
        }

        public string Namespace
        {
            get { return this.namespaceName; }
        }

        public string Name
        {
            get { return this.containerName; }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.EntityContainer; }
        }

        #endregion IEdmEntityContainer Members

        #region IEdmModel Members

        /// <summary>
        /// Gets the only one entity container of the model.
        /// </summary>
        public IEdmEntityContainer EntityContainer
        {
            get { return this; }
        }

        public IEnumerable<IEdmVocabularyAnnotation> VocabularyAnnotations
        {
            get { return Enumerable.Empty<IEdmVocabularyAnnotation>(); }
        }

        public IEnumerable<IEdmModel> ReferencedModels
        {
            get { return new[] { EdmCoreModel.Instance }; }
        }

        public IEnumerable<IEdmDirectValueAnnotation> AttachedAnnotations
        {
            get { return Enumerable.Empty<IEdmDirectValueAnnotation>(); }
        }

        public IEdmDirectValueAnnotationsManager DirectValueAnnotationsManager
        {
            get { return this.directValueAnnotationsManager; }
        }

        public IEnumerable<IEdmSchemaElement> SchemaElements
        {
            get
            {
                // NOTE: we only support entity types, complex types, operations and primitive types
                //       (no association types)
                return this.entityTypes.Values.Cast<IEdmSchemaElement>()
                    .Concat(this.complexTypes.Values.Cast<IEdmSchemaElement>())
                    .Concat(this.operations.SelectMany(s => s.Value).Cast<IEdmSchemaElement>())
                    .Concat(new[] { this }).ToList();
            }
        }

        public IEnumerable<string> DeclaredNamespaces
        {
            get { return new string[] { this.namespaceName }; }
        }

        /// <summary>
        /// Searches for a schema type with the given name in this model and returns null if no such schema element exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the schema element being found.</param>
        /// <returns>The requested schema element, or null if no such schema element exists.</returns>
        public IEdmSchemaType FindDeclaredType(string qualifiedName)
        {
            // NOTE: we only support entity types, complex types and primitive types
            //       (no association types)
            IEdmSchemaType coreType = EdmCoreModel.Instance.FindDeclaredType(qualifiedName);
            if (coreType != null)
            {
                return null;
            }

            IEdmEntityType entityType;
            if (this.entityTypes.TryGetValue(qualifiedName, out entityType))
            {
                return entityType;
            }

            IEdmComplexType complexType;
            if (this.complexTypes.TryGetValue(qualifiedName, out complexType))
            {
                return complexType;
            }

            return null;
        }

        /// <summary>
        /// Searches for functions with the given name in this model and returns null if no such function exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the function being found.</param>
        /// <returns>A set functions sharing the specified qualified name, or an empty enumerable if no such function exists.</returns>
        public IEnumerable<IEdmOperation> FindDeclaredOperations(string qualifiedName)
        {
            if (this.operations.ContainsKey(qualifiedName))
            {
                return this.operations[qualifiedName];
            }

            return Enumerable.Empty<IEdmOperation>();
        }


        /// <summary>
        /// Finds all operations with the given name which are bindable to an instance of the giving binding type or a more derived type.
        /// </summary>
        /// <param name="bindingType">The binding entity type.</param>
        /// <param name="operationName">The name of the operations to find. May be qualified with an entity container name.</param>
        /// <returns>The operations that match the search criteria.</returns>
        public IEnumerable<IEdmOperation> FindDeclaredBoundOperations(IEdmType bindingType)
        {
            foreach (var operationList in this.operations.Values)
            {
                foreach (var operation in operationList.Where(o => o.IsBound && o.Parameters.Any() && o.Parameters.First().Type.Definition.IsOrInheritsFrom(bindingType)))
                {
                    yield return operation;
                }
            }
        }

        /// <summary>
        /// Searches for bound operations based on the qualified name and binding type, returns an empty enumerable if no operation exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the operation.</param>
        /// <param name="bindingType">Type of the binding.</param>
        /// <returns>
        /// A set of operations that share the qualified name and binding type or empty enumerable if no such operation exists.
        /// </returns>
        public IEnumerable<IEdmOperation> FindDeclaredBoundOperations(string qualifiedName, IEdmType bindingType)
        {
            foreach (IEdmOperation operation in this.FindDeclaredOperations(qualifiedName))
            {
                if (operation.IsBound && operation.Parameters.Any() && operation.HasEquivalentBindingType(bindingType))
                {
                    yield return operation;
                }
            }
        }

        /// <summary>
        /// Searches for a term with the given name in this model and returns null if no such term exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the term being found.</param>
        /// <returns>The requested term, or null if no such term exists.</returns>
        public IEdmTerm FindDeclaredTerm(string qualifiedName)
        {
            return null;
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
        /// Finds a list of types that derive directly from the supplied type.
        /// </summary>
        /// <param name="baseType">The base type that derived types are being searched for.</param>
        /// <returns>A list of types from this model that derive directly from the given type.</returns>
        public IEnumerable<IEdmStructuredType> FindDirectlyDerivedTypes(IEdmStructuredType baseType)
        {
            List<IEdmStructuredType> types;
            if (this.derivedTypeMappings.TryGetValue(baseType, out types))
            {
                return types;
            }

            return Enumerable.Empty<IEdmStructuredType>();
        }

        #endregion IEdmModel Members

        /// <summary>
        /// Adds a key fragment to the key of the specified entity type.
        /// </summary>
        /// <param name="entityType">The entity type to add the key fragment to.</param>
        /// <param name="keyProperty">The key property to add.</param>
        private static void AddKeyFragment(EdmEntityType entityType, IEdmStructuralProperty keyProperty)
        {
            Debug.Assert(entityType != null, "entityType != null");
            Debug.Assert(keyProperty != null, "keyProperty != null");
            entityType.AddKeys(keyProperty);
        }

        private void AddOperationImport(string name, IEdmOperationImport function)
        {
            List<IEdmOperationImport> operationImport;
            if (this.operationImports.TryGetValue(name, out operationImport))
            {
                operationImport.Add(function);
            }
            else
            {
                this.operationImports.Add(name, new List<IEdmOperationImport>() { function });
            }
        }

        private void AddOperation(IEdmOperation operation)
        {
            List<IEdmOperation> operationsFound;
            if (this.operations.TryGetValue(operation.FullName(), out operationsFound))
            {
                operationsFound.Add(operation);
            }
            else
            {
                this.operations.Add(operation.FullName(), new List<IEdmOperation>() { operation });
            }
        }
    }
}
