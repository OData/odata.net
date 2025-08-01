//---------------------------------------------------------------------
// <copyright file="CustomUriLiteralPrefixesAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using System;
    using System.Collections.Concurrent;
    using Microsoft.OData.Edm;

    #endregion Namespaces
    /// <summary>
    /// Represents an annotation for custom URI literal prefixes.
    /// </summary>
    internal class CustomUriLiteralPrefixesAnnotation
    {
        /// <summary>
        /// Container for custom URI literal prefixes.
        /// </summary>
        public ConcurrentDictionary<string, IEdmTypeReference> CustomUriLiteralPrefixes { get; }
            = new ConcurrentDictionary<string, IEdmTypeReference>(StringComparer.Ordinal);
    }
}
