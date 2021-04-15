//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsPrimitiveTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides the semantics of a reference to an EDM primitive type.
    /// </summary>
    internal class CsdlSemanticsPrimitiveTypeReference : CsdlSemanticsElement, IEdmPrimitiveTypeReference
    {
        internal readonly CsdlPrimitiveTypeReference Reference;

        /// <summary>
        /// This doesn't need the full caching mechanism because the computation is cheap, and the likelihood of computing a primitive type reference without needing its definition is remote.
        /// </summary>
        public CsdlSemanticsPrimitiveTypeReference(CsdlSemanticsModel model, CsdlPrimitiveTypeReference reference)
            : base(reference)
        {
            Model = model;
            Reference = reference;
            Definition = EdmCoreModel.Instance.GetPrimitiveType(reference.Kind);
        }

        public bool IsNullable => this.Reference.IsNullable;

        public IEdmType Definition { get; }

        public override CsdlSemanticsModel Model { get; }

        public override CsdlElement Element => this.Reference;

        public override string ToString()
        {
            return this.ToTraceString();
        }
    }
}
