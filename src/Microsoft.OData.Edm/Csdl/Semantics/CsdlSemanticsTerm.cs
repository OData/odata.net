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
        private readonly Cache<CsdlSemanticsTerm, IEdmTerm> baseTermCache = new Cache<CsdlSemanticsTerm, IEdmTerm>();
        private static readonly Func<CsdlSemanticsTerm, IEdmTerm> ComputeBaseTermFunc = (me) => me.ComputeBaseTerm();
        private static readonly Func<CsdlSemanticsTerm, IEdmTerm> OnCycleBaseTermFunc = (me) => new CyclicTerm(me.GetCyclicBaseTermName(me.term.BaseTermName), me.Location);

        public CsdlSemanticsTerm(CsdlSemanticsSchema context, CsdlTerm valueTerm)
            : base(valueTerm)
        {
            this.Context = context;
            this.term = valueTerm;
            this.fullName = EdmUtil.GetFullNameForSchemaElement(this.Context?.Namespace, this.term?.Name);
        }

        public string Name
        {
            get { return this.term.Name; }
        }

        public string Namespace
        {
            get { return this.Context.Namespace; }
        }

        /// <summary>
        /// Gets the full name of this schema element.
        /// </summary>
        public string FullName
        {
            get { return this.fullName; }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.Term; }
        }

        public IEdmTypeReference Type
        {
            get { return this.typeCache.GetValue(this, ComputeTypeFunc, null); }
        }

        public IEdmTerm BaseTerm
        {
            get { return this.baseTermCache.GetValue(this, ComputeBaseTermFunc, OnCycleBaseTermFunc); }
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

        private IEdmTerm ComputeBaseTerm()
        {
            if (this.term.BaseTermName != null)
            {
                IEdmTerm baseTerm = this.Context.FindTerm(this.term.BaseTermName);
                if (baseTerm != null)
                {
                    // Evaluate the inductive step to detect cycles.
                    // Overriding BaseTerm getter from concrete type implementing IEdmTerm will be invoked to
                    // detect cycles. The object assignment is required by compiler only.
                    IEdmTerm baseTerm2 = baseTerm.BaseTerm;
                }

                return baseTerm ?? new UnresolvedVocabularyTerm(this.term.BaseTermName);
            }

            return null;
        }

        // Resolves the real name of the base term, in case it was using an alias before.
        protected string GetCyclicBaseTermName(string baseTermName)
        {
            IEdmTerm schemaBaseTerm = this.Context.FindTerm(baseTermName);
            return (schemaBaseTerm != null) ? schemaBaseTerm.FullName() : baseTermName;
        }
    }
}
