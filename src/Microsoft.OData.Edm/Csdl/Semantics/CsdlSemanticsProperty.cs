//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsProperty.cs" company="Microsoft">
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
    /// Provides semantics for a CsdlProperty.
    /// </summary>
    internal class CsdlSemanticsProperty : CsdlSemanticsElement, IEdmStructuralProperty
    {
        protected CsdlProperty property;
        private readonly CsdlSemanticsStructuredTypeDefinition declaringType;

        private readonly Cache<CsdlSemanticsProperty, IEdmTypeReference> typeCache = new Cache<CsdlSemanticsProperty, IEdmTypeReference>();
        private static readonly Func<CsdlSemanticsProperty, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        public CsdlSemanticsProperty(CsdlSemanticsStructuredTypeDefinition declaringType, CsdlProperty property)
            : base(property)
        {
            this.property = property;
            this.declaringType = declaringType;
        }

        public string Name
        {
            get { return this.property.Name; }
        }

        public IEdmStructuredType DeclaringType
        {
            get { return this.declaringType; }
        }

        public IEdmTypeReference Type
        {
            get { return this.typeCache.GetValue(this, ComputeTypeFunc, null); }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.declaringType.Model; }
        }

        public string DefaultValueString
        {
            get { return this.property.DefaultValue; }
        }

        public EdmPropertyKind PropertyKind
        {
            get { return EdmPropertyKind.Structural; }
        }

        public override CsdlElement Element
        {
            get { return this.property; }
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.declaringType.Context);
        }

        private IEdmTypeReference ComputeType()
        {
            return CsdlSemanticsModel.WrapTypeReference(this.declaringType.Context, this.property.Type);
        }
    }
}
