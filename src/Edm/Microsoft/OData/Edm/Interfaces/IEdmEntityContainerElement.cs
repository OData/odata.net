//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Defines EDM container element types.
    /// </summary>
    public enum EdmContainerElementKind
    {
        /// <summary>
        /// Represents an element where the container kind is unknown or in error.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents an element implementing <see cref="IEdmEntitySet"/>. 
        /// </summary>
        EntitySet,

        /// <summary>
        /// Represents an element implementing <see cref="IEdmActionImport"/>.
        /// </summary>
        ActionImport,

        /// <summary>
        /// Represents an element implementing <see cref="IEdmFunctionImport"/>. 
        /// </summary>
        FunctionImport,

        /// <summary>
        /// Represents an element implementing <see cref="IEdmSingleton"/>.
        /// </summary>
        Singleton
    }

    /// <summary>
    /// Represents the common elements of all EDM entity container elements.
    /// </summary>
    public interface IEdmEntityContainerElement : IEdmNamedElement, IEdmVocabularyAnnotatable
    {
        /// <summary>
        /// Gets the kind of element of this container element.
        /// </summary>
        EdmContainerElementKind ContainerElementKind { get; }

        /// <summary>
        /// Gets the container that contains this element.
        /// </summary>
        IEdmEntityContainer Container { get; }
    }
}
