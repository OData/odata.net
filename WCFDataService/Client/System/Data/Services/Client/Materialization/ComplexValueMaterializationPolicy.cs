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

namespace System.Data.Services.Client.Materialization
{
    using System;
    using System.Data.Services.Client;
    using System.Data.Services.Client.Metadata;
    using Microsoft.Data.OData;
    using DSClient = System.Data.Services.Client;

    /// <summary>
    /// Use this class to materialize objects provided from an <see cref="ComplexValueMaterializationPolicy"/>.
    /// </summary>
    internal class ComplexValueMaterializationPolicy : StructuralValueMaterializationPolicy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexValueMaterializationPolicy" /> class.
        /// </summary>
        /// <param name="materializerContext">The materializer context.</param>
        /// <param name="lazyPrimitivePropertyConverter">The lazy primitive property converter.</param>
        internal ComplexValueMaterializationPolicy(IODataMaterializerContext materializerContext, DSClient.SimpleLazy<PrimitivePropertyConverter> lazyPrimitivePropertyConverter) 
            : base(materializerContext, lazyPrimitivePropertyConverter)
        {
        }

        /// <summary>Materializes a complex type property.</summary>
        /// <param name="propertyType">Type of the complex type to set.</param>
        /// <param name="complexValue">The OData complex value.</param>
        internal void MaterializeComplexTypeProperty(Type propertyType, ODataComplexValue complexValue)
        {
            if (complexValue == null || complexValue.HasMaterializedValue())
            {
                return;
            }

            ClientTypeAnnotation complexType = null;

            if (WebUtil.IsWireTypeCollection(complexValue.TypeName))
            {
                complexType = this.MaterializerContext.ResolveTypeForMaterialization(propertyType, complexValue.TypeName);
            }
            else
            {
                ClientEdmModel edmModel = this.MaterializerContext.Model;
                complexType = edmModel.GetClientTypeAnnotation(edmModel.GetOrCreateEdmType(propertyType));
            }

            object complexInstance = this.CreateNewInstance(complexType.EdmType.ToEdmTypeReference(true), propertyType);
            this.MaterializeDataValues(complexType, complexValue.Properties, this.MaterializerContext.IgnoreMissingProperties);
            this.ApplyDataValues(complexType, complexValue.Properties, complexInstance);
            complexValue.SetMaterializedValue(complexInstance);
        }
    }
}
