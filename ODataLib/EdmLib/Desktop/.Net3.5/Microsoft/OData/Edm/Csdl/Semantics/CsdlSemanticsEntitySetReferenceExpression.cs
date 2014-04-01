//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class CsdlSemanticsEntitySetReferenceExpression : CsdlSemanticsExpression, IEdmEntitySetReferenceExpression, IEdmCheckable
    {
        private readonly CsdlEntitySetReferenceExpression expression;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsEntitySetReferenceExpression, IEdmEntitySet> referencedCache = new Cache<CsdlSemanticsEntitySetReferenceExpression, IEdmEntitySet>();
        private static readonly Func<CsdlSemanticsEntitySetReferenceExpression, IEdmEntitySet> ComputeReferencedFunc = (me) => me.ComputeReferenced();

        public CsdlSemanticsEntitySetReferenceExpression(CsdlEntitySetReferenceExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
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
            get { return EdmExpressionKind.EntitySetReference; }
        }

        public IEdmEntitySet ReferencedEntitySet
        {
            get { return this.referencedCache.GetValue(this, ComputeReferencedFunc, null); }
        }

        public IEnumerable<EdmError> Errors
        {
            get
            {
                if (this.ReferencedEntitySet is IUnresolvedElement)
                {
                    return this.ReferencedEntitySet.Errors();
                }

                return Enumerable.Empty<EdmError>();
            }
        }

        private IEdmEntitySet ComputeReferenced()
        {
            string[] path = this.expression.EntitySetPath.Split('/');
            Debug.Assert(path.Count() == 2, "Enum member path was not correctly split");

            return new UnresolvedEntitySet(path[1], new UnresolvedEntityContainer(path[0], this.Location), this.Location);
        }
    }
}
