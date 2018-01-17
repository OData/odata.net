//---------------------------------------------------------------------
// <copyright file="UnresolvedEnumType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class UnresolvedEnumType : BadEnumType, IUnresolvedElement
    {
        public UnresolvedEnumType(string qualifiedName, EdmLocation location)
            : base(qualifiedName, new EdmError[] { new EdmError(location, EdmErrorCode.BadUnresolvedEnumType, Edm.Strings.Bad_UnresolvedEnumType(qualifiedName)) })
        {
        }
    }
}
