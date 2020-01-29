//---------------------------------------------------------------------
// <copyright file="CsdlPropertyPathExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL property Path expression.
    /// </summary>
    internal class CsdlPropertyPathExpression : CsdlPathExpression
    {
        public CsdlPropertyPathExpression(string path, CsdlLocation location)
            : base(path, location)
        {
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.PropertyPath; }
        }
    }
}
