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

namespace Microsoft.Data.OData.Query.SemanticAst
{
    using Microsoft.Data.Edm;

    /// <summary>
    /// Class to represent the selection of all the actions and functions in a specified container.
    /// </summary>
    public sealed class ContainerQualifiedWildcardSelectItem : SelectItem
    {
        /// <summary>
        /// The <see cref="IEdmEntityContainer"/> whose actions and functions should be selected.
        /// </summary>
        private readonly IEdmEntityContainer container;

        /// <summary>
        /// Creates an instance of this class with the specified <paramref name="container"/>.
        /// </summary>
        /// <param name="container">The <see cref="IEdmEntityContainer"/> whose actions and functions should be selected.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input container is null.</exception>
        public ContainerQualifiedWildcardSelectItem(IEdmEntityContainer container)
        {
            ExceptionUtils.CheckArgumentNotNull(container, "container");
            this.container = container;
        }

        /// <summary>
        /// Gets the <see cref="IEdmEntityContainer"/> whose actions and functions should be selected.
        /// </summary>
        public IEdmEntityContainer Container
        {
            get { return this.container; }
        }
    }
}
