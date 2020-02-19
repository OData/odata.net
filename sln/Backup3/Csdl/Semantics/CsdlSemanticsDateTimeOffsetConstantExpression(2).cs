//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsDateTimeOffsetConstantExpression.cs" company="Microsoft">
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
    /// Provides semantics for a Csdl datetime with offset constant expression.
    /// </summary>
    internal class CsdlSemanticsDateTimeOffsetConstantExpression : CsdlSemanticsExpression, IEdmDateTimeOffsetConstantExpression, IEdmCheckable
    {
        private readonly CsdlConstantExpression expression;

        private readonly Cache<CsdlSemanticsDateTimeOffsetConstantExpression, DateTimeOffset> valueCache = new Cache<CsdlSemanticsDateTimeOffsetConstantExpression, DateTimeOffset>();
        private static readonly Func<CsdlSemanticsDateTimeOffsetConstantExpression, DateTimeOffset> ComputeValueFunc = (me) => me.ComputeValue();

        private readonly Cache<CsdlSemanticsDateTimeOffsetConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsDateTimeOffsetConstantExpression, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsDateTimeOffsetConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

        public CsdlSemanticsDateTimeOffsetConstantExpression(CsdlConstantExpression expression, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
        }

        public override CsdlElement Element
        {
            get { return this.expression; }
        }

        public DateTimeOffset Value
        {
            get { return this.valueCache.GetValue(this, ComputeValueFunc, null); }
        }

        public IEdmTypeReference Type
        {
            get { return null; }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.DateTimeOffsetConstant; }
        }

        public EdmValueKind ValueKind
        {
            get { return this.expression.ValueKind; }
        }

        public IEnumerable<EdmError> Errors
        {
            get { return this.errorsCache.GetValue(this, ComputeErrorsFunc, null); }
        }

        private DateTimeOffset ComputeValue()
        {
            DateTimeOffset? value;
            return EdmValueParser.TryParseDateTimeOffset(this.expression.Value, out value) ? value.Value : DateTimeOffset.MinValue;
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            DateTimeOffset? value;
            if (!EdmValueParser.TryParseDateTimeOffset(this.expression.Value, out value))
            {
                return new EdmError[] { new EdmError(this.Location, EdmErrorCode.InvalidDateTimeOffset, Edm.Strings.ValueParser_InvalidDateTimeOffset(this.expression.Value)) };
            }
            else
            {
                return Enumerable.Empty<EdmError>();
            }
        }
    }
}
