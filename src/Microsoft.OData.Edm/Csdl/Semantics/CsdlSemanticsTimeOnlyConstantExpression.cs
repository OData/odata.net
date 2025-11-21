//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsTimeOnlyConstantExpression.cs" company="Microsoft">
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
    /// Provides semantics for a Csdl TimeOnly constant expression.
    /// </summary>
    internal class CsdlSemanticsTimeOnlyConstantExpression : CsdlSemanticsExpression, IEdmTimeOfDayConstantExpression, IEdmCheckable
    {
        private readonly CsdlConstantExpression expression;

        private readonly Cache<CsdlSemanticsTimeOnlyConstantExpression, TimeOnly> valueCache = new Cache<CsdlSemanticsTimeOnlyConstantExpression, TimeOnly>();
        private static readonly Func<CsdlSemanticsTimeOnlyConstantExpression, TimeOnly> ComputeValueFunc = (me) => me.ComputeValue();

        private readonly Cache<CsdlSemanticsTimeOnlyConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsTimeOnlyConstantExpression, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsTimeOnlyConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

        public CsdlSemanticsTimeOnlyConstantExpression(CsdlConstantExpression expression, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
        }

        public override CsdlElement Element
        {
            get { return this.expression; }
        }

        public TimeOnly Value
        {
            get { return this.valueCache.GetValue(this, ComputeValueFunc, null); }
        }

        public IEdmTypeReference Type
        {
            get { return null; }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.TimeOfDayConstant; }
        }

        public EdmValueKind ValueKind
        {
            get { return this.expression.ValueKind; }
        }

        public IEnumerable<EdmError> Errors
        {
            get { return this.errorsCache.GetValue(this, ComputeErrorsFunc, null); }
        }

        private TimeOnly ComputeValue()
        {
            TimeOnly? value;
            return EdmValueParser.TryParseTimeOnly(this.expression.Value, out value) ? value.Value : TimeOnly.MinValue;
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            TimeOnly? value;
            if (!EdmValueParser.TryParseTimeOnly(this.expression.Value, out value))
            {
                return new EdmError[] { new EdmError(this.Location, EdmErrorCode.InvalidTimeOfDay, Error.Format(SRResources.ValueParser_InvalidTimeOnly, this.expression.Value)) };
            }
            else
            {
                return Enumerable.Empty<EdmError>();
            }
        }
    }
}
