//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a Csdl collection expression.
    /// </summary>
    internal class CsdlSemanticsCollectionExpression : CsdlSemanticsExpression, IEdmCollectionExpression
    {
        private readonly CsdlCollectionExpression expression;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsCollectionExpression, IEdmTypeReference> declaredTypeCache = new Cache<CsdlSemanticsCollectionExpression, IEdmTypeReference>();
        private static readonly Func<CsdlSemanticsCollectionExpression, IEdmTypeReference> ComputeDeclaredTypeFunc = (me) => me.ComputeDeclaredType();

        private readonly Cache<CsdlSemanticsCollectionExpression, IEnumerable<IEdmExpression>> elementsCache = new Cache<CsdlSemanticsCollectionExpression, IEnumerable<IEdmExpression>>();
        private static readonly Func<CsdlSemanticsCollectionExpression, IEnumerable<IEdmExpression>> ComputeElementsFunc = (me) => me.ComputeElements();

        public CsdlSemanticsCollectionExpression(CsdlCollectionExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
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
            get { return EdmExpressionKind.Collection; }
        }

        public IEdmTypeReference DeclaredType
        {
            get { return this.declaredTypeCache.GetValue(this, ComputeDeclaredTypeFunc, null); }
        }

        public IEnumerable<IEdmExpression> Elements
        {
            get { return this.elementsCache.GetValue(this, ComputeElementsFunc, null); }
        }

        private IEnumerable<IEdmExpression> ComputeElements()
        {
            List<IEdmExpression> elements = new List<IEdmExpression>();

            foreach (CsdlExpressionBase elementValue in this.expression.ElementValues)
            {
                elements.Add(CsdlSemanticsModel.WrapExpression(elementValue, this.bindingContext, this.Schema));
            }

            return elements;
        }

        private IEdmTypeReference ComputeDeclaredType()
        {
            return this.expression.Type != null ? CsdlSemanticsModel.WrapTypeReference(this.Schema, this.expression.Type) : null;
        }
    }
}
