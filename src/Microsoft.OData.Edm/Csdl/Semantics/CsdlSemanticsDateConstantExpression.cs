//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsDateConstantExpression.cs" company="Microsoft">
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
    internal class CsdlSemanticsDateConstantExpression : CsdlSemanticsExpression, IEdmDateConstantExpression, IEdmCheckable
    {
        private readonly CsdlConstantExpression expression;

        private readonly Cache<CsdlSemanticsDateConstantExpression, Date> valueCache = new Cache<CsdlSemanticsDateConstantExpression, Date>();
        private static readonly Func<CsdlSemanticsDateConstantExpression, Date> ComputeValueFunc = (me) => me.ComputeValue();

        private readonly Cache<CsdlSemanticsDateConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsDateConstantExpression, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsDateConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

        public CsdlSemanticsDateConstantExpression(CsdlConstantExpression expression, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
        }

        public override CsdlElement Element
        {
            get { return this.expression; }
        }

        public Date Value
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

        private Date ComputeValue()
        {
            Date? value;
            return EdmValueParser.TryParseDate(this.expression.Value, out value) ? value.Value : Date.MinValue;
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            Date? value;
            if (!EdmValueParser.TryParseDate(this.expression.Value, out value))
            {
                return new EdmError[] { new EdmError(this.Location, EdmErrorCode.InvalidDate, Edm.Strings.ValueParser_InvalidDate(this.expression.Value)) };
            }
            else
            {
                return Enumerable.Empty<EdmError>();
            }
        }
    }
}
