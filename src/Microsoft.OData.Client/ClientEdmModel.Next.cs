//---------------------------------------------------------------------
// <copyright file="ClientEdmModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#region Namespaces.

using System;
using System.Collections.Generic;
using Microsoft.OData.Client.Metadata;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
#endregion

namespace Microsoft.OData.Client
{
    /// <summary>
    /// EdmModel describing the client metadata
    /// </summary>
    internal sealed partial class ClientEdmModel : EdmElement, IEdmModel
    {

#if NET9_0_OR_GREATER
        /// <summary>A cache that maps a client type name to the corresponding client type annotation.</summary>
        private readonly Dictionary<string, ClientTypeAnnotation>.AlternateLookup<ReadOnlyMemory<char>> typeNameToClientTypeAnnotationCacheMemoryLookup;
        private readonly Dictionary<string, ClientTypeAnnotation>.AlternateLookup<ReadOnlySpan<char>> typeNameToClientTypeAnnotationCacheSpanLookup;

        public ClientEdmModel()
        {
            this.typeNameToClientTypeAnnotationCacheMemoryLookup = this.typeNameToClientTypeAnnotationCache.GetAlternateLookup<ReadOnlyMemory<char>>();
            this.typeNameToClientTypeAnnotationCacheSpanLookup = this.typeNameToClientTypeAnnotationCache.GetAlternateLookup<ReadOnlySpan<char>>();
        }
        public IEdmSchemaType FindDeclaredType(ReadOnlyMemory<char> qualifiedName)
        {
            ClientTypeAnnotation clientTypeAnnotation = null;
            if (this.typeNameToClientTypeAnnotationCacheMemoryLookup.TryGetValue(qualifiedName, out clientTypeAnnotation))
            {
                return (IEdmSchemaType)clientTypeAnnotation.EdmType;
            }

            return null;
        }

        public IEnumerable<IEdmOperation> FindDeclaredBoundOperations(ReadOnlyMemory<char> qualifiedName, IEdmType bindingType)
        {
            return [];
        }

        public IEnumerable<IEdmOperation> FindDeclaredOperations(ReadOnlyMemory<char> qualifiedName)
        {
            return [];
        }

        public IEdmTerm FindDeclaredTerm(ReadOnlyMemory<char> qualifiedName)
        {
           return null;
        }

        public IEdmSchemaType FindDeclaredType(ReadOnlySpan<char> qualifiedName)
        {
            ClientTypeAnnotation clientTypeAnnotation = null;
            if (this.typeNameToClientTypeAnnotationCacheSpanLookup.TryGetValue(qualifiedName, out clientTypeAnnotation))
            {
                return (IEdmSchemaType)clientTypeAnnotation.EdmType;
            }

            return null;
        }

        public IEnumerable<IEdmOperation> FindDeclaredBoundOperations(ReadOnlySpan<char> qualifiedName, IEdmType bindingType)
        {
            return [];
        }

        public IEnumerable<IEdmOperation> FindDeclaredOperations(ReadOnlySpan<char> qualifiedName)
        {
            return [];
        }

        public IEdmTerm FindDeclaredTerm(ReadOnlySpan<char> qualifiedName)
        {
           return null;
        }
#endif
    }
}