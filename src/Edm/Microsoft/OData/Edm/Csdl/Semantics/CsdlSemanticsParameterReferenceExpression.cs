//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class CsdlSemanticsParameterReferenceExpression : CsdlSemanticsExpression, IEdmParameterReferenceExpression, IEdmCheckable
    {
        private readonly CsdlParameterReferenceExpression expression;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsParameterReferenceExpression, IEdmOperationParameter> referencedCache = new Cache<CsdlSemanticsParameterReferenceExpression, IEdmOperationParameter>();
        private static readonly Func<CsdlSemanticsParameterReferenceExpression, IEdmOperationParameter> ComputeReferencedFunc = (me) => me.ComputeReferenced();

        public CsdlSemanticsParameterReferenceExpression(CsdlParameterReferenceExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
            this.bindingContext = bindingContext;
        }

        public override CsdlElement Element
        {
            get { return this.expression; }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.ParameterReference; }
        }

        public IEdmOperationParameter ReferencedParameter
        {
            get { return this.referencedCache.GetValue(this, ComputeReferencedFunc, null); }
        }

        public IEnumerable<EdmError> Errors
        {
            get
            {
                if (this.ReferencedParameter is IUnresolvedElement)
                {
                    return this.ReferencedParameter.Errors();
                }

                return Enumerable.Empty<EdmError>();
            }
        }

        private IEdmOperationParameter ComputeReferenced()
        {
            return new UnresolvedParameter(new UnresolvedOperation(String.Empty, Edm.Strings.Bad_UnresolvedOperation(String.Empty), this.Location), this.expression.Parameter, this.Location);
        }
    }
}
