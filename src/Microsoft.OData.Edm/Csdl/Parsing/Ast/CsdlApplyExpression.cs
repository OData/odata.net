//---------------------------------------------------------------------
// <copyright file="CsdlApplyExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    internal class CsdlApplyExpression : CsdlExpressionBase
    {
        private readonly string function;
        private readonly List<CsdlExpressionBase> arguments;

        public CsdlApplyExpression(string function, IEnumerable<CsdlExpressionBase> arguments, CsdlLocation location)
            : base(location)
        {
            this.function = function;
            this.arguments = new List<CsdlExpressionBase>(arguments);
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.FunctionApplication; }
        }

        public string Function
        {
            get { return this.function; }
        }

        public IEnumerable<CsdlExpressionBase> Arguments
        {
            get { return this.arguments; }
        }
    }
}
