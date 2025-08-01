//---------------------------------------------------------------------
// <copyright file="CustomUriLiteralParsersAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Represents an annotation for custom URI literal parsers.
    /// </summary>
    internal class CustomUriLiteralParsersAnnotation
    {
        /// <summary>
        /// Container for custom URI literal parsers by Edm type.
        /// </summary>
        public ConcurrentDictionary<IEdmTypeReference, IUriLiteralParser> CustomUriLiteralParsersByEdmType { get; }
            = new ConcurrentDictionary<IEdmTypeReference, IUriLiteralParser>(new EdmTypeEqualityComparer());

        /// <summary>
        /// Container for custom URI literal parsers.
        /// </summary>
        /// <remarks>
        /// We don't care about the value in this dictionary but a ConcurrentDictionary is a better choice
        /// because it's thread-safe and will support add, remove, and contains operations without locking.
        /// </remarks>
        public ConcurrentDictionary<IUriLiteralParser, byte> CustomUriLiteralParsers { get; }
            = new ConcurrentDictionary<IUriLiteralParser, byte>(ReferenceEqualityComparer<IUriLiteralParser>.Instance);

        /// <summary>
        /// Provides value-based equality comparison and hash code generation for <see cref="IEdmTypeReference"/> instances.
        /// Considers two EDM type references equal if they have the same type definition and nullability.
        /// Used as a key comparer for dictionaries that map custom URI literal parsers to EDM types.
        /// </summary>
        private sealed class EdmTypeEqualityComparer : IEqualityComparer<IEdmTypeReference>
        {
            public bool Equals(IEdmTypeReference obj, IEdmTypeReference other)
            {
                return EdmElementComparer.IsEquivalentTo(obj, other);
            }

            public int GetHashCode(IEdmTypeReference obj)
            {
                if (obj == null)
                {
                    return 0;
                }

                // Combine the hash code of the type's full name and its nullability
                int hash = obj.FullName()?.GetHashCode(StringComparison.Ordinal) ?? 0;
                hash = HashCode.Combine(hash, obj.IsNullable);

                return hash;
            }
        }
    }
}
