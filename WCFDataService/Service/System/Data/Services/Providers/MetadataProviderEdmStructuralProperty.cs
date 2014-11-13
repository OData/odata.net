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

namespace System.Data.Services.Providers
{
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;

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
