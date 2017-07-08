//---------------------------------------------------------------------
// <copyright file="ValidationHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Validation
{
    internal static class ValidationHelper
    {
        internal static bool IsEdmSystemNamespace(string namespaceName)
        {
            return (namespaceName == EdmConstants.TransientNamespace ||
                    namespaceName == EdmConstants.EdmNamespace);
        }

        internal static bool AddMemberNameToHashSet(IEdmNamedElement item, HashSetInternal<string> memberNameList, ValidationContext context, EdmErrorCode errorCode, string errorString, bool suppressError)
        {
            IEdmSchemaElement schemaElement = item as IEdmSchemaElement;
            string name = (schemaElement != null) ? schemaElement.FullName() : item.Name;
            if (!memberNameList.Add(name))
            {
                if (!suppressError)
                {
                    context.AddError(item.Location(), errorCode, errorString);
                }

                return false;
            }

            return true;
        }

        internal static bool AllPropertiesAreNullable(IEnumerable<IEdmStructuralProperty> properties)
        {
            return properties.Where(p => !p.Type.IsNullable).Count() == 0;
        }

        internal static bool HasNullableProperty(IEnumerable<IEdmStructuralProperty> properties)
        {
            return properties.Where(p => p.Type.IsNullable).Count() > 0;
        }

        internal static bool PropertySetIsSubset(IEnumerable<IEdmStructuralProperty> set, IEnumerable<IEdmStructuralProperty> subset)
        {
            return subset.Except(set).Count() <= 0;
        }

        internal static bool PropertySetsAreEquivalent(IEnumerable<IEdmStructuralProperty> set1, IEnumerable<IEdmStructuralProperty> set2)
        {
            if (set1.Count() != set2.Count())
            {
                return false;
            }

            IEnumerator<IEdmStructuralProperty> set2Enum = set2.GetEnumerator();
            foreach (IEdmStructuralProperty prop1 in set1)
            {
                set2Enum.MoveNext();
                if (prop1 != set2Enum.Current)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool ValidateValueCanBeWrittenAsXmlElementAnnotation(IEdmValue value, string annotationNamespace, string annotationName, out EdmError error)
        {
            IEdmStringValue edmStringValue = value as IEdmStringValue;
            if (edmStringValue == null)
            {
                error = new EdmError(value.Location(), EdmErrorCode.InvalidElementAnnotation, Edm.Strings.EdmModel_Validator_Semantic_InvalidElementAnnotationNotIEdmStringValue);
                return false;
            }

            string rawString = edmStringValue.Value;

            XmlReader reader = XmlReader.Create(new StringReader(rawString));

            try
            {
                // Skip to root element.
                if (reader.NodeType != XmlNodeType.Element)
                {
                    while (reader.Read() && reader.NodeType != XmlNodeType.Element)
                    {
                    }
                }

                // The annotation must be an element.
                if (reader.EOF)
                {
                    error = new EdmError(value.Location(), EdmErrorCode.InvalidElementAnnotation, Edm.Strings.EdmModel_Validator_Semantic_InvalidElementAnnotationValueInvalidXml);
                    return false;
                }

                // The root element must corespond to the term of the annotation
                string elementNamespace = reader.NamespaceURI;
                string elementName = reader.LocalName;

                if (EdmUtil.IsNullOrWhiteSpaceInternal(elementNamespace) || EdmUtil.IsNullOrWhiteSpaceInternal(elementName))
                {
                    error = new EdmError(value.Location(), EdmErrorCode.InvalidElementAnnotation, Edm.Strings.EdmModel_Validator_Semantic_InvalidElementAnnotationNullNamespaceOrName);
                    return false;
                }

                if (!((annotationNamespace == null || elementNamespace == annotationNamespace) && (annotationName == null || elementName == annotationName)))
                {
                    error = new EdmError(value.Location(), EdmErrorCode.InvalidElementAnnotation, Edm.Strings.EdmModel_Validator_Semantic_InvalidElementAnnotationMismatchedTerm);
                    return false;
                }

                // Parse the entire fragment to determine if the XML is valid
                while (reader.Read())
                {
                }

                error = null;
                return true;
            }
            catch (XmlException)
            {
                error = new EdmError(value.Location(), EdmErrorCode.InvalidElementAnnotation, Edm.Strings.EdmModel_Validator_Semantic_InvalidElementAnnotationValueInvalidXml);
                return false;
            }
        }

        internal static bool IsInterfaceCritical(EdmError error)
        {
            return error.ErrorCode >= EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull && error.ErrorCode <= EdmErrorCode.InterfaceCriticalCycleInTypeHierarchy;
        }

        internal static bool ItemExistsInReferencedModel(this IEdmModel model, string fullName, bool checkEntityContainer)
        {
            foreach (IEdmModel referenced in model.ReferencedModels)
            {
                if (referenced.FindDeclaredType(fullName) != null ||
                    referenced.FindDeclaredTerm(fullName) != null ||
                    (checkEntityContainer && referenced.ExistsContainer(fullName)) ||
                    (referenced.FindDeclaredOperations(fullName) ?? Enumerable.Empty<IEdmOperation>()).FirstOrDefault() != null)
                {
                    return true;
                }
            }

            return false;
        }

        // Take operation name to avoid recomputing it
        internal static bool OperationOrNameExistsInReferencedModel(this IEdmModel model, IEdmOperation operation, string operationFullName)
        {
            foreach (IEdmModel referenced in model.ReferencedModels)
            {
                if (referenced.FindDeclaredType(operationFullName) != null ||
                    referenced.ExistsContainer(operationFullName) ||
                    referenced.FindDeclaredTerm(operationFullName) != null)
                {
                    return true;
                }
                else
                {
                    IEnumerable<IEdmOperation> operationList = referenced.FindDeclaredOperations(operationFullName) ?? Enumerable.Empty<IEdmOperation>();
                    if (DuplicateOperationValidator.IsDuplicateOperation(operation, operationList))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal static bool TypeIndirectlyContainsTarget(IEdmEntityType source, IEdmEntityType target, HashSetInternal<IEdmEntityType> visited, IEdmModel context)
        {
            if (visited.Add(source))
            {
                if (source.IsOrInheritsFrom(target))
                {
                    return true;
                }

                foreach (IEdmNavigationProperty navProp in source.NavigationProperties())
                {
                    if (navProp.ContainsTarget && TypeIndirectlyContainsTarget(navProp.ToEntityType(), target, visited, context))
                    {
                        return true;
                    }
                }

                foreach (IEdmStructuredType derived in context.FindAllDerivedTypes(source))
                {
                    IEdmEntityType derivedEntity = derived as IEdmEntityType;
                    if (derivedEntity != null && TypeIndirectlyContainsTarget(derivedEntity, target, visited, context))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal static IEdmEntityType ComputeNavigationPropertyTarget(IEdmNavigationProperty property)
        {
            Debug.Assert(property != null, "Navigation property can't be null");
            IEdmType target = property.Type.Definition;
            if (target.TypeKind == EdmTypeKind.Collection)
            {
                target = ((IEdmCollectionType)target).ElementType.Definition;
            }

            return (IEdmEntityType)target;
        }
    }
}
