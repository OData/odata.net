//---------------------------------------------------------------------
// <copyright file="CsdlLabeledExpressionReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    internal class CsdlLabeledExpressionReferenceExpression : CsdlExpressionBase
    {
        private readonly string label;

        public CsdlLabeledExpressionReferenceExpression(string label, CsdlLocation location)
            : base(location)
        {
            this.label = label;
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.LabeledExpressionReference; }
        }

        public string Label
        {
            get { return this.label; }
        }
    }
}
