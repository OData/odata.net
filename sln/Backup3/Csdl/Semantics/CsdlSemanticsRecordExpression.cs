//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsRecordExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

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
