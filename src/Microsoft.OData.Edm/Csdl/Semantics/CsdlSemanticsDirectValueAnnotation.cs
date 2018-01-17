//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsDirectValueAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

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
