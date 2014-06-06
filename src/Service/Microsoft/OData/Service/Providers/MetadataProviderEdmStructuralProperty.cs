//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Providers
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;

    /// <summary>
    /// Implementation of <see cref="IEdmStructuralProperty"/> based on a <see cref="ResourceProperty"/>.
    /// </summary>
    internal class MetadataProviderEdmStructuralProperty : EdmStructuralProperty, IResourcePropertyBasedEdmProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataProviderEdmStructuralProperty"/> class.
        /// </summary>
        /// <param name="declaringType">The type that declares this property.</param>
        /// <param name="resourceProperty">The resource-property this edm property is based on.</param>
        /// <param name="type">The type of the property.</param>
        /// <param name="defaultValue">The default value of this property.</param>
        /// <param name="concurrencyMode">The concurrency mode of this property.</param>
        public MetadataProviderEdmStructuralProperty(
            IEdmStructuredType declaringType,
            ResourceProperty resourceProperty,
            IEdmTypeReference type, 
            string defaultValue,
            EdmConcurrencyMode concurrencyMode)
            : base(declaringType, resourceProperty.Name, type, defaultValue, concurrencyMode)
        {
            this.ResourceProperty = resourceProperty;
        }

        /// <summary>
        /// The <see cref="ResourceProperty"/> this edm property was created from.
        /// </summary>
        public ResourceProperty ResourceProperty { get; private set; }
    }
}
