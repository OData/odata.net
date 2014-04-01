//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Values;
using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
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
