//---------------------------------------------------------------------
// <copyright file="FunctionSignature.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Class representing a function signature using EDM types.
    /// </summary>
    internal sealed class FunctionSignature
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
        public IEdmTypeReference[] ArgumentTypes 
        {
            get
            {
                return this.argumentTypes;
            }
        }
    }
}