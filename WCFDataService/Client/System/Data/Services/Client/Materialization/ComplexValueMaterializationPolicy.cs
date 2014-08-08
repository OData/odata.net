//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
