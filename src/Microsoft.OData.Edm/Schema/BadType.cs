//---------------------------------------------------------------------
// <copyright file="BadType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm
{
    internal class BadType : BadElement, IEdmType
    {
        public BadType(IEnumerable<EdmError> errors)
            : base(errors)
        {
        }

        public virtual EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.None; }
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
