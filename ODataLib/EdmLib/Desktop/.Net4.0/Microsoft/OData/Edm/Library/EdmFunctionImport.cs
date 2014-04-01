//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents an EDM function import.
    /// </summary>
    public class EdmFunctionImport : EdmOperationImport, IEdmFunctionImport
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EdmFunctionImport"/> class.
        /// </summary>
        /// <param name="container">An <see cref="IEdmEntityContainer"/> containing this function import.</param>
        /// <param name="name">Name of the function import.</param>
        /// <param name="returnType">Return type of the function import.</param>
        public EdmFunctionImport(IEdmEntityContainer container, string name, IEdmTypeReference returnType)
            : this(container, name, returnType, null, false /*isBindable*/, true /*isComposable*/)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EdmFunctionImport"/> class.
        /// </summary>
        /// <param name="container">An <see cref="IEdmEntityContainer"/> containing this function import.</param>
        /// <param name="name">Name of the function import.</param>
        /// <param name="returnType">Return type of the function import.</param>
        /// <param name="entitySet">An entity set containing entities returned by this function import. 
        /// The two expression kinds supported are <see cref="IEdmEntitySetReferenceExpression"/> and <see cref="IEdmPathExpression"/>.</param>
        public EdmFunctionImport(IEdmEntityContainer container, string name, IEdmTypeReference returnType, IEdmExpression entitySet)
            : this(container, name, returnType, entitySet, false /*isBindable*/, true /*isComposable*/)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EdmFunctionImport"/> class.
        /// </summary>
        /// <param name="container">An <see cref="IEdmEntityContainer"/> containing this function import.</param>
        /// <param name="name">Name of the function import.</param>
        /// <param name="returnType">Return type of the function import.</param>
        /// <param name="entitySet">An entity set containing entities returned by this function import. 
        /// The two expression kinds supported are <see cref="IEdmEntitySetReferenceExpression"/> and <see cref="IEdmPathExpression"/>.</param>
        /// <param name="isBindable">A value indicating whether this function import can be used as an extension method for the type of the first parameter of this action import.</param>
        /// <param name="isComposable">A value indicating whether this function import is composable.</param>
        public EdmFunctionImport(IEdmEntityContainer container, string name, IEdmTypeReference returnType, IEdmExpression entitySet, bool isBindable, bool isComposable)
            : base(container, name, returnType, entitySet, false /*isSideAffecting*/, isComposable, isBindable)
        {
        }
    }
}
