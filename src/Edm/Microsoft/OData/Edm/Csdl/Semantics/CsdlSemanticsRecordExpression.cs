//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
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
