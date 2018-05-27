//---------------------------------------------------------------------
// <copyright file="CsdlLabeledExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    internal class CsdlLabeledExpression : CsdlExpressionBase
    {
        private readonly string label;
        private readonly CsdlExpressionBase element;

        public CsdlLabeledExpression(string label, CsdlExpressionBase element, CsdlLocation location)
            : base(location)
        {
            this.label = label;
            this.element = element;
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Labeled; }
        }

        public string Label
        {
            get { return this.label; }
        }

        public CsdlExpressionBase Element
        {
            get { return this.element; }
        }
    }
}
