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
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlValueAnnotation.
    /// </summary>
    internal class CsdlSemanticsValueAnnotation : CsdlSemanticsTermAnnotation, IEdmValueAnnotation
    {
        private readonly Cache<CsdlSemanticsValueAnnotation, IEdmExpression> valueCache = new Cache<CsdlSemanticsValueAnnotation, IEdmExpression>();
        private readonly static Func<CsdlSemanticsValueAnnotation, IEdmExpression> s_computeValue = (me) => me.ComputeValue();

        public CsdlSemanticsValueAnnotation(CsdlSemanticsSchema schema, IEdmElement targetContext, CsdlSemanticsAnnotations annotationsContext, CsdlValueAnnotation annotation, string externalQualifier)
            : base(schema, targetContext, annotationsContext, annotation, externalQualifier)
        {
        }

        public EdmAnnotationKind Kind
        {
            get { return EdmAnnotationKind.TermValue; }
        }

        public IEdmExpression Value
        {
            get { return this.valueCache.GetValue(this, s_computeValue, null); }
        }

        protected override IEdmTerm ComputeTerm()
        {
            IEdmValueTerm term = this.schema.FindValueTerm(this.annotation.Term) as IEdmValueTerm;
            if (term == null)
            {
                string namespaceName;
                string namespaceUri;
                string name;
                TermName(out namespaceName, out namespaceUri, out name);
                
                term = new BadValueTerm(namespaceName, namespaceUri, name, new EdmError[] { new EdmError(this.Location, EdmErrorCode.BadUnresolvedValueTerm, Edm.Strings.Bad_UnresolvedValueTerm(this.annotation.Term)) });
            }

            return term;
        }

        private IEdmExpression ComputeValue()
        {
            return CsdlSemanticsModel.WrapExpression(((CsdlValueAnnotation)this.annotation).Expression, TargetBindingContext, this.schema);
        }
    }
}
