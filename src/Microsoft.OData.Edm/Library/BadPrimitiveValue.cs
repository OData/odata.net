//---------------------------------------------------------------------
// <copyright file="BadPrimitiveValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Edm.Library
{
    internal class BadPrimitiveValue : BadElement, IEdmPrimitiveValue
    {
        private readonly IEdmPrimitiveTypeReference type;

        public BadPrimitiveValue(IEdmPrimitiveTypeReference type, IEnumerable<EdmError> errors)
            : base(errors)
        {
            this.type = type;
        }

        public IEdmTypeReference Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public EdmValueKind ValueKind
        {
            get { return EdmValueKind.None; }
        }
    }
}
