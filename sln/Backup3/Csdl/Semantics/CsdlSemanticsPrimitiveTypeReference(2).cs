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
        private readonly CsdlSemanticsSchema schema;

        /// <summary>
        /// This doesn't need the full caching mechanism because the computation is cheap, and the likelihood of computing a primitive type reference without needing its definition is remote.
        /// </summary>
        private readonly IEdmPrimitiveType definition;

        public CsdlSemanticsPrimitiveTypeReference(CsdlSemanticsSchema schema, CsdlPrimitiveTypeReference reference)
            : base(reference)
        {
            this.schema = schema;
            this.Reference = reference;
            this.definition = EdmCoreModel.Instance.GetPrimitiveType(this.Reference.Kind);
        }

        public bool IsNullable
        {
            get { return this.Reference.IsNullable; }
        }

        public IEdmType Definition
        {
            get { return this.definition; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.schema.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.Reference; }
        }

        public override string ToString()
        {
            return this.ToTraceString();
        }
    }
}
