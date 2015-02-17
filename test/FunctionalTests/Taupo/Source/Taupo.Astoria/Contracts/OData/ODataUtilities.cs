//---------------------------------------------------------------------
// <copyright file="ODataUtilities.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Collection of helpful methods for working with OData that don't make sense as extensions or components
    /// </summary>
    public static class ODataUtilities
    {
        /// <summary>
        /// Tries to get the element type name from a multi-value type name like 'MultiValue(Edm.String)'
        /// </summary>
        /// <param name="typeName">The multivalue type name</param>
        /// <param name="elementTypeName">The element type name if found</param>
        /// <returns>A value indicating whether the element type name could be found</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API but is explicitly blocked by code-analysis")]
        public static bool TryGetMultiValueElementTypeName(string typeName, out string elementTypeName)
        {
            if (!string.IsNullOrEmpty(typeName) && typeName.StartsWith(ODataConstants.BeginMultiValueTypeIdentifier, StringComparison.Ordinal))
            {
                ExceptionUtilities.Assert(typeName.EndsWith(ODataConstants.EndMultiValueTypeNameIdentifier, StringComparison.Ordinal), "Multi-value type name did not end with '{0}'. Value was: '{1}'", ODataConstants.EndMultiValueTypeNameIdentifier, typeName);
                elementTypeName = typeName.Substring(ODataConstants.BeginMultiValueTypeIdentifier.Length, typeName.Length - ODataConstants.BeginMultiValueTypeIdentifier.Length - ODataConstants.EndMultiValueTypeNameIdentifier.Length);
                return true;
            }

            elementTypeName = null;
            return false;
        }

        /// <summary>
        /// Extracts the names of properties specified in the $select option, if any.
        /// </summary>
        /// <param name="uri">The URI to parse.</param>
        /// <param name="uriToStringConverter">The IODataUriToStringConverter implementation.</param>
        /// <returns>The list of property names.</returns>
        public static IEnumerable<string> GetSelectedPropertyNamesFromUri(ODataUri uri, IODataUriToStringConverter uriToStringConverter)
        {
            return uri.SelectSegments.Select(s => uriToStringConverter.ConcatenateSegments(s.Where(s2 => !(s2 is EntityTypeSegment))));
        }

        /// <summary>
        /// Determines whether a list of property names contains all identity properties for a given entity type.
        /// </summary>
        /// <param name="propertyNames">The list of property names.</param>
        /// <param name="entityType">The entity type.</param>
        /// <returns>Whether or not all identity property names appear in the list.</returns>
        public static bool ContainsAllIdentityPropertyNames(IEnumerable<string> propertyNames, EntityType entityType)
        {
            return propertyNames.Contains(Endpoints.SelectAll) || entityType.AllKeyProperties.All(p => propertyNames.Contains(p.Name));
        }

        /// <summary>
        /// Determines whether a list of property names contains all concurrency properties for a given entity type.
        /// </summary>
        /// <param name="propertyNames">The list of property names.</param>
        /// <param name="entityType">The entity type.</param>
        /// <returns>Whether or not all concurrency property names appear in the list.</returns>
        /// <remarks>The entity type is expected to have concurrency properties and the method will fail if this is not the case.</remarks>
        public static bool ContainsAllConcurrencyPropertyNames(IEnumerable<string> propertyNames, EntityType entityType)
        {
            var concurrencyProperties = entityType.AllProperties.Where(p => p.Annotations.OfType<ConcurrencyTokenAnnotation>().Any()).ToList();
            ExceptionUtilities.Assert(concurrencyProperties.Any(), "Entity type does not have concurrency properties");
            return propertyNames.Contains(Endpoints.SelectAll) || concurrencyProperties.All(p => propertyNames.Contains(p.Name));
        }
    }
}
