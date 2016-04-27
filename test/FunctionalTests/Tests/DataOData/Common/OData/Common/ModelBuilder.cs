//---------------------------------------------------------------------
// <copyright file="ModelBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System.Diagnostics;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.EntityModel;
    using Microsoft.Test.Taupo.OData.Atom;
    using contractsOData = Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// A helper class to dynamically build models.
    /// </summary>
    public static class ModelBuilder
    {
        /// <summary>Default namespace for the test model.</summary>
        private const string ModelNamespace = "TestModel";

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
        public static EntityContainer EntityContainer(this EntityModelSchema model, string containerName, string namespaceName = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(containerName, "containerName");

            EntityContainer entityContainer = new EntityContainer(namespaceName ?? ModelNamespace, containerName);
            model.Add(entityContainer);
            return entityContainer;
        }

        /// <summary>
        /// Creates a new entity type with the specified name.
        /// </summary>
        /// <param name="model">The <see cref="EntityModelSchema"/> to create the entity type in.</param>
        /// <param name="localName">The local name (without namespace) for the entity type to create.</param>
        /// <param name="namespaceName">The (optional) namespace name for the type to create.</param>
        /// <returns>The newly created entity type instance.</returns>
        public static EntityType EntityType(this EntityModelSchema model, string localName, string namespaceName = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(localName, "localName");

            namespaceName = namespaceName ?? ModelNamespace;
            EntityType entityType = new EntityType(namespaceName, localName)
                {
                    Annotations = { new EntityModelSchemaAnnotation(model) }
                };
            model.Add(entityType);
            return entityType;
        }

        /// <summary>
        /// Adds an annotation to an <see cref="EntityModelSchema"/> that indicates the minimum version of the OData protocol
        /// that is required for the model to be used.
        /// </summary>
        /// <param name="model">The <see cref="EntityModelSchema"/> to annotate.</param>
        /// <param name="minimumVersion">The minimally required version for the <paramref name="model"/> to make sense.</param>
        /// <returns>The <paramref name="model"/> (for composability reasons).</returns>
        public static EntityModelSchema MinimumVersion(this EntityModelSchema model, ODataVersion minimumVersion)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            model.Annotations.Add(new MinimumRequiredVersionAnnotation(minimumVersion));
            return model;
        }

        /// <summary>
        /// Creates an <see cref="AssociationType"/> between the <paramref name="from"/> and <paramref name="to"/>
        /// entity types. It uses computed names for the association and its ends.
        /// </summary>
        /// <param name="from">The entity type the association starts on.</param>
        /// <param name="to">The entity type the association goes to.</param>
        /// <param name="isSingletonRelationship">true if the navigation property is of singleton cardinality; false for a cardinality many. Default is false.</param>
        /// <returns>A new <see cref="AssociationType"/> between the <paramref name="from"/> and <paramref name="to"/> entity types.</returns>
        public static AssociationType AssociationType(this EntityModelSchema model, EntityType from, EntityType to, bool isSingletonRelationship)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.CheckArgumentNotNull(from, "from");
            ExceptionUtilities.CheckArgumentNotNull(to, "to");

            string associationTypeName = from.Name + "_" + to.Name;

            int index = 0;
            while (model.Associations.Where(a => a.Name == associationTypeName).Count() > 0)
            {
                index++;
                associationTypeName = from.Name + "_" + to.Name + index;
            }

            // create the association type
            AssociationType associationType = new AssociationType(associationTypeName)
            {
                new AssociationEnd("From" + from.Name, from, EndMultiplicity.ZeroOne),
                new AssociationEnd("To" + to.Name, to, isSingletonRelationship ? EndMultiplicity.One : EndMultiplicity.Many),
            };

            model.Add(associationType);
            return associationType;
        }

        /// <summary>
        /// Creates a new function import with the specified name.
        /// </summary>
        /// <param name="container">The <see cref="EntityContainer"/> to create the function import in.</param>
        /// <param name="localName">The name for the function import to create.</param>
        /// <returns>The newly created function import instance.</returns>
        public static FunctionImport FunctionImport(this EntityContainer container, string localName)
        {
            ExceptionUtilities.CheckArgumentNotNull(container, "container");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(localName, "localName");

            FunctionImport functionImport = new FunctionImport(localName);
            container.Add(functionImport);
            return functionImport;
        }

        /// <summary>
        /// Creates a new parameter for the specified function import.
        /// </summary>
        /// <param name="functionImport">The <see cref="FunctionImport"/> to add the parameter to.</param>
        /// <param name="name">The local name of the parameter.</param>
        /// <param name="dataType">The type of the function parameter.</param>
        /// <param name="mode">The paramter mode.</param>
        /// <returns>The <paramref name="functionImport"/> (for composability).</returns>
        public static FunctionImport Parameter(this FunctionImport functionImport, string name, DataType dataType, FunctionParameterMode mode = FunctionParameterMode.In)
        {
            ExceptionUtilities.CheckArgumentNotNull(functionImport, "functionImport");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(name, "localName");

            FunctionParameter parameter = new FunctionParameter(name, dataType, mode);
            functionImport.Parameters.Add(parameter);
            return functionImport;
        }

        /// <summary>
        /// Creates a new parameter for the specified function import.
        /// </summary>
        /// <param name="functionImport">The <see cref="FunctionImport"/> to add the parameter to.</param>
        /// <param name="name">The local name of the parameter.</param>
        /// <param name="returnType">The type of the function parameter.</param>
        /// <param name="mode">The paramter mode.</param>
        /// <returns>The <paramref name="functionImport"/> (for composability).</returns>
        public static FunctionImport ReturnType(this FunctionImport functionImport, DataType returnDataType, EntitySet entitySet = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(functionImport, "functionImport");
            ExceptionUtilities.CheckArgumentNotNull(returnDataType, "returnDataType");

            FunctionImportReturnType returnType = new FunctionImportReturnType(returnDataType, entitySet);
            functionImport.ReturnTypes.Add(returnType);
            return functionImport;
        }

        /// <summary>
        /// Sets the HttpMethod annotation of the <paramref name="functionImport"/> to <paramref name="value"/>
        /// </summary>
        /// <param name="functionImport">The <see cref="FunctionImport"/> to set the annotation on.</param>
        /// <param name="value">The value of the annotation to set</param>
        /// <returns></returns>
        public static FunctionImport SetHttpMethod(this FunctionImport functionImport, string value)
        {
            ExceptionUtilities.CheckArgumentNotNull(functionImport, "functionImport");
            functionImport.Annotations.Add(new AttributeAnnotation(new XAttribute(contractsOData.ODataConstants.DataServicesMetadataNamespace + EdmConstants.HttpMethodAttributeName, value)));
            return functionImport;
        }

        /// <summary>
        /// Sets the MIME type for the given annotatable.
        /// </summary>
        /// <param name="annotatable">The annotatable to set the MIME type for.</param>
        /// <param name="mimeType">The MIME type to set.</param>
        /// <returns>The <paramref name="annotatable"/> with the MIME type set (for composability reasons).</returns>
        public static T MimeType<T>(this T annotatable, string mimeType) where T : AnnotatedItem
        {
            ExceptionUtilities.CheckArgumentNotNull(annotatable, "annotatable");

            annotatable.Annotations.Add(new AttributeAnnotation(new XAttribute(contractsOData.ODataConstants.DataServicesMetadataNamespace + EdmConstants.MimeTypeAttributeName, mimeType)));
            return annotatable;
        }

        /// <summary>
        /// Adds a (primitive, complex or collection) property to the <paramref name="entityType"/>. 
        /// Returns the modified entity type for composability.
        /// </summary>
        /// <param name="entityType">The <see cref="EntityType"/> to add the new property to.</param>
        /// <param name="propertyName">The name of the property to add.</param>
        /// <param name="type">The data type of the property.</param>
        /// <param name="isETagProperty">A flag indicating whether the property is an ETag property (default = false).</param>
        /// <returns>The <paramref name="entityType"/> instance after adding the property to it.</returns>
        public static EntityType KeyProperty(this EntityType entityType, string propertyName, DataType type, bool isETagProperty = false)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            MemberProperty property = new MemberProperty(propertyName, type);
            property.IsPrimaryKey = true;
            entityType.Add(property);
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
        /// <param name="isSingletonRelationship">true if the navigation property is of singleton cardinality; false for a cardinality many. Default is false.</param>
        /// <returns>The <paramref name="entityType"/> instance after adding the navigation property to it.</returns>
        public static EntityType NavigationProperty(this EntityType entityType, string propertyName, EntityType otherEndType, bool isSingletonRelationship = false)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");

            EntityModelSchema model = entityType.Model();

            // create the association type between the two entity types
            AssociationType associationType = model.AssociationType(entityType, otherEndType, isSingletonRelationship);

            // add the navigation property to the generated entity type
            entityType.Add(new NavigationProperty(propertyName, associationType, associationType.Ends[0], associationType.Ends[1]));

            return entityType;
        }

        /// <summary>
        /// Adds a stream reference property to the <paramref name="entityType"/>.
        /// Returns the modified entity type for composability.
        /// </summary>
        /// <param name="entityType">The <see cref="EntityType"/> to add the navigation property to.</param>
        /// <param name="propertyName">The name of the property to add.</param>
        /// <returns>The <paramref name="entityType"/> instance after adding the stream reference property to it.</returns>
        public static EntityType StreamProperty(this EntityType entityType, string propertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");

            // add the named stream property to the generated entity type
            entityType.Add(new MemberProperty(propertyName, EdmDataTypes.Stream));
            return entityType;
        }

        /// <summary>
        /// Sets a base type for the <paramref name="entityType"/>.
        /// </summary>
        /// <param name="entityType">The entity type to set the base type for.</param>
        /// <param name="baseType">The base type to set.</param>
        /// <returns>The <paramref name="entityType"/> instance after setting its base type.</returns>
        public static EntityType WithBaseType(this EntityType entityType, EntityType baseType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");

            entityType.BaseType = baseType;
            return entityType;
        }

        /// <summary>
        /// Marks the <paramref name="entityType"/> as open.
        /// </summary>
        /// <param name="entityType">The entity type to mark as open.</param>
        /// <returns>The <paramref name="entityType"/> once it is marked as open.</returns>
        public static EntityType OpenType(this EntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            entityType.IsOpen = true;
            return entityType;
        }

        /// <summary>
        /// Marks the <paramref name="entityType"/> as MLE, with default stream.
        /// </summary>
        /// <param name="entityType">The entity type to mark as MLE.</param>
        /// <returns>The <paramref name="entityType"/> once it is marked as MLE.</returns>
        public static EntityType DefaultStream(this EntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            entityType.Add(new HasStreamAnnotation());
            return entityType;
        }

        /// <summary>
        /// Finds property on structural type by its name.
        /// </summary>
        /// <param name="structuralType">The structural type to search for the property.</param>
        /// <param name="propertyName">The name of the property to look for.</param>
        /// <returns>The member property found, or null if no such property exists.</returns>
        /// <remarks>The method will fail if there's more than one matching property.</remarks>
        public static MemberProperty GetProperty(this NamedStructuralType structuralType, string propertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(structuralType, "structuralType");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");

            return structuralType.Properties.SingleOrDefault(property => property.Name == propertyName);
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
        public static T Property<T>(this T structuralType, string propertyName, DataType type, bool isETagProperty = false)
            where T : NamedStructuralType
        {
            ExceptionUtilities.CheckArgumentNotNull(structuralType, "structuralType");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            MemberProperty property = new MemberProperty(propertyName, type);
            structuralType.Add(property);
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
        public static T Property<T>(this T structuralType, string propertyName, ComplexType type, bool isNullable = false)
            where T : NamedStructuralType
        {
            ExceptionUtilities.CheckArgumentNotNull(structuralType, "structuralType");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            return structuralType.Property(propertyName, DataTypes.ComplexType.WithName(type.Name).Nullable(isNullable));
        }

        /// <summary>
        /// Creates a new complex type with the specified name.
        /// </summary>
        /// <param name="model">The <see cref="EntityModelSchema"/> to create the complex type in.</param>
        /// <param name="localName">The local name (without namespace) for the complex type to create.</param>
        /// <param name="namespaceName">The (optional) namespace name for the type to create.</param>
        /// <returns>The newly created complex type instance.</returns>
        public static ComplexType ComplexType(this EntityModelSchema model, string localName, string namespaceName = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");

            namespaceName = namespaceName ?? ModelNamespace;
            ComplexType complexType = new ComplexType(namespaceName, localName)
            {
                Annotations = { new EntityModelSchemaAnnotation(model) }
            };
            model.Add(complexType);
            return complexType;
        }

        /// <summary>
        /// Adds an entity set to the <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The model to add the entity set to.</param>
        /// <param name="name">The name of the entity set to add.</param>
        /// <param name="entityType">The entity type for the entity set.</param>
        /// <returns>The newly created entity set.</returns>
        public static EntitySet EntitySet(this EntityModelSchema model, string name, EntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(name, "name");
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");

            EntitySet entitySet = new EntitySet(name, entityType)
            {
                Annotations = { new EntityModelSchemaAnnotation(model) }
            };

            var container = model.EntityContainers.Single();
            container.Add(entitySet);

            return entitySet;
        }

        /// <summary>
        /// Runs a set of fixup operations on the constructed model:
        ///   * resolves all the references in the model
        ///   * applies the default namespace
        ///   * ensures the existence and consistency of the default container
        /// </summary>
        /// <param name="model">The Model to run the fixup operations on.</param>
        /// <returns>The <paramref name="model"/> after having executed all fixup operations.</returns>
        public static EntityModelSchema Fixup(this EntityModelSchema model)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");

            // Apply default fixups
            new ResolveReferencesFixup().Fixup(model);
            new ApplyDefaultNamespaceFixup(ModelNamespace).Fixup(model);
            new AddDefaultContainerFixup().Fixup(model);

            return model;
        }

        /// <summary>
        /// Finds a complex type by its name.
        /// </summary>
        /// <param name="name">The name or full name to look for.</param>
        /// <returns>The entity type found or null if none exists.</returns>
        public static ComplexType GetComplexType(this EntityModelSchema model, string name)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(name, "name");

            ComplexType complexType = model.ComplexTypes.SingleOrDefault(c => c.FullName == name);
            if (complexType == null)
            {
                complexType = model.ComplexTypes.SingleOrDefault(c => c.Name == name);
            }

            return complexType;
        }

        /// <summary>
        /// Finds an entity type by its name.
        /// </summary>
        /// <param name="name">The name or full name to look for.</param>
        /// <returns>The entity type found or null if none exists.</returns>
        public static EntityType GetEntityType(this EntityModelSchema model, string name)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(name, "name");

            EntityType entityType = model.EntityTypes.SingleOrDefault(e => e.FullName == name);
            if (entityType == null)
            {
                entityType = model.EntityTypes.SingleOrDefault(e => e.Name == name);
            }

            return entityType;
        }

        /// <summary>
        /// Finds an entity set by its name.
        /// </summary>
        /// <param name="name">The name to look for.</param>
        /// <returns>The entity set found or null if none exists.</returns>
        public static EntitySet GetEntitySet(this EntityModelSchema model, string name)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(name, "name");

            EntityContainer container = model.EntityContainers.Single();
            return container.EntitySets.SingleOrDefault(es => es.Name == name);
        }

        /// <summary>
        /// Finds a function import by its name.
        /// </summary>
        /// <param name="name">The name to look for.</param>
        /// <returns>The function import found or null if none exists.</returns>
        public static FunctionImport GetFunctionImport(this EntityModelSchema model, string name)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(name, "name");

            EntityContainer container = model.EntityContainers.Single();
            return container.FunctionImports.SingleOrDefault(fi => fi.Name == name);
        }
       
        /// <summary>
        /// Gets the <see cref="EntityType"/> of the specified entry or feed payload element.
        /// </summary>
        /// <param name="payloadElement">The payload element to get the entity type for.</param>
        /// <param name="model">The model to find the entity type in.</param>
        /// <returns>The <see cref="EntityType"/> of the <paramref name="payloadElement"/>.</returns>
        public static IEdmEntityType GetPayloadElementEntityType(ODataPayloadElement payloadElement, IEdmModel model)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckArgumentNotNull(model, "model");

            ODataPayloadKind payloadKind = payloadElement.GetPayloadKindFromPayloadElement();
            ExceptionUtilities.Assert(
                payloadKind == ODataPayloadKind.Resource || payloadKind == ODataPayloadKind.ResourceSet,
                "Can only determine entity type for entry or feed payloads.");

            EntityModelTypeAnnotation typeAnnotation = payloadElement.GetAnnotation<EntityModelTypeAnnotation>();
            if (typeAnnotation != null)
            {
                var entityDataType = typeAnnotation.EdmModelType;
                    Debug.Assert(entityDataType != null, "entityDataType != null");
                return model.EntityTypes().Single(et => et.FullName() == entityDataType.FullName());
            }

            string entityTypeName;
            if (payloadKind == ODataPayloadKind.Resource)
            {
                EntityInstance entity = payloadElement as EntityInstance;
                Debug.Assert(entity != null, "entity != null");
                entityTypeName = entity.FullTypeName;
                return model.EntityTypes().Single(et => et.TestFullName() == entityTypeName);
            }
            else
            {
                //if feed has entries figure out type otherwise use first entity type
                EntitySetInstance feed = payloadElement as EntitySetInstance;
                Debug.Assert(feed != null, "feed != null");
                if (feed.Count > 0)
                {
                    return model.EntityTypes().Single(et => et.TestFullName() == feed.First().FullTypeName);
                }
                else
                {
                    return model.EntityTypes().First();
                }

            }
        }

        /// <summary>
        /// Gets the data type of a property value specified in the property instance payload element.
        /// </summary>
        /// <param name="propertyInstance">The property instance payload element to inspect.</param>
        /// <returns>The data type of the property value (can be used to define the metadata for this property).</returns>
        public static IEdmTypeReference GetPayloadEdmElementPropertyValueType(PropertyInstance propertyInstance)
        {
            ExceptionUtilities.CheckArgumentNotNull(propertyInstance, "propertyInstance");

            IEdmTypeReference result = GetEdmTypeFromEntityModelTypeAnnotation(propertyInstance);

            if (result == null)
            {
                switch (propertyInstance.ElementType)
                {
                    case ODataPayloadElementType.NullPropertyInstance:
                        NullPropertyInstance nullPropertyInstance = (NullPropertyInstance)propertyInstance;
                        if (nullPropertyInstance.FullTypeName != null)
                        {
                            result = GetPrimitiveEdmType(nullPropertyInstance.FullTypeName);
                            if (result == null)
                            {
                                result = CreateComplexTypeReference(nullPropertyInstance.FullTypeName);
                            }
                        }

                        break;
                    case ODataPayloadElementType.PrimitiveProperty:
                        result = GetEdmTypeFromEntityModelTypeAnnotation(((PrimitiveProperty)propertyInstance).Value);
                        if (result == null)
                        {
                            result = GetPrimitiveEdmType(((PrimitiveProperty)propertyInstance).Value.FullTypeName);
                        }

                        break;
                    case ODataPayloadElementType.ComplexProperty:
                        result = GetEdmTypeFromEntityModelTypeAnnotation(((ComplexProperty)propertyInstance).Value);
                        if (result == null)
                        {
                            result = CreateComplexTypeReference(((ComplexProperty)propertyInstance).Value.FullTypeName);
                        }

                        break;

                    case ODataPayloadElementType.NamedStreamInstance:
                        result = EdmCoreModel.Instance.GetStream(isNullable: false);

                        break;
                    case ODataPayloadElementType.PrimitiveMultiValueProperty:
                        PrimitiveMultiValue primitiveMultiValue = ((PrimitiveMultiValueProperty)propertyInstance).Value;
                        result = GetEdmTypeFromEntityModelTypeAnnotation(primitiveMultiValue);
                        if (result == null && primitiveMultiValue.FullTypeName != null)
                        {
                            string itemTypeName = EntityModelUtils.GetCollectionItemTypeName(primitiveMultiValue.FullTypeName);
                            if (itemTypeName != null)
                            {
                                result = EdmCoreModel.GetCollection(GetPrimitiveEdmType(itemTypeName));
                            }
                        }

                        break;
                    case ODataPayloadElementType.ComplexMultiValueProperty:
                        ComplexMultiValue complexMultiValue = ((ComplexMultiValueProperty)propertyInstance).Value;
                        result = GetEdmTypeFromEntityModelTypeAnnotation(complexMultiValue);
                        if (result == null && complexMultiValue.FullTypeName != null)
                        {
                            string itemTypeName = EntityModelUtils.GetCollectionItemTypeName(complexMultiValue.FullTypeName);
                            if (itemTypeName != null)
                            {
                                return EdmCoreModel.GetCollection(CreateComplexTypeReference(itemTypeName));
                            }
                        }

                        break;
                    case ODataPayloadElementType.PrimitiveCollection:
                    case ODataPayloadElementType.ComplexInstanceCollection:
                        ExceptionUtilities.Assert(false, "Primitive and complex collections cannot be used in properties but only at the top-level.");
                        return null;

                    default:
                        ExceptionUtilities.Assert(false, "GetPayloadElementPropertyValueType doesn't support '{0}' yet.", propertyInstance.ElementType);
                        return null;
                }
            }

            // Use the expected type if there's any since it also specifies metadata
            if (result == null)
            {
                ExpectedTypeODataPayloadElementAnnotation expectedTypeAnnotation = propertyInstance.GetAnnotation<ExpectedTypeODataPayloadElementAnnotation>();
                if (expectedTypeAnnotation != null && expectedTypeAnnotation.ExpectedType != null)
                {
                    result = expectedTypeAnnotation.EdmExpectedType;
                }
            }

            return result;
        }

        private static EdmComplexTypeReference CreateComplexTypeReference(string fullTypeName)
        {
            return new EdmComplexTypeReference(new EdmComplexType(fullTypeName.Split('.')[0], fullTypeName.Split('.')[1]), true);
        }

        private static IEdmPrimitiveTypeReference GetPrimitiveEdmType(string typeName)
        {
            var primitiveType = EdmCoreModel.Instance.SchemaElements.OfType<IEdmPrimitiveType>().SingleOrDefault(type => type.FullName() == typeName);
            
            if (primitiveType == null)
                return null;

            if (primitiveType.PrimitiveKind == EdmPrimitiveTypeKind.String 
                || primitiveType.PrimitiveKind == EdmPrimitiveTypeKind.Binary)
            {
                return new EdmPrimitiveTypeReference(primitiveType, isNullable: true);
            }

            return new EdmPrimitiveTypeReference(primitiveType, isNullable: false);
        }

        /// <summary>
        /// Applies EPM mapping to the specified entity type on the model and returns it for composablity.
        /// </summary>
        /// <param name="model">Model to use</param>
        /// <param name="entityTypeName">Entity type name</param>
        /// <param name="sourcePath">Source path</param>
        /// <param name="targetSyndicationItem">TargetSyndicationItem to use</param>
        /// <param name="targetTextContentKind">TargetTextContentKind to use</param>
        /// <param name="keepInContent">true to keep the mapped value in content; otherwise false (defaults to true).</param>
        /// <returns>Enity Model Schema with the mapping applied</returns>
        public static EntityModelSchema EntityPropertyMapping(
            this EntityModelSchema model,
            string entityTypeName,
            string sourcePath,
            SyndicationItemProperty targetSyndicationItem = SyndicationItemProperty.AuthorName,
            SyndicationTextContentKind targetTextContentKind = SyndicationTextContentKind.Plaintext,
            bool keepInContent = true)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.CheckArgumentNotNull(entityTypeName, "entityTypeName");
            ExceptionUtilities.CheckArgumentNotNull(sourcePath, "sourcePath");
            var entityType = model.GetEntityType(entityTypeName);
            ExceptionUtilities.CheckObjectNotNull(entityType, "There are no entity types matching the name:{0} in the model", entityTypeName);
            entityType.EntityPropertyMapping(sourcePath, targetSyndicationItem, targetTextContentKind, keepInContent);

            return model;
        }

        /// <summary>
        /// Creates a Syndication EPM annotation on the <paramref name="annotatable"/>.
        /// </summary>
        /// <typeparam name="T">The type of the annotatable being passed in.</typeparam>
        /// <param name="annotatable">The annotatable to add the EPM annotation to.</param>
        /// <param name="sourcePath">The source path to be used in the mapping.</param>
        /// <param name="targetSyndicationItem">The syndication item to be used in the mapping (defaults to AuthorName).</param>
        /// <param name="targetTextContentKind">The target content kind (defaults to plain text).</param>
        /// <param name="keepInContent">true to keep the mapped value in content; otherwise false (defaults to true).</param>
        /// <returns>The <paramref name="annotatable"/> being passed in for composability.</returns>
        public static T EntityPropertyMapping<T>(
            this T annotatable,
            string sourcePath,
            SyndicationItemProperty targetSyndicationItem = SyndicationItemProperty.AuthorName,
            SyndicationTextContentKind targetTextContentKind = SyndicationTextContentKind.Plaintext,
            bool keepInContent = true) where T : AnnotatedItem
        {
            ExceptionUtilities.CheckArgumentNotNull(annotatable, "annotatable");
            ExceptionUtilities.CheckArgumentNotNull(sourcePath, "sourcePath");

            ExceptionUtilities.Assert(annotatable is EntityType, "Entity Property Mappings should only be applied to entity types");

            annotatable.Add(new PropertyMappingAnnotation(sourcePath, targetSyndicationItem, targetTextContentKind, keepInContent));
            return annotatable;
        }

        /// <summary>
        /// Creates a custom EPM annotation on the <paramref name="annotatable"/>.
        /// </summary>
        /// <typeparam name="T">The type of the annotatable being passed in.</typeparam>
        /// <param name="annotatable">The annotatable to add the EPM annotation to.</param>
        /// <param name="sourcePath">The source path to be used in the mapping.</param>
        /// <param name="targetPath">The target path to be used in the mapping.</param>
        /// <param name="prefix">The namespace prefix to be used in the mapping.</param>
        /// <param name="ns">The namespace name to be used in the mapping.</param>
        /// <param name="keepInContent">true to keep the mapped value in content; otherwise false (defaults to true).</param>
        /// <returns>The <paramref name="annotatable"/> being passed in for composability.</returns>
        public static T EntityPropertyMapping<T>(
            this T annotatable,
            string sourcePath,
            string targetPath,
            string prefix,
            string ns,
            bool keepInContent = true) where T : AnnotatedItem
        {
            ExceptionUtilities.CheckArgumentNotNull(annotatable, "annotatable");
            ExceptionUtilities.CheckArgumentNotNull(sourcePath, "sourcePath");
            ExceptionUtilities.CheckArgumentNotNull(targetPath, "targetPath");

            ExceptionUtilities.Assert(annotatable is EntityType, "Entity Property Mappings should only be applied to entity types");

            annotatable.Add(new PropertyMappingAnnotation(sourcePath, targetPath, prefix, ns, keepInContent));
            return annotatable;
        }

        /// <summary>
        /// Adds an attribute annotation to the <paramref name="annotatable"/>.
        /// </summary>
        /// <typeparam name="T">The type of the annotatable being passed in.</typeparam>
        /// <param name="annotatable">The annotatable to add the attribute annotation to.</param>
        /// <param name="attributeName">The name of the attribute to add.</param>
        /// <param name="attributeValue">The value of the attribute to add.</param>
        /// <returns>The <paramref name="annotatable"/> being passed in for composability.</returns>
        public static T MetadataAttributeAnnotation<T>(
            this T annotatable,
            string attributeName,
            object attributeValue) where T : AnnotatedItem
        {
            ExceptionUtilities.CheckArgumentNotNull(annotatable, "annotatable");
            ExceptionUtilities.CheckArgumentNotNull(attributeName, "attributeName");

            annotatable.Add(new AttributeAnnotation(new XAttribute(TestAtomConstants.ODataMetadataXNamespace + attributeName, attributeValue)));
            return annotatable;
        }

        /// <summary>
        /// Helper extension method to extract the Model annotation from an entity type.
        /// </summary>
        /// <param name="entityType">The entity type to get the annotation from.</param>
        /// <returns>The model associated with the entity.</returns>
        private static EntityModelSchema Model(this EntityType entityType)
        {
            Debug.Assert(entityType != null, "entityType != null");

            EntityModelSchemaAnnotation annotation = (EntityModelSchemaAnnotation)
                entityType.Annotations.Where(a => a.GetType() == typeof(EntityModelSchemaAnnotation)).FirstOrDefault();
            Debug.Assert(annotation != null, "Expected to find entity model on entity type.");
            return annotation.Model;
        }
        

        /// <summary>
        /// Returns the type from the entity model type annotation for this payload element.
        /// </summary>
        /// <param name="payloadElement">The payload element to return the type for.</param>
        /// <returns>The type from the annotation or null if none was present.</returns>
        private static IEdmTypeReference GetEdmTypeFromEntityModelTypeAnnotation(ODataPayloadElement payloadElement)
        {
            Debug.Assert(payloadElement != null, "payloadElement != null");

            EntityModelTypeAnnotation typeAnnotation = payloadElement.GetAnnotation<EntityModelTypeAnnotation>();
            return typeAnnotation == null ? null : typeAnnotation.EdmModelType;
        }
    }
}
