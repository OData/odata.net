//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser
{
    #region Namespaces
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Class representing a function signature using EDM types.
    /// </summary>
    internal class FunctionSignature
    {
        /// <summary>The argument types for this function signature.</summary>
        private readonly IEdmTypeReference[] argumentTypes;

        /// <summary>
        /// Constructor taking all the argument types.
        /// </summary>
        /// <param name="argumentTypes">The argument types for this function signature.</param>
        internal FunctionSignature(params IEdmTypeReference[] argumentTypes)
        {
            this.argumentTypes = argumentTypes;
        }

        /// <summary>
        /// The argument types for this function signature.
        /// </summary>
        internal IEdmTypeReference[] ArgumentTypes 
        {
            get
            {
                return this.argumentTypes;
            }
        }
    }

    /// <summary>
    /// Class representing a function signature using EDM types.
    /// </summary>
    internal sealed class FunctionSignatureWithReturnType : FunctionSignature
    {
        /// <summary>
        /// The return type of this function signature.
        /// </summary>
        private readonly IEdmTypeReference returnType;

        /// <summary>
        /// Constructor taking all the argument types.
        /// </summary>
        /// <param name="returnType">The return type of this function signature.</param>
        /// <param name="argumentTypes">The argument types for this function signature.</param>
        internal FunctionSignatureWithReturnType(IEdmTypeReference returnType, params IEdmTypeReference[] argumentTypes)
            : base(argumentTypes)
        {
            this.returnType = returnType;
        }

        /// <summary>
        /// The return type of this function signature.
        /// </summary>
        internal IEdmTypeReference ReturnType 
        {
            get
            {
                return this.returnType;
            }
        }
    }
}
