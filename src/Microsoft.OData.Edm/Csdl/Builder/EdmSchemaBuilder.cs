//---------------------------------------------------------------------
// <copyright file="EdmSchemaBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Builder
{
    /// <summary>
    /// Provides CSDL-JSON parsing services for EDM models.
    /// Complex, Entity, Enum, TypeDefinition -> IEdmSchemaType
    /// </summary>
    internal class EdmSchemaBuilder
    {
        private IDictionary<CsdlModel, EdmModel> _modelMapping;

        private IDictionary<CsdlSchema, CsdlModel> _schemaMapping = new Dictionary<CsdlSchema, CsdlModel>();

        private IDictionary<CsdlStructuredType, CsdlSchema> _schemaTypesMapping = new Dictionary<CsdlStructuredType, CsdlSchema>();

        private IDictionary<string, CsdlStructuredType> _namespaceNameToStructuralTypeMapping = new Dictionary<string, CsdlStructuredType>();

        // save the built types
        private readonly IDictionary<string, EdmStructuredType> _structuredTypes = new Dictionary<string, EdmStructuredType>();
        private readonly IDictionary<string, EdmEnumType> _enumTypes = new Dictionary<string, EdmEnumType>();
        private readonly IDictionary<string, EdmTypeDefinition> _typeDefinitions = new Dictionary<string, EdmTypeDefinition>();
        private readonly IDictionary<string, EdmTerm> _terms = new Dictionary<string, EdmTerm>();
        private readonly IList<EdmAction> _actions = new List<EdmAction>();
        private readonly IList<EdmFunction> _functions = new List<EdmFunction>();
        private readonly IDictionary<string, EdmNavigationSource> _navigationSources = new Dictionary<string, EdmNavigationSource>(); // key is the "entityContainerFullName.EntitySetname"

        internal EdmSchemaBuilder(IDictionary<CsdlModel, EdmModel> modelMapping)
        {
            Debug.Assert(modelMapping != null);

            _modelMapping = modelMapping;

            foreach (var csdlModel in modelMapping.Keys)
            {
                foreach (var csdlSchema in csdlModel.Schemata)
                {
                    _schemaMapping[csdlSchema] = csdlModel;

                    foreach (var stype in csdlSchema.StructuredTypes)
                    {
                        _schemaTypesMapping[stype] = csdlSchema;

                        CsdlNamedStructuredType namedType = (CsdlNamedStructuredType)stype;
                        _namespaceNameToStructuralTypeMapping[csdlSchema.Namespace + "." + namedType.Name] = stype;
                    }
                }
            }
        }

        /// <summary>
        /// Build the schema items all together
        /// 1) Structured type start ==> Build the entity type, complex type, enum type, type definition
        /// 2) Term type start ( Term type relys on the types, and it's the based on other annotations.
        /// 3) Term type end ( Term can also have annotations, so finish the term after all terms build)
        /// 4) Structured type end ==> build properties, annotations for properties
        /// 5) Build the operations ( Operations is used in entity container's operation import)
        /// 6) Build the entity set container start ( should only build entity set, singleton, operation import)
        /// 7) Build the navigation property bindings
        /// 8) Outofline annotations.
        /// </summary>
        public void BuildSchemaItems()
        {
            // Create structured type, Type Definition, enum type header
            // Because all schema types are used everywhere.
            BuildSchemaTypeStart();

            // Now it's ready to build the term, term should build after the types' built
            // But, term should build before all other elements.
            BuildTermStart();

            // Now, it's time to finish the Term build
            BuildTermEnd();

            // Now, it's time to finish "Entity, complex, enum, typedefintion" body
            BuildSchemaTypeBodies();

            // Now, it's time to finish all operations
            BuildSchemaOperation();

            // Now, it's time to finish entity container, it should be after Operation built
            BuildSchemaEntityContainerHeader();

            // Now, it's time to finish entity container body
            BuildSchemaEntityContainerBody();

            // Now, it's time to finish outofLine annotations
            BuildSchemaOutOfLineAnnotations();
        }

        public static void BuildSchemaOutOfLineAnnotations()
        {

        }


        private void BuildSchemaTypeStart()
        {
            foreach (var modelItem in _modelMapping)
            {
                foreach (var csdlSchema in modelItem.Key.Schemata)
                {
                    foreach (var csdlStructuredType in csdlSchema.StructuredTypes)
                    {
                        BuildSchemaStructuredTypeHeader(csdlStructuredType, csdlSchema, modelItem.Key, modelItem.Value);
                    }

                    foreach (var csdlEnum in csdlSchema.EnumTypes)
                    {
                        BuildEnumTypeHeader(csdlEnum, csdlSchema, modelItem.Key, modelItem.Value);
                    }

                    foreach (var csdlTypeDefinition in csdlSchema.TypeDefinitions)
                    {
                        BuildTypeDefinitionHeader(csdlTypeDefinition, csdlSchema, modelItem.Key, modelItem.Value);
                    }
                }
            }
        }

        private void BuildComplexTypeHeader(CsdlComplexType csdlComplex, CsdlSchema csdlSchema, CsdlModel csdlModel, EdmModel edmModel)
        {
            string fullNamespaceQuafiledName = csdlSchema.Namespace + "." + csdlComplex.Name;

            EdmStructuredType edmStructuredType = GetStructuredType(fullNamespaceQuafiledName);
            if (edmStructuredType != null)
            {
                // created before
                return;
            }

            IEdmComplexType baseComplexType = null;
            if (csdlComplex.BaseTypeName != null)
            {
                string baseTypeFullNamespaceQualifiedName = csdlModel.ReplaceAlias(csdlComplex.BaseTypeName);

                CsdlStructuredType baseItem = GetStructuredTypeItem(baseTypeFullNamespaceQualifiedName);

                CsdlSchema baseSchema = GetRelatedSchema(baseItem);
                CsdlModel baseModel = GetRelatedCsdlModel(baseSchema);
                EdmModel baseEdmModel = GetRelatedModel(baseModel);
                BuildSchemaStructuredTypeHeader(baseItem, baseSchema, baseModel, baseEdmModel);

                baseComplexType = GetStructuredType(baseTypeFullNamespaceQualifiedName) as IEdmComplexType;

                Contract.Assert(baseComplexType != null);
            }

            EdmComplexType complexType = new EdmComplexType(csdlSchema.Namespace, csdlComplex.Name,
                baseComplexType, csdlComplex.IsAbstract, csdlComplex.IsOpen);

            _structuredTypes[complexType.FullTypeName()] = complexType;

            edmModel.AddElement(complexType);
        }

        private void BuildEntityTypeHeader(CsdlEntityType csdlEntity, CsdlSchema csdlSchema, CsdlModel csdlModel, EdmModel edmModel)
        {
            string fullNamespaceQuafiledName = csdlSchema.Namespace + "." + csdlEntity.Name;

            EdmStructuredType edmStructuredType = GetStructuredType(fullNamespaceQuafiledName);
            if (edmStructuredType != null)
            {
                // created before
                return;
            }

            IEdmEntityType baseEntityType = null;
            if (csdlEntity.BaseTypeName != null)
            {
                string baseTypeFullNamespaceQualifiedName = csdlModel.ReplaceAlias(csdlEntity.BaseTypeName);

                CsdlStructuredType baseItem = GetStructuredTypeItem(baseTypeFullNamespaceQualifiedName);
                CsdlSchema baseSchema = GetRelatedSchema(baseItem);
                CsdlModel baseModel = GetRelatedCsdlModel(baseSchema);
                EdmModel baseEdmModel = GetRelatedModel(baseModel);

                BuildSchemaStructuredTypeHeader(baseItem, baseSchema, baseModel, baseEdmModel);

                baseEntityType = GetStructuredType(baseTypeFullNamespaceQualifiedName) as IEdmEntityType;

                Contract.Assert(baseEntityType != null);
            }

            EdmEntityType entityType = new EdmEntityType(csdlSchema.Namespace, csdlEntity.Name, baseEntityType,
                csdlEntity.IsAbstract, csdlEntity.IsOpen, csdlEntity.HasStream);

            _structuredTypes[entityType.FullTypeName()] = entityType;

            edmModel.AddElement(entityType);
        }

        private void BuildSchemaStructuredTypeHeader(CsdlStructuredType structuredType, CsdlSchema csdlSchema, CsdlModel csdlModel, EdmModel edmModel)
        {
            CsdlEntityType csdlEntity = structuredType as CsdlEntityType;
            if (csdlEntity != null)
            {
                BuildEntityTypeHeader(csdlEntity, csdlSchema, csdlModel, edmModel);
                return;
            }

            CsdlComplexType csdlComplex = structuredType as CsdlComplexType;
            if (csdlComplex != null)
            {
                BuildComplexTypeHeader(csdlComplex, csdlSchema, csdlModel, edmModel);
                return;
            }
        }

        public void BuildSchemaTypeBodies()
        {
            // We should build the structural properties first, then build the navigation properties,
            // Because navigation property's reference contrains relies on the structural properties
            foreach (var modelItem in _modelMapping)
            {
                foreach (var csdlSchema in modelItem.Key.Schemata)
                {
                    foreach (var csdlStructuredType in csdlSchema.StructuredTypes)
                    {
                        BuildSchemaStructuralBody(csdlStructuredType, modelItem.Key, csdlSchema, modelItem.Value, true);
                    }

                    foreach (var csdlEnum in csdlSchema.EnumTypes)
                    {
                        BuildSchemaEnumBody(csdlEnum, modelItem.Key, csdlSchema, modelItem.Value);
                    }

                    foreach (var csdlTypeDefinition in csdlSchema.TypeDefinitions)
                    {
                        BuildSchemaTypeDefinitionBody(csdlTypeDefinition, modelItem.Key, csdlSchema, modelItem.Value);
                    }
                }
            }

            // Let's finish the navigation properties building
            foreach (var modelItem in _modelMapping)
            {
                foreach (var csdlSchema in modelItem.Key.Schemata)
                {
                    foreach (var csdlStructuredType in csdlSchema.StructuredTypes)
                    {
                        BuildSchemaStructuralBody(csdlStructuredType, modelItem.Key, csdlSchema, modelItem.Value, false);
                    }
                }
            }
        }

        private void BuildSchemaStructuralBody(CsdlStructuredType csdlStructured, CsdlModel csdlModel, CsdlSchema csdlSchema, EdmModel edmModel, bool structuralProperty)
        {
            CsdlComplexType csdlComplexType = csdlStructured as CsdlComplexType;
            if (csdlComplexType != null)
            {
                BuildSchemaComplexBody(csdlComplexType, csdlModel, csdlSchema, edmModel, structuralProperty);
            }
            else
            {
                BuildSchemaEntityBody(csdlStructured as CsdlEntityType, csdlModel, csdlSchema, edmModel, structuralProperty);
            }
        }

        private void BuildSchemaComplexBody(CsdlComplexType csdlComplex, CsdlModel csdlModel, CsdlSchema csdlSchema, EdmModel edmModel, bool structuralProperty)
        {
            EdmComplexType edmComplexType = _structuredTypes[csdlSchema.Namespace + "." + csdlComplex.Name] as EdmComplexType;

            if (structuralProperty)
            {
                BuildStructuralProperties(edmComplexType, csdlComplex.StructuralProperties, csdlModel, csdlSchema, edmModel);
            }
            else
            {
                BuildNavigationProperties(edmComplexType, csdlComplex.NavigationProperties, csdlModel, csdlSchema, edmModel);

                BuildAnnotations(edmComplexType, csdlComplex.VocabularyAnnotations, csdlSchema, csdlModel, edmModel);
            }
        }

        private void BuildSchemaEntityBody(CsdlEntityType csdlEntity, CsdlModel csdlModel, CsdlSchema csdlSchema, EdmModel edmModel, bool structuralProperty)
        {
            EdmEntityType edmEntityType = _structuredTypes[csdlSchema.Namespace + "." + csdlEntity.Name] as EdmEntityType;

            if (structuralProperty)
            {
                BuildStructuralProperties(edmEntityType, csdlEntity.StructuralProperties, csdlModel, csdlSchema, edmModel);
            }
            else
            {
                if (csdlEntity.Key != null && csdlEntity.Key.Properties.Any())
                {
                    // Add the keys
                    foreach (var keyProperty in csdlEntity.Key.Properties)
                    {
                        var  keyStructuralProperty = edmEntityType.DeclaredStructuralProperties().First(c => c.Name == keyProperty.PropertyName);
                        edmEntityType.AddKeys(keyStructuralProperty);
                    }
                }

                BuildNavigationProperties(edmEntityType, csdlEntity.NavigationProperties, csdlModel, csdlSchema, edmModel);
                BuildAnnotations(edmEntityType, csdlEntity.VocabularyAnnotations, csdlSchema, csdlModel, edmModel);
            }
        } 

        private void BuildStructuralProperties(EdmStructuredType structuredType, IEnumerable<CsdlProperty> structualProperties, CsdlModel csdlModel, CsdlSchema csdlSchema, EdmModel edmModel)
        {
            foreach (var csdlProperty in structualProperties)
            {
                IEdmTypeReference propertyType = BuildEdmTypeReference(csdlProperty.Type, csdlModel);

                var edmProperty = structuredType.AddStructuralProperty(csdlProperty.Name, propertyType, csdlProperty.DefaultValue);

                BuildAnnotations(edmProperty, csdlProperty.VocabularyAnnotations, csdlSchema, csdlModel, edmModel);
            }
        }

        private static bool ElementType(string typeName, out string elementType)
        {
            string[] typeInformation = typeName.Split(new char[] { '(', ')' });
            string name = typeInformation[0];
            if (name == "Collection")
            {
                elementType = typeInformation[1];
                return true;
            }

            elementType = typeName;
            return false;
        }

        private void BuildNavigationProperties(EdmStructuredType structuredType, IEnumerable<CsdlNavigationProperty> navigationProperties, CsdlModel csdlModel, CsdlSchema csdlSchema, EdmModel edmModel)
        {
            foreach (var csdlNavProperty in navigationProperties)
            {
                string elementTypeName;
                bool isCollection = ElementType(csdlNavProperty.Type, out elementTypeName);
                string fullName = csdlModel.ReplaceAlias(elementTypeName);

                EdmMultiplicity multiplicity;
                if (isCollection)
                {
                    multiplicity = EdmMultiplicity.Many;
                }
                else
                {
                    if (csdlNavProperty.Nullable == null || !csdlNavProperty.Nullable.Value)
                    {
                        multiplicity = EdmMultiplicity.One;
                    }
                    else
                    {
                        multiplicity = EdmMultiplicity.ZeroOrOne;
                    }
                }

                IEdmEntityType entityType = _structuredTypes[fullName] as IEdmEntityType;

                EdmNavigationPropertyInfo info = new EdmNavigationPropertyInfo
                {
                    Name = csdlNavProperty.Name,
                    ContainsTarget = csdlNavProperty.ContainsTarget,
                    TargetMultiplicity = multiplicity,
                    Target = entityType,
                    OnDelete = csdlNavProperty.OnDelete != null ? csdlNavProperty.OnDelete.Action : EdmOnDeleteAction.None
                };

                if (csdlNavProperty.ReferentialConstraints.Any())
                {
                    info.PrincipalProperties = GetDeclaringPropertyInfo(structuredType, csdlNavProperty.ReferentialConstraints.Select(c => c.PropertyName));
                    info.DependentProperties = GetDeclaringPropertyInfo(entityType, csdlNavProperty.ReferentialConstraints.Select(c => c.ReferencedPropertyName));
                }

                var edmNavProperty = structuredType.AddUnidirectionalNavigation(info);

                // ODL doesn't support annotation on : edmNavProperty.ReferentialConstraint, so skip it.

                BuildAnnotations(edmNavProperty, csdlNavProperty.VocabularyAnnotations, csdlSchema, csdlModel, edmModel);
            }
        }

        private static IList<IEdmStructuralProperty> GetDeclaringPropertyInfo(IEdmStructuredType type, IEnumerable<string> propertyNames)
        {
            IList<IEdmStructuralProperty> properties = new List<IEdmStructuralProperty>();

            foreach (var propertyName in propertyNames)
            {
                properties.Add(type.FindProperty(propertyName) as IEdmStructuralProperty);
            }

            return properties;
        }

        private void BuildSchemaEnumBody(CsdlEnumType csdlEnum, CsdlModel csdlModel, CsdlSchema csdlSchema, EdmModel edmModel)
        {
            string fullName = csdlSchema.Namespace + "." + csdlEnum.Name;
            EdmEnumType enumType = _enumTypes[fullName];

            foreach (var enumMember in csdlEnum.Members)
            {
                EdmEnumMemberValue edmEnumMemberValue = new EdmEnumMemberValue(enumMember.Value.Value); // json should always have a value.
                EdmEnumMember edmEnumMember = new EdmEnumMember(enumType, enumMember.Name, edmEnumMemberValue);
                enumType.AddMember(edmEnumMember);

                BuildAnnotations(edmEnumMember, enumMember.VocabularyAnnotations, csdlSchema, csdlModel, edmModel);
            }


            BuildAnnotations(enumType, csdlEnum.VocabularyAnnotations, csdlSchema, csdlModel, edmModel);
        }

        private void BuildSchemaTypeDefinitionBody(CsdlTypeDefinition csdlTypeDefinition, CsdlModel csdlModel, CsdlSchema csdlSchema, EdmModel edmModel)
        {
            string fullName = csdlSchema.Namespace + "." + csdlTypeDefinition.Name;
            IEdmTypeDefinition edmTypeDefinition = _typeDefinitions[fullName];

            BuildAnnotations(edmTypeDefinition, csdlTypeDefinition.VocabularyAnnotations, csdlSchema, csdlModel, edmModel);
        }

        private void BuildTermStart()
        {
            foreach (var modelItem in _modelMapping)
            {
                foreach (var csdlSchema in modelItem.Key.Schemata)
                {
                    foreach (var csdlTerm in csdlSchema.Terms)
                    {
                        BuildSchemaTerm(csdlTerm, csdlSchema, modelItem.Key, modelItem.Value);
                    }
                }
            }
        }

        private void BuildTermEnd()
        {
            foreach (var modelItem in _modelMapping)
            {
                foreach (var csdlSchema in modelItem.Key.Schemata)
                {
                    foreach (var csdlTerm in csdlSchema.Terms)
                    {
                        BuildSchemaTermBody(csdlTerm, modelItem.Key, csdlSchema, modelItem.Value);
                    }
                }
            }
        }

        public void BuildSchemaEntityContainerHeader()
        {
            foreach (var modelItem in _modelMapping)
            {
                foreach (var csdlSchema in modelItem.Key.Schemata)
                {
                    foreach (var csdlEntityContainer in csdlSchema.EntityContainers)
                    {
                        BuildEntityContainerHeader(csdlEntityContainer, modelItem.Key, csdlSchema, modelItem.Value);
                    }
                }
            }
        }

        public void BuildSchemaEntityContainerBody()
        {
            foreach (var modelItem in _modelMapping)
            {
                foreach (var csdlSchema in modelItem.Key.Schemata)
                {
                    foreach (var csdlEntityContainer in csdlSchema.EntityContainers)
                    {
                        BuildEntityContainerBody(csdlEntityContainer, modelItem.Key, csdlSchema, modelItem.Value);
                    }
                }
            }
        }

        private void BuildEntityContainerHeader(CsdlEntityContainer csdlEntityContainer, CsdlModel csdlModel, CsdlSchema csdlSchema, EdmModel edmModel)
        {
            EdmEntityContainer edmEntityContainer = new EdmEntityContainer(csdlSchema.Namespace, csdlEntityContainer.Name);
            edmModel.AddElement(edmEntityContainer);

            // first build all navigation sources, because the navigation binding will use it.
            foreach (var csdlEntitySet in csdlEntityContainer.EntitySets)
            {
                string namespaceQualifiedTypeName = csdlModel.ReplaceAlias(csdlEntitySet.ElementType);

                IEdmEntityType elementType = GetStructuredType(namespaceQualifiedTypeName) as IEdmEntityType;

                EdmEntitySet edmEntitySet = new EdmEntitySet(edmEntityContainer, csdlEntitySet.Name, elementType, csdlEntitySet.IncludeInServiceDocument);

                _navigationSources[edmEntityContainer.FullName + "." + edmEntitySet.Name] = edmEntitySet;
                edmEntityContainer.AddElement(edmEntitySet);

                BuildAnnotations(edmEntitySet, csdlEntitySet.VocabularyAnnotations, csdlSchema, csdlModel, edmModel);
            }

            foreach (var csdlSingleton in csdlEntityContainer.Singletons)
            {
                string namespaceQualifiedTypeName = csdlModel.ReplaceAlias(csdlSingleton.Type);

                IEdmEntityType elementType = GetStructuredType(namespaceQualifiedTypeName) as IEdmEntityType;

                EdmSingleton edmSingleton = new EdmSingleton(edmEntityContainer, csdlSingleton.Name, elementType);

                _navigationSources[edmEntityContainer.FullName + "." + edmSingleton.Name] = edmSingleton;
                edmEntityContainer.AddElement(edmSingleton);

                BuildAnnotations(edmSingleton, csdlSingleton.VocabularyAnnotations, csdlSchema, csdlModel, edmModel);
            }

            foreach (var csdlActionImport in csdlEntityContainer.OperationImports.OfType<CsdlActionImport>())
            {
                string namespaceQualifiedTypeName = csdlModel.ReplaceAlias(csdlActionImport.SchemaOperationQualifiedTypeName);
                IEdmAction action = FindAction(namespaceQualifiedTypeName, isBound: false);
                IEdmExpression entitySetExpression = BuildEntitySetPathExpression(csdlActionImport.EntitySet);

                EdmActionImport actionImport = new EdmActionImport(edmEntityContainer, csdlActionImport.Name, action, entitySetExpression);
                edmEntityContainer.AddElement(actionImport);

                BuildAnnotations(actionImport, csdlActionImport.VocabularyAnnotations, csdlSchema, csdlModel, edmModel);
            }

            foreach (var csdlFunctionImport in csdlEntityContainer.OperationImports.OfType<CsdlFunctionImport>())
            {
                string namespaceQualifiedTypeName = csdlModel.ReplaceAlias(csdlFunctionImport.SchemaOperationQualifiedTypeName);
                IEdmFunction function = FindFunction(namespaceQualifiedTypeName, isBound: false);
                IEdmExpression entitySetExpression = BuildEntitySetPathExpression(csdlFunctionImport.EntitySet);

                EdmFunctionImport functionImport = new EdmFunctionImport(edmEntityContainer, csdlFunctionImport.Name, function, entitySetExpression, csdlFunctionImport.IncludeInServiceDocument);
                edmEntityContainer.AddElement(functionImport);

                BuildAnnotations(functionImport, csdlFunctionImport.VocabularyAnnotations, csdlSchema, csdlModel, edmModel);
            }
        }

        private IEdmAction FindAction(string namespaceQualifiedName, bool isBound)
        {
            return _actions.Where(c => c.FullName == namespaceQualifiedName).First(c => c.IsBound == isBound);
        }

        private IEdmFunction FindFunction(string namespaceQualifiedName, bool isBound)
        {
            return _functions.Where(c => c.FullName == namespaceQualifiedName).First(c => c.IsBound == isBound);
        }

        private void BuildEntityContainerBody(CsdlEntityContainer csdlEntityContainer, CsdlModel csdlModel, CsdlSchema csdlSchema, EdmModel edmModel)
        {
            EdmEntityContainer edmEntityContainer = edmModel.SchemaElements.OfType<EdmEntityContainer>().First(c => c.Name == csdlEntityContainer.Name);

            // Now, it's time to build the navigation source bodies
            foreach (var csdlEntitySet in csdlEntityContainer.EntitySets)
            {
                EdmEntitySet entitySet = _navigationSources[edmEntityContainer.FullName + "." + csdlEntitySet.Name] as EdmEntitySet;

                BuildNavigationPropertyBinding(edmEntityContainer, entitySet, csdlEntitySet.NavigationPropertyBindings, csdlModel, csdlSchema, edmModel);
            }

            foreach (var csdlSingleton in csdlEntityContainer.Singletons)
            {
                EdmSingleton singleton = _navigationSources[edmEntityContainer.FullName + "." + csdlSingleton.Name] as EdmSingleton;
                BuildNavigationPropertyBinding(edmEntityContainer, singleton, csdlSingleton.NavigationPropertyBindings, csdlModel, csdlSchema, edmModel);
            }

            // annotations for container
            BuildAnnotations(edmEntityContainer, csdlEntityContainer.VocabularyAnnotations, csdlSchema, csdlModel, edmModel);
        }

        private void BuildNavigationPropertyBinding(EdmEntityContainer edmEntityContainer,
            EdmNavigationSource edmNavigationSource, IEnumerable<CsdlNavigationPropertyBinding> bindings, CsdlModel csdlModel, CsdlSchema csdlSchema, EdmModel edmModel)
        {
            IEdmEntityType entityType = edmNavigationSource.EntityType();

            foreach (var binding in bindings)
            {
                IEdmNavigationProperty edmNavigationProperty = FindNavigationProperty(entityType, binding.Path, csdlModel);
                IEdmNavigationSource target = FindNavigationSource(edmEntityContainer, binding.Target);
                IEdmPathExpression bindingPath = BuildBindingPath(binding.Path);
                edmNavigationSource.AddNavigationTarget(edmNavigationProperty, target, bindingPath);

                // NavigationPropertyBinding doesn't support annotations
            }
        }

        private static IEdmPathExpression BuildBindingPath(string path)
        {
            return new EdmNavigationPropertyPathExpression(path);
        }

        private IEdmNavigationProperty FindNavigationProperty(IEdmEntityType entityType, string path, CsdlModel csdlModel)
        {
            // Let's focus the simple path only with the navigation property name in the path
            string[] segments = path.Split('/');
            Stack<string> paths = new Stack<string>();
            foreach (var seg in segments.Reverse())
            {
                paths.Push(seg);
            }

            return FindNavigationProperty(entityType, csdlModel, paths);
        }

        private IEdmNavigationProperty FindNavigationProperty(IEdmStructuredType structuredType, CsdlModel csdlModel, Stack<string> paths)
        {
            if (paths.Count == 1)
            {
                return structuredType.FindProperty(paths.Peek()) as IEdmNavigationProperty;
            }
            else
            {
                string middelSegment = paths.Pop();

                if (middelSegment.IndexOf('.') != -1)
                {
                    middelSegment = csdlModel.ReplaceAlias(middelSegment);
                    IEdmStructuredType typeCastType = _structuredTypes[middelSegment];
                    return FindNavigationProperty(typeCastType, csdlModel, paths);
                }
                else
                {
                    IEdmProperty property = structuredType.FindProperty(middelSegment);
                    if (property.Type.IsCollection())
                    {
                        IEdmStructuredType propertyType = property.Type.AsCollection().ElementType().Definition as IEdmStructuredType;
                        return FindNavigationProperty(propertyType, csdlModel, paths);
                    }
                    else
                    {
                        IEdmStructuredType propertyType = property.Type.Definition as IEdmStructuredType;
                        return FindNavigationProperty(propertyType, csdlModel, paths);
                    }
                }
            }
        }

        private IEdmNavigationSource FindNavigationSource(EdmEntityContainer edmEntityContainer, string targetPath)
        {
            //  SomeModel.SomeContainer/SomeSet"
            string fullTargetName;
            int index = targetPath.IndexOf('/');
            if (index == -1)
            {
                fullTargetName = edmEntityContainer.FullName() + "." + targetPath;
            }
            else
            {
                fullTargetName = targetPath.Replace("/", ".");
            }

            return _navigationSources[fullTargetName];
        }

        public void BuildSchemaOperation()
        {
            foreach (var modelItem in _modelMapping)
            {
                foreach (var csdlSchema in modelItem.Key.Schemata)
                {
                    foreach (var csdlOperation in csdlSchema.Operations)
                    {
                        CsdlAction csdlAction = csdlOperation as CsdlAction;
                        if (csdlAction != null)
                        {
                            BuildSchemaAction(csdlAction, modelItem.Key, csdlSchema, modelItem.Value);
                        }
                        else
                        {
                            BuildSchemaFunction(csdlOperation as CsdlFunction, modelItem.Key, csdlSchema, modelItem.Value);
                        }
                    }
                }
            }

        }

        private void BuildSchemaAction(CsdlAction csdlAction, CsdlModel csdlModel, CsdlSchema csdlSchema, EdmModel edmModel)
        {
            IEdmTypeReference returnType = null;
            if (csdlAction.Return != null)
            {
                returnType = BuildEdmTypeReference(csdlAction.Return.ReturnType, csdlModel);
            }

            IEdmPathExpression entitySetPathExpression = BuildEntitySetPathExpression(csdlAction.EntitySetPath);

            EdmAction edmAction = new EdmAction(csdlSchema.Namespace, csdlAction.Name, returnType, csdlAction.IsBound, entitySetPathExpression);

            IEdmOperationReturn operationReturn = edmAction.Return;
            if (operationReturn != null)
            {
                BuildAnnotations(operationReturn, csdlAction.Return.VocabularyAnnotations, csdlSchema, csdlModel, edmModel);
            }

            BuildOperationParameters(edmAction, csdlAction.Parameters, csdlSchema, csdlModel, edmModel);

            _actions.Add(edmAction);
            edmModel.AddElement(edmAction);
        }

        private void BuildSchemaFunction(CsdlFunction csdlFunction, CsdlModel csdlModel, CsdlSchema csdlSchema, EdmModel edmModel)
        {
            IEdmTypeReference returnType = BuildEdmTypeReference(csdlFunction.Return.ReturnType,  csdlModel);

            IEdmPathExpression entitySetPathExpression = BuildEntitySetPathExpression(csdlFunction.EntitySetPath);

            EdmFunction edmFunction = new EdmFunction(csdlSchema.Namespace, csdlFunction.Name, returnType, csdlFunction.IsBound, entitySetPathExpression, csdlFunction.IsComposable);

            IEdmOperationReturn operationReturn = edmFunction.Return;
            BuildAnnotations(operationReturn, csdlFunction.Return.VocabularyAnnotations, csdlSchema, csdlModel, edmModel);

            BuildOperationParameters(edmFunction, csdlFunction.Parameters, csdlSchema, csdlModel, edmModel);

            _functions.Add(edmFunction);
            edmModel.AddElement(edmFunction);
        }

        private void BuildOperationParameters(EdmOperation operation, IEnumerable<CsdlOperationParameter> parameters, CsdlSchema csdlSchema, CsdlModel csdlModel,  EdmModel edmModel)
        {
            if (parameters == null)
            {
                return;
            }

            foreach (var parameter in parameters)
            {
                // for optional parameter, will todo later?
                IEdmTypeReference parameterType = BuildEdmTypeReference(parameter.Type, csdlModel);
                EdmOperationParameter edmParameter = new EdmOperationParameter(operation, parameter.Name, parameterType);
                operation.AddParameter(edmParameter);

                BuildAnnotations(edmParameter, parameter.VocabularyAnnotations, csdlSchema, csdlModel, edmModel);
            }
        }

        private static IEdmPathExpression BuildEntitySetPathExpression(string entitySetPath)
        {
            if (entitySetPath != null)
            {
                return new EdmPathExpression(entitySetPath);
            }

            return null;
        }


        private void BuildSchemaTerm(CsdlTerm csdlTerm, CsdlSchema csdlSchema, CsdlModel csdlModel, EdmModel edmModel)
        {
            IEdmTypeReference termType = BuildEdmTypeReference(csdlTerm.Type, csdlModel);
            EdmTerm edmTerm = new EdmTerm(csdlSchema.Namespace, csdlTerm.Name, termType, csdlTerm.AppliesTo, csdlTerm.DefaultValue);
            _terms[edmTerm.FullName] = edmTerm;
            edmModel.AddElement(edmTerm);
        }

        private void BuildAnnotations(IEdmVocabularyAnnotatable target, IEnumerable<CsdlAnnotation> csdlAnnotations, CsdlSchema csdlSchema, CsdlModel csdlModel,  EdmModel edmModel)
        {
            foreach (var csdlAnnotation in csdlAnnotations)
            {
                string namespaceQualifiedTermName = csdlModel.ReplaceAlias(csdlAnnotation.Term);
                IEdmTerm term = FindTerm(namespaceQualifiedTermName);
                if (term == null)
                {
                    term = new UnresolvedVocabularyTerm(namespaceQualifiedTermName);
                }

                IEdmTypeReference typeRef = GetTermTypeReference(term);
                IEdmExpression expression = BuildEdmExpression(csdlAnnotation.Expression, csdlModel, typeRef);

                EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(target, term, csdlAnnotation.Qualifier, expression);
                annotation.SetSerializationLocation(edmModel, EdmVocabularyAnnotationSerializationLocation.Inline);
                edmModel.AddVocabularyAnnotation(annotation);
            }
        }

        private void BuildSchemaTermBody(CsdlTerm csdlTerm, CsdlModel csdlModel, CsdlSchema csdlSchema, EdmModel edmModel)
        {
            IEdmTerm target = FindTerm(csdlSchema.Namespace + "." + csdlTerm.Name);
            Debug.Assert(target != null);

            // Term MAY contain annotations.
            foreach (var csdlAnnotation in csdlTerm.VocabularyAnnotations)
            {
                string namespaceQualifiedTermName = csdlModel.ReplaceAlias(csdlAnnotation.Term);
                IEdmTerm term = FindTerm(namespaceQualifiedTermName);
                if (term == null)
                {
                    term = new UnresolvedVocabularyTerm(namespaceQualifiedTermName);
                }

                IEdmTypeReference typeRef = GetTermTypeReference(term);
                IEdmExpression expression = BuildEdmExpression(csdlAnnotation.Expression, csdlModel, typeRef);

                EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(target, term, csdlAnnotation.Qualifier, expression);
                annotation.SetSerializationLocation(edmModel, EdmVocabularyAnnotationSerializationLocation.Inline);
                edmModel.AddVocabularyAnnotation(annotation);
            }
        }
        /*
        private IEdmExpression BuildEdmExpression(CsdlExpressionBase csdlExpression, CsdlModel csdlModel, IEdmTerm relatedTerm)
        {
            if (csdlExpression == null)
            {
                return null;
            }

            CsdlConstantExpression constantExpression = csdlExpression as CsdlConstantExpression;
            CsdlPathExpression pathExpression = csdlExpression as CsdlPathExpression;
            switch (csdlExpression.ExpressionKind)
            {
                case EdmExpressionKind.Cast:
                    CsdlCastExpression csdlCast = (CsdlCastExpression)csdlExpression;
                    IEdmExpression castExpression = BuildEdmExpression(csdlCast.Operand, csdlModel, relatedTerm);
                    IEdmTypeReference edmTypeReference = BuildEdmTypeReference(csdlCast.Type, csdlModel);
                    return new EdmCastExpression(castExpression, edmTypeReference);

                case EdmExpressionKind.BinaryConstant:
                    byte[] binary;
                    EdmValueParser.TryParseBinary(constantExpression.Value, out binary);
                    return new EdmBinaryConstant(binary);

                case EdmExpressionKind.BooleanConstant:
                    bool? local;
                    bool boolValue = EdmValueParser.TryParseBool(constantExpression.Value, out local) ? local.Value : false;
                    return new EdmBooleanConstant(boolValue);

                case EdmExpressionKind.StringConstant:
                    //  return new CsdlSemanticsStringConstantExpression((CsdlConstantExpression)expression, schema);
                    break;

                case EdmExpressionKind.DurationConstant:
                    TimeSpan? result;
                    EdmValueParser.TryParseDuration(constantExpression.Value, out result);
                    return new EdmDurationConstant(result.Value);

                case EdmExpressionKind.DateConstant:
                    Date? dateResult;
                    EdmValueParser.TryParseDate(constantExpression.Value, out dateResult);
                    return new EdmDateConstant(dateResult.Value);

                case EdmExpressionKind.TimeOfDayConstant:
                    TimeOfDay? timeOfResult;
                    EdmValueParser.TryParseTimeOfDay(constantExpression.Value, out timeOfResult);
                    return new EdmTimeOfDayConstant(timeOfResult.Value);

                case EdmExpressionKind.Collection:
                    CsdlCollectionExpression collectionExpression = (CsdlCollectionExpression)csdlExpression;

                    IList<IEdmExpression> expressions = new List<IEdmExpression>();
                    foreach (var item in collectionExpression.ElementValues)
                    {
                        expressions
                    }

                    return new EdmCollectionExpression(expressions);
                    break;
             //      return new CsdlSemanticsCollectionExpression((CsdlCollectionExpression)expression, bindingContext, schema);
                case EdmExpressionKind.DateTimeOffsetConstant:
               //     return new CsdlSemanticsDateTimeOffsetConstantExpression((CsdlConstantExpression)expression, schema);
                case EdmExpressionKind.DecimalConstant:
               //     return new CsdlSemanticsDecimalConstantExpression((CsdlConstantExpression)expression, schema);
                case EdmExpressionKind.EnumMember:
               //     return new CsdlSemanticsEnumMemberExpression((CsdlEnumMemberExpression)expression, bindingContext, schema);
                case EdmExpressionKind.FloatingConstant:
                //    return new CsdlSemanticsFloatingConstantExpression((CsdlConstantExpression)expression, schema);
                case EdmExpressionKind.Null:
                 //   return new CsdlSemanticsNullExpression((CsdlConstantExpression)expression, schema);
                case EdmExpressionKind.FunctionApplication:
                 //   return new CsdlSemanticsApplyExpression((CsdlApplyExpression)expression, bindingContext, schema);
                case EdmExpressionKind.GuidConstant:
                 //   return new CsdlSemanticsGuidConstantExpression((CsdlConstantExpression)expression, schema);
                case EdmExpressionKind.If:
                 //   return new CsdlSemanticsIfExpression((CsdlIfExpression)expression, bindingContext, schema);
                case EdmExpressionKind.IntegerConstant:
                 //   return new CsdlSemanticsIntConstantExpression((CsdlConstantExpression)expression, schema);
                case EdmExpressionKind.IsType:
                 //   return new CsdlSemanticsIsTypeExpression((CsdlIsTypeExpression)expression, bindingContext, schema);
                case EdmExpressionKind.LabeledExpressionReference:
                  //  return new CsdlSemanticsLabeledExpressionReferenceExpression((CsdlLabeledExpressionReferenceExpression)expression, bindingContext, schema);
                case EdmExpressionKind.Labeled:
                   // return schema.WrapLabeledElement((CsdlLabeledExpression)expression, bindingContext);

                case EdmExpressionKind.Record:
                    // return new CsdlSemanticsRecordExpression((CsdlRecordExpression)expression, bindingContext, schema);
                    break;

                case EdmExpressionKind.Path:
                    return new EdmPathExpression(pathExpression.Path);

                case EdmExpressionKind.PropertyPath:
                    return new EdmPropertyPathExpression(pathExpression.Path);

                case EdmExpressionKind.NavigationPropertyPath:
                    return new EdmNavigationPropertyPathExpression(pathExpression.Path);

                case EdmExpressionKind.AnnotationPath:
                    return new EdmAnnotationPathExpression(pathExpression.Path);
            }

            return null;
        }*/

        //  a string containing the numeric : "@self.HasPattern": "1"
        // symbolic enumeration value: "@self.HasPattern": "Red,Striped"
        private static IEnumerable<IEdmEnumMember> ParseEnumMember(string enumMember, IEdmEnumType enumType)
        {
            bool isFlag = enumType.IsFlags;
            long result;
            if (long.TryParse(enumMember, out result))
            {
                if (!isFlag)
                {
                    yield return enumType.Members.First(m => m.Value.Value == result);
                }
                else
                {
                    ulong mask = 0x1;
                    long item = 1;
                    IList<long> finded = new List<long>();
                    ulong remaining = (ulong)result;
                    while(remaining != 0)
                    {
                        ulong test = remaining & mask;
                        if (test == mask)
                        {
                            finded.Add(item);
                        }

                        remaining >>= 1;
                        item *= 2;
                    }

                    foreach (var v in finded)
                    {
                        yield return enumType.Members.First(m => m.Value.Value == v);
                    }
                }
            }
            else
            {
                string[] members = enumMember.Split(',');
                foreach (var member in members)
                {
                    yield return enumType.Members.First(m => m.Name == member);
                }
            }
        }

        private static IEdmExpression RebuildStringExpression(CsdlConstantExpression constantExpression, IEdmTypeReference termType)
        {
            // it could be "enum" or "path"
            IEdmEnumTypeReference enumTypeReference = termType as IEdmEnumTypeReference;
            if (enumTypeReference != null)
            {
                IEnumerable<IEdmEnumMember> enumMembers = ParseEnumMember(constantExpression.Value, enumTypeReference.EnumDefinition());
                return new EdmEnumMemberExpression(enumMembers.ToArray());
            }

            IEdmPathTypeReference pathTypeReference = termType as IEdmPathTypeReference;
            if (pathTypeReference != null)
            {
                switch (pathTypeReference.PathKind())
                {
                    case EdmPathTypeKind.PropertyPath:
                        return new EdmPropertyPathExpression(constantExpression.Value);

                    case EdmPathTypeKind.NavigationPropertyPath:
                        return new EdmNavigationPropertyPathExpression(constantExpression.Value);

                    case EdmPathTypeKind.AnnotationPath:
                        return new EdmAnnotationPathExpression(constantExpression.Value);

                    default: // for any others
                        return new EdmPathExpression(constantExpression.Value);
                }
            }

            IEdmPrimitiveTypeReference primitiveTypeReference = termType as IEdmPrimitiveTypeReference;
            if (primitiveTypeReference != null)
            {
                switch (primitiveTypeReference.PrimitiveKind())
                {
                    case EdmPrimitiveTypeKind.Binary:
                        byte[] binary;
                        EdmValueParser.TryParseBinary(constantExpression.Value, out binary);
                        return new EdmBinaryConstant(binary);

                    case EdmPrimitiveTypeKind.Boolean:
                        bool? local;
                        bool boolValue = EdmValueParser.TryParseBool(constantExpression.Value, out local) ? local.Value : false;
                        return new EdmBooleanConstant(boolValue);

                    case EdmPrimitiveTypeKind.Date:
                        Date? dateResult;
                        EdmValueParser.TryParseDate(constantExpression.Value, out dateResult);
                        return new EdmDateConstant(dateResult.Value);

                    case EdmPrimitiveTypeKind.DateTimeOffset:
                        DateTimeOffset? dateTimeOffsetResult;
                        EdmValueParser.TryParseDateTimeOffset(constantExpression.Value, out dateTimeOffsetResult);
                        return new EdmDateTimeOffsetConstant(dateTimeOffsetResult.Value);

                    case EdmPrimitiveTypeKind.Decimal:
                        decimal? decimalResult;
                        EdmValueParser.TryParseDecimal(constantExpression.Value, out decimalResult);
                        return new EdmDecimalConstant(decimalResult.Value);

                    case EdmPrimitiveTypeKind.Duration:
                        TimeSpan? result;
                        EdmValueParser.TryParseDuration(constantExpression.Value, out result);
                        return new EdmDurationConstant(result.Value);

                    case EdmPrimitiveTypeKind.Single:
                    case EdmPrimitiveTypeKind.Double:
                        // NaN, INF, -INF are special values both for decimal and floating.
                        // System.Decimal doesn't have special value defined
                        // So try to use "double"
                        if (constantExpression.Value == "NaN")
                        {
                            return new EdmFloatingConstant(double.NaN);
                        }

                        if (constantExpression.Value == "INF")
                        {
                            return new EdmFloatingConstant(double.PositiveInfinity);
                        }

                        if (constantExpression.Value == "-INF")
                        {
                            return new EdmFloatingConstant(double.NegativeInfinity);
                        }

                        double? doubelResult;
                        EdmValueParser.TryParseFloat(constantExpression.Value, out doubelResult);
                        return new EdmFloatingConstant(doubelResult.Value);

                    case EdmPrimitiveTypeKind.Guid:
                        Guid? guidResult;
                        EdmValueParser.TryParseGuid(constantExpression.Value, out guidResult);
                        return new EdmGuidConstant(guidResult.Value);

                    case EdmPrimitiveTypeKind.Int16:
                    case EdmPrimitiveTypeKind.Int32:
                    case EdmPrimitiveTypeKind.Int64:
                        int? longResult;
                        EdmValueParser.TryParseInt(constantExpression.Value, out longResult);
                        return new EdmIntegerConstant(longResult.Value);

                    case EdmPrimitiveTypeKind.TimeOfDay:
                        TimeOfDay? timeOfResult;
                        EdmValueParser.TryParseTimeOfDay(constantExpression.Value, out timeOfResult);
                        return new EdmTimeOfDayConstant(timeOfResult.Value);

                    case EdmPrimitiveTypeKind.String:
                        return new EdmStringConstant(constantExpression.Value);

                    default: // others we don't support for the expression type.
                        break;
                }
            }

            throw new Exception();
        }

        private IEdmExpression BuildEdmExpression(CsdlExpressionBase csdlExpression, CsdlModel csdlModel, IEdmTypeReference termType)
        {
            if (csdlExpression == null)
            {
                return null;
            }

            CsdlConstantExpression constantExpression = csdlExpression as CsdlConstantExpression;
            CsdlPathExpression pathExpression = csdlExpression as CsdlPathExpression;
            switch (csdlExpression.ExpressionKind)
            {
                case EdmExpressionKind.Cast:
                    CsdlCastExpression csdlCast = (CsdlCastExpression)csdlExpression;
                    IEdmExpression castExpression = BuildEdmExpression(csdlCast.Operand, csdlModel, termType);
                    IEdmTypeReference edmTypeReference = BuildEdmTypeReference(csdlCast.Type, csdlModel);
                    return new EdmCastExpression(castExpression, edmTypeReference);

                case EdmExpressionKind.BinaryConstant:
                    byte[] binary;
                    EdmValueParser.TryParseBinary(constantExpression.Value, out binary);
                    return new EdmBinaryConstant(binary);

                case EdmExpressionKind.BooleanConstant:
                    bool? local;
                    bool boolValue = EdmValueParser.TryParseBool(constantExpression.Value, out local) ? local.Value : false;
                    return new EdmBooleanConstant(boolValue);

                case EdmExpressionKind.StringConstant:
                    if (termType == null)
                    {
                        return new EdmStringConstant(constantExpression.Value);
                    }

                    return RebuildStringExpression(constantExpression, termType);

                case EdmExpressionKind.DurationConstant:
                    TimeSpan? result;
                    EdmValueParser.TryParseDuration(constantExpression.Value, out result);
                    return new EdmDurationConstant(result.Value);

                case EdmExpressionKind.DateConstant:
                    Date? dateResult;
                    EdmValueParser.TryParseDate(constantExpression.Value, out dateResult);
                    return new EdmDateConstant(dateResult.Value);

                case EdmExpressionKind.TimeOfDayConstant:
                    TimeOfDay? timeOfResult;
                    EdmValueParser.TryParseTimeOfDay(constantExpression.Value, out timeOfResult);
                    return new EdmTimeOfDayConstant(timeOfResult.Value);

                case EdmExpressionKind.Collection:
                    CsdlCollectionExpression collectionExpression = (CsdlCollectionExpression)csdlExpression;

                    IEdmTypeReference elementType = termType == null ? null : termType.AsCollection().ElementType();
                    IList<IEdmExpression> expressions = new List<IEdmExpression>();
                    foreach (var item in collectionExpression.ElementValues)
                    {
                        expressions.Add(BuildEdmExpression(item, csdlModel, elementType));
                    }

                    if (termType != null)
                    {
                        return new EdmCollectionExpression(termType, expressions);
                    }
                    else
                    {
                        return new EdmCollectionExpression(expressions);
                    }

                //      return new CsdlSemanticsCollectionExpression((CsdlCollectionExpression)expression, bindingContext, schema);
                case EdmExpressionKind.DateTimeOffsetConstant:
                //     return new CsdlSemanticsDateTimeOffsetConstantExpression((CsdlConstantExpression)expression, schema);
                case EdmExpressionKind.DecimalConstant:
                //     return new CsdlSemanticsDecimalConstantExpression((CsdlConstantExpression)expression, schema);
                case EdmExpressionKind.EnumMember:
                //     return new CsdlSemanticsEnumMemberExpression((CsdlEnumMemberExpression)expression, bindingContext, schema);
                case EdmExpressionKind.FloatingConstant:
                //    return new CsdlSemanticsFloatingConstantExpression((CsdlConstantExpression)expression, schema);
                case EdmExpressionKind.Null:
                //   return new CsdlSemanticsNullExpression((CsdlConstantExpression)expression, schema);
                case EdmExpressionKind.FunctionApplication:
                //   return new CsdlSemanticsApplyExpression((CsdlApplyExpression)expression, bindingContext, schema);
                case EdmExpressionKind.GuidConstant:
                //   return new CsdlSemanticsGuidConstantExpression((CsdlConstantExpression)expression, schema);
                case EdmExpressionKind.If:
                //   return new CsdlSemanticsIfExpression((CsdlIfExpression)expression, bindingContext, schema);
                case EdmExpressionKind.IntegerConstant:
                //   return new CsdlSemanticsIntConstantExpression((CsdlConstantExpression)expression, schema);
                case EdmExpressionKind.IsType:
                //   return new CsdlSemanticsIsTypeExpression((CsdlIsTypeExpression)expression, bindingContext, schema);
                case EdmExpressionKind.LabeledExpressionReference:
                //  return new CsdlSemanticsLabeledExpressionReferenceExpression((CsdlLabeledExpressionReferenceExpression)expression, bindingContext, schema);
                case EdmExpressionKind.Labeled:
                // return schema.WrapLabeledElement((CsdlLabeledExpression)expression, bindingContext);

                case EdmExpressionKind.Record:
                    // return new CsdlSemanticsRecordExpression((CsdlRecordExpression)expression, bindingContext, schema);
                    break;

                case EdmExpressionKind.Path:
                    return new EdmPathExpression(pathExpression.Path);

                case EdmExpressionKind.PropertyPath:
                    return new EdmPropertyPathExpression(pathExpression.Path);

                case EdmExpressionKind.NavigationPropertyPath:
                    return new EdmNavigationPropertyPathExpression(pathExpression.Path);

                case EdmExpressionKind.AnnotationPath:
                    return new EdmAnnotationPathExpression(pathExpression.Path);
            }

            return null;
        }

        private static IEdmTypeReference GetTermTypeReference(IEdmTerm edmTerm)
        {
            if (edmTerm is UnresolvedVocabularyTerm)
            {
                return null;
            }

            return edmTerm.Type;
        }


        private EdmTerm FindTerm(string namespaceQualifiedTermName)
        {
            EdmTerm term;
            _terms.TryGetValue(namespaceQualifiedTermName, out term);
            return term;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private EdmStructuredType GetStructuredType(string namespaceQualifiedName)
        {
            EdmStructuredType schemaType;
            _structuredTypes.TryGetValue(namespaceQualifiedName, out schemaType);
            return schemaType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private CsdlSchema GetRelatedSchema(CsdlStructuredType structuredType)
        {
            CsdlSchema schema;
            _schemaTypesMapping.TryGetValue(structuredType, out schema);
            return schema;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private CsdlModel GetRelatedCsdlModel(CsdlSchema csdlSchema)
        {
            CsdlModel model;
            _schemaMapping.TryGetValue(csdlSchema, out model);
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private EdmModel GetRelatedModel(CsdlModel csdlModel)
        {
            EdmModel model;
            _modelMapping.TryGetValue(csdlModel, out model);
            return model;
        }

        private CsdlStructuredType GetStructuredTypeItem(string namespaceQualifiedName)
        {
            CsdlStructuredType baseItem;
            _namespaceNameToStructuralTypeMapping.TryGetValue(namespaceQualifiedName, out baseItem);
            return baseItem;
        }

       

        private void BuildTypeDefinitionHeader(CsdlTypeDefinition csdlTypeDefinition, CsdlSchema csdlSchema, CsdlModel csdlModel, EdmModel edmModel)
        {
            // EdmTypeDefinition doesn't support the type facts now.
            EdmTypeDefinition typeDefinition = new EdmTypeDefinition(csdlSchema.Namespace, csdlTypeDefinition.Name,
                        GetUnderlyingType(csdlTypeDefinition.UnderlyingTypeName, EdmPrimitiveTypeKind.String));

            _typeDefinitions[typeDefinition.FullTypeName()] = typeDefinition;
            edmModel.AddElement(typeDefinition);
        }

        private void BuildEnumTypeHeader(CsdlEnumType csdlEnum, CsdlSchema csdlSchema, CsdlModel csdlModel, EdmModel edmModel)
        {
            EdmEnumType enumType = new EdmEnumType(csdlSchema.Namespace,
                csdlEnum.Name,
                GetUnderlyingType(csdlEnum.UnderlyingTypeName, EdmPrimitiveTypeKind.Int32),
                csdlEnum.IsFlags);

            _enumTypes[enumType.FullTypeName()] = enumType;
            edmModel.AddElement(enumType);
        }

        private static IEdmPrimitiveType GetUnderlyingType(string underlyingType, EdmPrimitiveTypeKind defaultKind)
        {
            if (underlyingType != null)
            {
                var underlyingTypeKind = EdmCoreModel.Instance.GetPrimitiveTypeKind(underlyingType);
                if ( underlyingTypeKind != EdmPrimitiveTypeKind.None)
                {
                    return EdmCoreModel.Instance.GetPrimitiveType(underlyingTypeKind);
                }

                throw new Exception();
            }

            return EdmCoreModel.Instance.GetPrimitiveType(defaultKind);
        }

        public IEdmTypeReference BuildEdmTypeReference(CsdlTypeReference csdlTypeReference, CsdlModel csdlModel)
        {
            if (csdlTypeReference == null)
            {
                return null;
            }

            var namedTypeReference = csdlTypeReference as CsdlNamedTypeReference;
            if (namedTypeReference != null)
            {
                var primitiveReference = namedTypeReference as CsdlPrimitiveTypeReference;
                if (primitiveReference != null)
                {
                    IEdmPrimitiveTypeReference primitiveType = EdmCoreModel.Instance.GetPrimitive(primitiveReference.Kind, primitiveReference.IsNullable);
                    switch (primitiveReference.Kind)
                    {
                        case EdmPrimitiveTypeKind.Boolean:
                        case EdmPrimitiveTypeKind.Byte:
                        case EdmPrimitiveTypeKind.Date:
                        case EdmPrimitiveTypeKind.Double:
                        case EdmPrimitiveTypeKind.Guid:
                        case EdmPrimitiveTypeKind.Int16:
                        case EdmPrimitiveTypeKind.Int32:
                        case EdmPrimitiveTypeKind.Int64:
                        case EdmPrimitiveTypeKind.SByte:
                        case EdmPrimitiveTypeKind.Single:
                        case EdmPrimitiveTypeKind.Stream:
                            return primitiveType;

                        case EdmPrimitiveTypeKind.Binary:
                            return new EdmBinaryTypeReference(primitiveType.PrimitiveDefinition(), primitiveReference.IsNullable, primitiveReference.IsUnbounded, primitiveReference.MaxLength);

                        case EdmPrimitiveTypeKind.DateTimeOffset:
                        case EdmPrimitiveTypeKind.Duration:
                        case EdmPrimitiveTypeKind.TimeOfDay:
                            return new EdmTemporalTypeReference(primitiveType.PrimitiveDefinition(), primitiveReference.IsNullable);

                        case EdmPrimitiveTypeKind.Decimal:
                            return new EdmDecimalTypeReference(primitiveType.PrimitiveDefinition(), primitiveReference.IsNullable, primitiveReference.Precision, primitiveReference.Scale);

                        case EdmPrimitiveTypeKind.String:
                            return new EdmStringTypeReference(primitiveType.PrimitiveDefinition(), primitiveReference.IsNullable, primitiveReference.IsUnbounded, primitiveReference.MaxLength, primitiveReference.IsUnicode);

                        case EdmPrimitiveTypeKind.Geography:
                        case EdmPrimitiveTypeKind.GeographyPoint:
                        case EdmPrimitiveTypeKind.GeographyLineString:
                        case EdmPrimitiveTypeKind.GeographyPolygon:
                        case EdmPrimitiveTypeKind.GeographyCollection:
                        case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                        case EdmPrimitiveTypeKind.GeographyMultiLineString:
                        case EdmPrimitiveTypeKind.GeographyMultiPoint:
                        case EdmPrimitiveTypeKind.Geometry:
                        case EdmPrimitiveTypeKind.GeometryPoint:
                        case EdmPrimitiveTypeKind.GeometryLineString:
                        case EdmPrimitiveTypeKind.GeometryPolygon:
                        case EdmPrimitiveTypeKind.GeometryCollection:
                        case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                        case EdmPrimitiveTypeKind.GeometryMultiLineString:
                        case EdmPrimitiveTypeKind.GeometryMultiPoint:
                            return new EdmSpatialTypeReference(primitiveType.PrimitiveDefinition(), primitiveReference.IsNullable, primitiveReference.SpatialReferenceIdentifier);
                    }
                }
                else
                {
                    // Untyped
                    CsdlUntypedTypeReference csdlUntypedTypeReference = namedTypeReference as CsdlUntypedTypeReference;
                    if (csdlUntypedTypeReference != null)
                    {
                        return EdmCoreModel.Instance.GetUntyped();
                    }

                    // Path
                    EdmPathTypeKind pathTypeKind = EdmCoreModel.Instance.GetPathTypeKind(namedTypeReference.FullName);
                    if (pathTypeKind != EdmPathTypeKind.None)
                    {
                        return EdmCoreModel.Instance.GetPathType(pathTypeKind, namedTypeReference.IsNullable);
                    }

                    string fullNamespaceQualifiedName = csdlModel.ReplaceAlias(namedTypeReference.FullName);

                    // Type-Definition
                    EdmTypeDefinition typeDefinition;
                    if (_typeDefinitions.TryGetValue(fullNamespaceQualifiedName, out typeDefinition))
                    {
                        return new EdmTypeDefinitionReference(typeDefinition,
                            namedTypeReference.IsNullable,
                            namedTypeReference.IsUnbounded,
                            namedTypeReference.MaxLength,
                            namedTypeReference.IsUnicode,
                            namedTypeReference.Precision,
                            namedTypeReference.Scale,
                            namedTypeReference.SpatialReferenceIdentifier);
                    }

                    // Enum
                    EdmEnumType enumType;
                    if (_enumTypes.TryGetValue(fullNamespaceQualifiedName, out enumType))
                    {
                        return new EdmEnumTypeReference(enumType, namedTypeReference.IsNullable);
                    }

                    // Complex or entity
                    EdmStructuredType structuredType;
                    if (_structuredTypes.TryGetValue(fullNamespaceQualifiedName, out structuredType))
                    {
                        IEdmComplexType complexType = structuredType as IEdmComplexType;
                        if (complexType != null)
                        {
                            return new EdmComplexTypeReference(complexType, namedTypeReference.IsNullable);
                        }
                        else
                        {
                            return new EdmEntityTypeReference(structuredType as IEdmEntityType, namedTypeReference.IsNullable);
                        }
                    }
                }
            }

            var typeExpression = csdlTypeReference as CsdlExpressionTypeReference;
            if (typeExpression != null)
            {
                var collectionType = typeExpression.TypeExpression as CsdlCollectionType;
                if (collectionType != null)
                {
                    IEdmTypeReference elementType = BuildEdmTypeReference(collectionType.ElementType, csdlModel);
                    return new EdmCollectionTypeReference(new EdmCollectionType(elementType));
                }

                var entityReferenceType = typeExpression.TypeExpression as CsdlEntityReferenceType;
                if (entityReferenceType != null)
                {
                    // Json doesn't support this.
                    // return new CsdlSemanticsEntityReferenceTypeExpression(typeExpression, new CsdlSemanticsEntityReferenceTypeDefinition(schema, entityReferenceType));
                }
            }

            throw new Exception();
        }

        public static IEdmTypeReference BuildEdmPrimitiveTypeReference(string typeString, // if it's collection, it's element type string
            bool isCollection,
            bool isNullable,
            bool isUnbounded,
            int? maxLength,
            bool? unicode,
            int? precision,
            int? scale,
            int? srid)
        {
            IEdmTypeReference type = BuildEdmElementPrimitiveTypeReference(typeString, isNullable, isUnbounded, maxLength, unicode, precision, scale, srid);
            if (isCollection)
            {
                type = new EdmCollectionTypeReference(new EdmCollectionType(type));
            }

            return type;
        }

        private static IEdmTypeReference BuildEdmElementPrimitiveTypeReference(string typeName, bool isNullable,
             bool isUnbounded,
             int? maxLength,
             bool? unicode,
             int? precision,
             int? scale,
             int? srid)
        {
            IEdmPrimitiveTypeReference primitiveType;
            EdmPrimitiveTypeKind kind = EdmCoreModel.Instance.GetPrimitiveTypeKind(typeName);
            switch (kind)
            {
                case EdmPrimitiveTypeKind.Boolean:
                case EdmPrimitiveTypeKind.Byte:
                case EdmPrimitiveTypeKind.Double:
                case EdmPrimitiveTypeKind.Guid:
                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int64:
                case EdmPrimitiveTypeKind.SByte:
                case EdmPrimitiveTypeKind.Single:
                case EdmPrimitiveTypeKind.Stream:
                case EdmPrimitiveTypeKind.Date:
                case EdmPrimitiveTypeKind.PrimitiveType:
                    return EdmCoreModel.Instance.GetPrimitive(kind, isNullable);

                case EdmPrimitiveTypeKind.Binary:
                    primitiveType = EdmCoreModel.Instance.GetPrimitive(kind, isNullable);
                    return new EdmBinaryTypeReference(primitiveType.PrimitiveDefinition(), primitiveType.IsNullable, isUnbounded, maxLength);

                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Duration:
                case EdmPrimitiveTypeKind.TimeOfDay:
                    primitiveType = EdmCoreModel.Instance.GetPrimitive(kind, isNullable);
                    return new EdmTemporalTypeReference(primitiveType.PrimitiveDefinition(), primitiveType.IsNullable, precision);

                case EdmPrimitiveTypeKind.Decimal:
                    primitiveType = EdmCoreModel.Instance.GetPrimitive(kind, isNullable);
                    return new EdmDecimalTypeReference(primitiveType.PrimitiveDefinition(), primitiveType.IsNullable, precision, scale);

                case EdmPrimitiveTypeKind.String:
                    primitiveType = EdmCoreModel.Instance.GetPrimitive(kind, isNullable);
                    return new EdmStringTypeReference(primitiveType.PrimitiveDefinition(), primitiveType.IsNullable, isUnbounded, maxLength, unicode);

                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.GeographyPoint:
                case EdmPrimitiveTypeKind.GeographyLineString:
                case EdmPrimitiveTypeKind.GeographyPolygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometryPoint:
                case EdmPrimitiveTypeKind.GeometryLineString:
                case EdmPrimitiveTypeKind.GeometryPolygon:
                case EdmPrimitiveTypeKind.GeometryCollection:
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                    primitiveType = EdmCoreModel.Instance.GetPrimitive(kind, isNullable);
                    return new EdmSpatialTypeReference(primitiveType.PrimitiveDefinition(), primitiveType.IsNullable, srid);

                case EdmPrimitiveTypeKind.None:
                    break;
            }

            EdmPathTypeKind pathTypeKind = EdmCoreModel.Instance.GetPathTypeKind(typeName);
            if (pathTypeKind != EdmPathTypeKind.None)
            {
                return EdmCoreModel.Instance.GetPathType(pathTypeKind, isNullable);
            }

            return null;
        }
    }
}
