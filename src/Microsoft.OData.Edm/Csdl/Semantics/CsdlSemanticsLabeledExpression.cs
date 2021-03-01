//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsLabeledExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class CsdlSemanticsLabeledExpression : CsdlSemanticsElement, IEdmLabeledExpression
    {
        private readonly string name;
        private readonly CsdlExpressionBase sourceElement;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsLabeledExpression, IEdmExpression> expressionCache = new Cache<CsdlSemanticsLabeledExpression, IEdmExpression>();
        private static readonly Func<CsdlSemanticsLabeledExpression, IEdmExpression> ComputeExpressionFunc = (me) => me.ComputeExpression();

        public CsdlSemanticsLabeledExpression(string name, CsdlExpressionBase element, IEdmEntityType bindingContext, CsdlSemanticsModel model)
            : base(element)
        {
            this.name = name;
            this.sourceElement = element;
            this.bindingContext = bindingContext;
            this.Model = model;
        }

        public override CsdlElement Element => this.sourceElement;

        public override CsdlSemanticsModel Model { get; }

        public IEdmEntityType BindingContext => this.bindingContext;

        public IEdmExpression Expression
        {
            get { return this.expressionCache.GetValue(this, ComputeExpressionFunc, null); }
        }

        public EdmExpressionKind ExpressionKind => EdmExpressionKind.Labeled;

        public string Name => this.name;

        private IEdmExpression ComputeExpression()
        {
            return CsdlSemanticsModel.WrapExpression(this.sourceElement, this.BindingContext, this.Model);
        }
    }
}
