//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsCollectionTypeExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class CsdlSemanticsCollectionTypeExpression : CsdlSemanticsTypeExpression, IEdmCollectionTypeReference
    {
        public CsdlSemanticsCollectionTypeExpression(CsdlExpressionTypeReference expressionUsage, CsdlSemanticsTypeDefinition type)
            : base(expressionUsage, type)
        {
        }
    }
}
