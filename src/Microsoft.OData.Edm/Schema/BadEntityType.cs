//---------------------------------------------------------------------
// <copyright file="BadEntityType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a semantically invalid EDM entity type.
    /// </summary>
    internal class BadEntityType : BadNamedStructuredType, IEdmEntityType, IEdmKeyPropertyRef
    {
        public BadEntityType(string qualifiedName, IEnumerable<EdmError> errors)
            : base(qualifiedName, errors)
        {
        }

        public IEnumerable<IEdmStructuralProperty> DeclaredKey => null;

        public IEnumerable<IEdmPropertyRef> DeclaredKeyRef => null;

        public override EdmTypeKind TypeKind => EdmTypeKind.Entity;

        public bool HasStream => false;

    }
}
