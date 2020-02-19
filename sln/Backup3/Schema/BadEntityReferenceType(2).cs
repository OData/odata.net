//---------------------------------------------------------------------
// <copyright file="BadEntityReferenceType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a semantically invalid EDM entity reference type.
    /// </summary>
    internal class BadEntityReferenceType : BadType, IEdmEntityReferenceType
    {
        private readonly IEdmEntityType entityType;

        public BadEntityReferenceType(IEnumerable<EdmError> errors)
            : base(errors)
        {
            this.entityType = new BadEntityType(String.Empty, this.Errors);
        }

        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.EntityReference; }
        }

        public IEdmEntityType EntityType
        {
            get { return this.entityType; }
        }
    }
}
