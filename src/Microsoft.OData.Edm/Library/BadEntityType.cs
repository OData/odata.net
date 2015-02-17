//---------------------------------------------------------------------
// <copyright file="BadEntityType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a semantically invalid EDM entity type.
    /// </summary>
    internal class BadEntityType : BadNamedStructuredType, IEdmEntityType
    {
        public BadEntityType(string qualifiedName, IEnumerable<EdmError> errors)
            : base(qualifiedName, errors)
        {
        }

        public IEnumerable<IEdmStructuralProperty> DeclaredKey
        {
            get { return null; }
        }

        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Entity; }
        }

        public EdmTermKind TermKind
        {
            get { return EdmTermKind.Type; }
        }

        public bool HasStream
        {
            get { return false; }
        }
    }
}
