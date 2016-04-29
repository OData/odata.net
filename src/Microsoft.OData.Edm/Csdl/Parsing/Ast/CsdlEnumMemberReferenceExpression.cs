//---------------------------------------------------------------------
// <copyright file="CsdlEnumMemberReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    internal class CsdlEnumMemberReferenceExpression : CsdlExpressionBase
    {
        private readonly string enumMemberPath;

        public CsdlEnumMemberReferenceExpression(string enumMemberPath, CsdlLocation location)
            : base(location)
        {
            this.enumMemberPath = enumMemberPath;
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.EnumMemberReference; }
        }

        public string EnumMemberPath
        {
            get { return this.enumMemberPath; }
        }
    }
}
