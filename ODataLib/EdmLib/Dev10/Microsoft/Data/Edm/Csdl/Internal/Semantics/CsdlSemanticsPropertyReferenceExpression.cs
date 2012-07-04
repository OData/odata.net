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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    internal class CsdlSemanticsPropertyReferenceExpression : CsdlSemanticsExpression, IEdmPropertyReferenceExpression, IEdmCheckable
    {
        private readonly CsdlPropertyReferenceExpression expression;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsPropertyReferenceExpression, IEdmExpression> baseCache = new Cache<CsdlSemanticsPropertyReferenceExpression, IEdmExpression>();
        private static readonly Func<CsdlSemanticsPropertyReferenceExpression, IEdmExpression> ComputeBaseFunc = (me) => me.ComputeBase();

        private readonly Cache<CsdlSemanticsPropertyReferenceExpression, IEdmProperty> elementCache = new Cache<CsdlSemanticsPropertyReferenceExpression, IEdmProperty>();
        private static readonly Func<CsdlSemanticsPropertyReferenceExpression, IEdmProperty> ComputeReferencedFunc = (me) => me.ComputeReferenced();

        public CsdlSemanticsPropertyReferenceExpression(CsdlPropertyReferenceExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
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
            get { return EdmExpressionKind.PropertyReference; }
        }

        public IEdmExpression Base
        {
            get { return this.baseCache.GetValue(this, ComputeBaseFunc, null); }
        }

        public IEdmProperty ReferencedProperty
        {
            get { return this.elementCache.GetValue(this, ComputeReferencedFunc, null); }
        }

        public IEnumerable<EdmError> Errors
        {
            get
            {
                if (this.ReferencedProperty is IUnresolvedElement)
                {
                    return this.ReferencedProperty.Errors();
                }

                return Enumerable.Empty<EdmError>();
            }
        }

        private IEdmExpression ComputeBase()
        {
            return (this.expression.BaseExpression != null) ? CsdlSemanticsModel.WrapExpression(this.expression.BaseExpression, this.bindingContext, this.Schema) : null;
        }

        private IEdmProperty ComputeReferenced()
        {
            return new UnresolvedProperty(this.bindingContext ?? new Library.Internal.BadEntityType("", new EdmError[] { }), this.expression.Property, this.Location);
        }
    }
}
