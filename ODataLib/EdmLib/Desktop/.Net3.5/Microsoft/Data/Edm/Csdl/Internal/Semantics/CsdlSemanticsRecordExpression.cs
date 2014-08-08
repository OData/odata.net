//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
