//---------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Contains extension methods for <see cref="IEdmModel"/> interfaces.
    /// </summary>
    public static partial class ExtensionMethods
    {
#if NET9_0_OR_GREATER
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
        /// Finds the entity set with qualified entity set name (not simple entity set name).
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="containerQualifiedEntitySetName">Name of the container qualified element, can be an OperationImport or an EntitySet.</param>
        /// <param name="entitySet">The Entity Set that was found.</param>
        /// <returns>True if an entityset was found from the qualified container name, false if none were found.</returns>
        public static bool TryFindContainerQualifiedEntitySet(this IEdmModel model, ReadOnlySpan<char> containerQualifiedEntitySetName, out IEdmEntitySet entitySet)
        {
            entitySet = null;
            string containerName = null;
            string simpleEntitySetName = null;

            if (!containerQualifiedEntitySetName.IsEmpty &&
                containerQualifiedEntitySetName.IndexOf(".", StringComparison.Ordinal) > -1 &&
                EdmUtil.TryParseContainerQualifiedElementName(containerQualifiedEntitySetName, out containerName, out simpleEntitySetName))
            {
                if (model.ExistsContainer(containerName))
                {
                    IEdmEntityContainer container = model.EntityContainer;
                    if (container != null)
                    {
                        entitySet = container.FindEntitySetExtended(simpleEntitySetName.AsSpan());
                    }
                }
            }

            return (entitySet != null);
        }

        internal static IEdmNavigationSource FindNavigationSource(this IEdmEntityContainer container, ReadOnlySpan<char> path)
        {
            EdmUtil.CheckArgumentNull(container, "container");

            // the path could be:
            // "NS.Default.Customers/ContainedOrders"(for backward-compatibility) or
            // "NS.Default/Customers/ContainedOrders" (for top-level entity set in the Default entity container) or
            // "Customers" (unqualified)
            // "Customers/ContainedOrders" (unqualified)

            MemoryExtensions.SpanSplitEnumerator<char> pathSegments = path.Split('/');
            Range current = pathSegments.Current;
            ReadOnlySpan<char> firstElementName = path[current];

            int nextIndex = 1;
            if (firstElementName.Contains(".", StringComparison.Ordinal))
            {
                if (firstElementName.Equals(container.FullName().AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    if (pathSegments.MoveNext())
                    {
                        // NS.Default/Customers/ContainedOrders
                        firstElementName = path[pathSegments.Current];
                        nextIndex = 2;
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
                    firstElementName = path[pathSegments.Current][path[pathSegments.Current].LastIndexOf('.')..];
                }
            }

            // Starting segment must be a singleton or entity set
            IEdmNavigationSource navigationSource = container.FindEntitySet(firstElementName);

            navigationSource ??= container.FindSingleton(firstElementName);

            // Subsequent segments may be single-valued complex or containment nav props
            if (nextIndex == 2)
            {
                pathSegments.MoveNext();
            }

            List<string> subPathSegments = new List<string>();

            foreach(var pathSegment in pathSegments)
            {
                if (navigationSource != null)
                {
                    break;
                }
                subPathSegments.Add(path[pathSegment].ToString());

                if (navigationSource.EntityType.FindProperty(path[pathSegment].ToString()) is IEdmNavigationProperty navProp)
                {
                    navigationSource = navigationSource.FindNavigationTarget(navProp, new EdmPathExpression(subPathSegments));
                    subPathSegments.Clear();
                }

            }

            return navigationSource;
        }
#endif
    }
}
