//---------------------------------------------------------------------
// <copyright file="CyclicTerm.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM term that cannot be determined due to a cyclic reference.
    /// </summary>
    internal class CyclicTerm : BadTerm
    {
        public CyclicTerm(string qualifiedName, EdmLocation location)
            : base(qualifiedName, new EdmError[] { new EdmError(location, EdmErrorCode.BadCyclicTerm, Edm.Strings.Bad_CyclicTerm(qualifiedName)) })
        {
        }
    }
}