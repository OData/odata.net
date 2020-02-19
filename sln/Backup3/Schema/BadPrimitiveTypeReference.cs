//---------------------------------------------------------------------
// <copyright file="BadPrimitiveTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a reference to a semantically invalid EDM primitive type.
    /// </summary>
    internal class BadPrimitiveTypeReference : EdmPrimitiveTypeReference, IEdmCheckable
    {
        private readonly IEnumerable<EdmError> errors;

        public BadPrimitiveTypeReference(string qualifiedName, bool isNullable, IEnumerable<EdmError> errors)
            : base(new BadPrimitiveType(qualifiedName, EdmPrimitiveTypeKind.None, errors), isNullable)
        {
            this.errors = errors;
        }

        public IEnumerable<EdmError> Errors
        {
            get { return this.errors; }
        }

        public override string ToString()
        {
            EdmError error = this.Errors.FirstOrDefault();
            Debug.Assert(error != null, "error != null");
            string prefix = error != null ? error.ErrorCode.ToString() + ":" : "";
            return prefix + this.ToTraceString();
        }
    }
}
