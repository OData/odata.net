//---------------------------------------------------------------------
// <copyright file="BadCollectionType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a semantically invalid EDM collection type.
    /// </summary>
    internal class BadCollectionType : BadType, IEdmCollectionType
    {
        private readonly IEdmTypeReference elementType;

        public BadCollectionType(IEnumerable<EdmError> errors)
            : base(errors)
        {
            this.elementType = new BadTypeReference(new BadType(errors), true);
        }

        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Collection; }
        }

        public IEdmTypeReference ElementType
        {
            get { return this.elementType; }
        }
    }
}
