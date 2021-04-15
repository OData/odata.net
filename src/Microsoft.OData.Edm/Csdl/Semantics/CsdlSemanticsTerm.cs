//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsTerm.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlTerm.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("CsdlSemanticsTerm({Name})")]
    internal class CsdlSemanticsTerm : CsdlSemanticsElement, IEdmTerm, IEdmFullNamedElement
    {
        protected readonly CsdlSemanticsSchema Context;
        protected CsdlTerm term;

        private readonly string fullName;

        private readonly Cache<CsdlSemanticsTerm, IEdmTypeReference> typeCache = new Cache<CsdlSemanticsTerm, IEdmTypeReference>();
        private static readonly Func<CsdlSemanticsTerm, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        public CsdlSemanticsTerm(CsdlSemanticsSchema context, CsdlTerm valueTerm)
            : base(valueTerm)
        {
            this.Context = context;
            this.term = valueTerm;
            this.fullName = EdmUtil.GetFullNameForSchemaElement(this.Context?.Namespace, this.term?.Name);
        }

        public string Name => this.term.Name;

        public string Namespace => this.Context.Namespace;

        /// <summary>
        /// Gets the full name of this schema element.
        /// </summary>
        public string FullName => this.fullName;

        public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.Term;

        public IEdmTypeReference Type
        {
            get { return this.typeCache.GetValue(this, ComputeTypeFunc, null); }
        }

        public string AppliesTo => this.term.AppliesTo;

        public string DefaultValue => this.term.DefaultValue;

        public override CsdlSemanticsModel Model => this.Context.Model;

        public override CsdlElement Element => this.term;

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.Context);
        }

        private IEdmTypeReference ComputeType()
        {
            return CsdlSemanticsModel.WrapTypeReference(this.Model, this.term.Type);
        }
    }
}
