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
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Library.Values;
using Microsoft.Data.Edm.Values;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlDirectValueAnnotation.
    /// </summary>
    internal class CsdlSemanticsDirectValueAnnotation : CsdlSemanticsElement, IEdmDirectValueAnnotation
    {
        private readonly CsdlDirectValueAnnotation annotation;
        private readonly CsdlSemanticsModel model;

        private readonly Cache<CsdlSemanticsDirectValueAnnotation, IEdmValue> valueCache = new Cache<CsdlSemanticsDirectValueAnnotation, IEdmValue>();
        private static readonly Func<CsdlSemanticsDirectValueAnnotation, IEdmValue> ComputeValueFunc = (me) => me.ComputeValue();

        public CsdlSemanticsDirectValueAnnotation(CsdlDirectValueAnnotation annotation, CsdlSemanticsModel model)
            : base(annotation)
        {
            this.annotation = annotation;
            this.model = model;
        }

        public override CsdlElement Element
        {
            get { return this.annotation; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.model; }
        }

        public string NamespaceUri
        {
            get { return this.annotation.NamespaceName; }
        }

        public string Name
        {
            get { return this.annotation.Name; }
        }

        public object Value
        {
            get { return this.valueCache.GetValue(this, ComputeValueFunc, null); }
        }

        private IEdmValue ComputeValue()
        {
            IEdmStringValue value = new EdmStringConstant(new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false), this.annotation.Value);
            value.SetIsSerializedAsElement(this.model, !this.annotation.IsAttribute);
            return value;
        }
    }
}
