//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsPropertyReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
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
            return new UnresolvedProperty(this.bindingContext ?? new BadEntityType("", new EdmError[] { }), this.expression.Property, this.Location);
        }
    }
}
