//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal abstract class CsdlSemanticsExpression : CsdlSemanticsElement, IEdmExpression
    {
        private readonly CsdlSemanticsSchema schema;

        protected CsdlSemanticsExpression(CsdlSemanticsSchema schema, CsdlExpressionBase element)
            : base(element)
        {
            this.schema = schema;
        }

        public abstract EdmExpressionKind ExpressionKind
        {
            get;
        }

        public CsdlSemanticsSchema Schema
        {
            get { return this.schema; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.schema.Model; }
        }
    }
}
