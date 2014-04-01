//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents an EDM operation.
    /// </summary>
    public abstract class EdmOperation : EdmFunctionBase, IEdmOperation
    {
        private readonly string namespaceName;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmOperation"/> class.
        /// </summary>
        /// <param name="namespaceName">Namespace of the operation.</param>
        /// <param name="name">Name of the operation.</param>
        /// <param name="returnType">Return type of the operation.</param>
        protected EdmOperation(string namespaceName, string name, IEdmTypeReference returnType)
            : base(name, returnType)
        {
              EdmUtil.CheckArgumentNull(namespaceName, "namespaceName");
            EdmUtil.CheckArgumentNull(returnType, "returnType");

            this.namespaceName = namespaceName;
        }

        /// <summary>
        /// Gets the element kind of this operation, which is always Operation.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.Operation; }
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
