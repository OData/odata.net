//---------------------------------------------------------------------
// <copyright file="CsdlEntitySetReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    internal class CsdlEntitySetReferenceExpression : CsdlExpressionBase
    {
        private readonly string entitySetPath;

        public CsdlEntitySetReferenceExpression(string entitySetPath, CsdlLocation location)
            : base(location)
        {
            this.entitySetPath = entitySetPath;
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.EntitySetReference; }
        }

        public string EntitySetPath
        {
            get { return this.entitySetPath; }
        }
    }
}
