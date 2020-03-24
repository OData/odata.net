//---------------------------------------------------------------------
// <copyright file="SchemaTypeBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Json.Builder
{
    /// <summary>
    /// Provides CSDL-JSON parsing services for EDM models.
    /// Complex, Entity, Enum, TypeDefinition -> IEdmSchemaType
    /// </summary>
    internal class SchemaTypeJsonBuilder
    {
        private IDictionary<CsdlModel, EdmModel> _modelMapping;
        private AliasNamespaceHelper _aliasNameMapping;

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

        public IDictionary<string, EdmStructuredType> StructuredTypes
        {
            get { return _structuredTypes; }
        }

        public IDictionary<string, EdmEnumType> EnumTypes
        {
            get { return _enumTypes; }
        }

        public IDictionary<string, EdmTypeDefinition> TypeDefinitions
        {
            get { return _typeDefinitions; }
        }

        internal SchemaTypeJsonBuilder(IDictionary<CsdlModel, EdmModel> modelMapping, AliasNamespaceHelper aliasNsMapping)
        {
            _modelMapping = modelMapping;
            _aliasNameMapping = aliasNsMapping;

            foreach (var csdlModel in modelMapping.Keys)
            {
                foreach (var csdlSchema in csdlModel.Schemata)
                {
                    _schemaMapping[csdlSchema] = csdlModel;

                    foreach (var stype in csdlSchema.StructuredTypes)
                    {
                        _schemaTypesMapping[stype] = csdlSchema;

                        CsdlNamedStructuredType namedType = (CsdlNamedStructuredType)stype;
                        _namespaceNameToStructuralTypeMapping[csdlSchema + "." + namedType.Name] = stype;
                    }
                }
            }
        }

        public void BuildSchemaItems()
        {
            _structuredTypes.Clear();
            _enumTypes.Clear();
            _typeDefinitions.Clear();

            // Create structured type, Type Definition, enum type header
            BuildSchemaTypeHeader();

            // Now it's ready to build the term,
            // term should build after the types' built
            foreach (var modelItem in _modelMapping)
            {
                foreach (var csdlSchema in modelItem.Key.Schemata)
                {
                    foreach (var csdlTerm in csdlSchema.Terms)
                    {
                        BuildSchemaTerm(csdlTerm, csdlSchema, modelItem.Value);
                    }
                }
            }

            // Now, it's time to finish the Term build
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

        private void BuildSchemaTypeHeader()
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
                string baseTypeFullNamespaceQualifiedName = _aliasNameMapping.ReplaceAlias(csdlModel, csdlComplex.BaseTypeName);

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
                string baseTypeFullNamespaceQualifiedName = _aliasNameMapping.ReplaceAlias(csdlModel, csdlEntity.BaseTypeName);

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
            foreach (var modelItem in _modelMapping)
            {
                foreach (var csdlSchema in modelItem.Key.Schemata)
                {
                    foreach (var csdlStructuredType in csdlSchema.StructuredTypes)
                    {
                        BuildSchemaStructuralBody(csdlStructuredType, modelItem.Key, csdlSchema, modelItem.Value);
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
        }

        private void BuildSchemaStructuralBody(CsdlStructuredType csdlStructured, CsdlModel csdlModel, CsdlSchema csdlSchema, EdmModel edmModel)
        {
            CsdlComplexType csdlComplexType = csdlStructured as CsdlComplexType;
            if (csdlComplexType != null)
            {
                BuildSchemaComplexBody(csdlComplexType, csdlModel, csdlSchema, edmModel);
            }
            else
            {
                BuildSchemaEntityBody(csdlStructured as CsdlEntityType, csdlModel, csdlSchema, edmModel);
            }
        }

        private void BuildSchemaComplexBody(CsdlComplexType csdlComplex, CsdlModel csdlModel, CsdlSchema csdlSchema, EdmModel edmModel)
        {
            EdmComplexType edmComplexType = _structuredTypes[csdlSchema.Namespace + "." + csdlComplex.Name] as EdmComplexType;

            BuildStructuralProperties(edmComplexType, csdlComplex.StructuralProperties, csdlModel, csdlSchema, edmModel);

            BuildNavigationProperties(edmComplexType, csdlComplex.NavigationProperties, csdlModel, csdlSchema, edmModel);

            BuildAnnotations(edmComplexType, csdlComplex.VocabularyAnnotations, csdlModel, csdlSchema, edmModel);
        }

        private void BuildSchemaEntityBody(CsdlEntityType csdlEntity, CsdlModel csdlModel, CsdlSchema csdlSchema, EdmModel edmModel)
        {
            EdmEntityType edmEntityType = _structuredTypes[csdlSchema.Namespace + "." + csdlEntity.Name] as EdmEntityType;

            BuildStructuralProperties(edmEntityType, csdlEntity.StructuralProperties, csdlModel, csdlSchema, edmModel);

            BuildNavigationProperties(edmEntityType, csdlEntity.NavigationProperties, csdlModel, csdlSchema, edmModel);

            // Add the key??

            BuildAnnotations(edmEntityType, csdlEntity.VocabularyAnnotations, csdlModel, csdlSchema, edmModel);
        }

        private void BuildStructuralProperties(EdmStructuredType structuredType, IEnumerable<CsdlProperty> structualProperties, CsdlModel csdlModel, CsdlSchema csdlSchema, EdmModel edmModel)
        {
            foreach (var csdlProperty in structualProperties)
            {
                IEdmTypeReference propertyType = BuildEdmTypeReference(csdlProperty.Type);

                var edmProperty = structuredType.AddStructuralProperty(csdlProperty.Name, propertyType, csdlProperty.DefaultValue);

                BuildAnnotations(edmProperty, csdlProperty.VocabularyAnnotations, csdlModel, csdlSchema, edmModel);
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
                string fullName = _aliasNameMapping.ReplaceAlias(csdlModel, elementTypeName);

                EdmMultiplicity multiplicity;
                if (isCollection)
                {
                    multiplicity = EdmMultiplicity.Many;
                }
                else
                {
                    if (csdlNavProperty.Nullable.Value)
                    {
                        multiplicity = EdmMultiplicity.ZeroOrOne;
                    }
                    else
                    {
                        multiplicity = EdmMultiplicity.One;
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

                BuildAnnotations(edmNavProperty, csdlNavProperty.VocabularyAnnotations, csdlModel, csdlSchema, edmModel);
            }
        }

        private static IEnumerable<IEdmStructuralProperty> GetDeclaringPropertyInfo(IEdmStructuredType type, IEnumerable<string> propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                yield return type.FindProperty(propertyName) as IEdmStructuralProperty;
            }
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

                BuildAnnotations(edmEnumMember, enumMember.VocabularyAnnotations, csdlModel, csdlSchema, edmModel);
            }


            BuildAnnotations(enumType, csdlEnum.VocabularyAnnotations, csdlModel, csdlSchema, edmModel);
        }

        private void BuildSchemaTypeDefinitionBody(CsdlTypeDefinition csdlTypeDefinition, CsdlModel csdlModel, CsdlSchema csdlSchema, EdmModel edmModel)
        {
            string fullName = csdlSchema.Namespace + "." + csdlTypeDefinition.Name;
            IEdmTypeDefinition edmTypeDefinition = _typeDefinitions[fullName];

            BuildAnnotations(edmTypeDefinition, csdlTypeDefinition.VocabularyAnnotations, csdlModel, csdlSchema, edmModel);
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
                string namespaceQualifiedTypeName = _aliasNameMapping.ReplaceAlias(csdlModel, csdlEntitySet.ElementType);

                IEdmEntityType elementType = GetStructuredType(namespaceQualifiedTypeName) as IEdmEntityType;

                EdmEntitySet edmEntitySet = new EdmEntitySet(edmEntityContainer, csdlEntitySet.Name, elementType, csdlEntitySet.IncludeInServiceDocument);

                _navigationSources[edmEntityContainer.FullName + "." + edmEntitySet.Name] = edmEntitySet;
                edmEntityContainer.AddElement(edmEntitySet);

                BuildAnnotations(edmEntitySet, csdlEntitySet.VocabularyAnnotations, csdlModel, csdlSchema, edmModel);
            }

            foreach (var csdlSingleton in csdlEntityContainer.Singletons)
            {
                string namespaceQualifiedTypeName = _aliasNameMapping.ReplaceAlias(csdlModel, csdlSingleton.Type);

                IEdmEntityType elementType = GetStructuredType(namespaceQualifiedTypeName) as IEdmEntityType;

                EdmSingleton edmSingleton = new EdmSingleton(edmEntityContainer, csdlSingleton.Name, elementType);

                _navigationSources[edmEntityContainer.FullName + "." + edmSingleton.Name] = edmSingleton;
                edmEntityContainer.AddElement(edmSingleton);

                BuildAnnotations(edmSingleton, csdlSingleton.VocabularyAnnotations, csdlModel, csdlSchema, edmModel);
            }

            foreach (var csdlActionImport in csdlEntityContainer.OperationImports.OfType<CsdlActionImport>())
            {
                string namespaceQualifiedTypeName = _aliasNameMapping.ReplaceAlias(csdlModel, csdlActionImport.SchemaOperationQualifiedTypeName);
                IEdmAction action = FindAction(namespaceQualifiedTypeName, isBound: false);
                IEdmExpression entitySetExpression = BuildEntitySetPathExpression(csdlActionImport.EntitySet);

                EdmActionImport actionImport = new EdmActionImport(edmEntityContainer, csdlActionImport.Name, action, entitySetExpression);
                edmEntityContainer.AddElement(actionImport);

                BuildAnnotations(actionImport, csdlActionImport.VocabularyAnnotations, csdlModel, csdlSchema, edmModel);
            }

            foreach (var csdlFunctionImport in csdlEntityContainer.OperationImports.OfType<CsdlFunctionImport>())
            {
                string namespaceQualifiedTypeName = _aliasNameMapping.ReplaceAlias(csdlModel, csdlFunctionImport.SchemaOperationQualifiedTypeName);
                IEdmFunction function = FindFunction(namespaceQualifiedTypeName, isBound: false);
                IEdmExpression entitySetExpression = BuildEntitySetPathExpression(csdlFunctionImport.EntitySet);

                EdmFunctionImport functionImport = new EdmFunctionImport(edmEntityContainer, csdlFunctionImport.Name, function, entitySetExpression, csdlFunctionImport.IncludeInServiceDocument);
                edmEntityContainer.AddElement(functionImport);

                BuildAnnotations(functionImport, csdlFunctionImport.VocabularyAnnotations, csdlModel, csdlSchema, edmModel);
            }
        }
        private static IEdmExpression BuildEntitySetPathExpression(string entitySet)
        {
            return null;
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

                BuildNavigationPropertyBinding(entitySet, csdlEntitySet.NavigationPropertyBindings, csdlModel, csdlSchema, edmModel);
            }

            foreach (var csdlSingleton in csdlEntityContainer.Singletons)
            {
                EdmSingleton singleton = _navigationSources[edmEntityContainer.FullName + "." + csdlSingleton.Name] as EdmSingleton;
                BuildNavigationPropertyBinding(singleton, csdlSingleton.NavigationPropertyBindings, csdlModel, csdlSchema, edmModel);
            }

            // annotations for container
            BuildAnnotations(edmEntityContainer, csdlEntityContainer.VocabularyAnnotations, csdlModel, csdlSchema, edmModel);
        }

        private static void BuildNavigationPropertyBinding(EdmNavigationSource edmNavigationSource, IEnumerable<CsdlNavigationPropertyBinding> bindings, CsdlModel csdlModel, CsdlSchema csdlSchema, EdmModel edmModel)
        {
            IEdmEntityType entityType = edmNavigationSource.EntityType();

            foreach (var binding in bindings)
            {
                IEdmNavigationProperty edmNavigationProperty = FindNavigationProperty(entityType, binding.Path);
                IEdmNavigationSource target = FindNavigationSource(binding.Target);
                IEdmPathExpression bindingPath = BuildBindingPath(binding.Path);
                edmNavigationSource.AddNavigationTarget(edmNavigationProperty, target, bindingPath);

                // NavigationPropertyBinding doesn't support annotations
            }
        }

        private static IEdmPathExpression BuildBindingPath(string path)
        {
            return null;
        }

        private static IEdmNavigationProperty FindNavigationProperty(IEdmEntityType entityType, string path)
        {
            return null;
        }

        private static IEdmNavigationSource FindNavigationSource(string targetPath)
        {
            //  SomeModel.SomeContainer/SomeSet"
            return null;
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
            IEdmTypeReference returnType = BuildReturnType(csdlAction.Return);

            IEdmPathExpression entitySetPathExpression = BuildEntitySetPathExpression();

            EdmAction edmAction = new EdmAction(csdlSchema.Namespace, csdlAction.Name, returnType, csdlAction.IsBound, entitySetPathExpression);

            _actions.Add(edmAction);
            edmModel.AddElement(edmAction);
        }

        private void BuildSchemaFunction(CsdlFunction csdlFunction, CsdlModel csdlModel, CsdlSchema csdlSchema, EdmModel edmModel)
        {
            IEdmTypeReference returnType = BuildReturnType(csdlFunction.Return);

            IEdmPathExpression entitySetPathExpression = BuildEntitySetPathExpression();

            EdmFunction edmFunction = new EdmFunction(csdlSchema.Namespace, csdlFunction.Name, returnType, csdlFunction.IsBound, entitySetPathExpression, csdlFunction.IsComposable);

            _functions.Add(edmFunction);
            edmModel.AddElement(edmFunction);
        }

        private static IEdmPathExpression BuildEntitySetPathExpression()
        {
            return null;
        }

        private static IEdmTypeReference BuildReturnType(CsdlOperationReturn returnType)
        {
            return null;
        }

        private void BuildSchemaTerm(CsdlTerm csdlTerm, CsdlSchema csdlSchema, EdmModel edmModel)
        {
            IEdmTypeReference termType = BuildEdmTypeReference(csdlTerm.Type);
            EdmTerm edmTerm = new EdmTerm(csdlSchema.Namespace, csdlTerm.Name, termType, csdlTerm.AppliesTo);
            _terms[edmTerm.FullName] = edmTerm;
            edmModel.AddElement(edmTerm);
        }

        private void BuildAnnotations(IEdmVocabularyAnnotatable target, IEnumerable<CsdlAnnotation> csdlAnnotations, CsdlModel csdlModel, CsdlSchema csdlSchema, EdmModel edmModel)
        {
            foreach (var csdlAnnotation in csdlAnnotations)
            {
                string namespaceQualifiedTermName = _aliasNameMapping.ReplaceAlias(csdlModel, csdlAnnotation.Term);
                IEdmTerm term = FindTerm(namespaceQualifiedTermName);
                if (term == null)
                {
                    term = new UnresolvedVocabularyTerm(namespaceQualifiedTermName);
                }

                IEdmExpression expression = BuildEdmExpression(csdlAnnotation.Expression, term);

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
                string namespaceQualifiedTermName = _aliasNameMapping.ReplaceAlias(csdlModel, csdlAnnotation.Term);
                IEdmTerm term = FindTerm(namespaceQualifiedTermName);
                if (term == null)
                {
                    term = new UnresolvedVocabularyTerm(namespaceQualifiedTermName);
                }

                IEdmExpression expression = BuildEdmExpression(csdlAnnotation.Expression, term);

                EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(target, term, csdlAnnotation.Qualifier, expression);
                annotation.SetSerializationLocation(edmModel, EdmVocabularyAnnotationSerializationLocation.Inline);
                edmModel.AddVocabularyAnnotation(annotation);
            }
        }

        private static IEdmExpression BuildEdmExpression(CsdlExpressionBase csdlExpression, IEdmTerm relatedTerm)
        {
            return null;
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
            EdmTypeDefinition typeDefinition = new EdmTypeDefinition(csdlSchema.Namespace, csdlTypeDefinition.Name,
                        GetUnderlyingType(csdlTypeDefinition));

            _typeDefinitions[typeDefinition.FullTypeName()] = typeDefinition;
            edmModel.AddElement(typeDefinition);
        }

        private void BuildEnumTypeHeader(CsdlEnumType csdlEnum, CsdlSchema csdlSchema, CsdlModel csdlModel, EdmModel edmModel)
        {
            EdmEnumType enumType = new EdmEnumType(csdlSchema.Namespace,
                csdlEnum.Name,
                GetEnumUnderlyingType(csdlEnum.UnderlyingTypeName),
                csdlEnum.IsFlags);

            _enumTypes[enumType.FullTypeName()] = enumType;
            edmModel.AddElement(enumType);
        }

       

        private static IEdmPrimitiveType GetUnderlyingType(CsdlTypeDefinition typeDefinitionItem)
        {
            IEdmTypeReference underlyingTypeRef = BuildTypeReference(typeDefinitionItem.UnderlyingTypeName,
                false,
                true,
                false,
                typeDefinitionItem.MaxLength,
                typeDefinitionItem.Unicode,
                typeDefinitionItem.Precision,
                typeDefinitionItem.Scale,
                typeDefinitionItem.Srid);

            IEdmType underlyingType = underlyingTypeRef.Definition;
            if (underlyingType.TypeKind != EdmTypeKind.Primitive)
            {
                throw new Exception();
            }

            return (IEdmPrimitiveType)underlyingType;
        }

        private static IEdmPrimitiveType GetEnumUnderlyingType(string underlyingType)
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

            return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32);
        }

        public static IEdmTypeReference BuildEdmTypeReference(CsdlTypeReference csdlTypeReference)
        {
            return null;
        }

        public static IEdmTypeReference BuildTypeReference(string typeString, // if it's collection, it's element type string
            bool isCollection,
            bool isNullable,
            bool isUnbounded,
            int? maxLength,
            bool? unicode,
            int? precision,
            int? scale,
            int? srid)
        {
            IEdmTypeReference type = ParseNamedTypeReference(typeString, isNullable, isUnbounded, maxLength, unicode, precision, scale, srid);
            if (isCollection)
            {
                type = new EdmCollectionTypeReference(new EdmCollectionType(type));
            }

            return type;
        }

        private static IEdmTypeReference ParseNamedTypeReference(string typeName, bool isNullable,
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

            // If we can't find the type, find it from referenced model.
            // IEdmType edmType = _edmModel.FindType(typeName);
            return null;
            //return GetEdmTypeReference(edmType, isNullable, isUnbounded, maxLength, unicode, precision, scale, srid);
        }

        //private static IEdmTypeReference GetEdmTypeReference(IEdmType edmType, bool isNullable,
        //     bool isUnbounded,
        //     int? maxLength,
        //     bool? unicode,
        //     int? precision,
        //     int? scale,
        //     int? srid)
        //{
        //    switch (edmType.TypeKind)
        //    {
        //        case EdmTypeKind.Complex:
        //            return new EdmComplexTypeReference((IEdmComplexType)edmType, isNullable);

        //        case EdmTypeKind.Entity:
        //            return new EdmEntityTypeReference((IEdmEntityType)edmType, isNullable);

        //        case EdmTypeKind.Enum:
        //            return new EdmEnumTypeReference((IEdmEnumType)edmType, isNullable);

        //        case EdmTypeKind.TypeDefinition:
        //            return new EdmTypeDefinitionReference((IEdmTypeDefinition)edmType, isNullable, isUnbounded, maxLength, isUnbounded, precision, scale, srid);
        //    }

        //    throw new CsdlParseException();
        //}
    }
}
