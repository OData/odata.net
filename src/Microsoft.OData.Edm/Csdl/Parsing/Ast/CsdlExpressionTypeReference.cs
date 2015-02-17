//---------------------------------------------------------------------
// <copyright file="CsdlExpressionTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL type reference based on a type expression.
    /// </summary>
    internal class CsdlExpressionTypeReference : CsdlTypeReference
    {
        private readonly ICsdlTypeExpression typeExpression;

        public CsdlExpressionTypeReference(ICsdlTypeExpression typeExpression, bool isNullable, CsdlLocation location)
            : base(isNullable, location)
        {
            this.typeExpression = typeExpression;
        }

        public ICsdlTypeExpression TypeExpression
        {
            get { return this.typeExpression; }
        }
    }
}
