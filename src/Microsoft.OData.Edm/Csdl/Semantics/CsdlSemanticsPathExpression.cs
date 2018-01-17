//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsPathExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a Csdl Path expression.
    /// </summary>
    internal class CsdlSemanticsPathExpression : CsdlSemanticsExpression, IEdmPathExpression
    {
        protected readonly CsdlPathExpression Expression;

        protected readonly IEdmEntityType BindingContext;

        protected readonly Cache<CsdlSemanticsPathExpression, IEnumerable<string>> PathCache = new Cache<CsdlSemanticsPathExpression, IEnumerable<string>>();

        protected static readonly Func<CsdlSemanticsPathExpression, IEnumerable<string>> ComputePathFunc = (me) => me.ComputePath();

        public CsdlSemanticsPathExpression(CsdlPathExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.Expression = expression;
            this.BindingContext = bindingContext;
        }

        public override CsdlElement Element
        {
            get { return this.Expression; }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Path; }
        }

        public IEnumerable<string> PathSegments
        {
            get { return this.PathCache.GetValue(this, ComputePathFunc, null); }
        }

        public string Path
        {
            get { return this.Expression.Path; }
        }

        private IEnumerable<string> ComputePath()
        {
            return this.Expression.Path.Split(new char[] { '/' }, StringSplitOptions.None);
        }
    }
}