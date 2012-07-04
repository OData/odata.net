//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlValueAnnotation.
    /// </summary>
    internal class CsdlSemanticsValueAnnotation : CsdlSemanticsVocabularyAnnotation, IEdmValueAnnotation
    {
        private readonly Cache<CsdlSemanticsValueAnnotation, IEdmExpression> valueCache = new Cache<CsdlSemanticsValueAnnotation, IEdmExpression>();
        private static readonly Func<CsdlSemanticsValueAnnotation, IEdmExpression> ComputeValueFunc = (me) => me.ComputeValue();

        public CsdlSemanticsValueAnnotation(CsdlSemanticsSchema schema, IEdmVocabularyAnnotatable targetContext, CsdlSemanticsAnnotations annotationsContext, CsdlValueAnnotation annotation, string externalQualifier)
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
            return CsdlSemanticsModel.WrapExpression(((CsdlValueAnnotation)this.Annotation).Expression, TargetBindingContext, this.Schema);
        }
    }
}
