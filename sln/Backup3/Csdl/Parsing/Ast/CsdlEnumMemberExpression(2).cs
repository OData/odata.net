//---------------------------------------------------------------------
// <copyright file="CsdlEnumMemberExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    internal class CsdlEnumMemberExpression : CsdlExpressionBase
    {
        private readonly string enumMemberPath;

        public CsdlEnumMemberExpression(string enumMemberPath, CsdlLocation location)
            : base(location)
        {
            this.enumMemberPath = enumMemberPath;
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.EnumMember; }
        }

        public string EnumMemberPath
        {
            get { return this.enumMemberPath; }
        }
    }
}
