//---------------------------------------------------------------------
// <copyright file="BadEdmEnumMemberValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm
{
    internal class BadEdmEnumMemberValue : BadElement, IEdmEnumMemberValue
    {
        public BadEdmEnumMemberValue(IEnumerable<EdmError> errors)
            : base(errors)
        {
        }

        public long Value
        {
            get { return 0; }
        }
    }
}
