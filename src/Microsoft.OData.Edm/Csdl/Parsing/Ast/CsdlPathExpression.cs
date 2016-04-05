//---------------------------------------------------------------------
// <copyright file="CsdlPathExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL Path expression.
    /// </summary>
    internal class CsdlPathExpression : CsdlExpressionBase
    {
        private readonly string path;

        public CsdlPathExpression(string path, CsdlLocation location)
            : base(location)
        {
            this.path = path;
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Path; }
        }

        public string Path
        {
            get { return this.path; }
        }
    }
}