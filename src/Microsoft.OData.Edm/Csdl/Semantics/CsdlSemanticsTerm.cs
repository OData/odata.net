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
    internal class CsdlSemanticsTerm : CsdlSemanticsElement, IEdmTerm
    {
        protected readonly CsdlSemanticsSchema Context;
        protected CsdlTerm term;

        private readonly Cache<CsdlSemanticsTerm, IEdmTypeReference> typeCache = new Cache<CsdlSemanticsTerm, IEdmTypeReference>();
        private static readonly Func<CsdlSemanticsTerm, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        public CsdlSemanticsTerm(CsdlSemanticsSchema context, CsdlTerm valueTerm)
            : base(valueTerm)
        {
            this.Context = context;
            this.term = valueTerm;
        }

        public string Name
        {
            get { return this.term.Name; }
        }

        public string Namespace
        {
            get { return this.Context.Namespace; }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.Term; }
        }

        public IEdmTypeReference Type
        {
            get { return this.typeCache.GetValue(this, ComputeTypeFunc, null); }
        }

        public string AppliesTo
        {
            get { return this.term.AppliesTo; }
        }

        public string DefaultValue
        {
            get { return this.term.DefaultValue; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.Context.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.term; }
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.Context);
        }

        private IEdmTypeReference ComputeType()
        {
            return CsdlSemanticsModel.WrapTypeReference(this.Context, this.term.Type);
        }
    }
}
