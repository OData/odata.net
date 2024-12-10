//---------------------------------------------------------------------
// <copyright file="UnresolvedProperty.cs" company="Microsoft">
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
                new EdmError(location, EdmErrorCode.BadUnresolvedPropertyRef,
                    Edm.Strings.Bad_UnresolvedPropertyRef(declaringType.FullTypeName(), name, alias))
            })
        {
        }

        public IEdmStructuralProperty ReferencedProperty => null;

        public string PropertyAlias { get; }

        public string Name { get; }

        public IEdmPathExpression Path { get; }
    }
}