//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsEntitySet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl.Parsing.Ast;

    /// <summary>
    /// Provides semantics for CsdlEntitySet.
    /// </summary>
    internal class CsdlSemanticsEntitySet : CsdlSemanticsNavigationSource, IEdmEntitySet
    {
        public CsdlSemanticsEntitySet(CsdlSemanticsEntityContainer container, CsdlEntitySet entitySet)
            : base(container, entitySet)
        {
        }

        public override IEdmType Type
        {
            get { return new EdmCollectionType(new EdmEntityTypeReference(this.typeCache.GetValue(this, ComputeElementTypeFunc, null), false)); }
        }

        public override EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.EntitySet; }
        }

        public bool IncludeInServiceDocument
        {
            get { return ((CsdlEntitySet)this.navigationSource).IncludeInServiceDocument; }
        }

        protected override IEdmEntityType ComputeElementType()
        {
            string type = ((CsdlEntitySet)this.navigationSource).ElementType;
            return this.container.Context.FindType(type) as IEdmEntityType ?? new UnresolvedEntityType(type, this.Location);
        }
    }
}