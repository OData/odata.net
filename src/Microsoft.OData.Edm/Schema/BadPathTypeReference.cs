//---------------------------------------------------------------------
// <copyright file="BadPathTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm
{
    internal class BadPathTypeReference : EdmPathTypeReference, IEdmCheckable
    {
        private readonly IEnumerable<EdmError> errors;

        public BadPathTypeReference(string qualifiedName, bool isNullable, IEnumerable<EdmError> errors)
            : base(new BadPathType(qualifiedName, errors), isNullable)
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
            string prefix = error.ErrorCode + ":";
            return prefix + this.ToTraceString();
        }
    }
}
