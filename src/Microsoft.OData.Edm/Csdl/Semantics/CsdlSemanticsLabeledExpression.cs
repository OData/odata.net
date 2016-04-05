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
        private readonly CsdlSemanticsSchema schema;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsLabeledExpression, IEdmExpression> expressionCache = new Cache<CsdlSemanticsLabeledExpression, IEdmExpression>();
        private static readonly Func<CsdlSemanticsLabeledExpression, IEdmExpression> ComputeExpressionFunc = (me) => me.ComputeExpression();

        public CsdlSemanticsLabeledExpression(string name, CsdlExpressionBase element, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
            : base(element)
        {
            this.name = name;
            this.sourceElement = element;
            this.bindingContext = bindingContext;
            this.schema = schema;
        }

        public override CsdlElement Element
        {
            get { return this.sourceElement; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.schema.Model; }
        }

        public IEdmEntityType BindingContext
        {
            get { return this.bindingContext; }
        }

        public IEdmExpression Expression
        {
            get { return this.expressionCache.GetValue(this, ComputeExpressionFunc, null); }
        }

        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Labeled; }
        }

        public string Name
        {
            get { return this.name; }
        }

        private IEdmExpression ComputeExpression()
        {
            return CsdlSemanticsModel.WrapExpression(this.sourceElement, this.BindingContext, this.schema);
        }
    }
}
