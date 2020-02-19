//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsIntConstantExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a Csdl Int constant expression.
    /// </summary>
    internal class CsdlSemanticsIntConstantExpression : CsdlSemanticsExpression, IEdmIntegerConstantExpression, IEdmCheckable
    {
        private readonly CsdlConstantExpression expression;

        private readonly Cache<CsdlSemanticsIntConstantExpression, Int64> valueCache = new Cache<CsdlSemanticsIntConstantExpression, Int64>();
        private static readonly Func<CsdlSemanticsIntConstantExpression, Int64> ComputeValueFunc = (me) => me.ComputeValue();

        private readonly Cache<CsdlSemanticsIntConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsIntConstantExpression, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsIntConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

        public CsdlSemanticsIntConstantExpression(CsdlConstantExpression expression, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
        }

        public override CsdlElement Element
        {
            get { return this.expression; }
        }

        public Int64 Value
        {
            get { return this.valueCache.GetValue(this, ComputeValueFunc, null); }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.IntegerConstant; }
        }

        public EdmValueKind ValueKind
        {
            get { return this.expression.ValueKind; }
        }

        public IEdmTypeReference Type
        {
            get { return null; }
        }

        public IEnumerable<EdmError> Errors
        {
            get { return this.errorsCache.GetValue(this, ComputeErrorsFunc, null); }
        }

        private Int64 ComputeValue()
        {
            Int64? value;
            return EdmValueParser.TryParseLong(this.expression.Value, out value) ? value.Value : 0;
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            Int64? value;
            if (!EdmValueParser.TryParseLong(this.expression.Value, out value))
            {
                return new EdmError[] { new EdmError(this.Location, EdmErrorCode.InvalidInteger, Edm.Strings.ValueParser_InvalidInteger(this.expression.Value)) };
            }
            else
            {
                return Enumerable.Empty<EdmError>();
            }
        }
    }
}
