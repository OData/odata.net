//---------------------------------------------------------------------
// <copyright file="CsdlToEdmModelComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Utils.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.Test.OData.Utils.Common;

    /// <summary>
    /// Compares a collection of CSDL elements (expected values) against an IEdmModel (actual result).
    /// </summary>
    public static class CsdlToEdmModelComparer
    {
        /// <summary>
        /// Compares an IEdmModel against the expected CSDL elements.
        /// </summary>
        /// <param name="expectedCsdl">The expected result in CSDL format.</param>
        /// <param name="actualModel">The actual result as an IEdmModel.</param>
        public static void Compare(IEnumerable<XElement> expectedCsdl, IEdmModel actualModel)
        {
            CompareEntityTypes(expectedCsdl, actualModel);
            CompareComplexTypes(expectedCsdl, actualModel);
            CompareEnumTypes(expectedCsdl, actualModel);
            CompareActionsAndFunctions(expectedCsdl, actualModel);
            CompareEntityContainers(expectedCsdl, actualModel);
        }

        /// <summary>
        /// Compares EnumType elements from CSDL to IEdmEnumTypes in the model.
        /// </summary>
        /// <param name="schemaElements">The CSDL schema elements to extract EnumType elements from.</param>
        /// <param name="model">The Edm model to extract the IEdmEnumTypes from.</param>
        private static void CompareEnumTypes(IEnumerable<XElement> schemaElements, IEdmModel model)
        {
            var typeIndex = BuildNamedElementIndex(schemaElements, "EnumType");
            var enumModelTypes = model.SchemaElements.OfType<IEdmEnumType>().ToArray();
            ExceptionUtilities.Assert(typeIndex.Count == enumModelTypes.Count(), "Unexpected number of enum types in model");

            foreach (var enumType in typeIndex)
            {
                var modelType = enumModelTypes.SingleOrDefault(t => t.FullName() == enumType.Key);
                ExceptionUtilities.CheckObjectNotNull(modelType, "Failed to find enum type " + enumType.Key);

                var enumTypeElement = enumType.Value;

                CompareStringAttribute(enumTypeElement, "UnderlyingType", modelType.UnderlyingType.FullName(), "Edm.Int32");
                CompareBooleanAttribute(enumTypeElement, "IsFlags", modelType.IsFlags, false);

                var enumMemberElements = enumTypeElement.EdmElements("Member").ToArray();
                ExceptionUtilities.Assert(enumMemberElements.Count() == modelType.Members.Count(), "Unexpected number of enum members");

                long memberCount = 0;
                foreach (var enumMemberElement in enumMemberElements)
                {
                    var memberName = enumMemberElement.GetAttributeValue("Name");
                    var modelMember = modelType.Members.SingleOrDefault(m => m.Name == memberName);
                    ExceptionUtilities.CheckObjectNotNull(modelMember, "Failed to find enum member " + memberName);

                    // Member value defaults to the position in the list of members
                    var modelMemberValue = modelMember.Value;
                    CompareLongAttribute(enumMemberElement, "Value", modelMemberValue.Value, memberCount++);
                }
            }
        }

        /// <summary>
        /// Compares EntityType elements from CSDL to IEdmEntityTypes in the model.
        /// </summary>
        /// <param name="schemaElements">The CSDL schema elements to extract EntityType elements from.</param>
        /// <param name="model">The Edm model to extract the IEdmEntityTypes from.</param>
        private static void CompareEntityTypes(IEnumerable<XElement> schemaElements, IEdmModel model)
        {
            // Index all types in the CSDL by full name to facilitate derived type verification
            var typeIndex = BuildNamedElementIndex(schemaElements, "EntityType");
            ExceptionUtilities.Assert(typeIndex.Count == model.EntityTypes().Count(), "Unexpected type count in model");

            foreach (var entityType in typeIndex)
            {
                var modelType = model.EntityTypes().SingleOrDefault(t => t.FullName() == entityType.Key);
                ExceptionUtilities.Assert(modelType != null, "Failed to find entity type " + entityType.Key);
                CompareEntityTypeProperties(entityType.Value, modelType, typeIndex);
            }
        }

        /// <summary
        /// Compares ComplexType elements from CSDL to IEdmComplexTypes in the model.
        /// </summary>
        /// <param name="schemaElements">The CSDL schema elements to extract ComplexType elements from.</param>
        /// <param name="model">The Edm model to extract the IEdmComplexTypes from.</param>
        private static void CompareComplexTypes(IEnumerable<XElement> schemaElements, IEdmModel model)
        {
            // Index all types in the CSDL by full name to facilitate derived type verification
            var typeIndex = BuildNamedElementIndex(schemaElements, "ComplexType");
            ExceptionUtilities.Assert(typeIndex.Count == model.ComplexTypes().Count(), "Unexpected type count in model");

            foreach (var complexType in typeIndex)
            {
                var modelType = model.ComplexTypes().SingleOrDefault(t => t.FullName() == complexType.Key);
                ExceptionUtilities.Assert(modelType != null, "Failed to find complex type " + complexType.Key);

                var complexTypeElement = complexType.Value;
                CompareBaseType(complexTypeElement, modelType);

                var propertyElements = RecurseBaseTypes(complexTypeElement, typeIndex, (e) => e.EdmElements("Property"));
                CompareStructuralProperties(propertyElements, modelType);

                var navigationPropertyElements = RecurseBaseTypes(complexTypeElement, typeIndex, (e) => e.EdmElements("NavigationProperty"));
                CompareNavigationProperties(navigationPropertyElements, modelType);
            }
        }

        /// <summary
        /// Compares Action and Function elements from CSDL to IEdmComplexTypes in the model.
        /// </summary>
        /// <param name="schemaElements">The CSDL schema elements to extract ComplexType elements from.</param>
        /// <param name="model">The Edm model to extract the IEdmComplexTypes from.</param>
        private static void CompareActionsAndFunctions(IEnumerable<XElement> schemaElements, IEdmModel model)
        {
            foreach (var schemaElement in schemaElements)
            {
                var namespaceName = schemaElement.GetAttributeValue("Namespace");
                var operationElements = schemaElement.EdmElements("Action").Concat(schemaElement.EdmElements("Function"));

                foreach (var operationElement in operationElements)
                {
                    // Find matching operation from model, matching and validating
                    // parameters at the same time.
                    var parameterElements = operationElement.EdmElements("Parameter").ToList();
                    var operationName = operationElement.GetAttributeValue("Name");

                    var possibleMatches = model.FindDeclaredOperations(namespaceName + "." + operationName).ToArray();
                    ExceptionUtilities.Assert(possibleMatches.Any(), "Failed to find action or function import " + operationName);

                    IEdmOperation operation = null;
                    foreach (var possibleMatch in possibleMatches)
                    {
                        try
                        {
                            CompareOperationParameters(parameterElements, possibleMatch.Parameters);
                            operation = possibleMatch;
                            break;
                        }
                        catch (InvalidOperationException)
                        {
                            // Swallow exception for failed comparison
                        }
                    }

                    ExceptionUtilities.Assert(operation != null, "Failed to find a matching operation for " + operationName);

                    string returnTypeValue;
                    if (operationElement.TryGetAttributeValue("ReturnType", out returnTypeValue))
                    {
                        ExceptionUtilities.CheckObjectNotNull(operation.ReturnType, "Expected a return type for operation");
                        CompareTypeValue(returnTypeValue, operation.ReturnType);
                    }
                    else
                    {
                        var returnTypeElement = operationElement.EdmElements("ReturnType").SingleOrDefault();
                        if (returnTypeElement != null)
                        {
                            CompareType(returnTypeElement, operation.ReturnType);
                            // TODO: Enable this.
                            // CompareTypeFacets(propertyElement, propertyOnModel.Type);
                        }
                        else
                        {
                            ExceptionUtilities.Assert(operation.ReturnType == null, "Did not expect a return type for operation");
                        }
                    }

                    CompareBooleanAttribute(operationElement, "IsBound", operation.IsBound, false);

                    string entitySetPathValue;
                    if (operationElement.TryGetAttributeValue("EntitySetPath", out entitySetPathValue))
                    {
                        ExceptionUtilities.Assert(operation.EntitySetPath != null, "Expected a value for EntitySetPath");
                        CompareEntitySetPaths(entitySetPathValue, operation.EntitySetPath);
                    }
                    else
                    {
                        ExceptionUtilities.Assert(operation.EntitySetPath == null, "Did not expect a value for EntitySetPath");
                    }

                    bool isFunction = operationElement.Name.LocalName == "Function";
                    if (isFunction)
                    {
                        // Function specific verification
                        ExceptionUtilities.Assert(operation is IEdmFunction, "Model operation is not a function");

                        // TODO: Enable this.
                        // CompareBooleanAttribute(operationElement, "IsComposable", operation.IsComposable, false);
                    }
                    else
                    {
                        // Action specific verification
                        ExceptionUtilities.Assert(operation is IEdmAction, "Model operation is not an action");
                    }
                }
            }
        }

        /// <summary>
        /// Compares the properties (key, structural, navigation) of an EntityType element to those in the Edm model.
        /// </summary>
        /// <param name="typeElement">The EntityType element to compare to the model.</param>
        /// <param name="entityType">The corresponding EdmEntityType from the model.</param>
        /// <param name="typeIndex">All entity types from the model, indexed by qualified name.</param>
        private static void CompareEntityTypeProperties(XElement typeElement, IEdmEntityType entityType, IDictionary<string, XElement> typeIndex) 
        {
            string fullTypeName = entityType.FullName();
            CompareBaseType(typeElement, entityType);

            Func<XElement, IEnumerable<string>> getKeyPropertyNames = 
                (element) =>
                {
                    var keyElement = element.EdmElements("Key").SingleOrDefault();
                    return keyElement == null ? Enumerable.Empty<string>() : keyElement.EdmElements("PropertyRef").Select(e => e.GetAttributeValue("Name"));
                };

            // Collect all key properties from the type hierachy and compare
            var keyPropertyNames = RecurseBaseTypes(typeElement, typeIndex, getKeyPropertyNames).ToArray();

            var keyPropertiesOnModel = entityType.Key();
            ExceptionUtilities.Assert(keyPropertyNames.Count() == keyPropertiesOnModel.Count(), "Unexpected number of key properties on type " + fullTypeName);

            var missingKeyProperties = keyPropertyNames.Except(keyPropertiesOnModel.Select(p => p.Name)).ToArray();
            ExceptionUtilities.Assert(!missingKeyProperties.Any(), "Failed to find the key properties " + string.Join(",", missingKeyProperties) + " on type " + fullTypeName);

            // Collect all structural properties from the type hierachy and compare
            var propertyElements = RecurseBaseTypes(typeElement, typeIndex, (e) => e.EdmElements("Property"));
            CompareStructuralProperties(propertyElements, entityType);

            // Collect all navigation properties from the type hierachy and compare
            var navigationPropertyElements = RecurseBaseTypes(typeElement, typeIndex, (e) => e.EdmElements("NavigationProperty"));
            CompareNavigationProperties(navigationPropertyElements, entityType);
        }

        private static void CompareBaseType(XElement typeElement, IEdmStructuredType modelType)
        {
            var baseTypeAttribute = typeElement.Attribute("BaseType");

            if (baseTypeAttribute != null)
            {
                ExceptionUtilities.Assert(modelType.BaseType != null, "Expected base type for type " + modelType.TestFullName());
                CompareTypeValue(baseTypeAttribute.Value, modelType.BaseType.ToTypeReference());
            }
            else
            {
                ExceptionUtilities.Assert(modelType.BaseType == null, "Did not expect base type for type " + modelType.TestFullName());
            }
        }

        /// <summary>
        /// Recurses through the type hierachy expressed by the CSDL elements, and applies the selectFunction delegate
        /// to generate a collection of Ts per element.
        /// </summary>
        /// <typeparam name="T">The collection element type that will be generated from each traversed type.</typeparam>
        /// <param name="typeElement">The CSDL element representing the starting point of the type traversal.</param>
        /// <param name="typeIndex">All entity types from the model, indexed by qualified name.</param>
        /// <param name="selectFunction">A delegate that is applied to each type, generating a collection of T</param>
        /// <returns>A collection of type T generated from the type hierachy.</returns>
        private static IEnumerable<T> RecurseBaseTypes<T>(XElement typeElement, IDictionary<string, XElement> typeIndex, Func<XElement, IEnumerable<T>> selectFunction)
        {
            return RecurseBaseTypes(typeElement, typeIndex, selectFunction, new List<XElement>());
        }

        private static IEnumerable<T> RecurseBaseTypes<T>(XElement typeElement, IDictionary<string, XElement> typeIndex, Func<XElement, IEnumerable<T>> selectFunction, ICollection<XElement> seenElements)
        {
            string baseTypeValue = string.Empty;
            if (typeElement.TryGetAttributeValue("BaseType", out baseTypeValue))
            {
                ExceptionUtilities.Assert(typeIndex.ContainsKey(baseTypeValue), "Failed to find base type " + baseTypeValue);
                var baseTypeElement = typeIndex[baseTypeValue];
                if (seenElements.Contains(baseTypeElement))
                {
                    // Detect cyclical hierachies
                    return selectFunction(baseTypeElement);
                }

                seenElements.Add(baseTypeElement);
                return RecurseBaseTypes(baseTypeElement, typeIndex, selectFunction, seenElements).Concat(selectFunction(typeElement));
            }

            return selectFunction(typeElement);
        }

        /// <summary>
        /// Compares two sets of structural properties (expected value in CSDL, actual as IEdmStructuredType).
        /// This can apply to both entity types and complex types.
        /// </summary>
        /// <param name="propertyElements">The CSDL element representing the properties.</param>
        /// <param name="modelType">The EDM model type to compare against.</param>
        private static void CompareStructuralProperties(IEnumerable<XElement> propertyElements, IEdmStructuredType modelType)
        {
            ExceptionUtilities.Assert(propertyElements.Count() == modelType.StructuralProperties().Count(), "Unexpected number of properties on type " + modelType.TestFullName());
            foreach (var propertyElement in propertyElements)
            {
                var propertyName = propertyElement.GetAttributeValue("Name");
                var propertyOnModel = modelType.FindProperty(propertyName) as IEdmStructuralProperty;
                ExceptionUtilities.Assert(propertyOnModel != null, "Failed to find structural property " + propertyName + " on type " + modelType.TestFullName());
                CompareType(propertyElement, propertyOnModel.Type);
                // TODO: Enable this.
                // CompareTypeFacets(propertyElement, propertyOnModel.Type);

                string defaultValueString;
                if (propertyElement.TryGetAttributeValue("DefaultValue", out defaultValueString))
                {
                    ExceptionUtilities.Assert(string.Compare(defaultValueString, propertyOnModel.DefaultValueString) == 0, "Unexpected value for DefaultValue"); 
                }
                else
                {
                    ExceptionUtilities.Assert(string.IsNullOrEmpty(propertyOnModel.DefaultValueString), "Did not expect a value for DefaultValue property");
                }
            }
        }

        /// <summary>
        /// Compares two sets of navigation properties (expected value in CSDL, actual as IEdmStructuredType).
        /// </summary>
        /// <param name="navigationPropertyElements">The CSDL element representing the properties.</param>
        /// <param name="structuredType">The EDM model type to compare against.</param>
        private static void CompareNavigationProperties(IEnumerable<XElement> navigationPropertyElements, IEdmStructuredType structuredType)
        {
            string structuredTypeName = structuredType.TestFullName();
            var navigationProperties = structuredType.Properties().OfType<IEdmNavigationProperty>().ToArray();
            ExceptionUtilities.Assert(navigationPropertyElements.Count() == navigationProperties.Count(), "Unexpected number of navigation properties on type " + structuredTypeName);
            foreach (var navigationPropertyElement in navigationPropertyElements)
            {
                var propertyName = navigationPropertyElement.GetAttributeValue("Name");
                var navigationProperty = navigationProperties.SingleOrDefault(np => np.Name == propertyName);
                ExceptionUtilities.Assert(navigationProperty != null, "Failed to find navigation property " + propertyName + " on type " + structuredTypeName);

                CompareType(navigationPropertyElement, navigationProperty.Type);

                // Compare NavigationProperty Partner, if present
                var partner = navigationPropertyElement.Attribute("Partner");
                if (partner == null)
                {
                    ExceptionUtilities.Assert(navigationProperty.Partner == null, "Did not expect a Partner value for navigation property " + propertyName + " on type " + structuredTypeName);
                }
                else
                {
                    ExceptionUtilities.Assert(navigationProperty.Partner != null, "Expected a Partner value for navigation property " + propertyName + " on type " + structuredTypeName);
                    ExceptionUtilities.Assert(partner.Value == navigationProperty.Partner.Name, "Unexpected value for navigation property partner");
                }

                // Compare any referential constraints
                var referentialConstraintElements = navigationPropertyElement.EdmElements("ReferentialConstraint").ToArray();
                if (referentialConstraintElements.Any())
                {
                    ExceptionUtilities.Assert(navigationProperty.ReferentialConstraint != null, "Expected a referential constraint to be present");
                    ExceptionUtilities.Assert(referentialConstraintElements.Count() == navigationProperty.ReferentialConstraint.PropertyPairs.Count(), "Unexpected number of referential constraint pairs");
                    foreach (var referentialConstraintElement in referentialConstraintElements)
                    {
                        var principalProperty = referentialConstraintElement.GetAttributeValue("ReferencedProperty");
                        var referencedProperty = referentialConstraintElement.GetAttributeValue("Property");
                        var constraintPair = navigationProperty.ReferentialConstraint.PropertyPairs
                            .SingleOrDefault(p => p.PrincipalProperty.Name == principalProperty && p.DependentProperty.Name == referencedProperty);

                        ExceptionUtilities.Assert(constraintPair != null, "Failed to find constraint pair (" + principalProperty + ", " + referencedProperty + ")");
                    }
                }
                else
                {
                    ExceptionUtilities.Assert(navigationProperty.ReferentialConstraint == null, "Did not expect any referential constraints");
                }

                // Compare OnDelete, if present
                var onDeleteElement = navigationPropertyElement.EdmElements("OnDelete").SingleOrDefault();
                if (onDeleteElement != null)
                {
                    EdmOnDeleteAction expectedAction;
                    EdmOnDeleteAction.TryParse(onDeleteElement.GetAttributeValue("Action"), out expectedAction);
                    ExceptionUtilities.Assert(expectedAction == navigationProperty.OnDelete, "Unexpected value for OnDelete");
                }
                else
                {
                    ExceptionUtilities.Assert(EdmOnDeleteAction.None == navigationProperty.OnDelete, "Unexpected value for OnDelete");
                }
            }
            
        }

        /// <summary>
        /// Compare EntityContainer elements from CSDL against those in an Edm model.
        /// </summary>
        /// <param name="schemaElements">The CSDL schema elements containing the EntityContainer elements.</param>
        /// <param name="model">The Edm model to compare against.</param>
        private static void CompareEntityContainers(IEnumerable<XElement> schemaElements, IEdmModel model)
        {
            // Index by qualified name to facilitate lookups for extended containers
            var containerIndex = BuildNamedElementIndex(schemaElements, "EntityContainer");

            var modelContainers = model.SchemaElements.OfType<IEdmEntityContainer>();
            ExceptionUtilities.Assert(containerIndex.Count() == modelContainers.Count(), "Unexpected number of entity containers");

            foreach (var containerIndice in containerIndex)
            {
                var containerElement = containerIndice.Value;
                string containerName = containerElement.GetAttributeValue("Name");
                var modelContainer = modelContainers.SingleOrDefault(c => c.Name == containerName);
                ExceptionUtilities.Assert(modelContainer != null, "Failed to find container " + containerName);

                var containerElements = new List<XElement> {containerElement};

                string extendsContainer;
                var extendingContainerElement = containerElement;
                while(extendingContainerElement.TryGetAttributeValue("Extends", out extendsContainer))
                {
                    ExceptionUtilities.Assert(containerIndex.ContainsKey(extendsContainer), "Failed to find extended entity container " + extendsContainer);
                    extendingContainerElement = containerIndex[extendsContainer];
                    containerElements.Add(extendingContainerElement);

                    // TODO: remove this after container extension is supported.
                    break;
                }

                CompareEntitySets(containerElements, modelContainer, model.EntityTypes());
                CompareActionAndFunctionImports(containerElements, modelContainer, model);
            }
        }

        /// <summary>
        /// Compare EntitySet elements from CSDL against those in an Edm model.
        /// </summary>
        /// <param name="containerElements">The EntityContainer elements containing the EntitySet elements.</param>
        /// <param name="container">The EdmContainer from the model to compare against.</param>
        /// <param name="entityTypes">All entity types from the model, for verifying navigation property bindings.</param>
        private static void CompareEntitySets(IEnumerable<XElement> containerElements, IEdmEntityContainer container, IEnumerable<IEdmEntityType> entityTypes)
        {
            var entitySetElements = containerElements.SelectMany(e => e.EdmElements("EntitySet")).ToArray();
            var entitySets = container.EntitySets().ToArray();
            ExceptionUtilities.Assert(entitySetElements.Count() == entitySets.Count(), "Unexpected number of entity sets");

            foreach (var entitySetElement in entitySetElements)
            {
                var entitySetName = entitySetElement.GetAttributeValue("Name");
                var entitySet = entitySets.SingleOrDefault(e => e.Name == entitySetName);
                ExceptionUtilities.Assert(entitySet != null, "Failed to find entity set " + entitySetName);
                ExceptionUtilities.Assert(entitySetElement.GetAttributeValue("EntityType") == entitySet.EntityType().FullName(), "Unexpected type for entity set " + entitySetName);

                // Compare Navigation Property Bindings
                var navigationPropertyBindingElements = entitySetElement.EdmElements("NavigationPropertyBinding").ToArray();
                ExceptionUtilities.Assert(navigationPropertyBindingElements.Count() == entitySet.NavigationPropertyBindings.Count(), "Unexpected number of navigation targets");
                foreach (var navigationPropertyBindingElement in navigationPropertyBindingElements)
                {
                    var pathValue = navigationPropertyBindingElement.GetAttributeValue("Path");
                    var targetValue = navigationPropertyBindingElement.GetAttributeValue("Target");

                    string propertyName = pathValue;

                    if (pathValue.Contains("/"))
                    {
                        // Path contains a derived type - split it to determine the property name and verify the
                        // type is derived from the entity set type, in the model.
                        var splitPath = pathValue.Split(new[] {'/'}, StringSplitOptions.None);
                        ExceptionUtilities.Assert(2 == splitPath.Count(), "More than two forward slash characters in nav property binding path");

                        var derivedTypeName = splitPath.ElementAt(0);
                        propertyName = splitPath.ElementAt(1);

                        var derivedType = entityTypes.SingleOrDefault(t => t.FullName() == splitPath.ElementAt(0));
                        ExceptionUtilities.Assert(derivedType != null, "Failed to find entity type " + derivedTypeName + " described in navigation property binding path");
                        ExceptionUtilities.Assert(derivedType.DeclaredNavigationProperties().SingleOrDefault(p => p.Name == propertyName) != null, "Failed to find nav property on derived type");
                        EdmModelUtils.AssertEntityTypeIsDerivedFrom(derivedType, entitySet.EntityType());
                    }

                    var navigationTarget = entitySet.NavigationPropertyBindings.SingleOrDefault(nt => nt.NavigationProperty.Name == propertyName && nt.Target.Name == targetValue);
                    ExceptionUtilities.Assert(navigationTarget != null, "Failed to find navigation target for element " + navigationPropertyBindingElement);
                }
            }
        }

        /// <summary>
        /// Compare ActionImport and FunctionImport CSDL elements against those in the Edm model.
        /// </summary>
        /// <param name="containerElements">The CSDL EntityContainers containing the ActionImport and FunctionImport elements.</param>
        /// <param name="container">The Edm model EntityContainer to compare against.</param>
        /// <param name="model">The Edm model .</param>
        private static void CompareActionAndFunctionImports(IEnumerable<XElement> containerElements, IEdmEntityContainer container, IEdmModel model)
        {
            var actionImportElements = containerElements.SelectMany(e => e.EdmElements("ActionImport"));
            var functionImportElements = containerElements.SelectMany(e => e.EdmElements("FunctionImport"));
            var operationImportElements = actionImportElements.Concat(functionImportElements);

            var operationImports = container.OperationImports().ToArray();
            ExceptionUtilities.Assert(operationImportElements.Count() == operationImports.Count(), "Unexpected number of action and function imports");

            foreach (var operationImportElement in operationImportElements)
            {
                var operationImportName = operationImportElement.GetAttributeValue("Name");

                var operationImport = operationImports.SingleOrDefault(o => o.Name == operationImportName);
                ExceptionUtilities.CheckObjectNotNull(operationImport, "Failed to find operation import " + operationImportName);

                bool isActionImport = operationImportElement.Name.LocalName == "ActionImport";
                if (isActionImport)
                {
                    ExceptionUtilities.Assert(operationImport is IEdmActionImport, "Unexpected type for action import");
                }
                else
                {
                    ExceptionUtilities.Assert(operationImport is IEdmFunctionImport, "Unexpected type for function import");
                    var functionImport = (IEdmFunctionImport)operationImport;

                    // TODO: enable this when it is enabled in product
                    // CompareBooleanAttribute(operationImportElement, "IncludeInServiceDocument", functionImport.IncludeInServiceDocument, false);
                }

                string operationName = isActionImport ? operationImportElement.GetAttributeValue("Action") : operationImportElement.GetAttributeValue("Function");
                var operation = model.FindDeclaredOperations(operationName).FirstOrDefault();
                ExceptionUtilities.Assert(operation != null, "Failed to find operation " + operationName + " in model.");
                ExceptionUtilities.Assert(!operation.IsBound, "Operation Import cannot reference a bound operation");

                string entitySetValue;
                if (operationImportElement.TryGetAttributeValue("EntitySet", out entitySetValue))
                {
                    ExceptionUtilities.Assert(operationImport.EntitySet != null, "Did not expect EntitySet property to be null");
                    CompareEntitySetPaths(entitySetValue, operationImport.EntitySet);
                }
                else
                {
                    ExceptionUtilities.Assert(operationImport.EntitySet == null, "Did not expect an EntitySet value");
                }
            }
        }

        /// <summary>
        /// Compares FunctionImportParameter CSDL elements against those in an Edm model.
        /// </summary>
        /// <param name="parameterElements">The set of CSDL FunctionImportParameter elements.</param>
        /// <param name="parameters">The set of function parameters from the model to compare against.</param>
        private static void CompareOperationParameters(IEnumerable<XElement> parameterElements, IEnumerable<IEdmOperationParameter> parameters)
        {
            ExceptionUtilities.Assert(parameterElements.Count() == parameters.Count(), "Unexpected number of parameters");

            for (int i = 0; i < parameterElements.Count(); i++)
            {
                var parameterElement = parameterElements.ElementAt(i);
                var parameter = parameters.ElementAt(i);

                ExceptionUtilities.Assert(parameterElement.GetAttributeValue("Name") == parameter.Name, "Unexpected parameter name");
                CompareType(parameterElement, parameter.Type);
                // TODO: Enable this.
                // CompareTypeFacets(parameterElement, parameter.Type);
            }
        }

        /// <summary>
        /// Compares type information from a CSDL element against the actual type reference in the Edm model.
        /// </summary>
        /// <param name="element">CSDL element containing type information.</param>
        /// <param name="typeReference">The Edm model type reference to compare against.</param>
        private static void CompareType(XElement element, IEdmTypeReference typeReference)
        {
            var collectionTypeElement = element.EdmElements("CollectionType").SingleOrDefault();
            if (collectionTypeElement != null)
            {
                ExceptionUtilities.Assert(typeReference.IsCollection(), "Expected a collection type reference");
                ExceptionUtilities.Assert(!typeReference.IsNullable, "Collection type references cannot be nullable");
                var typeRefElement = collectionTypeElement.EdmElements("TypeRef").Single();
                CompareType(typeRefElement, ((IEdmCollectionTypeReference)typeReference).ElementType());
                return;
            }

            string typeName = element.GetAttributeValue("Type");
            CompareTypeValue(typeName, typeReference);

            var nullableAttribute = element.Attribute("Nullable");
            bool expectedNullable = (nullableAttribute == null) || bool.Parse(nullableAttribute.Value);
            ExceptionUtilities.Assert(expectedNullable == typeReference.IsNullable, "Unexpected nullability for element");
        }

        /// <summary>
        /// Compares the type name value against the model type reference, taking into account collection
        /// types and qualified/non-qualified type names.
        /// </summary>
        /// <param name="typeName">The CSDL type value.</param>
        /// <param name="typeReference">The Edm model type reference to compare against.</param>
        private static void CompareTypeValue(string typeName, IEdmTypeReference typeReference)
        {
            bool isCollectionType = typeName.StartsWith(EdmModelUtils.CollectionTypeNamePrefix);
            if (isCollectionType)
            {
                ExceptionUtilities.Assert(typeReference.IsCollection(), "Expected collection type");
                string elementTypeName = EdmModelUtils.GetCollectionItemTypeName(typeName);
                CompareTypeValue(elementTypeName, typeReference.GetCollectionItemType());
            }
            else
            {
                bool expectedTypeNameContainsNamespace = typeName.Contains(".");
                if (expectedTypeNameContainsNamespace)
                {
                    ExceptionUtilities.Assert(typeName == typeReference.FullName(), "Unexpected type");
                }
                else
                {
                    ExceptionUtilities.Assert("Edm." + typeName == typeReference.FullName(), "Unexpected type");
                }
            }
        }

        private static void CompareTypeFacets(XElement typeElement, IEdmTypeReference typeReference)
        {
            var stringTypeReference = typeReference as IEdmStringTypeReference;
            if (stringTypeReference != null)
            {
                CompareIntegerAttribute(typeElement, "MaxLength", stringTypeReference.MaxLength, null);
                CompareBooleanAttribute(typeElement, "Unicode", stringTypeReference.IsUnicode, true);
                return;
            }

            var binaryTypeReference = typeReference as IEdmBinaryTypeReference;
            if (binaryTypeReference != null)
            {
                CompareIntegerAttribute(typeElement, "MaxLength", binaryTypeReference.MaxLength, null);
                return;
            }

            var temporalTypeReference = typeReference as IEdmTemporalTypeReference;
            if (temporalTypeReference != null)
            {
                CompareIntegerAttribute(typeElement, "Precision", temporalTypeReference.Precision, 0);
                return;
            }

            var decimalTypeReference = typeReference as IEdmDecimalTypeReference;
            if (decimalTypeReference != null)
            {
                CompareIntegerAttribute(typeElement, "Precision", decimalTypeReference.Precision, null);
                CompareIntegerAttribute(typeElement, "Scale", decimalTypeReference.Scale, 0);
                return;
            }

            var spatialTypeReference = typeReference as IEdmSpatialTypeReference;
            if (spatialTypeReference != null)
            {
                CompareIntegerAttribute(typeElement, "SRID", spatialTypeReference.SpatialReferenceIdentifier, null);
                return;
            }
        }

        private static void CompareBooleanAttribute(XElement element, string attributeName, bool? modelValue, bool? defaultValue)
        {
            CompareAttribute(element, attributeName, (a) => bool.Parse(a), (a,b) => a == b, modelValue, defaultValue);
        }

        private static void CompareIntegerAttribute(XElement element, string attributeName, int? modelValue, int? defaultValue)
        {
            CompareAttribute(element, attributeName, (a) => int.Parse(a), (a,b) => a == b, modelValue, defaultValue);
        }

        private static void CompareLongAttribute(XElement element, string attributeName, long? modelValue, long? defaultValue)
        {
            CompareAttribute(element, attributeName, (a) => int.Parse(a), (a,b) => a == b, modelValue, defaultValue);
        }

        private static void CompareStringAttribute(XElement element, string attributeName, string modelValue, string defaultValue)
        {
            CompareAttribute(element, attributeName, (a) => a, (a,b) => string.Compare(a, b) == 0, modelValue, defaultValue);
        }

        private static void CompareAttribute<T>(XElement element, string attributeName, Func<string, T> parser, Func<T, T, bool> comparer, T modelValue, T defaultValue)
        {
            T attributeValue = defaultValue;
            string attributeValueString;
            if (element.TryGetAttributeValue(attributeName, out attributeValueString))
            {
                attributeValue = parser(attributeValueString);
            }

            ExceptionUtilities.Assert(comparer(attributeValue, modelValue), "Unexpected value for attribute " + attributeName);
        }

        private static void CompareEntitySetPaths(string entitySetPathValue, IEdmExpression entitySetExpression)
        {
            var modelPath = entitySetExpression as EdmPathExpression;
            ExceptionUtilities.CheckObjectNotNull(modelPath, "Expected a Path expression");
            ExceptionUtilities.Assert(entitySetPathValue == string.Join("/", modelPath.PathSegments), "Unexpected value for path expression");
        }

        private static Dictionary<string, XElement> BuildNamedElementIndex(IEnumerable<XElement> csdlElements, string elementName)
        {
            return csdlElements.SelectMany(s => s.EdmElements(elementName)
                    .Select(e => new {Namespace = s.GetAttributeValue("Namespace"), Type = e}))
                .ToDictionary(t => t.Namespace + "." + t.Type.GetAttributeValue("Name"), t => t.Type);
        }

    }
}
