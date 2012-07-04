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
