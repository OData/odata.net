//---------------------------------------------------------------------
// <copyright file="TargetPathHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm.Vocabularies;

    /// <summary>
    /// Helper for <see cref="EdmTargetPath"/> class.
    /// </summary>
    internal static class TargetPathHelper
    {
        /// <summary>
        /// Returns the segments of a target path string as a collection of Edm elements.
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="targetSegments">Segments for the target path string.</param>
        /// <param name="ignoreCase">Property name case-insensitive or not.</param>
        /// <returns>The collection of <see cref="IEdmElement"/>.</returns>
        public static IEnumerable<IEdmElement> GetTargetSegments(this IEdmModel model, string[] targetSegments, bool ignoreCase)
        {
            EdmUtil.CheckArgumentNull(model, nameof(model));
            EdmUtil.CheckArgumentNull(targetSegments, nameof(targetSegments));

            IEdmEntityContainer container = model.FindEntityContainer(targetSegments[0]);

            if (container == null)
            {
                return Enumerable.Empty<IEdmElement>();
            }

            List<IEdmElement> pathSegments = new List<IEdmElement>(targetSegments.Length);
            pathSegments.Add(container);
            int index = 1; // index of the targetSegments.

            IEdmVocabularyAnnotatable vocabularyAnnotatable =  HandleEntityContainer(model, pathSegments, container, targetSegments, index, ignoreCase);
            index++;

            while (index < targetSegments.Length)
            {
                if (vocabularyAnnotatable is IEdmEntityContainerElement containerElement)
                {
                    vocabularyAnnotatable = HandleContainerElement(model, pathSegments, containerElement, targetSegments, index, ignoreCase);
                }
                else if (vocabularyAnnotatable is IEdmSchemaType schemaType)
                {
                    vocabularyAnnotatable = HandleSchemaType(model, pathSegments, schemaType, targetSegments, index, ignoreCase);
                }
                else if (vocabularyAnnotatable is IEdmProperty property)
                {
                    vocabularyAnnotatable = HandleProperty(model, pathSegments, property, targetSegments, index, ignoreCase);
                }
                else
                {
                    return Enumerable.Empty<IEdmElement>();
                }

                index++;
            }

            return pathSegments;
        }

        /// <summary>
        /// Returns an entity container element from the specified entity container.
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="pathSegments">The path segments of already resolved target segments.</param>
        /// <param name="entityContainer">The entity container.</param>
        /// <param name="targetSegments">The array of segments in the target path string.</param>
        /// <param name="index">The index of the target segment.</param>
        /// <param name="ignoreCase">Property name case-insensitive or not.</param>
        /// <returns>An Edm entity container element as an <see cref="IEdmVocabularyAnnotatable"/>.</returns>
        private static IEdmVocabularyAnnotatable HandleEntityContainer(IEdmModel model, List<IEdmElement> pathSegments, IEdmEntityContainer entityContainer, string[] targetSegments, int index, bool ignoreCase)
        {
            IEdmEntitySet entitySet = entityContainer.FindEntitySetExtended(targetSegments[index]);

            if (entitySet != null)
            {
                pathSegments.Add(entitySet);

                return entitySet;
            }

            IEdmSingleton singleton = entityContainer.FindSingletonExtended(targetSegments[index]);

            if (singleton != null)
            {
                pathSegments.Add(singleton);

                return singleton;
            }

            return null;
        }

        /// <summary>
        /// Returns a schema type or property from an entity container element.
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="pathSegments">The path segments of already resolved target segments.</param>
        /// <param name="entityContainerElement">The entity container element.</param>
        /// <param name="targetSegments">The array of segments in the target path string.</param>
        /// <param name="index">The index of the target segment.</param>
        /// <param name="ignoreCase">Property name case-insensitive or not.</param>
        /// <returns>An Edm schema type or property element as an <see cref="IEdmVocabularyAnnotatable"/>.</returns>
        private static IEdmVocabularyAnnotatable HandleContainerElement(IEdmModel model, List<IEdmElement> pathSegments, IEdmEntityContainerElement entityContainerElement, string[] targetSegments, int index, bool ignoreCase)
        {
            // .../MyEntitySet/MySchema.MyEntityType/...
            // .../MyEntitySet/MyComplexProperty/...
            // .../MySingleton/MyComplexProperty/...
            IEdmSchemaType schemaType = model.FindType(targetSegments[index]);

            if (schemaType != null)
            {
                ValidateSchemaType(schemaType, pathSegments, index);
                pathSegments.Add(schemaType);

                return schemaType;
            }

            IEdmType edmType = null;

            if (entityContainerElement is IEdmEntitySet edmEntitySet)
            {
                edmType = edmEntitySet.Type.AsElementType();
            }

            if (entityContainerElement is IEdmSingleton edmSingleton)
            {
                edmType = edmSingleton.Type.AsElementType();
            }

            if (edmType is IEdmStructuredType structuredType)
            {
                // .../MyEntitySet/MyProperty/...
                // .../MyEntitySet/MyNavigationProperty/...
                // .../MySingleton/MyProperty/...
                // .../MySingleton/MyNavigationProperty/...

                return HandleStructuredType(model, pathSegments, structuredType, targetSegments, index, ignoreCase);
            }

            return null;
        }

        /// <summary>
        /// Returns a property from a schema type.
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="pathSegments">The path segments of already resolved target segments.</param>
        /// <param name="schemaType">The schema type.</param>
        /// <param name="targetSegments">The array of segments in the target path string.</param>
        /// <param name="index">The index of the target segment.</param>
        /// <param name="ignoreCase">Property name case-insensitive or not.</param>
        /// <returns>An Edm schema type or property element as an <see cref="IEdmVocabularyAnnotatable"/> object.</returns>
        private static IEdmVocabularyAnnotatable HandleSchemaType(IEdmModel model, List<IEdmElement> pathSegments, IEdmSchemaType schemaType, string[] targetSegments, int index, bool ignoreCase)
        {
            // Validate schema type is not followed by schema type e.g entitycontainer/entityset/NS.TypeCase1/NS.TypeCase2/...
            IEdmSchemaType nextSchemaType = model.FindType(targetSegments[index]);

            if (nextSchemaType != null)
            {
                throw new InvalidOperationException(Strings.TypeCast_HierarchyNotFollowed(schemaType, nextSchemaType));
            }

            if (schemaType is IEdmStructuredType structuredType)
            {
                return HandleStructuredType(model, pathSegments, structuredType, targetSegments, index, ignoreCase);
            }

            return null;
        }

        /// <summary>
        /// Validates a schema type.
        /// </summary>
        /// <param name="schemaType">The schema type.</param>
        /// <param name="pathSegments">The path segments of already resolved target segments.</param>
        /// <param name="index">The index of the target segment.</param>
        private static void ValidateSchemaType(IEdmSchemaType schemaType, List<IEdmElement> pathSegments, int index)
        {
            IEdmElement previousElement = pathSegments[index - 1];

            bool isValid = IsTypeCastValid(previousElement, schemaType);

            if (!isValid)
            {
                throw new InvalidOperationException(Strings.TypeCast_HierarchyNotFollowed(previousElement, schemaType));
            }
        }

        private static bool IsTypeCastValid(IEdmElement element, IEdmSchemaType schemaType)
        {
            IEdmType elementEdmType = null;
            if (element is IEdmProperty property)
            {
                elementEdmType = property.Type.Definition;
            }
            else if (element is IEdmSchemaType elementSchemaType)
            {
                elementEdmType = elementSchemaType.AsElementType();
            }
            else if (element is IEdmEntitySet entitySet)
            {
                elementEdmType = entitySet.EntityType.AsElementType();
            }
            else if (element is IEdmSingleton singleton)
            {
                elementEdmType = singleton.EntityType.AsElementType();
            }

            if (elementEdmType != null)
            {
                return schemaType.AsElementType().IsOrInheritsFrom(elementEdmType.AsElementType());
            }

            return false;
        }

        /// <summary>
        /// Returns a schema type or property from a property.
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="pathSegments">The path segments of already resolved target segments.</param>
        /// <param name="edmProperty">The edm property.</param>
        /// <param name="targetSegments">The array of segments in the target path string.</param>
        /// <param name="index">The index of the target segment.</param>
        /// <param name="ignoreCase">Property name case-insensitive or not.</param>
        /// <returns>An Edm schema type or property element as an <see cref="IEdmVocabularyAnnotatable"/> object.</returns>
        private static IEdmVocabularyAnnotatable HandleProperty(IEdmModel model, List<IEdmElement> pathSegments, IEdmProperty edmProperty, string[] targetSegments, int index, bool ignoreCase)
        {
            IEdmSchemaType schemaType = model.FindType(targetSegments[index]);

            if (schemaType != null)
            {
                ValidateSchemaType(schemaType, pathSegments, index);
                pathSegments.Add(schemaType);

                return schemaType;
            }

            IEdmTypeReference typeReference = edmProperty.Type;

            IEdmType edmType = typeReference.Definition.AsElementType();

            if (edmType is IEdmStructuredType structuredType)
            {
                // .../MyProperty1/MyProperty2/...

                return HandleStructuredType(model, pathSegments, structuredType, targetSegments, index, ignoreCase);
            }

            return null;
        }

        /// <summary>
        /// Returns a property from a structured type.
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="pathSegments">The path segments of already resolved target segments.</param>
        /// <param name="structuredType">The structured type.</param>
        /// <param name="targetSegments">The array of segments in the target path string.</param>
        /// <param name="index">The index of the target segment.</param>
        /// <param name="ignoreCase">Property name case-insensitive or not.</param>
        /// <returns>An Edm property element as an <see cref="IEdmVocabularyAnnotatable"/> object.</returns>
        private static IEdmVocabularyAnnotatable HandleStructuredType(IEdmModel model, List<IEdmElement> pathSegments, IEdmStructuredType structuredType, string[] targetSegments, int index, bool ignoreCase)
        {
            IEdmProperty property = structuredType.FindProperty(targetSegments[index], ignoreCase);

            if (property != null)
            {
                pathSegments.Add(property);

                return property;
            }

            return null;
        }
    }
}
