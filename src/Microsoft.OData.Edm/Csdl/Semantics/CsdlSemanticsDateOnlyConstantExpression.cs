//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsDateOnlyConstantExpression.cs" company="Microsoft">
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
    /// Provides semantics for a Csdl date constant expression.
    /// </summary>
    internal class CsdlSemanticsDateOnlyConstantExpression : CsdlSemanticsExpression, IEdmDateConstantExpression, IEdmCheckable
    {
        private readonly CsdlConstantExpression expression;

        private readonly Cache<CsdlSemanticsDateOnlyConstantExpression, DateOnly> valueCache = new Cache<CsdlSemanticsDateOnlyConstantExpression, DateOnly>();
        private static readonly Func<CsdlSemanticsDateOnlyConstantExpression, DateOnly> ComputeValueFunc = (me) => me.ComputeValue();

        private readonly Cache<CsdlSemanticsDateOnlyConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsDateOnlyConstantExpression, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsDateOnlyConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

        public CsdlSemanticsDateOnlyConstantExpression(CsdlConstantExpression expression, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
        }

        public override CsdlElement Element
        {
            get { return this.expression; }
        }

        public DateOnly Value
        {
            get { return this.valueCache.GetValue(this, ComputeValueFunc, null); }
        }

        public IEdmTypeReference Type
        {
            get { return null; }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.DateConstant; }
        }

        public EdmValueKind ValueKind
        {
            get { return this.expression.ValueKind; }
        }

        public IEnumerable<EdmError> Errors
        {
            get { return this.errorsCache.GetValue(this, ComputeErrorsFunc, null); }
        }

        private DateOnly ComputeValue()
        {
            DateOnly? value;
            return EdmValueParser.TryParseDateOnly(this.expression.Value, out value) ? value.Value : DateOnly.MinValue;
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            DateOnly? value;
            if (!EdmValueParser.TryParseDateOnly(this.expression.Value, out value))
            {
                return new EdmError[] { new EdmError(this.Location, EdmErrorCode.InvalidDate, Error.Format(SRResources.ValueParser_InvalidDateOnly, this.expression.Value)) };
            }
            else
            {
                return Enumerable.Empty<EdmError>();
            }
        }
    }
}
