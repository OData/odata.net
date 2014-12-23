//   OData .NET Libraries ver. 6.9
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

using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents an EDM function import.
    /// </summary>
    public class EdmFunctionImport : EdmOperationImport, IEdmFunctionImport
    {
        private const string FunctionArgumentNullParameterName = "function";

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmFunctionImport"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="name">The name.</param>
        /// <param name="function">The function.</param>
        public EdmFunctionImport(IEdmEntityContainer container, string name, IEdmFunction function)
            : this(container, name, function, null, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmFunctionImport"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="name">The name.</param>
        /// <param name="function">The function.</param>
        /// <param name="entitySetExpression">The entity set expression.</param>
        /// <param name="includeInServiceDocument">The value indicates if the function is to be include in the service document or not.</param>
        public EdmFunctionImport(IEdmEntityContainer container, string name, IEdmFunction function, IEdmExpression entitySetExpression, bool includeInServiceDocument)
            : base(container, function, name, entitySetExpression)
        {
            EdmUtil.CheckArgumentNull(function, "function");

            this.Function = function;
            this.IncludeInServiceDocument = includeInServiceDocument;
        }

        /// <summary>
        /// Gets the function that defines the function import.
        /// </summary>
        public IEdmFunction Function { get; private set; }

        /// <summary>
        /// Gets the kind of this operation, which is always FunctionImport.
        /// </summary>
        public override EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.FunctionImport; }
        }

        /// <summary>
        /// Gets a value indicating whether [include in service document].
        /// </summary>
        public bool IncludeInServiceDocument { get; private set; }

        /// <summary>
        /// Operations the name of the argument null parameter.
        /// </summary>
        /// <returns>Returns the name of the operation from this import.</returns>
        protected override string OperationArgumentNullParameterName()
        {
            return FunctionArgumentNullParameterName;
        }
    }
}
