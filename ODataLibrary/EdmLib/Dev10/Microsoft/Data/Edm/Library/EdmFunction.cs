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

using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents an EDM function.
    /// </summary>
    public class EdmFunction : EdmFunctionBase, IEdmFunction
    {
        private string namespaceName;
        private string definingExpression;

        /// <summary>
        /// Initializes a new instance of the EdmFunction class.
        /// </summary>
        public EdmFunction()
        {
            this.namespaceName = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the EdmFunction class.
        /// </summary>
        /// <param name="namespaceName">Namespace of the function.</param>
        /// <param name="name">Name of the function.</param>
        /// <param name="returnType">Return type of the function.</param>
        public EdmFunction(string namespaceName, string name, IEdmTypeReference returnType)
            : base(name, returnType)
        {
            this.namespaceName = namespaceName ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the defining expression of this function.
        /// </summary>
        public string DefiningExpression
        {
            get { return this.definingExpression; }
            set { this.SetField(ref this.definingExpression, value); }
        }

        /// <summary>
        /// Gets the element kind of this function, which is always Function.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.Function; }
        }

        /// <summary>
        /// Gets or sets the namespace of this function.
        /// </summary>
        public string Namespace
        {
            get { return this.namespaceName; }
            set { this.SetField(ref this.namespaceName, value ?? string.Empty); }
        }
    }
}
