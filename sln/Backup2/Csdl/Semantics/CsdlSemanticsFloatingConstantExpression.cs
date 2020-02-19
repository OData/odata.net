//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsFloatingConstantExpression.cs" company="Microsoft">
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
    /// Provides semantics for a Csdl Float constant expression.
    /// </summary>
    internal class CsdlSemanticsFloatingConstantExpression : CsdlSemanticsExpression, IEdmFloatingConstantExpression, IEdmCheckable
    {
        private readonly CsdlConstantExpression expression;

        private readonly Cache<CsdlSemanticsFloatingConstantExpression, double> valueCache = new Cache<CsdlSemanticsFloatingConstantExpression, double>();
        private static readonly Func<CsdlSemanticsFloatingConstantExpression, double> ComputeValueFunc = (me) => me.ComputeValue();

        private readonly Cache<CsdlSemanticsFloatingConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsFloatingConstantExpression, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsFloatingConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

        public CsdlSemanticsFloatingConstantExpression(CsdlConstantExpression expression, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
        }

        public override CsdlElement Element
        {
            get { return this.expression; }
        }

        public double Value
        {
            get { return this.valueCache.GetValue(this, ComputeValueFunc, null); }
        }

        public IEdmTypeReference Type
        {
            get { return null; }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.FloatingConstant; }
        }

        public EdmValueKind ValueKind
        {
            get { return this.expression.ValueKind; }
        }

        public IEnumerable<EdmError> Errors
        {
            get { return this.errorsCache.GetValue(this, ComputeErrorsFunc, null); }
        }

        private double ComputeValue()
        {
            double? value;
            return EdmValueParser.TryParseFloat(this.expression.Value, out value) ? value.Value : 0;
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            double? value;
            if (!EdmValueParser.TryParseFloat(this.expression.Value, out value))
            {
                return new EdmError[] { new EdmError(this.Location, EdmErrorCode.InvalidFloatingPoint, Edm.Strings.ValueParser_InvalidFloatingPoint(this.expression.Value)) };
            }
            else
            {
                return Enumerable.Empty<EdmError>();
            }
        }
    }
}
