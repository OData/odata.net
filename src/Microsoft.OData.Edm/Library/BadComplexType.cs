//---------------------------------------------------------------------
// <copyright file="BadComplexType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a semantically invalid EDM complex type definition.
    /// </summary>
    internal class BadComplexType : BadNamedStructuredType, IEdmComplexType
    {
        public BadComplexType(string qualifiedName, IEnumerable<EdmError> errors)
            : base(qualifiedName, errors)
        {
        }

        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Complex; }
        }

        public EdmTermKind TermKind
        {
            get { return EdmTermKind.Type; }
        }
    }
}
