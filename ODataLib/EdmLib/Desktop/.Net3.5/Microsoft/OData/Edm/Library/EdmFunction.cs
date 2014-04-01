//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Library
{
    using Microsoft.OData.Edm.Expressions;

    /// <summary>
    /// Represents an EDM function.
    /// </summary>
    public class EdmFunction : EdmOperation, IEdmFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmFunction"/> class.
        /// </summary>
        /// <param name="namespaceName">Name of the namespace.</param>
        /// <param name="name">The name.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="isBound">if set to <c>true</c> [is bound].</param>
        /// <param name="entitySetPathExpression">The entity set path expression.</param>
        /// <param name="isComposable">A value indicating if the function is composable or not.</param>
        public EdmFunction(string namespaceName, string name, IEdmTypeReference returnType, bool isBound, IEdmPathExpression entitySetPathExpression, bool isComposable)
            : base(namespaceName, name, returnType, isBound, entitySetPathExpression)
        {
            EdmUtil.CheckArgumentNull(returnType, "returnType");
            this.IsComposable = isComposable;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmFunction"/> class.
        /// </summary>
        /// <param name="namespaceName">Namespace of the function.</param>
        /// <param name="name">Name of the function.</param>
        /// <param name="returnType">Return type of the function.</param>
        public EdmFunction(string namespaceName, string name, IEdmTypeReference returnType)
            : this(namespaceName, name, returnType, false /*isBound*/, null, false /*isComposable*/)
        {
        }

        /// <summary>
        /// Gets the element kind of this operation, which is always Operation.
        /// virtual will be removed in the near future, stop gap to enable testing for now.
        /// </summary>
        public override EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.Function; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is composable.
        /// </summary>
        public bool IsComposable { get; private set; }
    }
}
