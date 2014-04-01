//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Internal;

namespace Microsoft.OData.Edm.Csdl.Internal.CsdlSemantics
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
