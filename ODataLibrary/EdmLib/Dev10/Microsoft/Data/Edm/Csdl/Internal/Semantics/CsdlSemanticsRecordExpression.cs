//   Copyright 2011 Microsoft Corporation
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
using System.Linq;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a Csdl Record expression.
    /// </summary>
    internal class CsdlSemanticsRecordExpression : IEdmRecordExpression
    {
        private readonly CsdlRecordExpression expression;
        private readonly CsdlSemanticsSchema schema;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsRecordExpression, IEdmTypeReference> declaredTypeCache = new Cache<CsdlSemanticsRecordExpression, IEdmTypeReference>();
        private readonly static Func<CsdlSemanticsRecordExpression, IEdmTypeReference> s_computeDeclaredType = (me) => me.ComputeDeclaredType();

        private readonly Cache<CsdlSemanticsRecordExpression, IEnumerable<IEdmPropertyConstructor>> propertiesCache = new Cache<CsdlSemanticsRecordExpression, IEnumerable<IEdmPropertyConstructor>>();
        private readonly static Func<CsdlSemanticsRecordExpression, IEnumerable<IEdmPropertyConstructor>> s_computeProperties = (me) => me.ComputeProperties();

        public CsdlSemanticsRecordExpression(CsdlRecordExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
        {
            this.expression = expression;
            this.bindingContext = bindingContext;
            this.schema = schema;
        }

        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Record; }
        }

        public IEdmTypeReference DeclaredType
        {
            get { return this.declaredTypeCache.GetValue(this, s_computeDeclaredType, null);}
        }

        public IEnumerable<IEdmPropertyConstructor> Properties
        {
            get { return this.propertiesCache.GetValue(this, s_computeProperties, null); }
        }

        public IEdmEntityType BindingContext
        {
            get { return this.bindingContext; }
        }

        public CsdlSemanticsSchema Schema
        {
            get { return this.schema; }
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

        private IEdmTypeReference ComputeDeclaredType()
        {
            return this.expression.Type != null ? CsdlSemanticsModel.WrapTypeReference(this.schema, this.expression.Type) : null;
        }
    }
}
