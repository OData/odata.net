//---------------------------------------------------------------------
// <copyright file="UnresolvedComplexType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class UnresolvedComplexType : BadComplexType, IUnresolvedElement
    {
        public UnresolvedComplexType(string qualifiedName, EdmLocation location)
            : base(qualifiedName, new EdmError[] { new EdmError(location, EdmErrorCode.BadUnresolvedComplexType, Edm.Strings.Bad_UnresolvedComplexType(qualifiedName)) })
        {
        }
    }
}
