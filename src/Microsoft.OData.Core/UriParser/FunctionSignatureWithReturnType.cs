//---------------------------------------------------------------------
// <copyright file="FunctionSignatureWithReturnType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Class representing a function signature using EDM types.
    /// </summary>
    public sealed class FunctionSignatureWithReturnType
    {
        /// <summary>The argument types for this function signature.</summary>
        private readonly IEdmTypeReference[] argumentTypes;

        /// <summary>
        /// The return type of this function signature.
        /// </summary>
        private readonly IEdmTypeReference returnType;

        /// <summary>
        /// Constructor taking all the argument types.
        /// </summary>
        /// <param name="returnType">The return type of this function signature.</param>
        /// <param name="argumentTypes">The argument types for this function signature.</param>
        public FunctionSignatureWithReturnType(IEdmTypeReference returnType, params IEdmTypeReference[] argumentTypes)
        {
            this.argumentTypes = argumentTypes;
            this.returnType = returnType;
        }

        /// <summary>
        /// The return type of this function signature.
        /// </summary>
        public IEdmTypeReference ReturnType
        {
            get
            {
                return this.returnType;
            }
        }

        /// <summary>
        /// The argument types for this function signature.
        /// </summary>
        public IEdmTypeReference[] ArgumentTypes
        {
            get
            {
                return this.argumentTypes;
            }
        }
    }
}
