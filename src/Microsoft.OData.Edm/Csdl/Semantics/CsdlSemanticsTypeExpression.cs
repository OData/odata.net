//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsTypeExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for <see cref="CsdlExpressionTypeReference"/>.
    /// </summary>
    internal abstract class CsdlSemanticsTypeExpression : CsdlSemanticsElement, IEdmTypeReference
    {
        private readonly CsdlExpressionTypeReference expressionUsage;
        private readonly CsdlSemanticsTypeDefinition type;

        protected CsdlSemanticsTypeExpression(CsdlExpressionTypeReference expressionUsage, CsdlSemanticsTypeDefinition type)
            : base(expressionUsage)
        {
            this.expressionUsage = expressionUsage;
            this.type = type;
        }

        public IEdmType Definition => this.type;

        public bool IsNullable => this.expressionUsage.IsNullable;

        public override CsdlSemanticsModel Model => this.type.Model;

        public override CsdlElement Element => this.expressionUsage;

        public override string ToString()
        {
            return this.ToTraceString();
        }
    }
}
