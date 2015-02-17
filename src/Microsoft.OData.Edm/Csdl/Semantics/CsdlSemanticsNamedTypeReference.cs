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
        private readonly CsdlSemanticsSchema schema;
        private readonly CsdlNamedTypeReference reference;

        private readonly Cache<CsdlSemanticsNamedTypeReference, IEdmType> definitionCache = new Cache<CsdlSemanticsNamedTypeReference, IEdmType>();
        private static readonly Func<CsdlSemanticsNamedTypeReference, IEdmType> ComputeDefinitionFunc = (me) => me.ComputeDefinition();

        public CsdlSemanticsNamedTypeReference(CsdlSemanticsSchema schema, CsdlNamedTypeReference reference)
            : base(reference)
        {
            this.schema = schema;
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

        public override CsdlSemanticsModel Model
        {
            get { return this.schema.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.reference; }
        }

        public override string ToString()
        {
            return this.ToTraceString();
        }

        private IEdmType ComputeDefinition()
        {
            IEdmType binding = this.schema.FindType(this.reference.FullName);

            return binding ?? new UnresolvedType(this.schema.ReplaceAlias(this.reference.FullName) ?? this.reference.FullName, this.Location);
        }
    }
}
