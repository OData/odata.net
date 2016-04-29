//---------------------------------------------------------------------
// <copyright file="CsdlPropertyReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    internal class CsdlPropertyReferenceExpression : CsdlExpressionBase
    {
        private readonly string property;
        private readonly CsdlExpressionBase baseExpression;

        public CsdlPropertyReferenceExpression(string property, CsdlExpressionBase baseExpression, CsdlLocation location)
            : base(location)
        {
            this.property = property;
            this.baseExpression = baseExpression;
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.PropertyReference; }
        }

        public string Property
        {
            get { return this.property; }
        }

        public CsdlExpressionBase BaseExpression
        {
            get { return this.baseExpression; }
        }
    }
}
