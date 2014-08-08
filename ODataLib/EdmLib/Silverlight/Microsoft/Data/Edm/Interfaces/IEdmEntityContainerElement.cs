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

namespace Microsoft.Data.Edm
{
    /// <summary>
    /// Defines EDM container element types.
    /// </summary>
    public enum EdmContainerElementKind
    {
        /// <summary>
        /// Represents an element where the container kind is unknown or in error.
        /// </summary>
        None,

        /// <summary>
        /// Represents an element implementing <see cref="IEdmEntitySet"/>. 
        /// </summary>
        EntitySet,

        /// <summary>
        /// Represents an element implementing <see cref="IEdmFunctionImport"/>.
        /// </summary>
        FunctionImport
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
