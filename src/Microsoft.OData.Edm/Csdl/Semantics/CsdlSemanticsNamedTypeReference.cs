//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsNamedTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlNamedTypeReference.
    /// </summary>
    internal class CsdlSemanticsNamedTypeReference : CsdlSemanticsElement, IEdmTypeReference
    {
        private readonly CsdlNamedTypeReference reference;

        private readonly Cache<CsdlSemanticsNamedTypeReference, IEdmType> definitionCache = new Cache<CsdlSemanticsNamedTypeReference, IEdmType>();
        private static readonly Func<CsdlSemanticsNamedTypeReference, IEdmType> ComputeDefinitionFunc = (me) => me.ComputeDefinition();

        public CsdlSemanticsNamedTypeReference(CsdlSemanticsModel model, CsdlNamedTypeReference reference)
            : base(reference)
        {
            Model = model;
            this.reference = reference;
        }

        public IEdmType Definition
        {
            get { return this.definitionCache.GetValue(this, ComputeDefinitionFunc, null); }
        }

        public bool IsNullable
        {
            get { return this.reference.IsNullable; }
        }

        public override CsdlSemanticsModel Model { get; }

        public override CsdlElement Element => this.reference;

        public override string ToString()
        {
            return this.ToTraceString();
        }

        private IEdmType ComputeDefinition()
        {
            IEdmType binding = this.Model.FindType(this.reference.FullName);

            return binding ?? new UnresolvedType(this.Model.ReplaceAlias(this.reference.FullName), this.Location);
        }
    }
}
