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
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
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

        public EdmConcurrencyMode ConcurrencyMode
        {
            get { return this.property.IsFixedConcurrency ? EdmConcurrencyMode.Fixed : EdmConcurrencyMode.None; }
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
