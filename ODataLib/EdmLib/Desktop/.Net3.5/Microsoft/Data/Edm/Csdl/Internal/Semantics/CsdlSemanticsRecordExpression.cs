//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a Csdl Record expression.
    /// </summary>
    internal class CsdlSemanticsRecordExpression : CsdlSemanticsExpression, IEdmRecordExpression
    {
        private readonly CsdlRecordExpression expression;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsRecordExpression, IEdmStructuredTypeReference> declaredTypeCache = new Cache<CsdlSemanticsRecordExpression, IEdmStructuredTypeReference>();
        private static readonly Func<CsdlSemanticsRecordExpression, IEdmStructuredTypeReference> ComputeDeclaredTypeFunc = (me) => me.ComputeDeclaredType();

        private readonly Cache<CsdlSemanticsRecordExpression, IEnumerable<IEdmPropertyConstructor>> propertiesCache = new Cache<CsdlSemanticsRecordExpression, IEnumerable<IEdmPropertyConstructor>>();
        private static readonly Func<CsdlSemanticsRecordExpression, IEnumerable<IEdmPropertyConstructor>> ComputePropertiesFunc = (me) => me.ComputeProperties();

        public CsdlSemanticsRecordExpression(CsdlRecordExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
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
            get { return EdmExpressionKind.Record; }
        }

        public IEdmStructuredTypeReference DeclaredType
        {
            get { return this.declaredTypeCache.GetValue(this, ComputeDeclaredTypeFunc, null); }
        }

        public IEnumerable<IEdmPropertyConstructor> Properties
        {
            get { return this.propertiesCache.GetValue(this, ComputePropertiesFunc, null); }
        }

        public IEdmEntityType BindingContext
        {
            get { return this.bindingContext; }
        }

        private IEnumerable<IEdmPropertyConstructor> ComputeProperties()
        {
            List<IEdmPropertyConstructor> properties = new List<IEdmPropertyConstructor>();

            foreach (CsdlPropertyValue propertyValue in this.expression.PropertyValues)
            {
                properties.Add(new CsdlSemanticsPropertyConstructor(propertyValue, this));
            }

            return properties;
        }

        private IEdmStructuredTypeReference ComputeDeclaredType()
        {
            return this.expression.Type != null ? CsdlSemanticsModel.WrapTypeReference(this.Schema, this.expression.Type).AsStructured() : null;
        }
    }
}
