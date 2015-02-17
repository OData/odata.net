//---------------------------------------------------------------------
// <copyright file="UnresolvedTypeTerm.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class UnresolvedTypeTerm : UnresolvedVocabularyTerm, IEdmEntityType
    {
        public UnresolvedTypeTerm(string qualifiedName)
            : base(qualifiedName)
        {
        }

        public IEnumerable<IEdmStructuralProperty> DeclaredKey
        {
            get { return Enumerable.Empty<IEdmStructuralProperty>(); }
        }

        public bool IsAbstract
        {
            get { return false; }
        }

        public bool IsOpen
        {
            get { return false; }
        }

        public bool HasStream
        {
            get { return false; }
        }

        public IEdmStructuredType BaseType
        {
            get { return null; }
        }

        public IEnumerable<IEdmProperty> DeclaredProperties
        {
            get { return Enumerable.Empty<IEdmProperty>(); }
        }

        public EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Entity; }
        }

        public override EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }

        public override EdmTermKind TermKind
        {
            get { return EdmTermKind.Type; }
        }

        public IEdmProperty FindProperty(string name)
        {
            return null;
        }
    }
}
