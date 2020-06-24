//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsEnumMemberExpression.cs" company="Microsoft">
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
    internal class CsdlSemanticsEnumMemberExpression : CsdlSemanticsExpression, IEdmEnumMemberExpression, IEdmCheckable
    {
        private readonly CsdlEnumMemberExpression expression;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsEnumMemberExpression, IEnumerable<IEdmEnumMember>> referencedCache = new Cache<CsdlSemanticsEnumMemberExpression, IEnumerable<IEdmEnumMember>>();
        private static readonly Func<CsdlSemanticsEnumMemberExpression, IEnumerable<IEdmEnumMember>> ComputeReferencedFunc = (me) => me.ComputeReferenced();

        private readonly Cache<CsdlSemanticsEnumMemberExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsEnumMemberExpression, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsEnumMemberExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

        public CsdlSemanticsEnumMemberExpression(CsdlEnumMemberExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
            this.bindingContext = bindingContext;
        }

        public override CsdlElement Element
        {
            get { return this.expression; }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.EnumMember; }
        }

        public IEnumerable<IEdmEnumMember> EnumMembers
        {
            get { return this.referencedCache.GetValue(this, ComputeReferencedFunc, null); }
        }

        public IEnumerable<EdmError> Errors
        {
            get { return this.errorsCache.GetValue(this, ComputeErrorsFunc, null); }
        }

        private IEnumerable<IEdmEnumMember> ComputeReferenced()
        {
            IEnumerable<IEdmEnumMember> member;

            // In OData Json CSDL, a enum member expression is a string value. "@self.HasPattern": "Red"
            // But in OData XML CSDL, a enum member expression is an element like: <EnumMember>org.example.Pattern/Red</EnumMember>
            // So, in OData JSON CSDL, we have to use the Term type to construct EnumMember.
            if (this.expression.EnumType != null)
            {
                return EdmEnumValueParser.TryParseJsonEnumMember(this.expression.EnumMemberPath, this.expression.EnumType, this.Location, out member) ? member : null;
            }

            return EdmEnumValueParser.TryParseEnumMember(this.expression.EnumMemberPath, this.Schema.Model, this.Location, out member) ? member : null;
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            IEnumerable<IEdmEnumMember> member;

            if (this.expression.EnumType != null)
            {
                if (!EdmEnumValueParser.TryParseJsonEnumMember(this.expression.EnumMemberPath, this.expression.EnumType, this.Location, out member))
                {
                    return new EdmError[] { new EdmError(this.Location, EdmErrorCode.InvalidEnumMemberPath, Edm.Strings.CsdlParser_InvalidEnumMemberPath(this.expression.EnumMemberPath)) };
                }
            }
            else
            {
                if (!EdmEnumValueParser.TryParseEnumMember(this.expression.EnumMemberPath, this.Schema.Model, this.Location, out member))
                {
                    return new EdmError[] { new EdmError(this.Location, EdmErrorCode.InvalidEnumMemberPath, Edm.Strings.CsdlParser_InvalidEnumMemberPath(this.expression.EnumMemberPath)) };
                }
            }

            return Enumerable.Empty<EdmError>();
        }
    }
}
