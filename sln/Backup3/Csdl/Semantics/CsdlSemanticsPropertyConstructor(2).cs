//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsPropertyConstructor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlPropertyValue used in a record expression.
    /// </summary>
    internal class CsdlSemanticsPropertyConstructor : CsdlSemanticsElement, IEdmPropertyConstructor
    {
        private readonly CsdlPropertyValue property;
        private readonly CsdlSemanticsRecordExpression context;

        private readonly Cache<CsdlSemanticsPropertyConstructor, IEdmExpression> valueCache = new Cache<CsdlSemanticsPropertyConstructor, IEdmExpression>();
        private static readonly Func<CsdlSemanticsPropertyConstructor, IEdmExpression> ComputeValueFunc = (me) => me.ComputeValue();

        public CsdlSemanticsPropertyConstructor(CsdlPropertyValue property, CsdlSemanticsRecordExpression context)
            : base(property)
        {
            this.property = property;
            this.context = context;
        }

        public string Name
        {
            get { return this.property.Property; }
        }

        public IEdmExpression Value
        {
            get { return this.valueCache.GetValue(this, ComputeValueFunc, null); }
        }

        public override CsdlElement Element
        {
            get { return this.property; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.context.Model; }
        }

        private IEdmExpression ComputeValue()
        {
            return CsdlSemanticsModel.WrapExpression(this.property.Expression, this.context.BindingContext, this.context.Schema);
        }
    }
}
