//---------------------------------------------------------------------
// <copyright file="ExtensionMethods.ReadOnlySpan.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Contains extension methods for <see cref="IEdmModel"/> interfaces.
    /// </summary>
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Searches for a type with the given name in this model and all referenced models and returns null if no such type exists.
        /// </summary>
        /// <param name="model">The model to search.</param>
        /// <param name="qualifiedName">The namespace or alias qualified name of the type being found.</param>
        /// <returns>The requested type, or null if no such type exists.</returns>
        public static IEdmSchemaType FindType(this IEdmModel model, ReadOnlySpan<char> qualifiedName)
        {
            EdmUtil.CheckArgumentNull(model, "model");

            ReadOnlySpan<char> fullyQualifiedName = model.ReplaceAlias(qualifiedName);

            // search built-in EdmCoreModel and CoreVocabularyModel.
            return FindAcrossModels(
                model,
                fullyQualifiedName,
                findType,
                (first, second) => RegistrationHelper.CreateAmbiguousTypeBinding(first, second));
        }

        /// <summary>
        /// Searches for bound operations based on the qualified name and binding type, returns an empty enumerable if no operation exists.
        /// </summary>
        /// <param name="model">The model to search.</param>
        /// <param name="qualifiedName">The qualified name of the operation.</param>
        /// <param name="bindingType">Type of the binding.</param>
        /// <returns>A set of operations that share the qualified name and binding type or empty enumerable if no such operation exists.</returns>
        public static IEnumerable<IEdmOperation> FindBoundOperations(this IEdmModel model, ReadOnlySpan<char> qualifiedName, IEdmType bindingType)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(bindingType, "bindingType");

            ReadOnlySpan<char> fullyQualifiedName = model.ReplaceAlias(qualifiedName);

            // the below is a copy of FindAcrossModels method but Func<IEdmModel, TInput, T> finder is replaced by FindDeclaredBoundOperations.
            IEnumerable<IEdmOperation> candidate = model.FindDeclaredBoundOperations(fullyQualifiedName, bindingType);

            foreach (IEdmModel reference in model.ReferencedModels)
            {
                IEnumerable<IEdmOperation> fromReference = reference.FindDeclaredBoundOperations(fullyQualifiedName, bindingType);
                if (fromReference != null)
                {
                    candidate = candidate == null ? fromReference : mergeFunctions(candidate, fromReference);
                }
            }

            return candidate;
        }

        /// <summary>
        /// Searches for a term with the given name in this model and all referenced models and returns null if no such term exists.
        /// </summary>
        /// <param name="model">The model to search.</param>
        /// <param name="qualifiedName">The qualified name of the term being found.</param>
        /// <returns>The requested term, or null if no such term exists.</returns>
        public static IEdmTerm FindTerm(this IEdmModel model, ReadOnlySpan<char> qualifiedName)
        {
            EdmUtil.CheckArgumentNull(model, "model");

            ReadOnlySpan<char> fullyQualifiedName = model.ReplaceAlias(qualifiedName);

            return FindAcrossModels(
                model,
                fullyQualifiedName,
                findTerm,
                (first, second) => RegistrationHelper.CreateAmbiguousTermBinding(first, second));
        }

        /// <summary>
        /// Searches for operations with the given name in this model and all referenced models and returns an empty enumerable if no such operations exist.
        /// </summary>
        /// <param name="model">The model to search.</param>
        /// <param name="qualifiedName">The qualified name of the operations being found.</param>
        /// <returns>The requested operations.</returns>
        public static IEnumerable<IEdmOperation> FindOperations(this IEdmModel model, ReadOnlySpan<char> qualifiedName)
        {
            EdmUtil.CheckArgumentNull(model, "model");

            return FindAcrossModels(model, qualifiedName, findOperations, mergeFunctions);
        }

        /// <summary>
        /// If the container name in the model is the same as the input name. The input name maybe full qualified name.
        /// </summary>
        /// <param name="model">The model to search.</param>
        /// <param name="containerName">Input container name to be searched. The container name may be full qualified with namespace prefix.</param>
        /// <returns>True if the model has a container called input name, otherwise false.</returns>
        public static bool ExistsContainer(this IEdmModel model, ReadOnlySpan<char> containerName)
        {
            if (model.EntityContainer == null)
            {
                return false;
            }

            // if the input is the container name
            if (containerName.Equals(model.EntityContainer.Name, StringComparison.Ordinal))
            {
                return true;
            }

            // if the input is the full container name (namespace.name)
            string fullContainerName = model.EntityContainer.FullName();
            if (containerName.Equals(fullContainerName, StringComparison.Ordinal))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Searches for an entity container with the given name in this model and all referenced models and returns null if no such entity container exists.
        /// </summary>
        /// <param name="model">The model to search.</param>
        /// <param name="qualifiedName">The qualified name of the entity container being found.</param>
        /// <returns>The requested entity container, or null if no such entity container exists.</returns>
        public static IEdmEntityContainer FindEntityContainer(this IEdmModel model, ReadOnlySpan<char> qualifiedName)
        {
            EdmUtil.CheckArgumentNull(model, "model");

            return FindAcrossModels(
                model,
                qualifiedName,
                findEntityContainer,
                (first, second) => RegistrationHelper.CreateAmbiguousEntityContainerBinding(first, second));
        }

        /// <summary>
        /// Finds the entity set with qualified entity set name (not simple entity set name).
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="containerQualifiedEntitySetName">Name of the container qualified element, can be an OperationImport or an EntitySet.</param>
        /// <param name="entitySet">The Entity Set that was found.</param>
        /// <returns>True if an entityset was found from the qualified container name, false if none were found.</returns>
        public static bool TryFindContainerQualifiedEntitySet(this IEdmModel model, ReadOnlySpan<char> containerQualifiedEntitySetName, out IEdmEntitySet entitySet)
        {
            entitySet = null;

            if (!containerQualifiedEntitySetName.IsEmpty &&
                containerQualifiedEntitySetName.IndexOf(".", StringComparison.Ordinal) > -1 &&
                EdmUtil.TryParseContainerQualifiedElementName(containerQualifiedEntitySetName, out ReadOnlySpan<char> containerName, out ReadOnlySpan<char> simpleEntitySetName))
            {
                if (model.ExistsContainer(containerName))
                {
                    IEdmEntityContainer container = model.EntityContainer;
                    if (container != null)
                    {
                        entitySet = container.FindEntitySetExtended(simpleEntitySetName);
                    }
                }
            }

            return (entitySet != null);
        }

        /// <summary>
        /// Searches for entity set by the given name that may be container qualified in default container and .Extends containers.
        /// </summary>
        /// <param name="container">The container to search.</param>
        /// <param name="qualifiedName">The name which might be container qualified. If no container name is provided, then default container will be searched.</param>
        /// <returns>The entity set found or empty if none found.</returns>
        internal static IEdmEntitySet FindEntitySetExtended(this IEdmEntityContainer container, ReadOnlySpan<char> qualifiedName)
        {
            return FindInContainerAndExtendsRecursively(container, qualifiedName, (c, n) => c.FindEntitySet(n), ContainerExtendsMaxDepth);
        }

        /// <summary>
        /// Finds the singleton.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="containerQualifiedSingletonName">Name of the container qualified singleton element.</param>
        /// <param name="singleton">The singleton that was found.</param>
        /// <returns>True if an singleton was found from the qualified container name, false if none were found.</returns>
        public static bool TryFindContainerQualifiedSingleton(this IEdmModel model, ReadOnlySpan<char> containerQualifiedSingletonName, out IEdmSingleton singleton)
        {
            singleton = null;

            if (!containerQualifiedSingletonName.IsEmpty &&
                containerQualifiedSingletonName.IndexOf(".", StringComparison.Ordinal) > -1 &&
                EdmUtil.TryParseContainerQualifiedElementName(containerQualifiedSingletonName, out ReadOnlySpan<char> containerName, out ReadOnlySpan<char> simpleSingletonName))
            {
                if (model.ExistsContainer(containerName))
                {
                    singleton = model.EntityContainer.FindSingletonExtended(simpleSingletonName);

                    if (singleton != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Searches for singleton by the given name that may be container qualified in default container and .Extends containers. If no container name is provided, then default container will be searched.
        /// </summary>
        /// <param name="container">The container to search.</param>
        /// <param name="qualifiedName">The name which might be container qualified. If no container name is provided, then default container will be searched.</param>
        /// <returns>The singleton found or empty if none found.</returns>
        internal static IEdmSingleton FindSingletonExtended(this IEdmEntityContainer container, ReadOnlySpan<char> qualifiedName)
        {
            return FindInContainerAndExtendsRecursively(container, qualifiedName, (c, n) => c.FindSingleton(n), ContainerExtendsMaxDepth);
        }

        /// <summary>
        /// Tries the find container qualified operation imports.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="containerQualifiedOperationImportName">Name of the container qualified operation import.</param>
        /// <param name="operationImports">The operation imports.</param>
        /// <returns>True if OperationImports are found, false if none were found.</returns>
        public static bool TryFindContainerQualifiedOperationImports(this IEdmModel model, ReadOnlySpan<char> containerQualifiedOperationImportName, out IEnumerable<IEdmOperationImport> operationImports)
        {
            operationImports = null;

            if (containerQualifiedOperationImportName.IndexOf(".", StringComparison.Ordinal) > -1 &&
                EdmUtil.TryParseContainerQualifiedElementName(containerQualifiedOperationImportName, out ReadOnlySpan<char> containerName, out ReadOnlySpan<char> simpleOperationName))
            {
                if (model.ExistsContainer(containerName))
                {
                    operationImports = model.EntityContainer.FindOperationImportsExtended(simpleOperationName);

                    if (operationImports != null && operationImports.Any())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Searches for the operation imports by the specified name in default container and .Extends containers, returns an empty enumerable if no operation import exists.
        /// </summary>
        /// <param name="container">The container to search.</param>
        /// <param name="qualifiedName">The qualified name of the operation import which may or may not include the container name.</param>
        /// <returns>All operation imports that can be found by the specified name, returns an empty enumerable if no operation import exists.</returns>
        internal static IEnumerable<IEdmOperationImport> FindOperationImportsExtended(this IEdmEntityContainer container, ReadOnlySpan<char> qualifiedName)
        {
            return FindInContainerAndExtendsRecursively(container, qualifiedName, (c, n) => c.FindOperationImports(n), ContainerExtendsMaxDepth);
        }

        /// <summary>
        /// Searches for entity set by the given name that may be container qualified in default container and .Extends containers.
        /// </summary>
        /// <param name="model">The model to search.</param>
        /// <param name="qualifiedName">The name which might be container qualified. If no container name is provided, then default container will be searched.</param>
        /// <returns>The entity set found or empty if none found.</returns>
        public static IEdmEntitySet FindDeclaredEntitySet(this IEdmModel model, ReadOnlySpan<char> qualifiedName)
        {
            IEdmEntitySet foundEntitySet;
            if (!model.TryFindContainerQualifiedEntitySet(qualifiedName, out foundEntitySet))
            {
                // try searching by entity set name in container and extended containers:
                IEdmEntityContainer container = model.EntityContainer;
                if (container != null)
                {
                    return container.FindEntitySetExtended(qualifiedName);
                }
            }

            return foundEntitySet;
        }

        /// <summary>
        /// Searches for singleton by the given name that may be container qualified in default container and .Extends containers. If no container name is provided, then default container will be searched.
        /// </summary>
        /// <param name="model">The model to search.</param>
        /// <param name="qualifiedName">The name which might be container qualified. If no container name is provided, then default container will be searched.</param>
        /// <returns>The singleton found or empty if none found.</returns>
        public static IEdmSingleton FindDeclaredSingleton(this IEdmModel model, ReadOnlySpan<char> qualifiedName)
        {
            IEdmSingleton foundSingleton;
            if (!model.TryFindContainerQualifiedSingleton(qualifiedName, out foundSingleton))
            {
                // try searching by singleton name in container and extended containers:
                IEdmEntityContainer container = model.EntityContainer;
                if (container != null)
                {
                    return container.FindSingletonExtended(qualifiedName);
                }
            }

            return foundSingleton;
        }

        /// <summary>
        /// Searches for entity set or singleton by the given name that may be container qualified in default container and .Extends containers. If no container name is provided, then default container will be searched.
        /// </summary>
        /// <param name="model">The model to search.</param>
        /// <param name="qualifiedName">The name which might be container qualified. If no container name is provided, then default container will be searched.</param>
        /// <returns>The entity set or singleton found or empty if none found.</returns>
        public static IEdmNavigationSource FindDeclaredNavigationSource(this IEdmModel model, ReadOnlySpan<char> qualifiedName)
        {
            IEdmEntitySet entitySet = model.FindDeclaredEntitySet(qualifiedName);
            if (entitySet != null)
            {
                return entitySet;
            }

            return model.FindDeclaredSingleton(qualifiedName);
        }

        /// <summary>
        /// Searches for the operation imports by the specified name in default container and .Extends containers, returns an empty enumerable if no operation import exists.
        /// </summary>
        /// <param name="model">The model to search.</param>
        /// <param name="qualifiedName">The qualified name of the operation import which may or may not include the container name.</param>
        /// <returns>All operation imports that can be found by the specified name, returns an empty enumerable if no operation import exists.</returns>
        public static IEnumerable<IEdmOperationImport> FindDeclaredOperationImports(this IEdmModel model, ReadOnlySpan<char> qualifiedName)
        {
            IEnumerable<IEdmOperationImport> foundOperationImports;
            if (!model.TryFindContainerQualifiedOperationImports(qualifiedName, out foundOperationImports))
            {
                // try searching by operation import name in container and extended containers:
                IEdmEntityContainer container = model.EntityContainer;
                if (container != null)
                {
                    return container.FindOperationImportsExtended(qualifiedName);
                }
            }

            return foundOperationImports ?? Enumerable.Empty<IEdmOperationImport>();
        }

        /// <summary>
        /// Finds a property from the definition of this reference.
        /// </summary>
        /// <param name="structuredType">Reference to the calling object.</param>
        /// <param name="propertyName">Name of the property to find.</param>
        /// <param name="caseInsensitive">Property name case-sensitive or not.</param>
        /// <returns>The requested property if it exists. Otherwise, null.</returns>
        public static IEdmProperty FindProperty(this IEdmStructuredTypeReference structuredType, ReadOnlySpan<char> propertyName, bool caseInsensitive)
        {
            EdmUtil.CheckArgumentNull(structuredType, "structuredType");
            return structuredType.StructuredDefinition().FindProperty(propertyName, caseInsensitive);
        }

        /// <summary>
        /// Finds a property from the definition of this reference.
        /// </summary>
        /// <param name="structuredType">Reference to the calling object.</param>
        /// <param name="propertyName">Name of the property to find.</param>
        /// <param name="caseInsensitive">Property name case-sensitive or not.</param>
        /// <returns>The requested property if it exists. Otherwise, null.</returns>
        public static IEdmProperty FindProperty(this IEdmStructuredType structuredType, ReadOnlySpan<char> propertyName, bool caseInsensitive)
        {
            EdmUtil.CheckArgumentNull(structuredType, "structuredType");

            // For example: a structured type has two properties in difference case:
            //  1) Name
            //  2) naMe
            //  3) Title
            // if input "propertyName="Name", returns the #1 property.
            // if input "propertyName="naMe", returns the #2 property.
            // if input "propertyName="name", throw exception because multiple found.
            // But for "Title", any property name, such as "tiTle", "Title", "title", etc returns #3 property.
            IEdmProperty edmProperty = structuredType.FindProperty(propertyName);
            if (edmProperty != null || !caseInsensitive)
            {
                return edmProperty;
            }

            // Since we call "FindProperty" using case-sensitive, we don't miss the "right case" property.
            // So, it's safety to throw exception if we meet second case.
            foreach (IEdmProperty property in structuredType.Properties())
            {
                if (propertyName.Equals(property.Name, StringComparison.OrdinalIgnoreCase))
                {
                    if (edmProperty != null)
                    {
                        throw new InvalidOperationException(Error.Format(SRResources.MultipleMatchingPropertiesFound, propertyName.ToString(), structuredType.FullTypeName()));
                    }

                    edmProperty = property;
                }
            }

            return edmProperty;
        }

        /// <summary>
        /// Finds a property from the definition of this reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <param name="name">Name of the property to find.</param>
        /// <returns>The requested property if it exists. Otherwise, null.</returns>
        public static IEdmProperty FindProperty(this IEdmStructuredTypeReference type, ReadOnlySpan<char> name)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.StructuredDefinition().FindProperty(name);
        }

        /// <summary>
        /// Finds a navigation property declared in the definition of this reference by name.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <param name="name">Name of the navigation property to find.</param>
        /// <returns>The requested navigation property if it exists. Otherwise, null.</returns>
        public static IEdmNavigationProperty FindNavigationProperty(this IEdmStructuredTypeReference type, ReadOnlySpan<char> name)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.StructuredDefinition().FindProperty(name) as IEdmNavigationProperty;
        }

        /// <summary>
        /// Searches for an entity set or contained navigation property according to the specified path that may be container qualified in default container and .Extends containers.
        /// </summary>
        /// <param name="container">The container to search.</param>
        /// <param name="path">The path which might be container qualified. If no container name is provided, then default container will be searched.</param>
        /// <returns>The navigation source found or empty if none found.</returns>
        internal static IEdmNavigationSource FindNavigationSource(this IEdmEntityContainer container, ReadOnlySpan<char> path)
        {
            EdmUtil.CheckArgumentNull(container, "container");

            // the path could be:
            // "NS.Default.Customers/ContainedOrders"(for backward-compatibility) or
            // "NS.Default/Customers/ContainedOrders" (for top-level entity set in the Default entity container) or
            // "Customers" (unqualified)
            // "Customers/ContainedOrders" (unqualified)
            MemoryExtensions.SpanSplitEnumerator<char> segmentRanges = path.Split('/');
            segmentRanges.MoveNext();

            ReadOnlySpan<char> firstElementName = path[segmentRanges.Current];
            if (firstElementName.Contains(".".AsSpan(), StringComparison.Ordinal))
            {
                if (firstElementName.Equals(container.FullName(), StringComparison.OrdinalIgnoreCase))
                {
                    if (segmentRanges.MoveNext())
                    {
                        // NS.Default/Customers/ContainedOrders
                        firstElementName = path[segmentRanges.Current];
                    }
                    else
                    {
                        // if path only includes the namespace, for example "NS.Default", just return null;
                        return null;
                    }
                }
                else
                {
                    // NS.Default.Customers/ContainedOrders
                    // Split the first item using "." and fetch the last segment.
                    MemoryExtensions.SpanSplitEnumerator<char> firstSegmentRanges = firstElementName.Split('.');
                    ReadOnlySpan<char> lastName = firstElementName[firstSegmentRanges.Current];
                    do
                    {
                        lastName = firstElementName[firstSegmentRanges.Current];
                    }
                    while (firstSegmentRanges.MoveNext());

                    firstElementName = lastName;
                }
            }

            // Starting segment must be a singleton or entity set
            IEdmNavigationSource navigationSource = container.FindEntitySet(firstElementName);

            if (navigationSource == null)
            {
                navigationSource = container.FindSingleton(firstElementName);
            }

            // Subsequent segments may be single-valued complex or containment nav props
            List<string> subPathSegments = new List<string>();
            List<Range> ranges = new List<Range>();
            while (segmentRanges.MoveNext() && navigationSource != null)
            {
                ranges.Add(segmentRanges.Current);
                ReadOnlySpan<char> propertyName = path[segmentRanges.Current];
                IEdmNavigationProperty navProp = navigationSource.EntityType.FindProperty(propertyName) as IEdmNavigationProperty;
                if (navProp != null)
                {
                    foreach (var r in ranges)
                    {
                        subPathSegments.Add(path[r].ToString());
                    }
                    navigationSource = navigationSource.FindNavigationTarget(navProp, new EdmPathExpression(subPathSegments));
                    ranges.Clear();
                    subPathSegments.Clear();
                }
            }

            return navigationSource;
        }

        /// <summary>
        /// Replace a possibly alias-qualified name with the full namespace qualified name.
        /// </summary>
        /// <param name="model">The model containing the element.</param>
        /// <param name="name">The alias- or namespace- qualified name of the element.</param>
        /// <returns>The namespace qualified name of the element.</returns>
        internal static ReadOnlySpan<char> ReplaceAlias(this IEdmModel model, ReadOnlySpan<char> name)
        {
            VersioningDictionary<string, string> mappings = model.GetNamespaceAliases();
            VersioningList<string> list = model.GetUsedNamespacesHavingAlias();
            int idx = name.IndexOf(".", StringComparison.Ordinal);

            if (list != null && mappings != null && idx > 0)
            {
                var typeAlias = name.Slice(0, idx);
                // this runs in a hot path, hence the use of for-loop instead of LINQ
                string ns = null;
                for (int i = 0; i < list.Count; i++)
                {
                    if (mappings.TryGetValue(list[i], out string alias) && typeAlias.Equals(alias, StringComparison.Ordinal))
                    {
                        ns = list[i];
                        break;
                    }
                }

                return (ns != null) ? $"{ns}{name.Slice(idx)}".AsSpan() : name;
            }

            return name;
        }
    }
}
