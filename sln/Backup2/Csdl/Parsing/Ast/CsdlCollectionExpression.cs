//---------------------------------------------------------------------
// <copyright file="CsdlCollectionExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL Collection expression.
    /// </summary>
    internal class CsdlCollectionExpression : CsdlExpressionBase
    {
        private readonly CsdlTypeReference type;
        private readonly List<CsdlExpressionBase> elementValues;

        public CsdlCollectionExpression(CsdlTypeReference type, IEnumerable<CsdlExpressionBase> elementValues, CsdlLocation location)
            : base(location)
        {
            this.type = type;
            this.elementValues = new List<CsdlExpressionBase>(elementValues);
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Collection; }
        }

        public CsdlTypeReference Type
        {
            get { return this.type; }
        }

        public IEnumerable<CsdlExpressionBase> ElementValues
        {
            get { return this.elementValues; }
        }
    }
}
