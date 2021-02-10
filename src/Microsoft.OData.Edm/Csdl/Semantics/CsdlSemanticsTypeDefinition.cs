//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsTypeDefinition.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Common base class for the semantics of EDM types.
    /// </summary>
    internal abstract class CsdlSemanticsTypeDefinition : CsdlSemanticsElement, IEdmType
    {
        protected CsdlSemanticsTypeDefinition(CsdlElement element)
            : base(element)
        {
        }

        public abstract EdmTypeKind TypeKind { get; }

        public virtual EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.TypeDefinition;

        public override string ToString()
        {
            return this.ToTraceString();
        }
    }
}
