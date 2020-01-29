//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsUntypedTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides the semantics of a reference to an EDM untyped type.
    /// </summary>
    internal class CsdlSemanticsUntypedTypeReference : CsdlSemanticsElement, IEdmUntypedTypeReference
    {
        internal readonly CsdlUntypedTypeReference Reference;
        private readonly CsdlSemanticsSchema schema;

        /// <summary>
        /// The IEdmUntypedType instance from EdmCoreModel.
        /// </summary>
        private readonly IEdmUntypedType definition;

        public CsdlSemanticsUntypedTypeReference(CsdlSemanticsSchema schema, CsdlUntypedTypeReference reference)
            : base(reference)
        {
            this.schema = schema;
            this.Reference = reference;
            this.definition = EdmCoreModel.Instance.GetUntypedType();
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
