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
