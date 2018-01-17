//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsActionImport.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class CsdlSemanticsActionImport : CsdlSemanticsOperationImport, IEdmActionImport
    {
        public CsdlSemanticsActionImport(CsdlSemanticsEntityContainer container, CsdlActionImport actionImport, IEdmAction backingAction)
            : base(container, actionImport, backingAction)
        {
        }

        public IEdmAction Action
        {
            get { return (IEdmAction)this.Operation; }
        }

        public override EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.ActionImport; }
        }
    }
}
