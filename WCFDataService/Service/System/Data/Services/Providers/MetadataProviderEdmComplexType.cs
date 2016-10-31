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
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Annotations;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.Edm.Library.Annotations;

    #endregion Namespaces

    /// <summary>
    /// An <see cref="IEdmComplexType"/> implementation that supports delay-loading of properties and remembers the <see cref="ResourceType"/> it was based on.
    /// </summary>
    internal sealed class MetadataProviderEdmComplexType : EdmComplexTypeWithDelayLoadedProperties, IEdmComplexType, IResourceTypeBasedEdmType
    {
        /// <summary>
        /// Initializes a new instance of the MetadataProviderEdmEntityType class.
        /// </summary>
        /// <param name="namespaceName">Namespace the entity belongs to.</param>
        /// <param name="resourceType">The resource type this edm type is based on.</param>
        /// <param name="baseType">The base type of this entity type.</param>
        /// <param name="isAbstract">Denotes an entity that cannot be instantiated.</param>
        /// <param name="propertyLoadAction">An action that is used to create the properties for this type.</param>
        internal MetadataProviderEdmComplexType(
            string namespaceName,
            ResourceType resourceType,
            IEdmComplexType baseType,
            bool isAbstract, 
            Action<EdmComplexTypeWithDelayLoadedProperties> propertyLoadAction)
            : base(namespaceName, resourceType.Name, baseType, isAbstract, propertyLoadAction)
        {
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(resourceType.ResourceTypeKind == ResourceTypeKind.ComplexType, "Must be a complex type.");
            this.ResourceType = resourceType;
        }

        /// <summary>
        /// The resource-type that this type was created from.
        /// </summary>
        public ResourceType ResourceType { get; private set; }
    }
}
