﻿//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal abstract class CsdlSemanticsExpression : CsdlSemanticsElement, IEdmExpression
    {
        protected CsdlSemanticsExpression(CsdlSemanticsModel model, CsdlExpressionBase element)
            : base(element)
        {
            Model = model;
        }

        public abstract EdmExpressionKind ExpressionKind { get; }

        public override CsdlSemanticsModel Model { get; }
    }
}
