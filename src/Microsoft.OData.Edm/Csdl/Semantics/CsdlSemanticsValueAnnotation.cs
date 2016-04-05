//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsValueAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlAnnotation.
    /// </summary>
    internal class CsdlSemanticsValueAnnotation : CsdlSemanticsVocabularyAnnotation, IEdmValueAnnotation
    {
        private readonly Cache<CsdlSemanticsValueAnnotation, IEdmExpression> valueCache = new Cache<CsdlSemanticsValueAnnotation, IEdmExpression>();
        private static readonly Func<CsdlSemanticsValueAnnotation, IEdmExpression> ComputeValueFunc = (me) => me.ComputeValue();

        public CsdlSemanticsValueAnnotation(CsdlSemanticsSchema schema, IEdmVocabularyAnnotatable targetContext, CsdlSemanticsAnnotations annotationsContext, CsdlAnnotation annotation, string externalQualifier)
            : base(schema, targetContext, annotationsContext, annotation, externalQualifier)
        {
        }

        public IEdmExpression Value
        {
            get { return this.valueCache.GetValue(this, ComputeValueFunc, null); }
        }

        protected override IEdmTerm ComputeTerm()
        {
            return this.Schema.FindValueTerm(this.Annotation.Term) as IEdmValueTerm ?? new UnresolvedValueTerm(this.Schema.UnresolvedName(this.Annotation.Term));
        }

        private IEdmExpression ComputeValue()
        {
            return CsdlSemanticsModel.WrapExpression((this.Annotation).Expression, TargetBindingContext, this.Schema);
        }
    }
}
