//---------------------------------------------------------------------
// <copyright file="FunctionSignature.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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