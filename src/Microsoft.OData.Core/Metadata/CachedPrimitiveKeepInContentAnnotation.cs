//---------------------------------------------------------------------
// <copyright file="CachedPrimitiveKeepInContentAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Metadata
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Annotation which stores a hashset of property names of a complex type that returned KeepInContent == true
    /// when written the first time. See the comments on ODataWriterBehavior.UseV1ProviderBehavior for more details.
    /// </summary>
    internal sealed class CachedPrimitiveKeepInContentAnnotation
    {
        /// <summary>
        /// A hash set with the property names of properties that are kept in the content.
        /// </summary>
        private readonly HashSet<string> keptInContentPropertyNames;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="keptInContentPropertyNames">Enumeration of property names that are kept in content.</param>
        internal CachedPrimitiveKeepInContentAnnotation(IEnumerable<string> keptInContentPropertyNames)
        {
            this.keptInContentPropertyNames = keptInContentPropertyNames == null
                ? null
                : new HashSet<string>(keptInContentPropertyNames, StringComparer.Ordinal);
        }

        /// <summary>
        /// Determines if a property is in a list of properties that are kept in the content.
        /// </summary>
        /// <param name="propertyName">The name of the property to lookup.</param>
        /// <returns>true if the property is kept in the content; false otherwise.</returns>
        internal bool IsKeptInContent(string propertyName)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            return this.keptInContentPropertyNames == null
                ? false
                : this.keptInContentPropertyNames.Contains(propertyName);
        }
    }
}
