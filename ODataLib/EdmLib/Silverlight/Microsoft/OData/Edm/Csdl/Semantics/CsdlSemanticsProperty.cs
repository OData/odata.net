//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

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
