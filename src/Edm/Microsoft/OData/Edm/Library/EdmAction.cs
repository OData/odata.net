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
    /// Represents an EDM action.
    /// </summary>
    public class EdmAction : EdmOperation, IEdmAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmAction"/> class.
        /// </summary>
        /// <param name="namespaceName">Name of the namespace.</param>
        /// <param name="name">The name.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="isBound">if set to <c>true</c> [is bound].</param>
        /// <param name="entitySetPathExpression">The entity set path expression.</param>
        public EdmAction(string namespaceName, string name, IEdmTypeReference returnType, bool isBound, IEdmPathExpression entitySetPathExpression)
            : base(namespaceName, name, returnType, isBound, entitySetPathExpression)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmAction"/> class.
        /// </summary>
        /// <param name="namespaceName">Name of the namespace.</param>
        /// <param name="name">The name.</param>
        /// <param name="returnType">Type of the return.</param>
        public EdmAction(string namespaceName, string name, IEdmTypeReference returnType)
            : base(namespaceName, name, returnType)
        {
        }

        /// <summary>
        /// Gets the element kind of this schema element kind which is an Action.
        /// </summary>
        public override EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.Action; }
        }
    }
}
