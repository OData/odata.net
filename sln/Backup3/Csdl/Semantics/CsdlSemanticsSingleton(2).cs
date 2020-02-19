//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsSingleton.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl.Parsing.Ast;

    /// <summary>
    /// Provides semantics for CsdlSingleton.
    /// </summary>
    internal class CsdlSemanticsSingleton : CsdlSemanticsNavigationSource, IEdmSingleton
    {
        public CsdlSemanticsSingleton(CsdlSemanticsEntityContainer container, CsdlSingleton singleton)
            : base(container, singleton)
        {
        }

        public override IEdmType Type
        {
            get { return this.typeCache.GetValue(this, ComputeElementTypeFunc, null); }
        }

        public override EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.Singleton; }
        }

        protected override IEdmEntityType ComputeElementType()
        {
            string type = ((CsdlSingleton)this.navigationSource).Type;
            return this.container.Context.FindType(type) as IEdmEntityType ?? new UnresolvedEntityType(type, this.Location);
        }
    }
}

