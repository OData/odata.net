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

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents an EDM function.
    /// </summary>
    public class EdmFunction : EdmFunctionBase, IEdmFunction
    {
        private readonly string namespaceName;
        private readonly string definingExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmFunction"/> class.
        /// </summary>
        /// <param name="namespaceName">Namespace of the function.</param>
        /// <param name="name">Name of the function.</param>
        /// <param name="returnType">Return type of the function.</param>
        public EdmFunction(string namespaceName, string name, IEdmTypeReference returnType)
            : this(namespaceName, name, returnType, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmFunction"/> class.
        /// </summary>
        /// <param name="namespaceName">Namespace of the function.</param>
        /// <param name="name">Name of the function.</param>
        /// <param name="returnType">Return type of the function.</param>
        /// <param name="definingExpression">Defining expression of the function (for example an eSQL expression).</param>
        public EdmFunction(string namespaceName, string name, IEdmTypeReference returnType, string definingExpression)
            : base(name, returnType)
        {
            EdmUtil.CheckArgumentNull(namespaceName, "namespaceName");
            EdmUtil.CheckArgumentNull(returnType, "returnType");

            this.namespaceName = namespaceName;
            this.definingExpression = definingExpression;
        }

        /// <summary>
        /// Gets the defining expression of this function.
        /// </summary>
        public string DefiningExpression
        {
            get { return this.definingExpression; }
        }

        /// <summary>
        /// Gets the element kind of this function, which is always Function.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.Function; }
        }

        /// <summary>
        /// Gets the namespace of this function.
        /// </summary>
        public string Namespace
        {
            get { return this.namespaceName; }
        }
    }
}
