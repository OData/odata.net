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
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlPropertyValue used in a type annotation.
    /// </summary>
    internal class CsdlSemanticsPropertyValueBinding : CsdlSemanticsElement, IEdmPropertyValueBinding
    {
        private readonly CsdlPropertyValue property;
        private readonly CsdlSemanticsTypeAnnotation context;

        private readonly Cache<CsdlSemanticsPropertyValueBinding, IEdmExpression> valueCache = new Cache<CsdlSemanticsPropertyValueBinding, IEdmExpression>();
        private static readonly Func<CsdlSemanticsPropertyValueBinding, IEdmExpression> ComputeValueFunc = (me) => me.ComputeValue();

        private readonly Cache<CsdlSemanticsPropertyValueBinding, IEdmProperty> boundPropertyCache = new Cache<CsdlSemanticsPropertyValueBinding, IEdmProperty>();
        private static readonly Func<CsdlSemanticsPropertyValueBinding, IEdmProperty> ComputeBoundPropertyFunc = (me) => me.ComputeBoundProperty();

        public CsdlSemanticsPropertyValueBinding(CsdlSemanticsTypeAnnotation context, CsdlPropertyValue property)
            : base(property)
        {
            this.context = context;
            this.property = property;
        }

        public IEdmProperty BoundProperty
        {
            get { return this.boundPropertyCache.GetValue(this, ComputeBoundPropertyFunc, null); }
        }

        public IEdmExpression Value
        {
            get { return this.valueCache.GetValue(this, ComputeValueFunc, null); }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.context.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.property; }
        }

        private IEdmProperty ComputeBoundProperty()
        {
            IEdmProperty boundProperty = ((IEdmStructuredType)this.context.Term).FindProperty(this.property.Property);
            return boundProperty ?? new UnresolvedBoundProperty(((IEdmStructuredType)this.context.Term), this.property.Property);
        }

        private IEdmExpression ComputeValue()
        {
            return CsdlSemanticsModel.WrapExpression(this.property.Expression, this.context.TargetBindingContext, this.context.Schema);
        }

        private class UnresolvedBoundProperty : EdmElement, IEdmStructuralProperty, IUnresolvedElement
        {
            private readonly IEdmStructuredType declaringType;
            private readonly string name;
            private readonly IEdmTypeReference type;

            public UnresolvedBoundProperty(IEdmStructuredType declaringType, string name)
            {
                this.declaringType = declaringType;
                this.name = name;
                this.type = new UnresolvedBoundPropertyType();
            }

            public string DefaultValueString
            {
                get { return null; }
            }

            public EdmConcurrencyMode ConcurrencyMode
            {
                get { return EdmConcurrencyMode.None; }
            }

            public EdmPropertyKind PropertyKind
            {
                get { return EdmPropertyKind.Structural; }
            }

            public IEdmTypeReference Type
            {
                get { return this.type; }
            }

            public IEdmStructuredType DeclaringType
            {
                get { return this.declaringType; }
            }

            public string Name
            {
                get { return this.name; }
            }

            private class UnresolvedBoundPropertyType : IEdmTypeReference, IEdmType
            {
                public bool IsNullable
                {
                    get { return true; }
                }

                public IEdmType Definition
                {
                    get { return this; }
                }

                public EdmTypeKind TypeKind
                {
                    get { return EdmTypeKind.None; }
                }
            }
        }
    }
}
