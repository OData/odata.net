//---------------------------------------------------------------------
// <copyright file="EdmExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.OData.Edm;

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
    }
}
