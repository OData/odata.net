//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
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
