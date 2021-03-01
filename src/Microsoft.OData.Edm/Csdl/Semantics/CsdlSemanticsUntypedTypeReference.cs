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

        /// <summary>
        /// The IEdmUntypedType instance from EdmCoreModel.
        /// </summary>
        private readonly IEdmUntypedType definition;

        public CsdlSemanticsUntypedTypeReference(CsdlSemanticsModel model, CsdlUntypedTypeReference reference)
            : base(reference)
        {
            Model = model;
            this.Reference = reference;
            this.definition = EdmCoreModel.Instance.GetUntypedType();
        }

        public bool IsNullable => this.Reference.IsNullable;

        public IEdmType Definition => this.definition;

        public override CsdlSemanticsModel Model { get; }

        public override CsdlElement Element => this.Reference;

        public override string ToString()
        {
            return this.ToTraceString();
        }
    }
}
