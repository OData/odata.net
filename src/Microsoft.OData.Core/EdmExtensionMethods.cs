//---------------------------------------------------------------------
// <copyright file="EdmExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace Microsoft.OData
{
    internal static class EdmExtensionMethods
    {
        /// <summary>
        /// Find the navigation target which is <paramref name="navigationProperty"/> of current <paramref name="navigationSource"/> targets.
        /// </summary>
        /// <param name="navigationSource">The navigation source to find.</param>
        /// <param name="navigationProperty">The navigation property</param>
        /// <param name="matchBindingPath">The function used to determine if the binding path matches.</param>
        /// <returns>The navigation target which matches the binding path.</returns>
        public static IEdmNavigationSource FindNavigationTarget(this IEdmNavigationSource navigationSource, IEdmNavigationProperty navigationProperty, Func<IEdmPathExpression, bool> matchBindingPath)
        {
            Debug.Assert(navigationSource != null);
            Debug.Assert(navigationProperty != null);
            Debug.Assert(matchBindingPath != null);

            if (navigationProperty.ContainsTarget)
            {
                return navigationSource.FindNavigationTarget(navigationProperty);
            }

            IEnumerable<IEdmNavigationPropertyBinding> bindings = navigationSource.FindNavigationPropertyBindings(navigationProperty);

            if (bindings != null)
            {
                foreach (var binding in bindings)
                {
                    if (matchBindingPath(binding.Path))
                    {
                        return binding.Target;
                    }
                }
            }

            return new UnknownEntitySet(navigationSource, navigationProperty);
        }

        /// <summary>
        /// Find the navigation target which is <paramref name="navigationProperty"/> of current <paramref name="navigationSource"/> targets.
        /// The function is specifically used in Uri parser.
        /// </summary>
        /// <param name="navigationSource">The navigation source to find.</param>
        /// <param name="navigationProperty">The navigation property</param>
        /// <param name="matchBindingPath">The function used to determine if the binding path matches.</param>
        /// <param name="parsedSegments">The parsed segments in path, which is used to match binding path.</param>
        /// <param name="bindingPath">The output binding path of the navigation property which matches the <paramref name="parsedSegments"/></param>
        /// <returns>The navigation target which matches the binding path.</returns>
        public static IEdmNavigationSource FindNavigationTarget(this IEdmNavigationSource navigationSource, IEdmNavigationProperty navigationProperty, Func<IEdmPathExpression, IReadOnlyList<ODataPathSegment>, bool> matchBindingPath, IReadOnlyList<ODataPathSegment> parsedSegments, out IEdmPathExpression bindingPath)
        {
            Debug.Assert(navigationSource != null);
            Debug.Assert(navigationProperty != null);
            Debug.Assert(matchBindingPath != null);
            Debug.Assert(parsedSegments != null);

            bindingPath = null;

            if (navigationProperty.ContainsTarget)
            {
                return navigationSource.FindNavigationTarget(navigationProperty);
            }

            IEnumerable<IEdmNavigationPropertyBinding> bindings = navigationSource.FindNavigationPropertyBindings(navigationProperty);

            if (bindings != null)
            {
                foreach (var binding in bindings)
                {
                    if (matchBindingPath(binding.Path, parsedSegments))
                    {
                        bindingPath = binding.Path;
                        return binding.Target;
                    }
                }
            }

            if (typeof(IEdmUnknownEntitySet).IsAssignableFrom(navigationSource.GetType()))
            {
                return new UnknownEntitySet(navigationSource, navigationProperty);
            }
            else
            {
                return navigationSource.FindNavigationTarget(navigationProperty);
            }
        }

        /// <summary>
        /// Decide whether <paramref name="currentNavigationSource"/> with type <paramref name="currentResourceType"/> should have key.
        /// </summary>
        /// <param name="currentNavigationSource">The navigation source to be evaluated.</param>
        /// <param name="currentResourceType">The resource type to be evaluated.</param>
        /// <returns>True if the navigation source should have key.</returns>
        public static bool HasKey(IEdmNavigationSource currentNavigationSource, IEdmStructuredType currentResourceType)
        {
            if (currentResourceType is IEdmComplexType)
            {
                return false;
            }

            if (currentNavigationSource is IEdmEntitySet)
            {
                return true;
            }

            var currentContainedEntitySet = currentNavigationSource as IEdmContainedEntitySet;
            if (currentContainedEntitySet != null && currentContainedEntitySet.NavigationProperty.Type.TypeKind() == EdmTypeKind.Collection)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parse an enum integral value to enum member.
        /// </summary>
        /// <param name="enumType">edm enum type</param>
        /// <param name="value">input integral value.</param>
        /// <param name="enumMember">parsed result.</param>
        /// <returns>true if parse succeeds, false if parse fails.</returns>
        public static bool TryParse(this IEdmEnumType enumType, long value, out IEdmEnumMember enumMember)
        {
            enumMember = null;
            foreach (IEdmEnumMember member in enumType.Members)
            {
                if (member.Value.Value == value)
                {
                    enumMember = member;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the given member name exists in the enum type.
        /// </summary>
        /// <param name="enumType">The enum type.</param>
        /// <param name="memberName">The member name to check.</param>
        /// <param name="comparison">The comparison type to use for string comparison. Default is Ordinal.</param>
        /// <returns>True if the member name exists in the enum type; otherwise, false.</returns>
        public static bool ContainsMember(this IEdmEnumType enumType, string memberName, StringComparison comparison = StringComparison.Ordinal)
        {
            foreach (IEdmEnumMember member in enumType.Members)
            {
                if (string.Equals(member.Name, memberName, comparison))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
