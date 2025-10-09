//---------------------------------------------------------------------
// <copyright file="UnresolvedPropertyRef.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class UnresolvedPropertyRef : BadElement, IUnresolvedElement, IEdmPropertyRef
    {
        public UnresolvedPropertyRef(IEdmStructuredType declaringType, string name, string alias, EdmLocation location)
            : base(new EdmError[]
            {
                new EdmError(location, EdmErrorCode.BadUnresolvedPropertyRef, Error.Format(SRResources.Bad_UnresolvedPropertyRef, declaringType.FullTypeName(), name, alias))
            })
        {
            Path = new EdmPathExpression(name);
            PropertyAlias = alias;
        }

        public IEdmPathExpression Path { get; }

        public IEdmStructuralProperty ReferencedProperty { get; }

        public string PropertyAlias { get; }
    }
}
