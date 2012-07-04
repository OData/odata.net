//   Copyright 2011 Microsoft Corporation
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

using Microsoft.Data.Edm.Expressions;

namespace Microsoft.Data.Edm
{
    /// <summary>
    /// Represents an EDM function import.
    /// </summary>
    public interface IEdmFunctionImport : IEdmFunctionBase, IEdmEntityContainerElement
    {
        /// <summary>
        /// Gets a value indicating whether this function import has side-effects.
        /// <see cref="IsSideEffecting"/> cannot be set to true if <see cref="IsComposable"/> is set to true.
        /// </summary>
        bool IsSideEffecting { get; }

        /// <summary>
        /// Gets a value indicating whether this functon import can be composed inside expressions.
        /// <see cref="IsComposable"/> cannot be set to true if <see cref="IsSideEffecting"/> is set to true.
        /// </summary>
        bool IsComposable { get; }

        /// <summary>
        /// Gets a value indicating whether this function import can be used as an extension method for the type of the first parameter of this function import.
        /// </summary>
        bool IsBindable { get; }

        /// <summary>
        /// Gets the entity set containing entities returned by this function import.
        /// </summary>
        IEdmExpression EntitySet { get; }
    }
}
