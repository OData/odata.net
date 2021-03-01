//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsIfExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class CsdlSemanticsIfExpression : CsdlSemanticsExpression, IEdmIfExpression
    {
        private readonly CsdlIfExpression expression;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsIfExpression, IEdmExpression> testCache = new Cache<CsdlSemanticsIfExpression, IEdmExpression>();
        private static readonly Func<CsdlSemanticsIfExpression, IEdmExpression> ComputeTestFunc = (me) => me.ComputeTest();

        private readonly Cache<CsdlSemanticsIfExpression, IEdmExpression> ifTrueCache = new Cache<CsdlSemanticsIfExpression, IEdmExpression>();
        private static readonly Func<CsdlSemanticsIfExpression, IEdmExpression> ComputeIfTrueFunc = (me) => me.ComputeIfTrue();

        private readonly Cache<CsdlSemanticsIfExpression, IEdmExpression> ifFalseCache = new Cache<CsdlSemanticsIfExpression, IEdmExpression>();
        private static readonly Func<CsdlSemanticsIfExpression, IEdmExpression> ComputeIfFalseFunc = (me) => me.ComputeIfFalse();

        public CsdlSemanticsIfExpression(CsdlIfExpression expression, IEdmEntityType bindingContext, CsdlSemanticsModel model)
            : base(model, expression)
        {
            this.expression = expression;
            this.bindingContext = bindingContext;
        }

        public override CsdlElement Element => this.expression;


        public IEdmEntityType BindingContext => this.bindingContext;

        public override EdmExpressionKind ExpressionKind => EdmExpressionKind.If;

        public IEdmExpression TestExpression
        {
            get { return this.testCache.GetValue(this, ComputeTestFunc, null); }
        }

        public IEdmExpression TrueExpression
        {
            get { return this.ifTrueCache.GetValue(this, ComputeIfTrueFunc, null); }
        }

        public IEdmExpression FalseExpression
        {
            get { return this.ifFalseCache.GetValue(this, ComputeIfFalseFunc, null); }
        }

        private IEdmExpression ComputeTest()
        {
            return CsdlSemanticsModel.WrapExpression(this.expression.Test, this.BindingContext, this.Model);
        }

        private IEdmExpression ComputeIfTrue()
        {
            return CsdlSemanticsModel.WrapExpression(this.expression.IfTrue, this.BindingContext, this.Model);
        }

        private IEdmExpression ComputeIfFalse()
        {
            return CsdlSemanticsModel.WrapExpression(this.expression.IfFalse, this.BindingContext, this.Model);
        }
    }
}
