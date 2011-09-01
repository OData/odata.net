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

namespace Microsoft.Data.Edm
{
    /// <summary>
    /// Represents an EDM function import.
    /// </summary>
    public interface IEdmFunctionImport : IEdmFunctionBase, IEdmEntityContainerElement
    {
        /// <summary>
        /// Gets a value indicating whether this function import has side-effects.
        /// <see cref="SideEffecting"/> cannot be set to true if <see cref="Composable"/> is set to true.
        /// </summary>
        bool SideEffecting { get; }

        /// <summary>
        /// Gets a value indicating whether this functon import can be composed inside expressions.
        /// <see cref="Composable"/> cannot be set to true if <see cref="SideEffecting"/> is set to true.
        /// </summary>
        bool Composable { get; }

        /// <summary>
        /// Gets a value indicating whether this function import can be used as an extension method for the type of the first parameter of this function import.
        /// </summary>
        bool Bindable { get; }

        /// <summary>
        /// Gets the entity set path of the function import.
        /// </summary>
        string EntitySetPath { get; }

        /// <summary>
        /// Gets the entity set that the result of this function import will be contained in.
        /// </summary>
        IEdmEntitySet EntitySet { get; }
    }
}
